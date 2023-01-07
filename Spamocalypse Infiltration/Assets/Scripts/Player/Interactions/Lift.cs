using UnityEngine;
using System.Collections;

public class Lift : PlayerInteraction {

//	public Transform platform;
	public Transform upPoint, downPoint;
	bool isAscending;

	public LiftPlatform platform;

	AudioSource myAudio;

	Animator myAnimator;

    public Collider buttonColl;

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		visibleObject = buttonColl.transform.GetComponent<MeshRenderer>();
		GameTagManager.LogMessage("The visible object for the lift controls {0} is {1}", this, visibleObject);
		myAudio = GetComponent<AudioSource>();
		myAnimator = GetComponent<Animator>();

		// find which point is closer at the start
		float upDistance = Vector3.Distance(platform.transform.position, upPoint.position);
		float downDistance = Vector3.Distance(platform.transform.position, downPoint.position);
		if (upDistance >= downDistance)
		{
			isAscending = false;
		}
		else
		{
			isAscending = true;
		}
	}
	
	protected override void Interact ()
	{
		myAnimator.SetBool("Activated", true);
		myAudio.Play();
		if (!isAscending)
		{
			Ascend();
		}
		else
		{
			Descend();
		}
		myAnimator.SetBool("Activated", false);
	}

	public void Ascend()
	{
		isAscending = true;
		StartCoroutine(platform.MoveTo(upPoint));
	}

	public void Descend()
	{
		isAscending = false;
		StartCoroutine(platform.MoveTo(downPoint));
	}
}
