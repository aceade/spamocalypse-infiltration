using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
// using jp.gulti.ColorBlind;

/// <summary>
/// A development console that is attached to the main camera. Used to enable/disable certain effects.
/// </summary>

public class DevConsole : MonoBehaviour {

	const string logFile = "SpamocalypseProfile.log";
	bool profiling = false;

	// Grayscale cameraGrayscale;
	// ColorBlindnessSimulator colour;

	public GameObject canvas;

	bool showSoundColliders = false;
	bool showSightColliders = false;

	public LevelManager currentLevelManager;
	List<SpammerFSM> agents;

	public PlayerControl player;

	bool isPlayerVisible = true;
	public Button playerVisibilityButton;
	Text visibilityButtonText;

	bool noclipEnabled = false;

    bool coloursInverted = false;
    Image inversionPlane;

	public enum ColourBlindness
	{
		protanopic,
		deuteranopic,
		achromatic,
		none
	}

	int levelIndex = 1;
	public Button gotoButton;

    public CanvasRenderer haxPanel;

    public CanvasRenderer levelPanel;

    public CanvasRenderer sillyPanel;

    public CanvasRenderer uiPanel;

    public CanvasRenderer controlsPanel;

    CanvasRenderer currentPanel;

	// Use this for initialization
	void Awake () {
		// cameraGrayscale = GetComponent<Grayscale>();
		// colour = GetComponent<ColorBlindnessSimulator>();
        inversionPlane = GetComponentInChildren<Image>();
        inversionPlane.gameObject.SetActive(false);
		visibilityButtonText = playerVisibilityButton.GetComponentInChildren<Text>();
        currentPanel = sillyPanel;

        #if DEVELOPMENT_BUILD || DEBUG
            GameTagManager.LogMessage("Development build, cheats are available");
        #else
            GameManager.LogMessage("Hiding some developer console controls");
            haxPanel.gameObject.SetActive(false);
            levelPanel.gameObject.SetActive(false);
            controlsPanel.transform.FindChild("Hax Button").gameObject.SetActive(false);
            controlsPanel.transform.FindChild("Levels Button").gameObject.SetActive(false);
        #endif
	}
	
	public void ToggleSightRenderers()
	{
		showSightColliders = !showSightColliders;
		for (int i = 0; i < agents.Count; i++)
		{
			agents[i].eyes.ToggleRenderer(showSoundColliders);
		}
	}

	/// <summary>
	/// Toggles the achromatopsia simulator (AKA greyscale).
	/// </summary>
	public void ToggleAchromatopsia()
	{
		ToggleColourBlindness(ColourBlindness.achromatic);
	}

	/// <summary>
	/// Toggles the deuteranopia simulator.
	/// </summary>
	public void ToggleDeuteranopia()
	{
		ToggleColourBlindness(ColourBlindness.deuteranopic);
	}

	/// <summary>
	/// Toggles the protanopia simulator.
	/// </summary>
	public void ToggleProtanopia()
	{
		ToggleColourBlindness(ColourBlindness.protanopic);
	}

	/// <summary>
	/// Resets the colour blindess simulation.
	/// </summary>
	public void ResetColourBlindess()
	{
		ToggleColourBlindness(ColourBlindness.none);
	}

	void ToggleColourBlindness(ColourBlindness type)
	{
		// switch (type)
		// {
		// case ColourBlindness.protanopic:
		// 	colour.enabled = true;
		// 	colour.BlindMode = ColorBlindnessSimulator.ColorBlindMode.Protanope;
		// 	// cameraGrayscale.enabled = false;
		// 	break;
		// case ColourBlindness.deuteranopic:
		// 	colour.enabled = true;
		// 	colour.BlindMode = ColorBlindnessSimulator.ColorBlindMode.Deuteranope;
		// 	// cameraGrayscale.enabled = false;
		// 	break;
		// case ColourBlindness.achromatic:
		// 	//cameraGrayscale.enabled = true;
		// 	break;
		// case ColourBlindness.none:
		// 	colour.enabled = false;
		// 	// cameraGrayscale.enabled = false;
		// 	break;
		// }
	}

