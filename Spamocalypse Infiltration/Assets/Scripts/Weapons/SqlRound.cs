using UnityEngine;
using System.Collections;

public class SqlRound : Projectile {

	public int damage = 30;

	int spammerLayer, playerLayer;

	// Use this for initialization
	protected new void Start () 
	{
		playerLayer = LayerMask.NameToLayer("Player");
		spammerLayer = LayerMask.NameToLayer("Spammer");
		base.Start();
	}

	/// <summary>
	/// Plays the hit effects. Damages the player or a spammer.
	/// </summary>
	/// <returns>The hit effects.</returns>
	/// <param name="coll">Coll.</param>
	protected override void PlayHitEffects(Collider coll)
	{
        GameTagManager.LogMessage("{0} hit {1}", this, coll);
		Transform collTrans = coll.transform.root;
		if (coll.gameObject.layer == playerLayer)
		{
			collTrans.GetComponent<PlayerControl>().DamagePlayer(damage, GameTagManager.AttackMode.sql);
            myAudio.clip = hitNoises[1];
		}
		else if (coll.gameObject.layer == spammerLayer)
		{
			collTrans.GetComponent<SpammerFSM>().DamageSpammer(GameTagManager.AttackMode.sql, damage, 
                    collTrans.position - transform.position);
            myAudio.clip = hitNoises[1];
		}
        else 
        {
            myAudio.clip = hitNoises[0];
        }
        myAudio.Play();
		Sleep();
	}
}
