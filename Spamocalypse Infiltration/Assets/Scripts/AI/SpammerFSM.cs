using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The finite-state machine for the spammers.
/// Handles the high-level AI
/// </summary>

[DisallowMultipleComponent]
public class SpammerFSM : MonoBehaviour 
{
	/// <summary>
	/// The spammer's health.
	/// </summary>
	public int health = 100;

	/// <summary>
	/// The critical health point.
	/// </summary>
	public int criticalHealth;

	[HideInInspector]
	public bool isDead = false;

	// details related to the sockpuppet alerts
	public int maxFalseAlerts;
	protected int falseAlerts;
	protected Vector3 lastPosition;		// a position to return to afterwards
	protected Vector3 destination;
	
	/// <summary>
	/// How long the sockpuppets should distract them.
	/// </summary>
	public float distractionTime;

	protected bool searchingForPuppet = false;

	protected bool firstTimeSearch = false;
	
	public float maxAlertTime;
	protected float alertTime;

	public float maxBlindedTime = 3f;
	protected float blindedTime;

	static float difficultyMultiplier;

	/// <summary>
	/// A series of points around which to patrol.
	/// </summary>
	public List<PatrolPoints> patrolPoints;
	int patrolIndex;

	public float normalTurnSpeed = 5f;
	public float startledTurnSpeed = 8f;
	float turnSpeed;

	/// <summary>
	/// The spammer changes path steps when they get this close to
	/// the current step.
	/// </summary>
	public float stepChangeDistance = 0.5f;

	/// <summary>
	/// Does this spammer take damage from firewalls?
	/// </summary>
	public bool takesFireDamage = true;

	/// <summary>
	/// The amount of time they are on fire
	/// </summary>
	public float maxFlameTime = 3f;
	protected float flameTime;


	public float maxAttackAngle;
	protected Weapon myWeapon;

	/// <summary>
	/// The target for attacking or searching.
	/// </summary>
	protected Transform target, prevTarget;

	/// <summary>
	/// The maximum range at which to attack
	/// </summary>
	public float maxAttackRange;

	/// <summary>
	/// The preferred distance at which to attack.
	/// </summary>
	public float optimalAttackRange;

	protected RaycastHit hit;

	/// <summary>
	/// The last known player position.
	/// </summary>
	protected Vector3 lastPlayerPosition;
	protected Transform myTransform;
	protected Quaternion myRotation;
	protected Vector3 myPosition;

	public LevelManager levelManager;

	protected DisplayQuestionMark moodIndicator;
	public static bool showQuestionMark = false;

	// four common states
	public enum SpammerState
	{
		attacking = 0,			// has detected the player
		idle = 1,				// standing still
		patrolling = 2,			// following a path
		searching = 3,			// has lost the player
		flamed = 4,				// appears to have burst into flames
		dead = 5,				// pining for the fjorums
		bamboozled = 6			// hit by a logic bomb
	}

	SpammerState currentState = SpammerState.idle;

	// three possible detection levels
	public enum DetectionStatus
	{
		confirmed = 0,
		possible = 1,
		unaware = 2
	}

	DetectionStatus alertStatus = DetectionStatus.unaware;

	public enum AlertType
	{
		visual = 0,
		auditory = 1,
		physical = 2,
		flamed = 3,
		other = 4
	}

	[HideInInspector]
	public SoundDetection ears;
	[HideInInspector]
	public LineOfSight eyes;
	protected MoveAgent movement;
	protected PlaySounds myAudio;

	/// <summary>
	/// Does the spammer have a direct line to something?
	/// </summary>
	protected bool directAttackLine;

	[HideInInspector]
	public float minIdleSpamTime, maxIdleSpamTime;
	float spamTime;

	public PlayerControl player;

	protected Animator myAnimator;

	[Tooltip("The maximum number of times they will respawn before they get suspicious")]
	public int maxRespawns;
	protected int respawns;

    protected SpammerEffects effects;

