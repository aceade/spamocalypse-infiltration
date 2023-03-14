using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class LogicBomb : Projectile {

	public float bombRadius = 10f;

	public int logicDamage = 10;

	public float launchSpeed = 20f;

	protected new void Start()
	{
		base.Start();
	}
	
	protected override void PlayHitEffects (Collider coll)
	{
		// get all the colliders within range, and damage them if they are the player or a spammer
		Collider[] colliders = Physics.OverlapSphere(transform.position, bombRadius);
		foreach (Collider collidee in colliders)
		{
			var collTrans = collidee.transform.root;
			var damageScript = collTrans.GetComponent<IDamage>();
			if (damageScript != null)
			{
				damageScript.Damage(GameTagManager.AttackMode.logic, logicDamage, collTrans.position - transform.position);
			}
		}
        base.PlayHitEffects(coll);
	}
}
