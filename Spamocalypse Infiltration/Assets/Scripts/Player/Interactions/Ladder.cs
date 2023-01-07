using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A ladder that the player can use.
/// </summary>
public class Ladder : PlayerInteraction {

    bool usingLadder;

    List<MeshRenderer> segments = new List<MeshRenderer>();

    protected override void Start ()
    {
        base.Start ();
        var children = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < children.Length; i++)
        {
            segments.Add(children[i]);
        }
    }

    protected override void Interact ()
    {
        usingLadder = !usingLadder;
        player.transform.root.GetComponent<PlayerControl>().ToggleClimbing(usingLadder);
    }

    protected override void Highlight ()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].material.EnableKeyword("_EMISSION");
            segments[i].material.SetColor("_EmissionColor", Color.white * 0.2f);
        }
    }

    protected override void StopHighlight ()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].material.DisableKeyword("_EMISSION");
            segments[i].material.SetColor("_EmissionColor", Color.clear);
        }
    }

    protected override void OnTriggerExit (Collider coll)
    {
        base.OnTriggerExit (coll);
        if (coll.CompareTag(GameTagManager.playerTag))
        {
            usingLadder = false;
            player.transform.root.GetComponent<PlayerControl>().ToggleClimbing(usingLadder);
        }
    }

}
