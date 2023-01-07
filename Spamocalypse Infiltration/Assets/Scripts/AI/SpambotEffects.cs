using UnityEngine;

public class SpambotEffects : SpammerEffects {

    public ParticleSystem exhaustPipe, launcher;
    Light myLight;

    public Transform head;

    protected override void Start ()
    {
        myLight = GetComponentInChildren<Light>();
        GetMovement();
        PlayMoveEffects();
    }

    public override void ToggleBigHeads ()
    {
        bigHeadsActivated = !bigHeadsActivated;
        head.localScale = Vector3.one * (bigHeadsActivated ? 2 : 1);
    }

    /// <summary>
    /// Plays the dying effects. Stops the exhaust pipe, launcher, light and plays a death noise.
    /// </summary>
    public override void PlayDyingEffects ()
    {
        exhaustPipe.Stop();
        launcher.Stop();
        myLight.enabled = false;
        PlayDeathNoise();
    }

    public override void PlayAttackEffects ()
    {
        launcher.Play();
    }

    public override void PlayMoveEffects ()
    {
        exhaustPipe.Play();
        if (!myLight.enabled)
        {
            myLight.enabled = true;
        }
        base.PlayMoveEffects();
    }
}
