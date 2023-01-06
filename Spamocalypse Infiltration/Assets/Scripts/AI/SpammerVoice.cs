using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds lists of sound clips for the NPCs.
/// </summary>
public class SpammerVoice : ScriptableObject {

	public string assetName = "SpammerVoice";

	[Tooltip("The sounds played when they think they heard something")]
	public List<AudioClip> auralSuspicionSounds;

	[Tooltip("The sounds played when they think they saw something")]
	public List<AudioClip> visualSuspicionSounds;

	[Tooltip("The sounds played when they bump into the player")]
	public List<AudioClip> contactSounds;

	[Tooltip("The sounds played when searching or when they suspect the player is nearby")]
	public List<AudioClip> searchingSounds;
	
	[Tooltip("The sounds played when attacking")]
	public List<AudioClip> attackingSounds;
	
	[Tooltip("The sounds played when idle or patrolling")]
	public List<AudioClip> idleSounds;
	
	[Tooltip("The sounds played when they realise they've met a sockpuppet")]
	public List<AudioClip> sockpuppetSounds;
	
	[Tooltip("The sounds played when they realise they've been damaged")]
	public List<AudioClip> damagedSounds;

	[Tooltip("The sounds played when dying")]
	public List<AudioClip> deathSounds;

	[Tooltip("The sounds played when on fire")]
	public List<AudioClip> flameSounds;

	/// <summary>
	/// Get a random clip from the specified list.
	/// </summary>
	/// <returns>A randomly-chosen clip.</returns>
	/// <param name="theList">The list.</param>
	AudioClip GetRandomClip(List<AudioClip> theList)
	{
		int index = Random.Range(0, theList.Count - 1);
		//GameManager.LogMessage("{0}: Returning clip {1} from list of {2}", assetName, index, theList.Count);
		return theList[index];
	}

	public AudioClip GetFlameSound()
	{
		return GetRandomClip(flameSounds);
	}

	public AudioClip GetDeathSound()
	{
		return GetRandomClip(deathSounds);
	}

	public AudioClip GetDamageSound()
	{
		return GetRandomClip(damagedSounds);
	}

	public AudioClip GetSockpuppetSound()
	{
		return GetRandomClip(sockpuppetSounds);
	}

	public AudioClip GetIdleSound()
	{
		return GetRandomClip(idleSounds);
	}

	public AudioClip GetAttackingSound()
	{
		return GetRandomClip(attackingSounds);
	}

	public AudioClip GetBumpSound()
	{
		return GetRandomClip(contactSounds);
	}

	public AudioClip GetSpottingSound()
	{
		return GetRandomClip(visualSuspicionSounds);
	}

	public AudioClip GetHearingSound()
	{
		return GetRandomClip(auralSuspicionSounds);
	}

	public AudioClip GetSearchingSound()
	{
		return GetRandomClip(searchingSounds);
	}

}
