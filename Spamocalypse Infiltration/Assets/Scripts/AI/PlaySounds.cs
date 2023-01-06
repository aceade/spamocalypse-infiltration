using UnityEngine;

/// <summary>
/// A dedicated class to play spammer sounds.
/// </summary>

public class PlaySounds : MonoBehaviour {

	public SpammerVoice myVoice;

	AudioSource myAudio;
	float voiceTime;

	// Use this for initialization
	void Start () 
	{
		myAudio = GetComponent<AudioSource>();
	}

	/// <summary>
	/// Plays the specified sound.
	/// </summary>
	/// <param name="theSound">The sound.</param>
	/// <param name="isPriority">If set to <c>true</c> is priority.</param>
	void PlaySound(AudioClip theSound, bool isPriority)
	{

		if (isPriority)
		{
			myAudio.Stop ();
			myAudio.PlayOneShot(theSound);
		}
		else 
		{
			if (!myAudio.isPlaying)
			{
				myAudio.PlayOneShot(theSound);
			}
		}
	}

	/// <summary>
	/// Plays an idle or patrolling sound.
	/// </summary>
	public void PlayIdleSound()
	{
		PlaySound(myVoice.GetIdleSound(), false);
	}
	
	public void PlaySockpuppetSound()
	{
		PlaySound(myVoice.GetSockpuppetSound(), true);
	}

	public void PlayDamagedSound()
	{
		PlaySound(myVoice.GetDamageSound(), true);
	}

	public void PlayDeathSound()
	{
		PlaySound(myVoice.GetDeathSound(), true);
	}

	public void PlaySearchSound()
	{
		PlaySound(myVoice.GetSearchingSound(), false);
	}

	/// <summary>
	/// Plays an attacking line.
	/// </summary>
	public void PlayAttackSound()
	{
		PlaySound(myVoice.GetAttackingSound(), false);
	}

	/// <summary>
	/// Plays the "on fire" sound.
	/// </summary>
	public void PlayFireSound()
	{
		PlaySound(myVoice.GetFlameSound(), false);
	}

	/// <summary>
	/// Play a sound to indicate that they've heard the player.
	/// </summary>
	public void PlayAuditorySound()
	{
		PlaySound(myVoice.GetHearingSound(), true);
	}

	/// <summary>
	/// Plays a sound to indicate that they've spotted the player.
	/// </summary>
	public void PlaySpottingSound()
	{
		PlaySound(myVoice.GetSpottingSound(), true);
	}

	/// <summary>
	/// Plays a contact (i.e. bumped into player) sound.
	/// </summary>
	public void PlayContactSound()
	{
		PlaySound(myVoice.GetBumpSound(), true);
	}

	/// <summary>
	/// Pause or resume spamming.
	/// </summary>
	/// <param name="stopTalking">If set to <c>true</c> stop talking.</param>
	public void ToggleTalking(bool stopTalking)
	{
		if (stopTalking)
		{
			voiceTime = myAudio.time;
			myAudio.Stop();
		}
		else
		{
			myAudio.time = voiceTime;
			if (voiceTime >= 0.1f)
			{
				myAudio.Play();
			}
		}
	}

}
