using UnityEngine;

/// <summary>
/// Player objective.
/// </summary>

public class PlayerObjective : PlayerInteraction {

	[Tooltip("Describe this goal")]
	public string goalText;

	/// <summary>
	/// The level manager.
	/// </summary>
	[Tooltip("The managager for this level")]
	public LevelManager manager;
	
	const string bulletPoint = "\u2022";
	const string checkMark = "COMPLETED";
	const string uncheckedBox = "INCOMPLETE";
	const string failedBox = "FAILED";
	const string optional = "(optional)";

	string statusText;
	
	public delegate void AlertLevelManager();
	AlertLevelManager myAlert;

	public enum GoalStatus
	{
		incomplete = 0,
		failed = 1,
		complete = 2
	}

	[Tooltip("Mark as true if it is to be triggered as soon as the player enters it")]
	public bool triggeredOnEnter;

	[Tooltip("What happens when the player interacts with this")]
	public GoalStatus interactionStatus;

	[Tooltip("Is this mandatory for the player?")]
	public bool mandatory;

	GoalStatus currentStatus = GoalStatus.incomplete;

	protected override void Start()
	{
		base.Start();
		statusText = uncheckedBox;

		// I'm using the Respawn tag to mark the exit
		if (gameObject.tag == "Respawn")
		{
			myAlert = LevelEnds;
		}
		else 
		{
			myAlert = Completed;
			if (!mandatory)
			{
				goalText += optional;
			}
		}

	}

	/// <summary>
	/// Gets the status of this objective.
	/// </summary>
	/// <returns>The status.</returns>
	public GoalStatus GetStatus()
	{
		return currentStatus;
	}

	/// <summary>
	/// Interact with this objective.
	/// </summary>
	protected override void Interact()
	{
		GameTagManager.LogMessage("Player has interacted with objective ({0}).\n New status is {1}", goalText, interactionStatus);
		SetStatus(interactionStatus);
	}

	/// <summary>
	/// Activate the trigger area for this area
	/// </summary>
	public void ActivateCollider()
	{
		GameTagManager.LogMessage("Collider for objective {0} is now enabled", this);
		GetComponent<Collider>().enabled = true;
	}

	/// <summary>
	/// Sets the status of the objective.
	/// </summary>
	/// <param name="newStatus">New status.</param>
	public void SetStatus(GoalStatus newStatus)
	{
		if (currentStatus != newStatus)
		{
			currentStatus = newStatus;

			if (newStatus == GoalStatus.complete)
			{
				statusText = checkMark;
			}
			else if (newStatus == GoalStatus.failed)
			{
				statusText = failedBox;
			}

			myAlert();
		}
	}

	/// <summary>
	/// Sets the interaction status (i.e. what happens if the player interacts with it).
	/// </summary>
	/// <param name="newStatus">New status.</param>
	public void SetInteractionStatus(GoalStatus newStatus)
	{
		interactionStatus = newStatus;
	}

	void OnTriggerEnter(Collider coll)
	{
        if (!coll.isTrigger)
        {
            GameTagManager.LogMessage("{0} has entered the radius for goal {1}", coll.transform, this);
            if (triggeredOnEnter)
            {
                Interact();
            }
        }
		
	}

	protected override void OnTriggerStay(Collider coll)
	{
		if(!coll.isTrigger && !triggeredOnEnter)
		{
			base.OnTriggerStay(coll);
		}
	}


	void Completed()
	{
		manager.ChangeObjectiveStatus(this);
		enabled = false;
	}

	void LevelEnds()
	{
		GameTagManager.LogMessage("Player has finished the level");
		manager.MissionCompleted();
	}

	public override string ToString()
	{
		return string.Format("{0} {1}: {2}", bulletPoint, goalText, statusText);
	}

	public string GetBriefingText()
	{
		return string.Format("{0} {1}", bulletPoint, goalText);
	}

}
