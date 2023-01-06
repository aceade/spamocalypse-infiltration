using UnityEngine;

/// <summary>
/// The Sockpuppet acts as a decoy.
/// </summary>

public class Sockpuppet : Projectile 
{
	public Vector3 launchPosition;

	protected override void PlayHitEffects(Collider coll)
	{
        GameTagManager.LogMessage("{0} hit {1}", this, coll);
		launchPosition = myLaunchPosition;
        base.PlayHitEffects(coll);
	}

}
