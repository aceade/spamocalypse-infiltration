using UnityEngine;
using System.Collections.Generic;

public class ToggleLights : PlayerInteraction {

    public List<LightRadius> lights;

    Animator switchAnimator;

    AudioSource myAudio;

    const string activated = "Activated";

    protected override void Start ()
    {
        switchAnimator = GetComponentInChildren<Animator>();
        myAudio = GetComponent<AudioSource>();
        base.Start ();
        GameTagManager.LogMessage("{0}", this);
    }

    /// <summary>
    /// Interact this instance.
    /// </summary>
    protected override void Interact ()
    {
        if (!switchAnimator.IsInTransition(0) || !switchAnimator.IsInTransition(1))
        {
            myAudio.Play();
            switchAnimator.SetBool(activated, !switchAnimator.GetBool (activated));
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].Toggle();
            }
        }
    }

    public override string ToString ()
    {
        return string.Format ("[ToggleLights] at {0} with {1} lights", myTransform.position, lights.Count);
    }

}
