using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileWeapon : Weapon {

	public AmmoPool ammoPool;

	public int maxAmmo;

	public int currentAmmo;

	public bool infiniteAmmo = false;

	public float spread = 0.05f;

	public Transform muzzle;

	/// <summary>
	/// Attack from the specified position and in the specified direction.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="direction">Direction.</param>
	public override IEnumerator PrimaryAttack (Vector3 position, Vector3 direction)
	{
		if (canAttack)
		{
            myAudio.Play();
			canAttack = false;
			Projectile currentProjectile = ammoPool.GetProjectile();
			direction.x += Random.Range(-spread, spread);
			direction.y += Random.Range(-spread, spread);
			direction.z += Random.Range(-spread, spread);
			currentProjectile.Launch(muzzle.position, direction);
			yield return primaryAttackCycle;
			canAttack = true;
		}
	}
}
