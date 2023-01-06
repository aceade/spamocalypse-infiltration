using UnityEngine;
using System.Collections;

public class HitscanWeapon : Weapon {

	public float radius;

	public float spread;

	RaycastHit hit;

	public override IEnumerator PrimaryAttack (Transform attackTarget)
	{
		if (canAttack)
		{
			
			canAttack = false;
			Vector3 fireDir = CalculateFireDir(attackTarget.position, myTrans.position);
			GameTagManager.LogMessage("Hitscan weapon {0} is attacking {1}. Fire direction is {2}", name, attackTarget, fireDir);
			CastSphere(fireDir);
			yield return primaryAttackCycle;
			canAttack = true;
		}
	}

	void CastSphere(Vector3 direction)
	{
		if (Physics.SphereCast(myTrans.position, radius, direction, out hit))
		{
			CheckHit(hit.transform);
		}
	}

	/// <summary>
	/// Calculates the spread in the firing direction.
	/// </summary>
	/// <returns>The fire dir.</returns>
	/// <param name="end">End position.</param>
	/// <param name="start">Start position.</param>
	Vector3 CalculateFireDir(Vector3 end, Vector3 start)
	{
		return ((end - start) + (Vector3.up * Random.Range(-spread, spread)) +
		        (Vector3.right * Random.Range(-spread, spread)) );
	}


}
