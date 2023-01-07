using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Plays local sound affects for ambience.
/// </summary>

public class LocalAudioSource : MonoBehaviour {

	public List<AudioClip> soundEffects;
	AudioSource myAudio;

	float currentTime;

	int clipIndex;

	// Use this for initialization
	void Start () 
	{
		myAudio = GetComponent<AudioSource>();
		StartCoroutine(PlayCurrentSound());
	}

	IEnumerator PlayCurrentSound()
	{
		myAudio.Stop();
		myAudio.clip = soundEffects[clipIndex];
		myAudio.Play();
		yield return new WaitForSeconds(myAudio.clip.length);
		myAudio.Stop();
		clipIndex++;
		if (clipIndex >= soundEffects.Count)
		{
			clipIndex = 0;
		}
		StartCoroutine(PlayCurrentSound());
	}

	public void PauseSound()
	{
		currentTime = myAudio.time;
		myAudio.Stop();
	}

	public void ResumeSound()
	{
		myAudio.time = currentTime;
		myAudio.Play();
	}

}
