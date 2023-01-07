using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MeleeWeapon : Weapon {
	
	public AnimationClip clip;

	// Use this for initialization
	protected override void Start () 
	{
		if (clip != null)
		{
			fireRate = clip.length;
		}
		else
		{
			fireRate = 2f;
		}
		base.Start();
	}

	public override IEnumerator PrimaryAttack (Transform attackTarget)
	{
		if (canAttack)
		{
//			GameManager.LogMessage("Melee weapon {0} is attacking {1}", name, attackTarget);
			PerformAttack();
			yield return primaryAttackCycle;
			canAttack = true;
			myAnimator.SetBool("Melee_Attacking", false);
		}
	}

	public override IEnumerator PrimaryAttack (Vector3 position, Vector3 direction)
	{
		if (canAttack)
		{
//			GameManager.LogMessage("Melee weapon {0} is attacking from position {1} in direction {2}", name, position, direction);
			PerformAttack();
			yield return primaryAttackCycle;
			canAttack = true;
			myAnimator.SetBool("Melee_Attacking", false);
		}
	}

	void PerformAttack()
	{
		canAttack = false;
		myCollider.enabled = true;
		myAnimator.SetBool("Melee_Attacking", true);
	}

	void OnTriggerEnter(Collider coll)
	{	
		if (!coll.isTrigger && !coll.CompareTag("Projectile"))
		{
			myCollider.enabled = false;
			CheckHit(coll.transform);
			myAudio.Play();
		}
	}

	public void IgnoreMyColliders(Collider colliderToIgnore)
	{
		Physics.IgnoreCollision(myCollider, colliderToIgnore);
	}
}
