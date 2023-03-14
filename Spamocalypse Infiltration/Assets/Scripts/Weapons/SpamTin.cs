using UnityEngine;
using System.Collections;

public class SpamTin : Projectile {

	int spammerLayer, playerLayer;

	public int damage = 20;

	// Use this for initialization
	protected new void Start () {
		playerLayer = LayerMask.NameToLayer("Player");
		spammerLayer = LayerMask.NameToLayer("Spammer");
		base.Start();
	}

	/// <summary>
	/// Plays the hit effects. In this case, it damages the target, if they are organic.
	/// </summary>
	/// <returns>The hit effects.</returns>
	/// <param name="coll">Coll.</param>
	protected override void PlayHitEffects(Collider coll)
	{
		Transform collTrans = coll.transform.root;
		var damageScript = collTrans.GetComponent<IDamage>();
		if (damageScript != null)
		{
			damageScript.Damage(GameTagManager.AttackMode.spam, damage, collTrans.position - transform.position);
		}
        base.PlayHitEffects(coll);
	}
}
