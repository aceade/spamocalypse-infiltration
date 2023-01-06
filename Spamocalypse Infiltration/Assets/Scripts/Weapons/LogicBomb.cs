using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class LogicBomb : Projectile {

	public float bombRadius = 10f;

	public int logicDamage = 10;

	public float launchSpeed = 20f;

	int playerLayer, spammerLayer;

	protected new void Start()
	{
		playerLayer = LayerMask.NameToLayer("Player");
		spammerLayer = LayerMask.NameToLayer("Spammer");
		base.Start();
	}
	
	protected override void PlayHitEffects (Collider coll)
	{
		// get all the colliders within range, and damage them if they are the player or a spammer
		Collider[] colliders = Physics.OverlapSphere(transform.position, bombRadius);
		foreach (Collider collidee in colliders)
		{
			if (collidee.gameObject.layer == playerLayer)
			{
				collidee.transform.root.GetComponent<PlayerControl>().DamagePlayer(logicDamage, GameTagManager.AttackMode.logic);
			}
			else if (collidee.gameObject.layer == spammerLayer)
			{
                // TODO: stop this happeneing multiple times
				collidee.transform.root.GetComponent<SpammerFSM>().ChangeStateTo(SpammerFSM.SpammerState.bamboozled);
			}
		}
        base.PlayHitEffects(coll);
	}
}
