using UnityEngine;

public class Door : PlayerInteraction {


	Animator myAnimator; 

	public bool isClosed = true;

	protected override void Start()
	{
        base.Start();
        myAnimator = GetComponentInChildren<Animator>();
	}

	protected override void Interact()
	{
		if (!myAnimator.IsInTransition(0))
		{
			if (isClosed)
			{
				isClosed = false;
				myAnimator.SetBool("IsOpening", true);
			}
			else 
			{
				isClosed = true;
				myAnimator.SetBool("IsOpening", false);
			}
		}
	}
}
