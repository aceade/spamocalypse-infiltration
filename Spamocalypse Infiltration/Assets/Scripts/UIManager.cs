using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Text healthbar;
	public Text breathBar;
	public Text objectiveText;

    public GameObject playerCanvas;
	public Canvas pauseCanvas;

    public void ShowPlayerHealth(int health) 
    {
        healthbar.text = string.Format("Health: {0}", health);
    }

    public void ShowPlayerBreath(float endurance, float maxEndurance) {
        breathBar.text = string.Format("Breath: {0}", (endurance / maxEndurance).ToString("#.##") );
    }

    public void DisplayObjectives(List<PlayerObjective> objectives)
	{
		objectiveText.text = string.Empty;
		for (int i = 0; i < objectives.Count; i++)
		{
			string text = string.Concat(objectives[i].ToString(), "\n");
			objectiveText.text += text;
		}
	}

    public void TogglePauseCanvas(bool enabled) {
        pauseCanvas.gameObject.SetActive(enabled);
        playerCanvas.SetActive(!enabled);
    }


    /// <summary>
    /// Disable buttons while waiting to save.
    /// </summary>
    public void Save() {
        StartCoroutine(WaitForSave());
    }

    private IEnumerator WaitForSave()
	{
		var buttons = pauseCanvas.GetComponentsInChildren<Button>();
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = false;
		}
		while (GameTagManager.isSaving)
		{
			yield return new WaitForSeconds(0.1f);
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = true;
		}
	}

    public void ShowPlayerCanvas(bool enabled)
    {
        playerCanvas.SetActive(enabled);
    }
}
