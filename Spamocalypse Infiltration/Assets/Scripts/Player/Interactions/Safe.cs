using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Safe : PlayerInteraction {

	public Canvas keypad;

	Text inputField;

	public string code;

	string input = "";

	bool unlocked = false;

	public AudioClip openingSound, keySound;

	Animator myAnimator;
	AudioSource myAudio;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();
		myAnimator = GetComponentInChildren<Animator>();
		myAudio = GetComponent<AudioSource>();
		visibleObject = GetComponentsInChildren<MeshRenderer>()[1];

		inputField = keypad.transform.Find("Input Feedback").GetComponent<Text>();
		
		if (code.Length < 4)
		{
			Debug.LogError(string.Format("{0} does not have a long enough keycode ({1})!", transform.name, code));
		}
	}

	protected override void Interact()
	{
		if (!unlocked)
		{
			Cursor.visible = true;
			keypad.gameObject.SetActive(true);
			FreezePlayer();
			input = "";
		}
		else
		{
			bool open = myAnimator.GetBool("IsOpening");
			myAnimator.SetBool("IsOpening", !open);
		}
	}

	public string GetInput()
	{
		return input;
	}

	public void AddNumber(string number)
	{
		input = string.Concat(input, number);
		inputField.text = input;
		myAudio.clip = keySound;
		myAudio.Play();

		if (input.Length == 4 && input.Equals (code))
		{
			Unlock();
		}
	}


	public void Hide()
	{
		keypad.gameObject.SetActive(false);
		Cursor.visible = false;
		UnfreezePlayer();
	}

	void Unlock()
	{
		GameTagManager.LogMessage("{0} has been unlocked", name);
		unlocked = true;
		myAudio.clip = openingSound;
		myAudio.Play();
		myAnimator.SetBool("IsOpening", true);
		Hide ();
	}
}
