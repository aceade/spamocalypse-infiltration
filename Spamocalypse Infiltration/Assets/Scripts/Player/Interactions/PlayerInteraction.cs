using UnityEngine;

/// <summary>
/// A general class for player-specific interactions with objects.
/// </summary>

[RequireComponent(typeof(Collider))]
public abstract class PlayerInteraction : MonoBehaviour {

	/// <summary>
	/// The player's camera.
	/// </summary>
	protected Camera player;

	protected bool isPlayerLooking = false;

	/// <summary>
	/// The visible object is a child of this trigger.
	/// </summary>
	protected MeshRenderer visibleObject;

	/// <summary>
	/// A RaycastHit is used to check that the player is looking.
	/// </summary>
	protected RaycastHit hit;

	protected Transform myTransform;

	protected Collider myColl;

	// Use this for initialization
	protected virtual void Start () 
	{
        myTransform = transform;
		if (player == null)
		{
			player = Camera.main;
		}

		myColl = GetComponent<Collider>();
		if (!myColl.isTrigger)
		{
			myColl.isTrigger = true;
		}
        
		visibleObject = GetComponentInChildren<MeshRenderer>();
	}

	/// <summary>
	/// While the player is inside the trigger area, cast a ray from the main camera
	/// to the actual object to check if they are looking at it.
	/// </summary>
	/// <param name="coll">Coll.</param>
	protected virtual void OnTriggerStay (Collider coll)
	{
		if (coll.CompareTag (GameTagManager.playerTag))
		{
			Vector3 playerDir = player.transform.forward;

			if (Physics.Raycast (player.transform.position, playerDir, out hit))
			{
				if (hit.transform.IsChildOf (myTransform) && !isPlayerLooking)
				{
					isPlayerLooking = true;
					Highlight ();
				} 
				else if (!hit.transform.IsChildOf (myTransform))
				{
					isPlayerLooking = false;
					StopHighlight ();
				}
			}

			if (isPlayerLooking && Input.GetButtonDown ("Use"))
			{
				Interact ();
			}
		}
	}

	protected virtual void OnTriggerExit(Collider coll)
	{
		if (coll.CompareTag(GameTagManager.playerTag))
		{
			StopHighlight();
		}

	}

	/// <summary>
	/// Highlight the object to let the player know they are lookng at it.
	/// </summary>
	protected virtual void Highlight()
	{
        if (visibleObject != null)
        {
            visibleObject.material.EnableKeyword("_EMISSION");
            visibleObject.material.SetColor ("_EmissionColor", Color.white * 0.2f);
        }
        else
        {
            Debug.LogErrorFormat("The PlayerInteraction {0} in scene {1} does not have a Renderer!", this, 
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

	}

	/// <summary>
	/// Stop highlighting the object.
	/// </summary>
	protected virtual void StopHighlight()
	{
        if (visibleObject != null)
        {
            visibleObject.material.DisableKeyword("_EMISSION");
            visibleObject.material.SetColor("_EmissionColor", Color.clear);
        }

	}

	protected abstract void Interact();

	protected void FreezePlayer()
	{
		player.GetComponentInParent<PlayerControl>().enabled = false;
	}

	protected void UnfreezePlayer()
	{
		player.GetComponentInParent<PlayerControl>().enabled = true;
	}

}