	public void ToggleSoundRenderers()
	{
		showSoundColliders = !showSoundColliders;

		for (int i = 0; i < agents.Count; i++)
		{
			agents[i].ears.ToggleRenderer(showSoundColliders);
		}
	}

	/// <summary>
	/// Toggles big heads mode. Because it looks funny.
	/// </summary>
	public void ToggleBigHeads()
	{
        currentLevelManager.ToggleBigHeadsMode();
	}

    /// <summary>
    /// Turns the NPCs inside out.
    /// </summary>
    public void TurnInsideOut()
    {

    }

    public void SetTimeScale(float newTimescale)
    {
        Time.timeScale = newTimescale;
    }

	public void SetLevel(Dropdown menu)
	{
		levelIndex = menu.value;
		GameTagManager.LogMessage("GOTO: level index is {0}, itemText is {1}", levelIndex, menu.captionText.text);
		gotoButton.GetComponentInChildren<Text>().text =  string.Format("GOTO {0}", (levelIndex + 1) * 10);
	}

	/// <summary>
	/// Opens the selected level.
	/// </summary>
	public void GotoLevel()
	{
		
		string levelName = "";
		if(levelIndex == 1)
		{
			levelName = "City Inbound";
		}
		else if (levelIndex == 2)
		{
			levelName = "Inner City";
		}
		else if (levelIndex == 3)
		{
			levelName = "Museum Model";
		}
        else if (levelIndex == 4)
        {
            levelName = "Escape Level";
        }
		else
		{
			levelName = "Main Menu";
		}

		GameTagManager.ChangeLevel(levelName);
	}

	public void Show()
	{
		canvas.SetActive(true);
		GameTagManager.PauseGame();

		if (agents == null || agents.Count == 0)
		{
			GameTagManager.LogMessage("The agents are null! Getting them now");
			agents = currentLevelManager.GetSpammers();
			GameTagManager.LogMessage("There are now {0} agents", agents.Count);
		}
	}

	public void Hide()
	{
		canvas.SetActive(false);
		player.ResumeGame();
	}

	/// <summary>
	/// Toggles the player's visibility, for when I can't be bothered to stealth.
	/// Note to self: remove from build. Doesn't allow me to turn off eyesight.
	/// </summary>
	public void ToggleVisibility()
	{
		isPlayerVisible = !isPlayerVisible;
		if (isPlayerVisible)
		{
			player.GetComponent<AudioSource>().enabled = true;
			visibilityButtonText.text = "Silent Running";
		}
		else
		{
			player.GetComponent<AudioSource>().enabled = false;
			visibilityButtonText.text = "Player Footsteps On";
		}
	}

	/// <summary>
	/// Toggles noclip mode. HAAAAAX!
	/// </summary>
	public void ToggleNoClip ()
	{
		noclipEnabled = !noclipEnabled;
		player.GetComponent<Rigidbody>().isKinematic = noclipEnabled;
	}

    /// <summary>
    /// Inverts colours. Does not affect UI, and requires night vision to be disabled.
    /// </summary>
    public void ToggleInvertedColours()
    {
        coloursInverted = !coloursInverted;
        inversionPlane.gameObject.SetActive(coloursInverted);
        if (coloursInverted)
        {
            GetComponent<NightVision>().enabled = false;
        }
    }

	public void ToggleProfiler()
	{
		profiling = !profiling;
		UnityEngine.Profiling.Profiler.enabled = profiling;
		UnityEngine.Profiling.Profiler.enableBinaryLog = profiling;
		UnityEngine.Profiling.Profiler.logFile = logFile;
	}

    public void SetCurrentPanel(CanvasRenderer newPanel)
    {
        newPanel.gameObject.SetActive(true);
        currentPanel.gameObject.SetActive(false);
        currentPanel = newPanel;
    }

    public void AssignPublicVariables()
    {

    }

}