	// Use this for initialization
	protected virtual void Start () 
	{
		myTransform = transform;
		myPosition = myTransform.position;
		myRotation = myTransform.rotation;
		turnSpeed = normalTurnSpeed;
		target = myTransform;
		myAudio = GetComponent<PlaySounds>();
		moodIndicator = GetComponentInChildren<DisplayQuestionMark>();
		myAnimator = GetComponentInChildren<Animator>();
		myWeapon = GetComponentInChildren<Weapon>();
		ears = GetComponentInChildren<SoundDetection>();
		eyes = GetComponentInChildren<LineOfSight>();
		movement = GetComponent<MoveAgent>();
        effects = GetComponent<SpammerEffects>();

		minIdleSpamTime = GameTagManager.minSpamTime;
		maxIdleSpamTime = GameTagManager.maxSpamTime;
		SetSpamTimes();

		// compensate for the difficulty
		difficultyMultiplier = GameTagManager.GetDifficultyMultiplier();
		maxAlertTime *= difficultyMultiplier;

		// if they have patrol points, start patrolling. Remove any that are null first.
        patrolPoints.RemoveAll(d=> d == null);
		if (patrolPoints.Count > 0)
		{
			StartPatrolling ();
		}
		else 
		{
			ChangeStateTo(SpammerState.idle);
		}

		if (CompareTag(GameTagManager.adminTag) )
		{
			takesFireDamage = false;		// admins don't take fire damage
			maxFlameTime = 0f;
		}

		myWeapon.Activate();
		ears.brain = this;
		eyes.brain = this;
	}

	/// <summary>
	/// Starts the patrolling.
	/// </summary>
	void StartPatrolling ()
	{
        // reverse their patrol points at random - just to keep the player on their toes :)
        if (Random.Range(0f,1f) > 0.5f)
        {
            patrolPoints.Reverse();
        }

		patrolIndex = 0;
		SetDestination (patrolPoints [0].GetPosition());
		ChangeStateTo (SpammerState.patrolling);
		Invoke("StartMoving", 1f);
	}

	/// <summary>
	/// Fix for bots randomly not moving
	/// </summary>
	void StartMoving()
	{
		if (patrolPoints.Count > 0 && (currentState == SpammerState.idle || movement.currentMovement == MoveAgent.MoveType.stopped) )
		{
			GameTagManager.LogMessage("The spammer {0} is idle, but should not be! Back to work!", this);
			SetDestination(patrolPoints[0].GetPosition());
		}
	}

	// Update is called once per frame
	protected virtual void Update () 
	{
		myPosition = myTransform.position;

		switch (currentState)
		{
		case SpammerState.attacking:
			MyAttack();
			break;
		case SpammerState.idle:

			spamTime += Time.deltaTime;
			if (spamTime > minIdleSpamTime)
			{
				myAudio.PlayIdleSound();
				spamTime = 0;
				SetSpamTimes();
			}

			break;
		case SpammerState.patrolling:

            spamTime += Time.deltaTime;
            if (spamTime > minIdleSpamTime)
            {
                myAudio.PlayIdleSound();
                spamTime = 0;
                SetSpamTimes();
            }
            if (Vector3.Distance(myPosition, destination) < stepChangeDistance)
            {
                patrolIndex++;
                if (patrolIndex >= patrolPoints.Count)
                {
                    patrolIndex = 0;
                }
                SetDestination(patrolPoints[patrolIndex].GetPosition());
            }

			break;
		case SpammerState.searching:

//            RotateTorwards(lastPosition);
			MySearch();
			break;

		case SpammerState.flamed:
			flameTime += Time.deltaTime;

			if (Mathf.Approximately(flameTime % 1f, 0))
			{
				DamageSpammer(GameTagManager.AttackMode.fire, GameTagManager.firewallDamagePerSecond, Vector3.zero);
				
				SetDestination(myTransform.forward * Random.Range(0, 11));
			}

			if (flameTime >= maxFlameTime)
			{
				SetAlert (myTransform, DetectionStatus.possible, AlertType.other);
			}
			break;

		case SpammerState.bamboozled:
			blindedTime += Time.deltaTime;
			if (blindedTime >= maxBlindedTime)
			{
				SetAlert(myPosition, DetectionStatus.possible, AlertType.other);
			}
			break;
		}

	}
 
	
	/// <summary>
	/// Changes the spammer's state to a new one.
	/// </summary>
	/// <param name="newState">New state.</param>
	public void ChangeStateTo(SpammerState newState)
	{
		if (currentState != newState)
		{
			// change the movement type to normal if not specifically idle or attacking
			movement.ChangeMovement(MoveAgent.MoveType.normal);
			turnSpeed = normalTurnSpeed;

			if (newState == SpammerState.idle)
			{
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
				turnSpeed = normalTurnSpeed;
				myAnimator.SetBool("IsMoving", false);
				myAnimator.SetBool("IsAttacking", false);
				myAnimator.SetBool("IsSearching", false);
			}
			else if(newState == SpammerState.attacking)
			{
				movement.ChangeMovement(MoveAgent.MoveType.attacking);
				turnSpeed = startledTurnSpeed;
			}
			else if(newState == SpammerState.searching)
			{
				myAnimator.SetBool("IsSearching", true);
				turnSpeed = startledTurnSpeed;
			}
			else if (newState == SpammerState.dead)
			{
				myAnimator.SetBool("IsAttacking", false);
				myAnimator.SetBool("IsMoving", false);
				myAnimator.SetBool("IsSearching", false);
				ears.Deactivate();
				eyes.Deactivate();
				myAudio.PlayDeathSound();
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
				gameObject.layer = LayerMask.NameToLayer("Dead Spammer");
			}
			else if (newState == SpammerState.bamboozled)
			{
				eyes.Deactivate();
				ears.Deactivate();
			}
			else
			{
				myAnimator.SetBool("IsMoving", true);
				myAnimator.SetBool("IsAttacking", false);
				myAnimator.SetBool("IsSearching", false);
                SetDestination(patrolPoints[patrolIndex].GetPosition());
				turnSpeed = normalTurnSpeed;
			}

			currentState = newState;
		}

	}

