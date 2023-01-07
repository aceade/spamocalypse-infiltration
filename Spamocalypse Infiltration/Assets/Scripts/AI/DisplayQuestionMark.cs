using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a question mark above a unit's head. Mainly for debugging,
/// and helping players with hearing problems.
/// </summary>

public class DisplayQuestionMark : MonoBehaviour {

	Text myText;

	const string questionMark = "?";
	const string exclamationMark = "!";

	public Camera uiCamera;

	Transform myTransform;

	public enum Mood
	{
		puzzled,
		alarmed,
		normal
	}

	public float rotateTimer = 0.25f;

	Mood myMood = Mood.normal;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		myText = GetComponentInChildren<Text>();
		if (uiCamera == null)
		{
			uiCamera = GameObject.Find("UI CAmera").GetComponent<Camera>();
		}
		InvokeRepeating("Rotate", rotateTimer, rotateTimer);
	}

	/// <summary>
	/// Changes the status of the spammer's mood.
	/// </summary>
	/// <param name="newMood">New mood.</param>
	public void ChangeStatus(Mood newMood)
	{
		if (myMood != newMood)
		{
			if (newMood == Mood.normal)
			{
				myText.text = "";
			}
			else if (newMood == Mood.alarmed)
			{
				myText.text = exclamationMark;
			}
			else
			{
				myText.text = questionMark;
			}
		}
	}

	void Rotate() 
	{
		myTransform.LookAt (myTransform.position + uiCamera.transform.rotation * Vector3.forward,
            uiCamera.transform.rotation * Vector3.up);
	}

    void OnDisable()
    {
        if (myText != null)
        {
            myText.enabled = false;
        }
    }

    void OnEnable()
    {
        if (myText != null)
        {
            myText.enabled = true;
        }

    }

}
