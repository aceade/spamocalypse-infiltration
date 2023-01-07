using UnityEngine;

/// <summary>
/// A utility class that holds the details of a spammer when saving.
/// </summary>

[System.Serializable]
public class SpammerSave {

	string name;

	int health;

	private SerializableVector3 position, rotation;

	SpammerFSM.SpammerState currentState;

	SpammerFSM.DetectionStatus alertness;

	float hearingPercent, sightPercent;

	int pathIndex, patrolIndex;

	/// <summary>
	/// Initializes a new instance of the <see cref="SpammerSave"/> class.
	/// </summary>
	public SpammerSave()
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SpammerSave"/> class.
	/// </summary>
	/// <param name="currentHealth">Current health.</param>
	/// <param name="currentPosition">Current position.</param>
	/// <param name="currentRotation">Current rotation.</param>
	/// <param name="state">State.</param>
	/// <param name="detection">Detection.</param>
	/// <param name="hearing">Hearing.</param>
	/// <param name="sight">Sight.</param>
	public SpammerSave(int currentHealth, Vector3 currentPosition, Vector3 currentRotation, SpammerFSM.SpammerState state,
	                   SpammerFSM.DetectionStatus detection, float hearing, float sight)
	{
		SetHealth(currentHealth);
		SetPosition(currentPosition);
		SetRotation(currentRotation);
		SetState(currentState);
		SetAlertness(detection);
		SetSoundAlertness(hearing);
		SetSightAwareness(sight);
	}

	/// <summary>
	/// Sets the spammer's name.
	/// </summary>
	/// <param name="newName">New name.</param>
	public void SetName(string newName)
	{
		name = newName;
	}

	/// <summary>
	/// Returns the spammer's name.
	/// </summary>
	/// <returns>The name.</returns>
	public string GetName()
	{
		return name;
	}


	/// <summary>
	/// Stores the spammer's health.
	/// </summary>
	/// <param name="newHealth">New health.</param>
	public void SetHealth(int newHealth)
	{
		health = newHealth;
	}

	/// <summary>
	/// Gets the spammer's health.
	/// </summary>
	/// <returns>The health.</returns>
	public int GetHealth()
	{
		return health;
	}

	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name="newPosition">Current position.</param>
	public void SetPosition(Vector3 newPosition)
	{
		position = newPosition;
	}

	/// <summary>
	/// Gets the current position.
	/// </summary>
	/// <returns>The current position.</returns>
	public Vector3 GetCurrentPosition()
	{
		return position;
	}

	/// <summary>
	/// Sets the player's rotation.
	/// </summary>
	/// <param name="newRotation">Current rotation.</param>
	public void SetRotation(Vector3 newRotation)
	{
		rotation = newRotation;
	}

	/// <summary>
	/// Gets the player's rotation.
	/// </summary>
	/// <returns>The rotation.</returns>
	public Vector3 GetRotation()
	{
		return rotation;
	}

	/// <summary>
	/// Stores the spammer's current state.
	/// </summary>
	/// <param name="newState">New state.</param>
	public void SetState(SpammerFSM.SpammerState newState)
	{
		currentState = newState;
	}

	/// <summary>
	/// Gets the spammer's current state.
	/// </summary>
	/// <returns>The state.</returns>
	public SpammerFSM.SpammerState GetState()
	{
		return currentState;
	}

	/// <summary>
	/// Sets the alertness enum.
	/// </summary>
	/// <param name="levelOfAlertness">Level of alertness.</param>
	public void SetAlertness(SpammerFSM.DetectionStatus levelOfAlertness)
	{
		alertness = levelOfAlertness;
	}

	/// <summary>
	/// Gets the level of alertness.
	/// </summary>
	/// <returns>The level of alertness.</returns>
	public SpammerFSM.DetectionStatus GetLevelOfAlertness()
	{
		return alertness;
	}

	/// <summary>
	/// Stores their ears' level of detection.
	/// </summary>
	/// <param name="newPercent">New percent.</param>
	public void SetSoundAlertness(float newPercent)
	{
		hearingPercent = newPercent;
	}

	/// <summary>
	/// Gets their ears' level of detection.
	/// </summary>
	/// <returns>The sound alertness.</returns>
	public float GetSoundAlertness()
	{
		return hearingPercent;
	}

	/// <summary>
	/// Stores their eyes' level of detection.
	/// </summary>
	/// <param name="newPercent">New percent.</param>
	public void SetSightAwareness(float newPercent)
	{
		sightPercent = newPercent;
	}

	/// <summary>
	/// Restores their eyes' level of detection.
	/// </summary>
	/// <returns>The sight awareness.</returns>
	public float GetSightAwareness()
	{
		return sightPercent;
	}

	/// <summary>
	/// Returns the spammer's current patrol index.
	/// </summary>
	/// <returns>The patrol index.</returns>
	public int GetPatrolIndex()
	{
		return patrolIndex;
	}

    /// <summary>
    /// Sets the index of the patrol.
    /// </summary>
    /// <param name="newIndex">New index.</param>
    public void SetPatrolIndex(int newIndex)
    {
        patrolIndex = newIndex;
    }
}
