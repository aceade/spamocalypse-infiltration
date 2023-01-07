using UnityEngine;

/// <summary>
/// Player voice.
/// </summary>
public class PlayerVoice : MonoBehaviour {

	public PlayerVoiceList maleVoiceList, femaleVoiceList;
	PlayerVoiceList voiceList;

	AudioSource voiceBox;

	public bool isBreathing;

	// Use this for initialization
	void Start () 
	{
		if (PlayerControl.isMale)
		{
			voiceList = maleVoiceList;
		}
		else
		{
			voiceList = femaleVoiceList;
		}
		voiceBox = GetComponent<AudioSource>();
	}

	/// <summary>
	/// Plays a noise to let the player know they've been damaged
	/// </summary>
	public void PlayDamageNoise()
	{
		AudioClip noise = voiceList.GetDamagedSound();
		PlayClip(noise, false);
	}

	/// <summary>
	/// Plays a noise to let the player know they're dead
	/// </summary>
	public void PlayDeathNoise()
	{
		AudioClip noise = voiceList.GetDyingNoise();
		PlayClip(noise, false);
	}

	public void PlayBreathing()
	{
		if (!isBreathing)
		{
			isBreathing = true;
			AudioClip noise = voiceList.GetBreathNoise();
			PlayClip(noise, true);
		}

	}

    public void OverrideClip(AudioClip newClip)
    {
        if (voiceBox == null)
        {
            voiceBox = GetComponent<AudioSource>();
        }
        PlayClip(newClip, false);
    }

    public void StopClip()
    {
        voiceBox.Stop();
    }

	public void StopBreathing()
	{
		if (isBreathing)
		{
			isBreathing = false;
			voiceBox.Stop();
			voiceBox.loop = false;
		}
	}

	/// <summary>
	/// Plays the specified clip.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="shouldLoop">If set to <c>true</c>, the clip should loop.</param>
	void PlayClip(AudioClip clip, bool shouldLoop)
	{
		voiceBox.Stop();
		voiceBox.loop = shouldLoop;
		voiceBox.clip = clip;
		voiceBox.Play();
	}
}
