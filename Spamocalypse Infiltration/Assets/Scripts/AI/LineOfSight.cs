using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the line of sight checks for the spammers.
/// Attached to an invisible cone/box in front of their head,
/// and causes the detection certainty to increment 
/// </summary>

public class LineOfSight : MonoBehaviour {

	/// <summary>
	/// The player's transform
	/// </summary>
	public Transform player;

	/// <summary>
	/// The speed at which the spammer becomes certain this is the player;
	/// if playerDetection is 100, the player is detected
	/// </summary>
	public float incrementDetection;
	[HideInInspector]
	public float playerDetection;

	/// <summary>
	/// The speed at which detection decreases.
	/// </summary>
	public float decrementDetection;

	/// <summary>
	/// Beyond what point should they start searching
	/// </summary>
	public float suspectPercent;

	/// <summary>
	/// The delay for the detection coroutine
	/// </summary>
	public float detectionRate = 0.25f;
	WaitForSeconds detectionPeriod;

	/// <summary>
	/// If losing sight of the player, wait this long before
	/// decreasing the detection
	/// </summary>
	public float delayWithoutLos = 2f;

	public float minLightSensitivity;

	float difficulty = 1f;
    
	/// <summary>
	/// A RaycastHit to check line of sight
	/// </summary>
	RaycastHit hit;

	/// <summary>
	/// A list of other spammers for which the spammer has a line of sight.
	/// </summary>
	Dictionary<Transform, Collider> visibleObjects = new Dictionary<Transform, Collider>();

	Transform myTransform;
	Collider myCollider;
	Renderer myRenderer;

	public SpammerFSM brain;
	
	string parentTag;

	Firewall currentFirewall;
	WaitForFixedUpdate firewallProcessTime = new WaitForFixedUpdate();
	delegate void FirewallMethod(Firewall wall);
	FirewallMethod processFirewall;

	bool isProcessing;

	int spammerLayer, corpseLayer;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		detectionPeriod = new WaitForSeconds(detectionRate);
		parentTag = transform.root.tag;

