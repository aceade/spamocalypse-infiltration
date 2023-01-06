using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Static class to hold death sounds.
/// </summary>

[System.Serializable]
public class DeathSounds : ScriptableObject {
	
	public string assetName = "New DeathSounds";

	public List<AudioClip> sounds;

	/// <summary>
	/// Gets the configured death sounds.
	/// </summary>
	/// <returns>The sounds.</returns>
	public List<AudioClip> GetSounds()
	{
		return sounds;
	}

	/// <summary>
	/// Sets the new sounds list.
	/// </summary>
	/// <returns>The sounds.</returns>
	/// <param name="newSounds">New sounds.</param>
	public void GetSounds(List<AudioClip> newSounds)
	{
		sounds = newSounds;
	}
}
