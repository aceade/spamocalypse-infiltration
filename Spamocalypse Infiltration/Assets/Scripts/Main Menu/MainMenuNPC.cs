using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls an NPC on the main menu.
/// </summary>

public class MainMenuNPC : MonoBehaviour {

	public MoveAgent movement;

	public List<Transform> patrolPoints;

	public float stepChangeDistance = 2f;

	int index;

	Transform myTransform;

	Vector3 myPosition;

	public Animator myAnimator;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		movement.SetMoveTarget (patrolPoints [0].position);
		myAnimator.SetBool("IsMoving", true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		myPosition = myTransform.position;
		if (Vector3.Distance (myPosition, patrolPoints [index].position) < stepChangeDistance) 
		{
			SetNextPatrolPoint();
		}
	}

	void SetNextPatrolPoint()
	{
		index++;
		if (index >= patrolPoints.Count ) 
		{
			index = 0;
		}
		movement.SetMoveTarget (patrolPoints [index].position);
	}
}
