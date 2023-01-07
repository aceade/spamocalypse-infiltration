using UnityEngine;
using System.Collections;

public class Sqler : ProjectileWeapon {

    Camera aimingCamera, mainCamera;

    bool canToggleCamera = true;

    protected override void Start ()
    {
        mainCamera = Camera.main;
        aimingCamera = GetComponentInChildren<Camera>();
        aimingCamera.cullingMask = calculateLayerMask();
        base.Start ();
    }

    /// <summary>
    /// Calculates the layer mask for the aiming camera.
    /// </summary>
    /// <returns>The layer mask.</returns>
    int calculateLayerMask()
    {
        int mask = (1 << LayerMask.NameToLayer("Spammer") | 1 << LayerMask.NameToLayer("Dead Spammer")
            | 1 << LayerMask.NameToLayer("Decoy") | 1 << LayerMask.NameToLayer("Ignore Raycast")
            | 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Walkable") 
            | 1 << LayerMask.NameToLayer("Link"));
        return mask;
    }

    public override IEnumerator SecondaryAttack ()
    {
        if (canToggleCamera)
        {
            canToggleCamera = false;
            mainCamera.enabled = !mainCamera.enabled;
            aimingCamera.enabled = !aimingCamera.enabled;
            yield return secondaryAttackCycle;
            canToggleCamera = true;
        }
    }

    /// <summary>
    /// Disables the aiming camera.
    /// </summary>
    public void DisableCamera()
    {
        canToggleCamera = false;
        aimingCamera.enabled = false;
        mainCamera.enabled = true;
    }

    /// <summary>
    /// Enables the aiming camera.
    /// </summary>
    public void EnableCamera()
    {
        canToggleCamera = true;
    }
}
