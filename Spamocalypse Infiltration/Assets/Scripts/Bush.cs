using UnityEngine;

/// <summary>
/// Bushes rustle when you walk into them at high speed.
/// </summary>
public class Bush : MonoBehaviour {

    AudioSource myAudio;

    public AudioClip soundToPlay;

	// Use this for initialization
	void Start () {
	    myAudio = GetComponent<AudioSource>();
        myAudio.clip = soundToPlay;
	}
	
    void OnTriggerEnter(Collider coll)
    {
        if(!coll.isTrigger)
        {
            ProcessCollider(coll);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if(!coll.isTrigger)
        {
            ProcessCollider(coll);
        }
    }

    void ProcessCollider(Collider collidee)
    {
        bool startRustling = false;
        if (collidee.CompareTag(GameTagManager.playerTag))
        {
            var player = collidee.transform.root.GetComponent<PlayerControl>();
            GameTagManager.LogMessage("{0} processing player. Speed is {1}", this, player.currentSpeed);
            if (player.currentSpeed > player.sneakSpeed)
            {
                startRustling = true;
            }
        }
        if (startRustling)
        {
            myAudio.Play();
        }
    }

    public override string ToString ()
    {
        return string.Format ("[Bush] at {0}", transform.position);
    }
}
