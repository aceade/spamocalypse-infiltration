using UnityEngine;
using System;

public class ModeratorEffects : SpammerEffects {

    protected override void Start ()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        GetMovement();
    }

    public override void PlayDyingEffects ()
    {
        particles.gravityModifier = 0f;
        PlayDeathNoise();
    }

    public override void PlayMoveEffects ()
    {
        particles.gravityModifier = -0.1f;
        base.PlayMoveEffects();
    }
}
