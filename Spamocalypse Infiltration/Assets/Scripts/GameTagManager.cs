using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// A static class to manage things like tags and layers
/// </summary>

public class GameTagManager : MonoBehaviour 
{
	public static GameTagManager myManager;

	public static readonly string playerTag = "Player";

	public static readonly string botTag = "Bot";
	public static readonly string spammerTag = "Spammer";
	public static readonly string adminTag = "Admin";
	public static readonly int spammerLayer = 8;

	public static float minSpamTime = 10f;
	public static float maxSpamTime = 30f;

	public static readonly string decoyTag = "Decoy";
	public static readonly float alertRadius = 20f;
	public static readonly int surprisedDamageMultiplier = 3;

	/// <summary>
	/// The damage multiplier for different difficulties; 1 by default
	/// </summary>
	public static float damageMultiplier = 1.0f;

	public static string firewallTag = "Firewall";

	public static int firewallDamagePerSecond = 10;

	public static bool isPaused;

	static string currentLevelName;
	public static bool isLoading;
	public static bool couldLoadSave;
	public static bool isSaving;

    public static readonly string numberOfKillsPrefs = "numberOfKills";
    public static readonly string numberOfDeathsPrefs = "numberOfDeaths";
    public static readonly string numberOfDetectionPrefs = "numberOfDetections";
    public static readonly string numberOfLootPrefs = "numberOfLoot";

    public static readonly string sqlerAmmoPrefs = "sqlerAmmo";
    public static readonly string logicBombsPrefs = "logicBombs";
    public static readonly string sockpuppetPrefs = "sockpuppets";

	// all attack modes
	public enum AttackMode
	{
		spam,
		sql,
		melee,
		fire,
		logic
	}

	/// <summary>
	/// Difficulty levels.
	/// </summary>
	public enum DifficultyLevel
	{
		normal,
		hard,
		expert,
	}

	/// <summary>
	/// The game difficulty.
	/// 1 = normal, 2 = hard,
	/// 3 = expert.
	/// </summary>
	public static DifficultyLevel gameDifficulty;

	public static readonly float normalMultiplier = 1f;
	public static readonly float hardMultiplier = 2f;
	public static readonly float expertMultiplier = 3f;

