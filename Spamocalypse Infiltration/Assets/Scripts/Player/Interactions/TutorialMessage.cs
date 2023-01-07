using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Displays a tutorial message. Can also be used for the player's internal monologue.
/// </summary>

public class TutorialMessage : MonoBehaviour {

	public Text tutorialText;

	[Tooltip("The delay interval in seconds. Defaults to taking 0.5 seconds to fade out")]
	public float fadeTime = 0.5f;

    [Tooltip("If true, this will disable itself when played the first time")]
    public bool playOnce = false;

	public string tutorialString;

	private WaitForSeconds fadeOutDelay;

	// Use this for initialization
	void Start () 
	{
		fadeOutDelay = new WaitForSeconds(fadeTime);
	}

	void OnTriggerEnter()
	{
		tutorialText.text = tutorialString;
		tutorialText.canvasRenderer.SetAlpha(1f);
	}

	void OnTriggerExit()
	{
    
        if (playOnce)
        {
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(FadeOut());
        }
	}

	IEnumerator FadeOut()
	{
		float alpha	= tutorialText.canvasRenderer.GetAlpha();
		while (alpha >= 0f)
		{
			alpha = tutorialText.canvasRenderer.GetAlpha();
			tutorialText.canvasRenderer.SetAlpha(alpha - 0.2f);
			yield return fadeOutDelay;
		}
		tutorialText.text = "";
	}

    void OnDisable()
    {
        tutorialText.text = "";
    }

}
