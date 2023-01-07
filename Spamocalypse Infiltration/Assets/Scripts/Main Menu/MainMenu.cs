
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Image mainPanel, optionsPanel, helpPanel, difficultyPanel, levelPanel;

    Image currentPanel;

	/// <summary>
	/// The difficulty text.
	/// </summary>
	public Text difficultyText;

	/// <summary>
	/// The mouse sensitivity slider.
	/// </summary>
	public Slider mouseSensitivity;

	/// <summary>
	/// The music volume slider.
	/// </summary>
	public Slider musicVolumeSlider;

	/// <summary>
	/// The menu audio source.
	/// </summary>
	public AudioSource menuAudio;

	public Dropdown sexDropdown;

	public Button loadButton;

	/// <summary>
	/// Toggles whether to show the spammer's mood.
	/// </summary>
	public Toggle moodToggle;

	const string normalDifficultyText = "Normal damage and detection speeds";
	const string hardDifficultyText = "You take more damage and enemies detect you more quickly. You must also find more loot";
	const string expertDifficultyText = "Take even more damage while trying to find yet more loot. Enemies detect you even faster, and you aren't allowed kill anyone";
	const string optionsPreviouslySet = "Options previously set";

    bool skipTutorial = true;

    public Text skipTutorialText;

	bool optionsSet;

	void Start()
	{
		float volume = PlayerPrefs.GetFloat(GameTagManager.musicVolume);
		bool.TryParse(PlayerPrefs.GetString(optionsPreviouslySet), out optionsSet);
		if (optionsSet)
		{
			menuAudio.volume = volume;
			musicVolumeSlider.value = volume;
			mouseSensitivity.value = PlayerPrefs.GetFloat(GameTagManager.mouseSensitivity);
		}
		else
		{
			volume = 0.25f;
			menuAudio.volume = volume;
			musicVolumeSlider.value = volume;
			mouseSensitivity.value = 70f;
		}
		PlayerPrefs.SetFloat(GameTagManager.musicVolume, volume);

		if (volume > 0 )
		{
			menuAudio.Play();
		}

		optionsPanel.gameObject.SetActive(false);
		helpPanel.gameObject.SetActive(false);
        currentPanel = mainPanel;

		if (!GameTagManager.couldLoadSave)
		{
			loadButton.interactable = false;
		}
	}


	/// <summary>
	/// Loads the specified level.
	/// </summary>
	/// <param name="levelName">Name of the level.</param>
	public void LoadSpecifiedLevel(string levelName)
	{
		GameTagManager.ChangeLevel(levelName);
	}


	/// <summary>
	/// Quit the game
	/// </summary>
	public void Quit()
	{
		GameTagManager.Quit();
	}

	public void ToggleControls()
	{
        currentPanel.gameObject.SetActive(false);
		optionsPanel.gameObject.SetActive(true);
        currentPanel = optionsPanel;
	}

	public void ToggleHelp()
	{
        currentPanel.gameObject.SetActive(false);
		helpPanel.gameObject.SetActive(true);
        currentPanel = helpPanel;
	}

	public void ToggleDifficulty()
	{
        currentPanel.gameObject.SetActive(false);
		difficultyPanel.gameObject.SetActive(true);
        currentPanel = difficultyPanel;
	}

    /// <summary>
    /// Toggles the main panel.
    /// </summary>
	public void ToggleMain()
	{
        currentPanel.gameObject.SetActive(false);
		mainPanel.gameObject.SetActive(true);
        currentPanel = mainPanel;
	}

    public void ToggleLevelSelection()
    {
        currentPanel.gameObject.SetActive(false);
        levelPanel.gameObject.SetActive(true);
        currentPanel = levelPanel;
    }

	/// <summary>
	/// Sets the mouse sensitivity.
	/// </summary>
	/// <returns>The mouse sensitivity.</returns>
	public void SetMouseSensitivity()
	{
		float value = mouseSensitivity.value;
		ShowMouseSensitivity.speed = value;
	}

	/// <summary>
	/// Sets the music volume. If the music isn't playing and the volume is greater than zero, start playing.
	/// </summary>
	public void SetVolume()
	{
		float value = musicVolumeSlider.value;
		menuAudio.volume = value;
		if (value > 0 && !menuAudio.isPlaying)
		{
			menuAudio.Play();
		}
		else if (value == 0 && menuAudio.isPlaying)
		{
			menuAudio.Stop();
		}
		PlayerPrefs.SetFloat(GameTagManager.musicVolume, menuAudio.volume);
	}

	/// <summary>
	/// Stores options in PlayerPrefs.
	/// </summary>
	/// <returns>The options.</returns>
	public void SetOptions()
	{
		PlayerPrefs.SetFloat(GameTagManager.mouseSensitivity, mouseSensitivity.value);
		PlayerPrefs.SetFloat(GameTagManager.musicVolume, musicVolumeSlider.value);
		if (!optionsSet)
		{
			PlayerPrefs.SetString(optionsPreviouslySet, bool.TrueString);
		}
	}

	/// <summary>
	/// Sets the normal difficulty.
	/// </summary>
	public void SetNormalDifficulty()
	{
		GameTagManager.SetDifficulty(GameTagManager.DifficultyLevel.normal);
		difficultyText.text = normalDifficultyText;
	}

	/// <summary>
	/// Sets the hard difficulty.
	/// </summary>
	public void SetHardDifficulty()
	{
		GameTagManager.SetDifficulty(GameTagManager.DifficultyLevel.hard);
		difficultyText.text = hardDifficultyText;
	}

	/// <summary>
	/// Sets the expert difficulty.
	/// </summary>
	public void SetExpertDifficulty()
	{
		GameTagManager.SetDifficulty(GameTagManager.DifficultyLevel.expert);
		difficultyText.text = expertDifficultyText;
	}

	public void LoadGame()
	{
        GameTagManager.ResetPlayerStats();
		GameTagManager.LoadGame();
	}

	/// <summary>
	/// Sets the character's sex.
	/// </summary>
	public void SetCharacter()
	{
		int setting = sexDropdown.value;
		if (setting == 0) 
		{
			PlayerControl.isMale = true;
		}
		else
		{
			PlayerControl.isMale = false;
		}

		// TODO: play sample voice clip
	}

	public void ToggleQuestionMark()
	{
		bool value = moodToggle.isOn;
		SpammerFSM.SetQuestionMark(value);
		GameTagManager.LogMessage("NPCs {0} show a question or exclamation mark", value ? "will" : "will not");
	}

    public void ToggleSkipTutorial()
    {
        skipTutorial = !skipTutorial;
        skipTutorialText.text = skipTutorial ? "Click to include the tutorial" : "Click to skip the tutorial";
    }

    public void StartGame()
    {
        GameTagManager.ResetPlayerStats();
        if (skipTutorial)
        {
            StartMainLevel();
        }
        else
        {
            StartTutorial();
        }
    }

	/// <summary>
	/// Skip the tutorial.
	/// </summary>
    void StartMainLevel()
	{
		GameTagManager.ChangeLevel("City Inbound");
	}

	/// <summary>
	/// Starts the tutorial level.
	/// </summary>
	void StartTutorial()
	{
		GameTagManager.ChangeLevel("Sound Test");
	}
}
