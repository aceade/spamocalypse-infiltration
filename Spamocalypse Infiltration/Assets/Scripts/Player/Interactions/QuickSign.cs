using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Allows the player to read signs without pausing.
/// </summary>
public class QuickSign : PlayerInteraction {

	[Tooltip("The Text component for this sign")]
	public Text playerSignText;

	[Tooltip("What the sign should say")]
	[TextArea]
	public string signText;

	[Tooltip("How long it should last before fading")]
	public float displayTime = 3f;

	[Tooltip("How long it should take to fade out")]
	public float fadeOutTime = 1f;

	WaitForSeconds displayDelay, fadeDelay;

	protected override void Start ()
	{
		base.Start ();
		displayDelay = new WaitForSeconds(displayTime);
		fadeDelay = new WaitForSeconds(fadeOutTime);
        GameTagManager.LogMessage("Visible object for QuickSign {0} is {1}", this, visibleObject);
	}

	protected override void Interact ()
	{
		StartCoroutine(ShowText());
	}

	IEnumerator ShowText()
	{
		playerSignText.CrossFadeAlpha(1f, 0f, true);
		playerSignText.text = signText;
		yield return displayDelay;
		playerSignText.CrossFadeAlpha(0f, fadeOutTime, false);
		yield return fadeDelay;
		playerSignText.text = "";
	}
}
