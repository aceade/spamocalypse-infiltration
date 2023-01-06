using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles sound detection for the spammers.
/// 
/// When an AudioSource enters the collider,
/// it is added to a list of AudioSources.
/// 
/// Only the player's noise is calculated
/// </summary>

public class SoundDetection : MonoBehaviour {

	/// <summary>
	/// The minimum detection distance is the minium distance
	/// at which detection is gauranteed
	/// </summary>
	public float minDetectionDistance;

	[HideInInspector]
	public float detection;

	public Transform player;
	AudioSource footsteps;
	PlayerControl playerInput;

	Transform collTransform, myTransform;

	/// <summary>
	/// The spammer's brain.
	/// </summary>
	public SpammerFSM brain;

	Dictionary<Transform, AudioSource> audibles = new Dictionary<Transform, AudioSource>();

	/// <summary>
	/// The player's noise.
	/// </summary>
	float playerNoise;

	/// <summary>
	/// The detection speed.
	/// </summary>
	public float detectionIncrement;

	/// <summary>
	/// How quickly does the detection decay.
	/// </summary>
	public float detectionDecay;

	/// <summary>
	/// The time after which the detection should decay if not alerted
	/// </summary>
	public float decayInterval;

	float decayTime;

	/// <summary>
	/// The treshold for a possible detection.
	/// </summary>
	public float possibleDetection = 50f;

	/// <summary>
	/// The treshold for a confirmed detection
	/// </summary>
	const float confirmedDetection = 100f;

	string decoyTag;

	public float rate = 0.2f;
	WaitForSeconds processInterval;

	public bool alerted = false;
	bool isProcessing = false;

	/// <summary>
	/// The noises made by a spammer when dying or taking damage.
	/// </summary>
	List<AudioClip> spammerDamageNoises;

	public DeathSounds sounds;

	WaitForSeconds decrementDelay;

	float difficulty = 1f;

	Collider myCollider;
	Renderer myRenderer;

	void Awake()
	{
		myTransform = transform;
		spammerDamageNoises = sounds.GetSounds();

		// ignore collisions with other sound shapes
		Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);

		footsteps = player.GetComponent<AudioSource>();
		myCollider = GetComponent<Collider>();
		myRenderer = GetComponent<Renderer>();
		playerInput = player.GetComponent<PlayerControl>();
		decoyTag = GameTagManager.decoyTag;
		decrementDelay = new WaitForSeconds(decayInterval);
		processInterval = new WaitForSeconds(rate);
		difficulty = GameTagManager.GetDifficultyMultiplier();
	}

	void OnTriggerEnter(Collider coll)
	{
		collTransform = coll.gameObject.transform;
		AudioSource sound = collTransform.GetComponent<AudioSource>();

		if (sound != null)
		{

			// if the sound is actually a target sound
			if (collTransform.CompareTag(decoyTag) || spammerDamageNoises.Contains(sound.clip) )
			{

				// then alert the brain if the sound is playing
				if (sound.isPlaying)
				{
					alerted = true;
					brain.SetAlert(collTransform, SpammerFSM.DetectionStatus.possible, SpammerFSM.AlertType.auditory);
				}
			}
			else if (collTransform == player)
			{
				CalculatePlayerNoise();
			}

			if (!audibles.ContainsKey(collTransform))
			{
				audibles.Add(collTransform, sound);
				if (!isProcessing)
				{
					StartCoroutine(ProcessColliders());
					isProcessing = true;
				}
			}
		}

	}

	IEnumerator ProcessColliders()
	{
        while (audibles.Count > 0)
        {
            foreach (KeyValuePair<Transform, AudioSource> pair in audibles)
            {
                var collTrans = pair.Key;
                var sound = audibles[collTrans];

                // if a decoy or an injured /dying spammer is playing, alert the brain
                // otherwise, check if the player is noisy enough
                if (collTrans.CompareTag(decoyTag) || spammerDamageNoises.Contains(sound.clip) )
                {
                    if (sound.isPlaying)
                    {
                        alerted = true;
                        brain.SetAlert(collTrans, SpammerFSM.DetectionStatus.possible, SpammerFSM.AlertType.auditory);
                    }
                }
                else if (collTrans.CompareTag(GameTagManager.playerTag))
                {
                    CalculatePlayerNoise();
                }
            }
            yield return processInterval;
        }

	}

	void OnTriggerExit(Collider coll)
	{
		if (audibles.ContainsKey(coll.gameObject.transform) )
		{
			if (coll.transform == player)
			{
				StartCoroutine(ReduceDetection() );
			}
			audibles.Remove(coll.gameObject.transform);
			if (audibles.Count <= 0)
			{
				isProcessing = false;
				StopCoroutine(ProcessColliders());
			}
		}

	}

	/// <summary>
	/// Sets the detection level.
	/// </summary>
	/// <param name="newDetection">New detection.</param>
	public void SetDetection(float newDetection)
	{
		detection = newDetection;
	}

	/// <summary>
	/// Calculates the player noise.
	/// </summary>
	void CalculatePlayerNoise()
	{

		// this should be inversely proportional to the square of the distance,
		// and proportional to the speed of the player.
		float playerSpeed = playerInput.currentSpeed;
		float distance = (myTransform.position - player.position).sqrMagnitude;
		float tempNoise = ( (footsteps.volume + playerSpeed) / distance );

		// if the player is close enough, detect them immediately
		if (distance < minDetectionDistance)
		{
			brain.SetAlert(player, SpammerFSM.DetectionStatus.confirmed, SpammerFSM.AlertType.auditory);
		}
		else
		{
			// if alerted to something else, ignore the player for now
			if (!alerted)
			{

				// if the sound has increased, increase the detection
				// otherwise, decrease it
				if (tempNoise > playerNoise && footsteps.isPlaying)
				{
					if (detection < 100)
					{
						detection += (detectionIncrement * difficulty);
					}
				}
				else
				{
					decayTime += Time.deltaTime;
					if (decayTime >= decayInterval)
					{
						decayTime = 0f;
						if(detection > 0)
						{
							detection -= detectionDecay;
						}
					}
				}

				if (detection >= possibleDetection)
				{
					if (detection < confirmedDetection)
					{
						brain.SetAlert(player, SpammerFSM.DetectionStatus.possible, SpammerFSM.AlertType.auditory);
					}
					else
					{
						brain.SetAlert(player, SpammerFSM.DetectionStatus.confirmed, SpammerFSM.AlertType.auditory);
					}
				}
			}
		}

		playerNoise = tempNoise;
	}

	public void ResetAlert()
	{
		alerted = false;
		SetDetection(0f);
		StopCoroutine(ReduceDetection() );
	}

	/// <summary>
	/// Reduces the detection.
	/// </summary>
	/// <returns>The detection.</returns>
	IEnumerator ReduceDetection()
	{
		while (detection > 0)
		{
			detection -= detectionDecay;
			yield return decrementDelay;
		}

		brain.SetAlert(player, SpammerFSM.DetectionStatus.unaware, SpammerFSM.AlertType.auditory);
		ResetAlert();
	}

	/// <summary>
	/// Deactivate this set of ears. Used mainly when dead.
	/// </summary>
	public void Deactivate()
	{
		myCollider.enabled = false;
		myRenderer.enabled = false;
		if (brain.isDead)
		{
			ResetAlert();
		}
	}

	/// <summary>
	/// Activate this set of ears.
	/// </summary>
	public void Activate()
	{
		myCollider.enabled = true;
		myRenderer.enabled = true;
	}

	public void ToggleRenderer(bool show)
	{
		myRenderer.enabled = show;
	}

}