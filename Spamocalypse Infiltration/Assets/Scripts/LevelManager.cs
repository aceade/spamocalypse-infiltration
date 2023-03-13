using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages an actual level.
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class LevelManager: MonoBehaviour {
	
	// Music-related variables
	public List<AudioClip> searchMusic;
	public List<AudioClip> normalMusic;
	public List<AudioClip> combatMusic;

	int musicIndex;
	float musicTime;

	List<LocalAudioSource> ambientNoises;

	public AudioClip victoryMusic;
	public AudioClip defeatMusic;

	public Transform player;

	AudioSource myAudio;

	float alertRadius;

	List<SpammerFSM> alertedSpammers = new List<SpammerFSM>();

	List<SpammerFSM> spammers;

	int playerDetected = 0;

	int spammersKilled = 0;

    [Tooltip("If true, stats on this level count towards the total score")]
    public bool doStatsCount = true;

	public Text objectiveStatusText, resultsText1, resultsText2;

	public CanvasRenderer playerCanvas;
	public CanvasRenderer victoryPanel;
	public CanvasRenderer defeatPanel;

	/// <summary>
	/// The list of player objectives.
	/// </summary>
	[Tooltip("List of objectives")]
	public List<PlayerObjective> objectives;

	[Tooltip("The objective to escape the level. Stored separately, and activated when others are complete")]
	public PlayerObjective escape;

	public PlayerObjective avoidingDeaths;
	public PlayerObjective lootGoal;

	int mandatoryGoals;

	public int normalLootLevel;
	public int hardLootLevel;
	public int expertLootLevel;

	int requiredLoot = 0;
	int gatheredLoot = 0;
	int totalLoot = 0;

	[Tooltip("The length of time after which a dead spammer respawns")]
	public float respawnTime = 30;
	
	public enum NorthDirection
	{
		forwards = 0,
		backwards = 1,
		left = 2,
		right = 3
	}

	public NorthDirection north;

	const string goalCompletedText = "Objective Complete!";

	public string nextLevel;

	// Use this for initialization
	void Awake () 
	{
		myAudio = GetComponent<AudioSource>();
        myAudio.Stop();
		ambientNoises = FindObjectsOfType<LocalAudioSource>().ToList();
		alertRadius = GameTagManager.alertRadius;

		GatherBots();
		GatherTotalLoot();

		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}
        
		if (GameTagManager.gameDifficulty == GameTagManager.DifficultyLevel.normal)
		{
			requiredLoot = normalLootLevel;
		}
		else if (GameTagManager.gameDifficulty == GameTagManager.DifficultyLevel.hard)
		{
			requiredLoot = hardLootLevel;
		}
		else if (GameTagManager.gameDifficulty == GameTagManager.DifficultyLevel.expert)
		{
			requiredLoot = expertLootLevel;

			// add the deaths objective automatically
			objectives.Add (avoidingDeaths);
			avoidingDeaths.mandatory = true;
			GameTagManager.LogMessage("Playing on expert difficulty - no deaths allowed.");
		}

        // add the loot goal, if mandatory and not present
		lootGoal.goalText = string.Format("Gather at least {0} Netcoins in loot", requiredLoot);
        if (requiredLoot == 0)
        {
            lootGoal.mandatory = false;
            if (objectives.Contains(lootGoal))
            {
                objectives.Remove(lootGoal);
            }
        }
        else 
        {
            if (!objectives.Contains(lootGoal))
            {
                objectives.Add(lootGoal);
            }
        }
		
		mandatoryGoals = objectives.Count(d=> d.mandatory);
        if (mandatoryGoals == 0)
        {
            ToggleExit();
        }

		// add the exit goal to the objectives list
		if (!objectives.Contains(escape))
		{
			objectives.Add(escape);
		}

		if (GameTagManager.isLoaded)
		{
			LoadLevelDetails(GameTagManager.GetCurrentSave());
		}
	}

	/// <summary>
	/// Gathers the total loot in the level.
	/// </summary>
	/// <returns>The total loot.</returns>
	void GatherTotalLoot()
	{
		List<Loot> lootObjects = FindObjectsOfType<Loot>().ToList();
		for (int i = 0; i < lootObjects.Count; i++)
		{
			totalLoot += lootObjects[i].value;
		}
	}

	void GatherBots()
	{
		spammers = FindObjectsOfType<SpammerFSM>().ToList();
	}

    public void StartMusic()
    {
        myAudio.volume = PlayerPrefs.GetFloat(GameTagManager.musicVolume);
        myAudio.clip = normalMusic[0];
        if (myAudio.volume > 0)
        {
            myAudio.Play();
        }
    }

	/// <summary>
	/// Changes the objective status. If a mandatory goal is failed, fail the mission.
	/// </summary>
	/// <param name="goal">The objective to change.</param>
	public void ChangeObjectiveStatus(PlayerObjective goal)
	{
		GameTagManager.LogMessage("Goal {0} is now in state {1}", goal, goal.GetStatus());

		if (goal.mandatory && goal.GetStatus() == PlayerObjective.GoalStatus.failed)
		{
			FailMission(false);
		}
		else 
		{
			StartCoroutine(PlayGoalText(goalCompletedText));
			int completeGoals = objectives.Count(d=> d.mandatory && d.GetStatus() == PlayerObjective.GoalStatus.complete && d != escape );
			GameTagManager.LogMessage("Player has completed {0} out of {1} goals", completeGoals, mandatoryGoals);
			if (completeGoals >= mandatoryGoals)
			{
				ToggleExit();
			}
		}

	}

	IEnumerator PlayGoalText(string text)
	{
		objectiveStatusText.text = text;
		objectiveStatusText.canvasRenderer.SetAlpha(1f);
		while (objectiveStatusText.canvasRenderer.GetAlpha() >= 0f)
		{
			yield return new WaitForSeconds(0.5f);
			objectiveStatusText.canvasRenderer.SetAlpha(objectiveStatusText.canvasRenderer.GetAlpha() - 0.1f);
		}
		objectiveStatusText.text = string.Empty;
	}

	/// <summary>
	/// Fails the mission.
	/// </summary>
	public void FailMission(bool playerDied)
	{
		defeatPanel.gameObject.SetActive(true);
        resultsText1.gameObject.SetActive(true);
        resultsText2.gameObject.SetActive(true);
        ShowResults();
		GameTagManager.LogMessage("Player failed");
		CleanupLevel(defeatMusic, playerDied);
		StartCoroutine(PlayGoalText("You failed"));
		Cursor.visible = true;

	}

	/// <summary>
	/// Cleans up the level when finished.
	/// </summary>
	/// <param name="music">Music.</param>
    /// <param name = "didPlayerDie"></param>
	void CleanupLevel(AudioClip music, bool didPlayerDie)
	{
		Cursor.visible = true;
		DisablePlayer();
		DisableAllBots();
		myAudio.Stop();
		myAudio.clip = music;
		myAudio.loop = true;
		myAudio.Play();

        if (doStatsCount)
        {
            var deathsIncrement = didPlayerDie ? 1 : 0;
            GameTagManager.UpdatePlayerStats(spammersKilled, deathsIncrement, gatheredLoot, playerDetected);
            PlayerPrefs.SetInt(GameTagManager.sqlerAmmoPrefs, player.GetComponent<Inventory>().sqlerAmmo);
            PlayerPrefs.SetInt(GameTagManager.logicBombsPrefs, player.GetComponent<Inventory>().bombCount);
            PlayerPrefs.SetInt(GameTagManager.sockpuppetPrefs, player.GetComponent<Inventory>().puppetCount);
        }
	}

	public void Pause()
	{
		musicTime = myAudio.time;
//		myAudio.Stop();
		for (int i = 0; i < spammers.Count; i++)
		{
			spammers[i].ToggleVoices(true);
		}
		for (int i = 0; i < ambientNoises.Count; i++)
		{
			ambientNoises[i].PauseSound();
		}
		GameTagManager.PauseGame();
	}

	public void Resume()
	{
		myAudio.time = musicTime;
//		myAudio.Play();
		for (int i = 0; i < spammers.Count; i++)
		{
			spammers[i].ToggleVoices(false);
		}
		for (int i = 0; i < ambientNoises.Count; i++)
		{
			ambientNoises[i].ResumeSound();
		}
		GameTagManager.ResumeGame();
	}

	/// <summary>
	/// Toggles the exit, if all other objectives are finished.
	/// </summary>
	public void ToggleExit()
	{
		GameTagManager.LogMessage("Exit unlocked!");
		escape.ActivateCollider();
	}

	/// <summary>
	/// Disables the player.
	/// </summary>
	public void DisablePlayer()
	{
		player.GetComponent<PlayerControl>().enabled = false;
		player.GetComponent<Inventory>().enabled = false;
		playerCanvas.gameObject.SetActive(false);
	}

	/// <summary>
	/// Enables the player.
	/// </summary>
	public void EnablePlayer()
	{
		player.GetComponent<PlayerControl>().enabled = true;
		player.GetComponent<Inventory>().enabled = true;
		playerCanvas.gameObject.SetActive(true);
	}

	/// <summary>
	/// Called when the player has exited the map and won.
	/// </summary>
	public void MissionCompleted()
	{
		GameTagManager.LogMessage("Player has won!");
		victoryPanel.gameObject.SetActive(true);
        resultsText1.gameObject.SetActive(true);
        resultsText2.gameObject.SetActive(true);
        ShowResults();
		CleanupLevel(victoryMusic, false);
		StartCoroutine(PlayGoalText("Victory!"));
	}

    void ShowResults()
    {
        gatheredLoot = player.GetComponent<Inventory>().GetLootCount();

        // add the objectives to the first list
        for (int i = 0; i < objectives.Count; i++)
        {
            resultsText1.text += ("\n" + objectives[i]);
        }

        string results = string.Format("Statistics\n\nSpotted: {0} \nKills: {1} \nLoot found: {2} of {3}", playerDetected, spammersKilled, gatheredLoot, totalLoot);
        GameTagManager.LogMessage("Results string is {0}", results);
        resultsText2.text = results;
    }

	/// <summary>
	/// Disables all bots.
	/// </summary>
	public void DisableAllBots()
	{
		if (spammers == null || spammers.Count == 0)
		{
			GatherBots();
		}
		for (int i = 0; i < spammers.Count; i++)
		{
			spammers[i].enabled = false;
		}
	}

	/// <summary>
	/// Enables all bots.
	/// </summary>
	public void EnableAllBots()
	{
		for (int i = 0; i < spammers.Count; i++)
		{
			spammers[i].enabled = true;
		}
	}

	/// <summary>
	/// Adds a spammer to the alert list.
	/// </summary>
	/// <param name="spammer">Spammer.</param>
	/// <param name="target">Target of the alert</param>
	public void AddAlert(SpammerFSM spammer, Transform target)
	{
		if (!alertedSpammers.Contains(spammer) )
		{
			if (alertedSpammers.Count == 0)
			{
				GeneralAlert(target);
			}

			alertedSpammers.Add(spammer);
			playerDetected++;
		}
	}

	/// <summary>
	/// Remove a spammer from the alert list.
	/// </summary>
	/// <param name="spammer">Spammer.</param>
	public void RemoveAlert(SpammerFSM spammer)
	{
		alertedSpammers.Remove(spammer);
		if (alertedSpammers.Count == 0)
		{
			ChangeMusic(normalMusic);
		}
	}

	/// <summary>
	/// When a spammer has confirmed
	/// </summary>
	public void GeneralAlert(Transform target)
	{
		alertedSpammers = spammers.Where(
			d=> !d.isDead && Vector3.Distance(d.transform.position, target.position) <= alertRadius).ToList();

		if (alertedSpammers.Count == 0)
		{
			ChangeMusic(normalMusic);
			return;
		}
		else
		{
			if (target == player)
			{
				ChangeMusic(combatMusic);
			}
			else
			{
				ChangeMusic(searchMusic);
			}
		}
        
		for (int i = 0; i < alertedSpammers.Count; i++)
		{
			if (target == player)
			{
				alertedSpammers[i].SetAlert(target, SpammerFSM.DetectionStatus.confirmed, SpammerFSM.AlertType.other);
			}
			else
			{
				alertedSpammers[i].SetAlert(target.position, SpammerFSM.DetectionStatus.possible, SpammerFSM.AlertType.other);
			}
		}
	}

	/// <summary>
	/// Changes the level music.
	/// </summary>
	/// <param name="newMusic">New music.</param>
	public void ChangeMusic(List<AudioClip> newMusic)
	{
		if (!newMusic.Contains(myAudio.clip))
		{
			musicIndex = 0;
		}

		StartCoroutine(PlayMusic(newMusic, newMusic[musicIndex]));
	}

	/// <summary>
	/// Adds the spammer death.
	/// </summary>
	/// <param name="spammer">Spammer.</param>
	public void AddSpammerDeath(SpammerFSM spammer)
	{
		spammersKilled++;

		if (objectives.Contains(avoidingDeaths))
		{
			avoidingDeaths.SetStatus(PlayerObjective.GoalStatus.failed);
		}
		else
		{
			GeneralAlert (spammer.transform);
			StartCoroutine(RespawnUnit (respawnTime, spammer));
		}

	}

	/// <summary>
	/// Checks the amount of loot obtained.
	/// </summary>
	/// <param name="loot">Loot.</param>
	public  void CheckLootObtained(int loot)
	{
		if (loot >= requiredLoot)
		{
			lootGoal.SetStatus(PlayerObjective.GoalStatus.complete);
		}
	}

	public void RestartLevel()
	{
		GameTagManager.RestartLevel();
	}

	public void LoadNextLevel()
	{
		GameTagManager.ChangeLevel(nextLevel);
	}

	/// <summary>
	/// Saves the game.
	/// </summary>
	/// <param name="health">The player's current health.</param>
	public void SaveGame(int health)
	{
		GameTagManager.LogMessage("Player is saving with health {0}", health);
		var currentSave = new SaveGame();

		// gather the player details first
		Vector3 playerPos = player.position, playerRot = player.forward;
		currentSave.SetPlayerPosition(playerPos);
		currentSave.SetPlayerRotation(playerRot);
		currentSave.SetPlayerHealth(health);
		Inventory inventory = player.GetComponent<Inventory>();
		currentSave.SetLoot(inventory.GetLootCount());
		currentSave.StoreAmmoCount(inventory.sqlerAmmo, inventory.puppetCount, inventory.bombCount);

        // store the stats
        currentSave.SetTotalLoot(PlayerPrefs.GetInt(GameTagManager.numberOfLootPrefs));
        currentSave.SetTotalDeaths(PlayerPrefs.GetInt(GameTagManager.numberOfDeathsPrefs));
        currentSave.SetDetections(PlayerPrefs.GetInt(GameTagManager.numberOfDetectionPrefs));
        currentSave.SetTotalKills(PlayerPrefs.GetInt(GameTagManager.numberOfKillsPrefs));

		// store the NPC variables now
		var saveStates = new Dictionary<string, SpammerSave>();
		for (int i = 0; i < spammers.Count; i++)
		{
			SpammerFSM spammer = spammers[i];
			var save = new SpammerSave(spammer.health, spammer.transform.position, spammer.transform.forward,
			                                   spammer.GetCurrentState(), spammer.GetAlertLevel(), spammer.ears.detection, spammer.eyes.playerDetection);
            save.SetPatrolIndex(spammer.GetPatrolIndex());
			save.SetName(spammer.name);
			saveStates.Add(spammer.name, save);
		}
		currentSave.SetSpammers(saveStates);

		// objectives
		var goalStatusList = new List<PlayerObjective.GoalStatus>();
		for (int i = 0; i < objectives.Count; i++)
		{
			goalStatusList.Add(objectives[i].GetStatus());
		}

		goalStatusList.Insert(0, escape.GetStatus());
		currentSave.SetObjectiveStatus(goalStatusList);

		GameTagManager.SaveGame(currentSave);
	}

	/// <summary>
	/// Loads the level details.
	/// </summary>
	public void LoadLevelDetails(SaveGame currentSave)
	{
		var detailsDic = currentSave.GetSpammers();

		// set up the spammers
		foreach (KeyValuePair<string, SpammerSave> pair in detailsDic)
		{
			SpammerSave details = pair.Value;
			SpammerFSM spammer = spammers.Find(d=> d.name == pair.Key);
			spammer.health = details.GetHealth();
			spammer.transform.position = details.GetCurrentPosition();
			spammer.transform.forward = details.GetRotation();
			spammer.ChangeStateTo(details.GetState());
			spammer.ears.detection = details.GetSoundAlertness();
			spammer.eyes.playerDetection = details.GetSightAwareness();
			spammer.LoadPathIndices(details.GetPatrolIndex());
		}

		// set up the player
		player.position = currentSave.GetPlayerPosition();
		player.forward = currentSave.GetPlayerRotation();
		int[] ammo = currentSave.GetAmmoCount();
		player.GetComponent<Inventory>().SetAmmoCount(ammo[0], ammo[1], ammo[2]);
		player.GetComponent<PlayerControl>().health = currentSave.GetPlayerHealth();
        GameTagManager.UpdatePlayerStats(currentSave.GetTotalKills(), currentSave.GetTotalDeaths(), currentSave.GetTotalLoot(), currentSave.GetDetections());

		// set up objectives
		List<PlayerObjective.GoalStatus> tmp = currentSave.GetObjectiveStatus();
		escape.SetStatus(tmp[0]);
		tmp.RemoveAt(0);
		for (int i = 0; i < tmp.Count; i++)
		{
			objectives[i].SetStatus(tmp[i]);
		}
	}

	/// <summary>
	/// Respawn the specified spammer.
	/// </summary>
	/// <param name="delay">Delay.</param>
	/// <param name="spammerToRespawn">Spammer.</param>
	public IEnumerator RespawnUnit(float delay, SpammerFSM spammerToRespawn)
	{
		GameTagManager.LogMessage("Respawning {0} after delay of {1}", spammerToRespawn, delay);
		yield return new WaitForSeconds(delay);
		spammerToRespawn.Respawn();
	}

	IEnumerator PlayMusic(List<AudioClip> currentList, AudioClip currentClip) 
	{
		myAudio.Stop();
		myAudio.clip = currentClip; 
		myAudio.Play();
		yield return new WaitForSeconds(currentClip.length);
		musicIndex++;
		if (musicIndex >= currentList.Count)
		{
			musicIndex = 0;
		}
		ChangeMusic(currentList);
	}

	public List<SpammerFSM> GetSpammers()
	{
		return spammers;
	}

    public void ToggleBigHeadsMode()
    {
        for (int i = 0; i < spammers.Count; i++)
        {
            spammers[i].GetComponent<SpammerEffects>().ToggleBigHeads();
        }
    }

}
