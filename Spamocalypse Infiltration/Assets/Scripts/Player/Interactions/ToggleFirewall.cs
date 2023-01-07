using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Toggles a firewall.
/// </summary>

public class ToggleFirewall : PlayerInteraction {
	
	public List<Firewall> firewalls;

	Animator myAnimator;

	bool isActive;

	public MeshRenderer myRenderer;

    AudioSource myAudio;

    LightRadius myLight;

	protected override void Start()
	{
		base.Start();
		myAnimator = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        myLight = GetComponentInChildren<LightRadius>();

		// flip to on if all firewalls active
		int activeCount = firewalls.Count(d=> d.active);
		if (activeCount > 0)
		{
			isActive = true;
		}
		else 
		{
			isActive = false;
		}
        myLight.enabled = isActive;

		visibleObject = myRenderer;
		myAnimator.SetBool("Activated", isActive);
		GameTagManager.LogMessage("Visible object for firewall control is {0}", visibleObject);
	}

	protected override void Interact()
	{
		GameTagManager.LogMessage("Player interacting with firewall {0}", this);
        if (!myAnimator.IsInTransition(0) || !myAnimator.IsInTransition(1))
        {
            ToggleActiveState();
        }
	}

	void ToggleActiveState()
	{
        isActive = !isActive;
        myAudio.Play();
		myAnimator.SetBool("Activated", isActive);
        if (isActive)
        {
            myLight.Activate();
        }
        else
        {
            myLight.Deactivate();
        }

        for (int i = 0; i < firewalls.Count; i++)
        {
            Firewall wall = firewalls[i];
            if (isActive)
            {
                wall.Activate();
            }
            else
            {
                wall.Deactivate();
            }
        }
	}
}