	/// <summary>
	/// Gets the spammer's current state.
	/// </summary>
	/// <returns>The current state.</returns>
	public SpammerState GetCurrentState()
	{
		return currentState;
	}

	public DetectionStatus GetAlertLevel()
	{
		return alertStatus;
	}

	/// <summary>
	/// Sets the destination for the path.
	/// </summary>
	/// <param name="newDestination">New destination.</param>
	protected void SetDestination(Vector3 newDestination)
	{
		destination = newDestination;
        movement.SetMoveTarget(destination);

		
		if (currentState == SpammerState.attacking)
		{
			movement.ChangeMovement(MoveAgent.MoveType.attacking);
		}
		else
		{
			movement.ChangeMovement(MoveAgent.MoveType.normal);
		}
	}

	void ProcessAlertType(AlertType alert)
	{
		switch (alert)
		{
			case AlertType.auditory:
				myAudio.PlayAuditorySound();
				break;
			case AlertType.visual:
				myAudio.PlaySpottingSound();
				break;
			case AlertType.physical:
				myAudio.PlayContactSound();
				break;
			case AlertType.other:
				// play any sound?
				break;
		}

	}

	public void SetAlert(Vector3 position, DetectionStatus newStatus, AlertType currentAlertType)
	{
        if (currentState == SpammerState.dead || currentState == SpammerState.bamboozled)
        {
            return;
        }

		if (alertStatus != newStatus)
		{
//			GameManager.LogMessage("{0} has a new status of {1} from a target near {2}", name, newStatus, position);
			if (newStatus == DetectionStatus.possible)
			{
				if (showQuestionMark)
				{
					moodIndicator.ChangeStatus(DisplayQuestionMark.Mood.puzzled);
				}

				ProcessAlertType(currentAlertType);
				lastPlayerPosition = position;
				lastPosition = myPosition;
				SetDestination(lastPlayerPosition);
				ChangeStateTo(SpammerState.searching);
			}
			if (newStatus == DetectionStatus.confirmed)
			{
				if (showQuestionMark)
				{
					moodIndicator.ChangeStatus(DisplayQuestionMark.Mood.alarmed);
				}
				myAudio.PlayAttackSound();
				levelManager.AddAlert(this, target);
				lastPosition = myPosition;
				lastPlayerPosition = position;            
				SetDestination(lastPlayerPosition);
				
				eyes.SetDetection(100f);
				ears.SetDetection(100f);
				
				ChangeStateTo(SpammerState.attacking);
			}
			if (newStatus == DetectionStatus.unaware)
			{
				if (showQuestionMark)
				{
					moodIndicator.ChangeStatus(DisplayQuestionMark.Mood.normal);
				}

				levelManager.RemoveAlert(this);
				alertTime = 0f;
				prevTarget = target;
				target = myTransform;
				
				// if the spammer has a patrol path, go back to it.
				// Otherwise, go back to their start position
				if (patrolPoints.Count > 0)
				{
                    SetDestination(patrolPoints[patrolIndex].GetPosition());
					ChangeStateTo(SpammerState.patrolling);
				}
				else
				{
					SetDestination(lastPosition);
					ChangeStateTo(SpammerState.idle);
				}
				ears.ResetAlert();
				eyes.ResetAlert();
			}
			
			alertStatus = newStatus;
		}
	}

