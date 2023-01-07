using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Saves a game's current state.
/// </summary>

[System.Serializable]
public class SaveGame {


	string levelName;

	DateTime saveDate;

	// player variables
	int playerHealth;
	SerializableVector3 playerPosition, playerRotation;
	int sqlerAmmo, puppetCount, logicBombCount;
	int loot;

    int deaths, spotted, kills, totalLoot;

	// objective variables
	List<PlayerObjective.GoalStatus> objectives;

	int[] ammoCount;

	// spammer variables are stored in a utility class
	Dictionary<string, SpammerSave> spammers;

	public SaveGame()
	{
		saveDate = DateTime.Now;
		GameTagManager.LogMessage("Created a new save game at {0}", saveDate.ToString());
	}

	
	/// <summary>
	/// Sets the player's current level. Should use Application.loadedLevelName.
	/// </summary>
	/// <param name="newLevelName">New level name.</param>
	public void SetLevelName(string newLevelName)
	{
		levelName = newLevelName;
	}

	public string GetLevelName()
	{
		return levelName;
	}


	/// <summary>
	/// Sets the player position.
	/// </summary>
	/// <param name="position">Position.</param>
	public void SetPlayerPosition(Vector3 position)
	{
		playerPosition = position;
	}

	/// <summary>
	/// Gets the player position.
	/// </summary>
	/// <returns>The player position.</returns>
	public Vector3 GetPlayerPosition()
	{
		return playerPosition;
	}

	/// <summary>
	/// Sets the player rotation.
	/// </summary>
	/// <param name="rotation">Rotation.</param>
	public void SetPlayerRotation(Vector3 rotation)
	{
		playerRotation = rotation;
	}

	/// <summary>
	/// Gets the player rotation.
	/// </summary>
	/// <returns>The player rotation.</returns>
	public Vector3 GetPlayerRotation()
	{
		return playerRotation;
	}

	/// <summary>
	/// Sets the player health.
	/// </summary>
	/// <param name="health">Health.</param>
	public void SetPlayerHealth(int health)
	{
		playerHealth = health;
	}

	/// <summary>
	/// Gets the player health.
	/// </summary>
	/// <returns>The player health.</returns>
	public int GetPlayerHealth()
	{
		return playerHealth;
	}

	/// <summary>
	/// Sets the loot.
	/// </summary>
	/// <param name="currentLoot">Current loot.</param>
	public void SetLoot(int currentLoot)
	{
		loot = currentLoot;
	}

	/// <summary>
	/// Gets the loot.
	/// </summary>
	/// <returns>The loot.</returns>
	public int GetLoot()
	{
		return loot;
	}

    public void SetTotalKills(int newKills)
    {
        kills = newKills;
    }

    public int GetTotalKills()
    {
        return kills;
    }

    public void SetDetections(int newSpotted)
    {
        spotted = newSpotted;
    }

    public int GetDetections()
    {
        return spotted;
    }

    /// <summary>
    /// Save the player's total deaths.
    /// </summary>
    /// <param name="newDeaths">New deaths.</param>
    public void SetTotalDeaths(int newDeaths)
    {
        deaths = newDeaths;
    }

    public int GetTotalDeaths()
    {
        return deaths;
    }

    /// <summary>
    /// Save the total loot.
    /// </summary>
    /// <param name="newLoot">New loot.</param>
    public void SetTotalLoot(int newLoot)
    {
        totalLoot = newLoot;
    }

    public int GetTotalLoot()
    {
        return totalLoot;
    }

	/// <summary>
	/// Stores the ammo count.
	/// </summary>
	/// <param name="newSqlerAmmo">New sqler ammo.</param>
	/// <param name="newPuppetCount">New puppet count.</param>
	/// <param name="newBombCount">New bomb count.</param>
	public void StoreAmmoCount(int newSqlerAmmo, int newPuppetCount, int newBombCount)
	{
		sqlerAmmo = newSqlerAmmo;
		puppetCount = newPuppetCount;
		logicBombCount = newBombCount;
		ammoCount = new int[]{sqlerAmmo, puppetCount, logicBombCount};
	}

	public int[] GetAmmoCount()
	{
		return ammoCount;
	}


	/// <summary>
	/// Sets the objective status. The goal to escape the map MUST inserted at position 0
	/// before being passed here.
	/// </summary>
	/// <param name="newList">New list.</param>
	public void SetObjectiveStatus(List<PlayerObjective.GoalStatus> newList)
	{
		objectives = newList;
	}

	/// <summary>
	/// Gets the objective status. When calling this, parse and remove the first objective,
	/// as this is the goal to escape the map.
	/// </summary>
	/// <returns>The objective status.</returns>
	public List<PlayerObjective.GoalStatus> GetObjectiveStatus()
	{
		return objectives;
	}


	/// <summary>
	/// Sets the dictionary of spammers.
	/// </summary>
	/// <param name="newDictionary">New dictionary.</param>
	public void SetSpammers(Dictionary<string, SpammerSave> newDictionary)
	{
		spammers = newDictionary;
	}

	/// <summary>
	/// Gets the dictionary of spammers.
	/// </summary>
	/// <returns>The spammer positions.</returns>
	public Dictionary<string, SpammerSave> GetSpammers()
	{
		return spammers;
	}

	public DateTime GetSaveDate()
	{
		return saveDate;
	}

}