	/// <summary>
	/// The save path. Contains the directory separator character at the end.
	/// </summary>
	static readonly string savePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar.ToString());

	const string fileSaveName = "spamocalypse.sav";

	static BinaryFormatter bf = new BinaryFormatter();

	/// <summary>
	/// The current save. Just one for now.
	/// </summary>
	static SaveGame currentSave;

	public static bool isLoaded;

	// constants for option names
	public static string musicVolume = "MusicVolume";
	public static string mouseSensitivity = "MouseSensitivity";

	/// <summary>
	/// Changes the level, and loads the relevant navmesh.
	/// </summary>
	/// <param name="newLevel">New level.</param>
	public static void ChangeLevel(string newLevel)
	{
		SceneManager.LoadScene(newLevel);
		currentLevelName = newLevel;
		Time.timeScale = 1f;
		isPaused = false;
	}

	public static void RestartLevel()
	{
		ChangeLevel(currentLevelName);
	}


	public static string GetCurrentLevelName()
	{
		return currentLevelName;
	}
    
	/// <summary>
	/// Pauses the game.
	/// </summary>
	public static void PauseGame()
	{
		Time.timeScale = 0f;
		ShowCursor();
		isPaused = true;
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public static void ResumeGame()
	{
		HideCursor();
		isPaused = false;
		Time.timeScale = 1f;
	}

	public static void ShowCursor()
	{
		Cursor.visible = true;
	}

	public static void HideCursor()
	{
		Cursor.visible = false;	
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public static void Quit()
	{
		Application.Quit();
	}

	/// <summary>
	/// Sets the spam times.
	/// </summary>
	/// <param name="newMax">New max.</param>
	/// <param name="newMin">New minimum.</param>
	public static void SetSpamTimes(float newMax, float newMin)
	{
		LogMessage("Setting the new spam times to {0} and {1}", newMin, newMax);
		minSpamTime = newMin;
		maxSpamTime = newMax;
	}

	public static void LogMessage(string msg, params object[] args)
	{
		Debug.Log (string.Format(msg, args));
	}

	/// <summary>
	/// Gets the difficulty multiplier.
	/// </summary>
	/// <returns>The difficulty multiplier.</returns>
	public static float GetDifficultyMultiplier()
	{
		switch (gameDifficulty)
		{
		case DifficultyLevel.hard:
			return hardMultiplier;
		case DifficultyLevel.expert:
			return expertMultiplier;
		default:
			return normalMultiplier;
		}
	}

	/// <summary>
	/// Sets the difficulty.
	/// </summary>
	public static void SetDifficulty(DifficultyLevel newDifficulty)
	{
		gameDifficulty = newDifficulty;
	}

	/// <summary>
	/// Saves the game.
	/// </summary>
	public static void SaveGame(SaveGame newSave)
	{
		LogMessage("Saving a new game at timestamp {0}", newSave.GetSaveDate().ToString());
		newSave.SetLevelName(currentLevelName);
		currentSave = newSave;
		isSaving = true;
		// Loom.RunAsync(()=>{
		// 	try 
		// 	{
		// 		var saveFile = File.Create(savePath + fileSaveName);
		// 		bf.Serialize(saveFile, currentSave);
		// 		saveFile.Close();
		// 		LogMessage("Saved the game as {0} at {1}", fileSaveName, savePath);
		// 	}
		// 	catch (IOException e)
		// 	{
		// 		Debug.LogError(e.Message);
		// 	}
		// 	finally
		// 	{
		// 		isSaving = false;
		// 	}
		// });

	}

	/// <summary>
	/// Loads the game.
	/// </summary>
	public static void LoadGame()
	{
		if (currentSave == null)
		{
			currentSave = LoadGameFromDisk(true);
		}

		isLoaded = true;
		ChangeLevel(currentSave.GetLevelName());
	}

	/// <summary>
	/// Loads the game from disk. Called to prepare the load file.
	/// </summary>
	/// <param name="showError">If set to <c>true</c>, will log general exceptions</param>
	public static SaveGame LoadGameFromDisk(bool showError)
	{
		LogMessage("Loading previous save game");
        isLoading = true;
		// Loom.RunAsync(()=>{
		// 	try
		// 	{
		// 		var loadFile = File.Open(savePath + fileSaveName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		// 		currentSave = (SaveGame)bf.Deserialize(loadFile);
		// 		loadFile.Close();
		// 		couldLoadSave = true;
		// 	}
		// 	catch (FileNotFoundException e)
		// 	{
		// 		GameManager.LogMessage("Could not find previous save file: {0}", e.Message);
		// 		couldLoadSave = false;
		// 	}
		// 	catch (IOException e)
		// 	{
		// 		if (showError)
		// 		{
		// 			Debug.LogError(e.Message);
		// 		}
		// 		couldLoadSave = false;

		// 	}
        //     finally
        //     {
        //         isLoading = false;
        //     }
		// });
		return currentSave;
	}

	public static SaveGame GetCurrentSave()
	{
		return currentSave;
	}

    /// <summary>
    /// Disables all children of the selected Transform.
    /// </summary>
    /// <param name="trans">Trans.</param>
    public static void DisableAllChildren(Transform trans)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            trans.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Resets the player stats. Call only when starting a new game.
    /// </summary>
    public static void ResetPlayerStats()
    {
        PlayerPrefs.SetInt(numberOfKillsPrefs, 0);
        PlayerPrefs.SetInt(numberOfDetectionPrefs, 0);
        PlayerPrefs.SetInt(numberOfDeathsPrefs, 0);
        PlayerPrefs.SetInt(numberOfLootPrefs, 0);
        PlayerPrefs.SetInt(sqlerAmmoPrefs, 0);
        PlayerPrefs.SetInt(logicBombsPrefs, 0);
        PlayerPrefs.SetInt(sockpuppetPrefs, 0);
    }

    /// <summary>
    /// Get the player stats, and add their current values.
    /// </summary>
    /// <param name="kills">Kills.</param>
    /// <param name="deaths">Deaths.</param>
    /// <param name="loot">Loot.</param>
    /// <param name="detected">Number of times detected.</param>
    public static void UpdatePlayerStats(int kills, int deaths, int loot, int detected)
    {
        int oldKills = PlayerPrefs.GetInt(numberOfKillsPrefs);
        PlayerPrefs.SetInt(numberOfKillsPrefs, oldKills + kills);
        int oldDeaths = PlayerPrefs.GetInt(numberOfDeathsPrefs);
        PlayerPrefs.SetInt(numberOfDeathsPrefs, oldDeaths + deaths);
        int oldLoot = PlayerPrefs.GetInt(numberOfLootPrefs);
        PlayerPrefs.SetInt(numberOfLootPrefs, oldLoot + loot);
        int oldDetections = PlayerPrefs.GetInt(numberOfDetectionPrefs);
        PlayerPrefs.SetInt(numberOfDetectionPrefs, oldDetections + detected);
    }

    public static void UpdateKills(int newKills)
    {
        PlayerPrefs.SetInt(numberOfKillsPrefs, newKills);
    }

}
