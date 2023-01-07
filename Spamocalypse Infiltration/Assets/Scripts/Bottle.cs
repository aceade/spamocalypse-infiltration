using UnityEngine;

/// <summary>
/// A bottle that can be kicked or knocked over.
/// </summary>
public class Bottle : MonoBehaviour {

    Rigidbody myBody;
    AudioSource myAudio;

    public AudioClip bottleNoise;

    public float minVelocity = 2f;

	// Use this for initialization
	void Start () {
	    myBody = GetComponent<Rigidbody>();
        myAudio = GetComponentInChildren<AudioSource>();
        myAudio.loop = false;
        myAudio.clip = bottleNoise;
	}

    void OnCollisionEnter(Collision coll)
    {
        GameTagManager.LogMessage("{0} hit by object {1} at speed {2}", this, coll.gameObject, coll.relativeVelocity.magnitude);
        if (coll.relativeVelocity.magnitude >= minVelocity)
        {
            myAudio.Play();
            myBody.AddForce(coll.relativeVelocity, ForceMode.Impulse);
        }
    }

    public override string ToString ()
    {
        return string.Format ("[Bottle] at {0}", transform.position);
    }
}