	/// <summary>
	/// Alerts the spammer to a possible intruder.
	/// </summary>
	/// <param name="alertTransform">The transform that triggered an alert.</param>
	/// <param name="newStatus">New status.</param>
	/// <param name = "type">The type of alert</param>
	public void SetAlert(Transform alertTransform, DetectionStatus newStatus, AlertType type)
	{

		// get the Sockpuppet component if it is a decoy
		if (alertTransform.CompareTag(GameTagManager.decoyTag) && falseAlerts <= maxFalseAlerts )
		{
			searchingForPuppet = true;
		}      
		if (target != alertTransform && target != myTransform)
		{
            firstTimeSearch = true;
			myAudio.PlaySearchSound();
		}
		target = alertTransform;
		SetAlert (target.position, newStatus, type);
	}

	/// <summary>
	/// The search technique used by regular spammers works as follows.
	/// </summary>
	protected virtual void MySearch()
	{
		UnityEngine.Profiling.Profiler.BeginSample("Spammer Search");
		if (searchingForPuppet)
		{
			if (Vector3.Distance(lastPlayerPosition, myPosition) < stepChangeDistance)
			{
				RotateTorwards(lastPlayerPosition);
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
				alertTime += Time.deltaTime;
				if (alertTime > distractionTime)
				{
					if (prevTarget != target)
					{
						falseAlerts++;
					}
					myAudio.PlaySockpuppetSound();
					searchingForPuppet = false;
					Physics.Raycast(myPosition, myPosition + myTransform.forward * Random.Range(-5f, 10f) + 
					                (myTransform.right * Random.Range(-10f, 10f)), out hit);
					lastPlayerPosition = hit.point;
					lastPosition.y = myPosition.y;
					alertTime = 0f;
					SetDestination(lastPlayerPosition);
				}
			}
		}
		else
		{
			alertTime += Time.deltaTime;
			if (Vector3.Distance(myPosition, lastPlayerPosition) < stepChangeDistance)
			{
				myAudio.PlaySearchSound();
				if (!firstTimeSearch)
				{
					Physics.Raycast(myPosition, myPosition + myTransform.forward * Random.Range(-10f, 10f) + 
					                (myTransform.right * Random.Range(-10f, 10f)), out hit);
					lastPlayerPosition = hit.point;
					lastPosition.y = myPosition.y;
					SetDestination(lastPlayerPosition);
				}
				else
				{
					if (alertTime > distractionTime)
					{
						firstTimeSearch = false;
					}
				}

			}

			if (alertTime > maxAlertTime)
			{
				SetAlert(myTransform, DetectionStatus.unaware, AlertType.other);
			}
				
		}
		UnityEngine.Profiling.Profiler.EndSample();
	}

	/// <summary>
	/// How the regular spammers attack.
	/// </summary>
	protected virtual void MyAttack()
	{
		UnityEngine.Profiling.Profiler.BeginSample("Spammer Attack");
		float distance = Vector3.Distance(myPosition, target.position);

		// if they're within attack range, start attacking
		if (distance < maxAttackRange)
		{
			if (myWeapon.IsAbleToAttack() && Vector3.Angle(myTransform.position, target.position) < maxAttackAngle )
			{
				myAnimator.SetBool ("IsAttacking", true);
				myAnimator.SetBool("IsSearching", false);
                effects.PlayAttackEffects();
				myWeapon.StartAttack(target);
			}

			// if close enough, stop moving
			if (distance < optimalAttackRange)
			{
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
			}
		}
		else
		{
			if (movement.currentMovement == MoveAgent.MoveType.stopped)
			{
                movement.SetMoveTarget(target.position);
				movement.ChangeMovement(MoveAgent.MoveType.attacking);
				myAnimator.SetBool("IsSearching", true);
				myAnimator.SetBool("IsAttacking", false);
			}
		}
		UnityEngine.Profiling.Profiler.EndSample();
	}

