using UnityEngine;

public class SwingDoor : MonoBehaviour {

	Rigidbody myBody;

	public float force = 5f;

	// Use this for initialization
	void Start () 
	{
		myBody = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter(Collision coll)
	{
		var collObject = coll.gameObject;
		GameTagManager.LogMessage("Swing door {0} hit by {1}", this, collObject);
		if (collObject.CompareTag(GameTagManager.playerTag) || collObject.CompareTag(GameTagManager.spammerTag) ||
			collObject.CompareTag(GameTagManager.adminTag) || collObject.CompareTag(GameTagManager.botTag))
		{
			var dir = transform.position - collObject.transform.position;
			GameTagManager.LogMessage("Swing door {0} applying force in direction {1}", this, dir);
			myBody.AddForce(dir * force, ForceMode.Impulse);
		}
	}
}