		// ignore collisions with other IgnoreRaycast objects
		Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);

		myCollider = GetComponent<Collider>();
		myRenderer = GetComponent<Renderer>();

		difficulty = GameTagManager.GetDifficultyMultiplier();

		spammerLayer = LayerMask.NameToLayer("Spammer");
		corpseLayer = LayerMask.NameToLayer ("Dead Spammer");

		if (parentTag == GameTagManager.adminTag)
		{
			processFirewall = AdminProcessFirewall;
		}
		else if (parentTag == GameTagManager.spammerTag)
		{
			processFirewall = RegularProcessFirewall;
		}
	}

	/// <summary>
	/// Processes the transforms that it can see.
	/// </summary>
	/// <returns>The transforms.</returns>
	IEnumerator ProcessTransforms()
	{
		while (visibleObjects.Count > 0)
		{
			foreach (KeyValuePair<Transform, Collider> pair in visibleObjects)
			{
				if (pair.Key == null || pair.Value == null || myTransform == null)
				{
					continue;
				}

                Transform trans = pair.Key;
                int layer = trans.gameObject.layer;

                if (layer == corpseLayer || trans == player)
                {
                    if (Physics.Linecast(myTransform.position, pair.Value.bounds.center, out hit) && hit.transform == trans)
                    {
                        var lightCalc = trans.GetComponent<CalculateLight>();
                        float illumination = lightCalc.GetIllumination();
                        if (illumination > minLightSensitivity)
                        {
                            if (trans == player)
                            {
                                DetectPlayer(illumination);
                            }
                            else
                            {
                                if (Vector3.Distance(myTransform.position, trans.position) < 5f)
                                {
                                    brain.SetAlert(trans.position, SpammerFSM.DetectionStatus.confirmed, SpammerFSM.AlertType.other);
                                }
                            }
                        }

                    }
                }
				

			}

			yield return detectionPeriod;
		}
	}

	/// <summary>
	/// Detects the player. If the light intensity is high enough, detection increases.
	/// Otherwise, it decreases.
	/// </summary>
	/// <param name="lightIntensity">Light intensity.</param>
	void DetectPlayer(float lightIntensity)
	{
		if (lightIntensity >= minLightSensitivity)
		{
			GameTagManager.LogMessage("Player is being tracked by {0}. Detection is {1}%", brain.name, playerDetection);

			if (playerDetection < 100 && Time.deltaTime > 0f)
			{
				playerDetection += (incrementDetection * difficulty);
			}
		}
		else
		{
			if (playerDetection > 0)
			{
				playerDetection -= decrementDetection;
			} 
		}

		// if the suspicion threshold is reached, start searching
		if (playerDetection > suspectPercent && playerDetection < 100)
		{
			brain.SetAlert(player, SpammerFSM.DetectionStatus.possible, SpammerFSM.AlertType.visual);
		}

		// if they know it's the player, sound an alert
		if (playerDetection >= 100)
		{
			brain.SetAlert(player, SpammerFSM.DetectionStatus.confirmed, SpammerFSM.AlertType.visual);
		}
	}

	/// <summary>
	/// Raises the trigger enter event. If the collider is not the player,
	/// an allied spammer or a corpse, it is ignored.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.layer == spammerLayer || coll.gameObject.layer == corpseLayer || coll.transform.root == player)
		{
			if (!visibleObjects.ContainsKey(coll.transform.root))
			{
				GameTagManager.LogMessage("{0} is now tracking {1}", brain.name, coll.transform.root);
				visibleObjects.Add(coll.transform.root, coll);

				if (!isProcessing)
				{
					isProcessing = true;
					StartCoroutine(ProcessTransforms());
				}

			}

		}
	}

	/// <summary>
	/// Sets the detection level. Mainly used to reset things.
	/// </summary>
	/// <param name="newDetectionLevel">New detection level.</param>
	public void SetDetection(float newDetectionLevel)
	{
		playerDetection = newDetectionLevel;
	}

	/// <summary>
	/// Raises the trigger exit event. If the object counts as visible, it is removed, and if none are left,
	/// the Coroutine is stopped.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerExit(Collider coll)
	{
		if (visibleObjects.ContainsKey(coll.gameObject.transform))
		{
			visibleObjects.Remove(coll.gameObject.transform);
			if (coll.gameObject.transform == player)
			{
				StartCoroutine(DecreasePlayerDetection() );
			}

			if (visibleObjects.Count < 1)
			{
				StopCoroutine(ProcessTransforms());
				isProcessing = false;
			}
		}
	}

	/// <summary>
	/// Tracks a firewall. Admins and spammers will alert if they see it active, bots will ignore it.
	/// </summary>
	/// <returns>The firewall.</returns>
	/// <param name="wallToTrack">Wall to track.</param>
	IEnumerator TrackFirewall(Firewall wallToTrack)
	{
		if (Physics.Linecast(myTransform.position, wallToTrack.transform.position))
		{
			if(wallToTrack.active)
			{
				processFirewall(wallToTrack);
			}
		}
		yield return firewallProcessTime;
	}

	/// <summary>
	/// Resets the alert status to zero.
	/// </summary>
	public void ResetAlert()
	{
		SetDetection(0);
	}

	/// <summary>
	/// Decreases the detection level when the player exits the collider.
	/// </summary>
	/// <returns>The detection.</returns>
	IEnumerator DecreasePlayerDetection()
	{
		while(playerDetection > 0)
		{
			playerDetection -= incrementDetection;

			yield return detectionPeriod;
		}

		yield return new WaitForSeconds(delayWithoutLos);
		brain.SetAlert(brain.transform, SpammerFSM.DetectionStatus.unaware, SpammerFSM.AlertType.visual);
		StopCoroutine(DecreasePlayerDetection());
	}

	/// <summary>
	/// How regular spammers process firewalls
	/// </summary>
	/// <param name="theWall">The wall.</param>
	void RegularProcessFirewall(Firewall theWall)
	{

	}

	/// <summary>
	/// How admins process firewalls
	/// </summary>
	/// <param name="theWall">The wall.</param>
	void AdminProcessFirewall(Firewall theWall)
	{

	}

	/// <summary>
	/// Deactivate this instance. Used mainly when dead.
	/// </summary>
	public void Deactivate()
	{
		myCollider.enabled = false;
		myRenderer.enabled = false;
		isProcessing = false;
		StopCoroutine(ProcessTransforms());
		if (brain.isDead)
		{
			ResetAlert();
		}

	}

	/// <summary>
	/// Activate this set of eyes
	/// </summary>
	public void Activate()
	{
		myCollider.enabled = true;
		myRenderer.enabled = true;
	}

	/// <summary>
	/// Toggles the renderer. Used for debugging.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show the renderer.</param>
	public void ToggleRenderer(bool show)
	{
		myRenderer.enabled = show;
	}
}