	/// <summary>
	/// Rotate to face a specific position.
	/// </summary>
	/// <param name="endPosition">End position.</param>
	protected void RotateTorwards(Vector3 endPosition)
	{
		Vector3 delta = (endPosition - myPosition);
		delta.y = 0f;

		// now they need to figure out which way to rotate
		if (delta != Vector3.zero)
		{
			myRotation = Quaternion.LookRotation(delta, Vector3.up);
			myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, 
			                                                myRotation, turnSpeed * Time.deltaTime);
		}
	}


	/// <summary>
	/// Damages the spammer with a particular type and value.
	/// </summary>
	/// <param name="damageType">Damage type.</param>
	/// <param name="damage">Damage.</param>
	/// <param name="direction">Direction from which the attack came.</param>
	public void DamageSpammer(GameTagManager.AttackMode damageType, int damage, Vector3 direction)
	{
//		movement.SetKinematic(true);
		if (alertStatus == DetectionStatus.unaware)
		{
			damage *= GameTagManager.surprisedDamageMultiplier;
		}

		health -= damage;

		if (damageType == GameTagManager.AttackMode.fire)
		{
			ChangeStateTo(SpammerState.flamed);
//			movement.SetKinematic(false);
		}
		else 
		{
			if (health < 0)
			{
				Die ();
			}
			else
			{
				myAudio.PlayDamagedSound();
				SetAlert(myPosition - direction * 10f, DetectionStatus.possible, AlertType.other);
//				movement.SetKinematic(false);
			}
		}
	}

	/// <summary>
	/// Sets the spam times. If min or max time is zero, default to 10 and 30 seconds respectively.
	/// </summary>
	void SetSpamTimes()
	{ 
		minIdleSpamTime = Random.Range(GameTagManager.minSpamTime, maxIdleSpamTime);
	}

	/// <summary>
	/// When dying, play any death animations
	/// and the dying sound effecs
	/// </summary>
	void Die()
	{
		myAnimator.SetBool("IsDying", true);
		myAnimator.SetBool ("IsMoving", false);
		myAnimator.SetBool ("IsAttacking", false);
        myAnimator.SetBool ("IsSearching", false);
		moodIndicator.ChangeStatus(DisplayQuestionMark.Mood.normal);
        movement.ChangeMovement(MoveAgent.MoveType.stopped);
		isDead = true;
		ears.ResetAlert();
		ears.enabled = false;
		eyes.ResetAlert();
		eyes.enabled = false;
		alertTime = 0f;
		myAudio.PlayDeathSound();
		ChangeStateTo(SpammerState.dead);
		levelManager.AddSpammerDeath(this);
        effects.PlayDyingEffects();
	}

	/// <summary>
	/// Sets the path index when reloading.
	/// </summary>
	/// <param name="newPatrolIndex">New patrol Index</param>
	public void LoadPathIndices(int newPatrolIndex)
	{
		patrolIndex = newPatrolIndex;
	}
   
	public int GetPatrolIndex()
	{
		return patrolIndex;
	}

	/// <summary>
	/// Respawn this instance. If they have died too many times before, they will be suspicious.
	/// </summary>
	public void Respawn()
	{
		gameObject.layer = LayerMask.NameToLayer("Spammer");
        myAnimator.SetBool("IsDying", false);
        myAnimator.SetBool("IsMoving", false);
        myAnimator.SetBool("IsSearching", false);
        myAnimator.SetBool("IsAttacking", false);
        GameTagManager.LogMessage("{0} is respawning. Animation now in state {1}", name, myAnimator.GetCurrentAnimatorClipInfo(0)[0]);
        myAnimator.Play("Respawn");
        movement.PlayRespawnNoise();
		Invoke("FinishRespawn", 3f);
	}

	void FinishRespawn()
	{
		respawns++;
		health = 100;
        effects.PlayMoveEffects();
		if (respawns > maxRespawns)
		{
			SetAlert(myPosition - Vector3.forward * 15f, DetectionStatus.possible, AlertType.other);
		}
		else
		{
			if (patrolPoints.Count > 0)
			{
				ChangeStateTo(SpammerState.patrolling);
			}
			else
			{
				ChangeStateTo(SpammerState.idle);
			}
		}
	}

	/// <summary>
	/// Toggles the voices.
	/// </summary>
	/// <param name="shutUp">If set to <c>true</c>, shut <em>up</em>!</param>
	public void ToggleVoices(bool shutUp)
	{
		if (!isDead)
		{
			myAudio.ToggleTalking(shutUp);
		}
	}

	public override string ToString ()
	{
		return string.Format ("{0} [health: {1}; position: {2}]", name, health, myPosition);
	}

	public static void SetQuestionMark(bool showMood)
	{
		showQuestionMark = showMood;
	}
}