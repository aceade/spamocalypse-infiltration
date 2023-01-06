using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class Briefing : MonoBehaviour {

	public LevelManager manager;
	public Canvas briefingCanvas;
	public Image levelMap;
	public Text briefingText;
	public ScrollRect briefingTextArea;
	public Text objectivesText;

	public Sun theSun;

	List<PlayerObjective> goals;
    
    public PlayerVoice voice;
    public AudioClip monologue;

	// Use this for initialization
	void Awake () 
	{
		Cursor.visible = true;
		manager.DisablePlayer();
		manager.DisableAllBots();
		goals = manager.objectives;
		SetUpGoals();
        if (theSun != null)
        {
            theSun.enabled = false;
        }
		
        voice.OverrideClip(monologue);

	}

	void SetUpGoals()
	{
		var goalText = string.Empty;
		if (!goals.Contains(manager.lootGoal))
		{
			goals.Add(manager.lootGoal);
		}
		if (!goals.Contains(manager.escape))
		{
			goals.Add(manager.escape);
		}
		if (GameTagManager.gameDifficulty == GameTagManager.DifficultyLevel.expert && !goals.Contains(manager.avoidingDeaths))
		{
			goals.Add(manager.avoidingDeaths);
		}
		for (int i = 0; i < goals.Count; i++)
		{
			goalText += (goals[i].GetBriefingText() + "\n");
		}
		objectivesText.text = goalText;
	}
	
	public void FinishBriefing()
	{
		manager.EnableAllBots();
		manager.EnablePlayer();
        manager.StartMusic();
        voice.StopClip();
		briefingCanvas.gameObject.SetActive(false);
		Cursor.visible = false;

        if (theSun != null)
        {
            theSun.enabled = true;
        }
	}

	public void ShowMap()
	{
		levelMap.gameObject.SetActive(!levelMap.gameObject.activeSelf);
	}

	public void ShowGoals()
	{
		briefingTextArea.gameObject.SetActive(false);
		objectivesText.gameObject.SetActive(true);
		levelMap.gameObject.SetActive(false);
	}

	public void ShowBriefing()
	{
		briefingTextArea.gameObject.SetActive(true);
		objectivesText.gameObject.SetActive(false);
		levelMap.gameObject.SetActive(false);
	}

	public void ScrollText(float newValue)
	{
		briefingText.rectTransform.Translate(Vector3.up * newValue);
	}

    public void CheckAssignments()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<LevelManager>();
        }

        if (briefingCanvas == null)
        {
            briefingCanvas = GameObject.Find("Briefing Canvas").GetComponent<Canvas>();
        }
        if (briefingText == null)
        {
            briefingText = GameObject.Find("Briefing Text").GetComponent<Text>();
        }
        if (levelMap == null)
        {
            levelMap = GameObject.Find("Briefing Map").GetComponent<Image>();
        }
        briefingTextArea = briefingCanvas.GetComponentInChildren<ScrollRect>();
        objectivesText = GameObject.Find("Briefings Goal Text").GetComponent<Text>();
        theSun = FindObjectOfType<Sun>();
    }
}
