using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Moves an AI agent
/// </summary>

public class MoveAgent : MonoBehaviour 
{
	/// <summary>
	/// The speed of a normal patrol
	/// </summary>
	public float normalSpeed;

	/// <summary>
	/// The speed for pursuits or attacking
	/// </summary>
	public float attackingSpeed;

	/// <summary>
	/// The speed for retreating when damaged
	/// </summary>
	public float damagedSpeed;

	/// <summary>
	/// The movement sounds audio source.
	/// </summary>
	[Tooltip("The AudioSource that plays movement-related sounds")]
	public AudioSource movementSoundSource;

	[Tooltip("The list of movement sounds")]
	public List<AudioClip> movementSounds;

    public AudioClip dyingNoise, respawnNoise;

	float currentSpeed;

	Animator myAnimator;

	Transform myTransform;
	Vector3 myPosition;
	Rigidbody body;

	/// <summary>
	/// The move target relative to the agent
	/// </summary>
	public Vector3 moveTarget;

	public enum MoveType
	{
		normal,
		attacking,
		stopped
	}

	public MoveType currentMovement = MoveType.normal;

	SpammerFSM brain;

    UnityEngine.AI.NavMeshAgent navAgent;

    WaitForSeconds dyingCycle, respawnCycle;

	// Use this for initialization
	void Awake () 
	{
		myTransform = transform;
		myPosition = myTransform.position;
		body = GetComponent<Rigidbody>();
        body.isKinematic = true;
		moveTarget = myPosition;
		currentSpeed = normalSpeed;
		brain = GetComponentInParent<SpammerFSM>();
		myAnimator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

		if (currentMovement == MoveType.stopped)
		{
			movementSoundSource.Stop();
		}
        dyingCycle = new WaitForSeconds(dyingNoise.length);
        respawnCycle = new WaitForSeconds(respawnNoise.length);
	}

	/// <summary>
	/// Sets the move target.
	/// </summary>
	/// <param name="newTarget">New target.</param>
	public void SetMoveTarget(Vector3 newTarget)
	{
		navAgent.SetDestination(newTarget);
	}

	/// <summary>
	/// Changes the movement type.
	/// </summary>
	/// <param name="newMovement">New movement.</param>
	public void ChangeMovement(MoveType newMovement)
	{
		if (currentMovement != newMovement)
		{
			currentMovement = newMovement;

			if (currentMovement == MoveType.attacking)
			{
				currentSpeed = attackingSpeed;
				movementSoundSource.Play();
			}
			else if (currentMovement == MoveType.stopped)
			{
				currentSpeed = 0f;
				movementSoundSource.Stop();
				myAnimator.SetBool("IsMoving", false);
			}
			else
			{
				currentSpeed = normalSpeed;
				movementSoundSource.Play();
				myAnimator.SetBool("IsMoving", true);
			}
            navAgent.speed = currentSpeed;
		}
	}

	void OnCollisionEnter(Collision coll)
	{
		int collLayer = coll.gameObject.layer;
        if (collLayer == LayerMask.NameToLayer("Player") && !coll.gameObject.CompareTag("Projectile") )
		{
            GameTagManager.LogMessage("{0} walked into player. Distance is {1}", this, Vector3.Distance(myTransform.position, coll.gameObject.transform.position));
			brain.SetAlert(coll.gameObject.transform, SpammerFSM.DetectionStatus.confirmed, SpammerFSM.AlertType.physical);
		}
		else if (collLayer == LayerMask.NameToLayer("Dead Spammer"))
		{
			GameTagManager.LogMessage("{0} walked into a corpse at {1}", name, coll.contacts[0].point);
			brain.SetAlert(coll.gameObject.transform, SpammerFSM.DetectionStatus.possible, SpammerFSM.AlertType.physical);
		}

	}

	void OnDisable()
	{
		movementSoundSource.enabled = false;
	}

	void OnEnable()
	{
		movementSoundSource.enabled = true;
	}

    public IEnumerator PlayDyingNoise(float delay)
    {
        movementSoundSource.Stop();
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        movementSoundSource.clip = dyingNoise;
        movementSoundSource.Play();
        yield return dyingCycle;
        movementSoundSource.Stop();
    }

    public IEnumerator PlayRespawnNoise()
    {
        movementSoundSource.clip = respawnNoise;
        movementSoundSource.Play();
        yield return respawnCycle;
        movementSoundSource.clip = movementSounds[0];
    }
}
