
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LiftPlatform : MonoBehaviour {

	Transform myTransform;
	AudioSource myAudio;
	WaitForSeconds frame;

	const float rate = 0.05f;

	public float speed = 10f;

	List<Transform> contacts = new List<Transform>();

	void Start()
	{
		myTransform = transform;
		myAudio = GetComponent<AudioSource>();
		frame = new WaitForSeconds(rate);
	}

	public IEnumerator MoveTo(Transform target)
	{
		Vector3 direction = target.position - myTransform.position;
		myAudio.Play();
		while (Vector3.Distance(target.position, myTransform.position) >= 0.1f)
		{
			myTransform.Translate(direction * speed * Time.smoothDeltaTime * rate);
			for (int i = 0; i < contacts.Count; i++)
			{
				contacts[i].Translate(direction * speed * Time.smoothDeltaTime * rate);
			}
			yield return frame;
		}
		myAudio.Stop();
	}

	/// <summary>
	/// When the player collides with the platform, they are made a child of the platform.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag(GameTagManager.playerTag))
		{
            var root = coll.transform.root;
            root.GetComponent<Rigidbody>().useGravity = false;
            contacts.Add(root);
		}
	}

	/// <summary>
	/// When the player leaves the collision, they are no longer a child of the platform.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerExit(Collider coll)
	{
		if (coll.CompareTag(GameTagManager.playerTag))
		{
            var root = coll.transform.root;
            root.GetComponent<Rigidbody>().useGravity = true;
            contacts.Remove(root);
		}
	}
}
