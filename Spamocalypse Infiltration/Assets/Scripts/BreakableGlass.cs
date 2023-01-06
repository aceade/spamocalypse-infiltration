using UnityEngine;

public class BreakableGlass : MonoBehaviour {

	AudioSource myAudio;
	Collider myCollider;
	Rigidbody myBody;
	Animator myAnimator;

	public Collider playerKnife;

	public float breakVelocity = 10f;

	// Use this for initialization
	void Start () 
	{
		myAudio = GetComponent<AudioSource>();
		myCollider = GetComponent<Collider>();
		myBody = GetComponentInParent<Rigidbody>();
		myBody.sleepThreshold = 0.2f;
		myAnimator = GetComponent<Animator>();

		if (playerKnife == null)
		{
			playerKnife = GameObject.Find("Cutters").GetComponent<Collider>();
		}
	}
	
	public void BreakGlass(bool silently, Vector3 direction)
	{
		
		GameTagManager.LogMessage("Glass at {0} is breaking ({1}). Pieces will go in direction {2}", transform.position, 
			silently ? "silently" : "loudly", direction);

		myAnimator.enabled = true;
		myCollider.enabled = false;

		if (!silently)
		{
			myAudio.Play();
		}
	}

	void OnCollisionEnter(Collision coll)
	{
		GameTagManager.LogMessage("Glass at {0} hit by {1} with speed {2}", transform.position, coll.collider, coll.relativeVelocity.magnitude);
		if (coll.collider == playerKnife)
		{
			BreakGlass(true, Vector3.down);
		}
		else
		{
			if (coll.relativeVelocity.magnitude > breakVelocity)
			{
				BreakGlass (false, coll.relativeVelocity);
			}
		}
	}
}
