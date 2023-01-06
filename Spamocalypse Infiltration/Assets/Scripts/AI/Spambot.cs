using UnityEngine;
using System.Collections;

/// <summary>
/// Spambots are a spin off from the main class.
/// </summary>
public class Spambot : SpammerFSM {


	protected override void MySearch ()
	{
		alertTime += Time.deltaTime;
		if (searchingForPuppet)
		{

			// if the current target is a sockpuppet,
			// then they move towards it
			if (Vector3.Distance(lastPlayerPosition, myPosition) < optimalAttackRange)
			{

				// and then just stare at it until it's finished
				RotateTorwards(lastPlayerPosition);
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
				if (alertTime > distractionTime)
				{
					// if they've met this puppet before, don't increment the false alerts
					if (prevTarget != target)
					{
						falseAlerts++;
					}
					SetAlert(myTransform, DetectionStatus.unaware, AlertType.other);
					myAudio.PlaySockpuppetSound();
				}
			}
		}
		else
		{
			if (Vector3.Distance(myPosition, lastPlayerPosition) < stepChangeDistance)
			{
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
			}
			// if the current target is the player:
			// move towards the last known position until they get bored
			if (alertTime > maxAlertTime)
			{
				SetAlert(myTransform, DetectionStatus.unaware, AlertType.other);
			}
		}
	}

	protected override void MyAttack ()
	{
		if (Vector3.Distance(myPosition, target.position) < maxAttackRange)
		{
			if (myWeapon.IsAbleToAttack() && Vector3.Angle(myTransform.position, target.position) < maxAttackAngle )
			{
				myAnimator.SetBool ("IsAttacking", true);
                effects.PlayAttackEffects();
				myWeapon.StartAttack(myWeapon.transform.position, myWeapon.transform.forward);
			}
			if (Vector3.Distance(myTransform.position, target.position) < optimalAttackRange)
			{
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
			}
		}
		else
		{
			if (movement.currentMovement == MoveAgent.MoveType.stopped)
			{
                movement.SetMoveTarget(target.position);
				movement.ChangeMovement(MoveAgent.MoveType.attacking);
			}
		}
	}
}
