using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Holds sounds for the player's voice
/// </summary>

public class PlayerVoiceList : ScriptableObject {

	public string assetName = "PlayerVoice";

	/// <summary>
	/// The player's sex. Used to distinguish male or female clips.
	/// </summary>
	public enum VoiceType
	{
		male,
		female
	}

	public VoiceType mySex;

	public AudioClip introClip;

	public List<AudioClip> damageSounds;

	public List<AudioClip> deathSounds;

	public List<AudioClip> breathNoises;

	public AudioClip GetIntroClip() 
	{
		return introClip;
	}

	int getRandomIndex(List<AudioClip> soundList)
	{
		return Random.Range(0, soundList.Count);
	}

	public AudioClip GetDamagedSound()
	{
		int index = getRandomIndex(damageSounds);
		return damageSounds[index];
	}

	public AudioClip GetDyingNoise()
	{
		int index = getRandomIndex(deathSounds);
		return deathSounds[index];
	}

	public AudioClip GetBreathNoise()
	{
		int index = getRandomIndex(breathNoises);
		return breathNoises[index];
	}
}
