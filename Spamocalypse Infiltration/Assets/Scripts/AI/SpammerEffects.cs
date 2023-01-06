using UnityEngine;
using System.Collections;

/// <summary>
/// Utility class to handle special effects on each spammer.
/// </summary>
public class SpammerEffects : MonoBehaviour {

    protected ParticleSystem particles;

    public Transform neck;

    public float bigHeadScale = 3f;

    protected bool bigHeadsActivated;

    protected MoveAgent movement;

    protected virtual void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particles.loop = false;
        GetMovement();
    }

    /// <summary>
    /// Toggles big head mode. Because it's hilarious.
    /// </summary>
	public virtual void ToggleBigHeads()
	{
		bigHeadsActivated = !bigHeadsActivated;
        if (bigHeadsActivated)
        {
            neck.localScale = Vector3.one * bigHeadScale;
        }
        else
        {
            neck.localScale = Vector3.one;
        }
	}

    /// <summary>
    /// Turns the spammer inside out. A bug I found amusing on a previous project.
    /// </summary>
    public void TurnInsideOut()
    {
        // no-op yet
    }

    /// <summary>
    /// Plays their attack effects.
    /// </summary>
    public virtual void PlayAttackEffects()
    {
        particles.Play();
    }

    /// <summary>
    /// Plays their movement/respawn effects.
    /// </summary>
    public virtual void PlayMoveEffects()
    {
        PlayRespawnNoise();
    }

    /// <summary>
    /// Plays the dying effects. Stops particles and plays a death noise.
    /// </summary>
    public virtual void PlayDyingEffects()
    {
        if (particles.isPlaying)
        {
            particles.Stop();
        }
        PlayDeathNoise();
    }

    /// <summary>
    /// Plays their death noise. (Corpse thumping on ground or bot breaking down).
    /// </summary>
    protected void PlayDeathNoise()
    {
        StartCoroutine(movement.PlayDyingNoise(2f));
    }

    /// <summary>
    /// Plays their respawn noise.
    /// </summary>
    protected void PlayRespawnNoise()
    {
        StartCoroutine(movement.PlayRespawnNoise());
    }

    protected void GetMovement()
    {
        movement = GetComponent<MoveAgent>();
    }
}
