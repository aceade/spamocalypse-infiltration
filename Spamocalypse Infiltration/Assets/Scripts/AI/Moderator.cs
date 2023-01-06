using UnityEngine;
using System.Collections;

public class Moderator : SpammerFSM {

	protected override void MyAttack ()
	{
		float distance = Vector3.Distance(myPosition, target.position);
		if (distance < maxAttackRange)
		{
			if (myWeapon.IsAbleToAttack() && Vector3.Angle(myTransform.position, target.position) < maxAttackAngle )
			{
				myAnimator.SetBool ("IsAttacking", true);
				myAnimator.SetBool("IsSearching", false);
				myWeapon.StartAttack(target);
			}
		} 

		if (distance < optimalAttackRange)
		{
			movement.ChangeMovement(MoveAgent.MoveType.stopped);
		}
		else 
		{
			if (movement.currentMovement == MoveAgent.MoveType.stopped)
			{
                movement.SetMoveTarget(target.position);
				myAnimator.SetBool("IsAttacking", false);
				myAnimator.SetBool("IsSearching", true);
				movement.ChangeMovement(MoveAgent.MoveType.attacking);
			}

		}
	}

	protected override void MySearch ()
	{
		// if it's a sockpuppet, check if the maxAlerts has been reached
		// if so, trace the launch position immediately; if not, check later
		if (searchingForPuppet)
		{
			if (Vector3.Distance(lastPlayerPosition, myPosition) < optimalAttackRange)
			{
				alertTime += Time.deltaTime;
				// and then just stare at it until they realise it's a fake
				RotateTorwards(lastPlayerPosition);
				movement.ChangeMovement(MoveAgent.MoveType.stopped);
				if (alertTime > distractionTime)
				{
					myAudio.PlaySockpuppetSound();
					alertTime = 0f;
					if (prevTarget != target)
					{
						falseAlerts++;
					}
					searchingForPuppet = false;
					lastPlayerPosition = target.GetComponent<Sockpuppet>().launchPosition;
					lastPlayerPosition.y = myPosition.y;
					SetDestination(lastPlayerPosition);
				}
			}

		}
		else
		{
			alertTime += Time.deltaTime;
			if (Vector3.Distance(myPosition, lastPlayerPosition) < stepChangeDistance)
			{
				// find a new path
				myAudio.PlaySearchSound();

				if (!firstTimeSearch)
				{
					Physics.Raycast(myPosition, myPosition + myTransform.forward * Random.Range(-5f, 20f) + 
					                (myTransform.right * Random.Range(-20f, 20f)), out hit);
					lastPlayerPosition = hit.point;
					//				lastPosition.y = myPosition.y;
//					GameManager.LogMessage("{0} is at {1} and will search near {2}", name, myPosition, lastPlayerPosition);
					SetDestination(lastPlayerPosition);
				}
				else
				{
					if (alertTime > distractionTime)
					{
						firstTimeSearch = false;
					}
				}
			}

			if (alertTime >= maxAlertTime)
			{
				SetAlert(myTransform, DetectionStatus.unaware, AlertType.other);
			}
		}
	}
}
