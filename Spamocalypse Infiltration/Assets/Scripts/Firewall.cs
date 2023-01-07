using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Firewalls set enemies on fire.
/// </summary>

public class Firewall : MonoBehaviour {

	/// <summary>
	/// The damage per second.
	/// </summary>
	public int damagePerSecond = 20;

	/// <summary>
	/// The light of the wall.
	/// </summary>
	public Light wallLight;
    LightRadius lightArea;

	/// <summary>
	/// The boundary of the wall.
	/// </summary>
	Bounds myBounds;

	/// <summary>
	/// The status of the wall - either on, or off.
	/// </summary>
	public bool active;

	List<SpammerFSM> victims = new List<SpammerFSM>();

	/// <summary>
	/// The player.
	/// </summary>
	public PlayerControl player;

	/// <summary>
	/// The array of ParticleSystems.
	/// </summary>
	List<ParticleSystem> particles;

	/// <summary>
	/// The controls.
	/// </summary>
	public ToggleFirewall controls;

	GameTagManager.AttackMode myAttack = GameTagManager.AttackMode.fire;

	Collider myColl;

	// Use this for initialization
	void Start () 
	{
		myColl = GetComponent<Collider>();
		myBounds = myColl.bounds;
		wallLight = GetComponentInChildren<Light>();
        lightArea = wallLight.GetComponent<LightRadius>();
		damagePerSecond = GameTagManager.firewallDamagePerSecond;
        particles = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
		if (active)
		{
			Activate();
		}

	}

	/// <summary>
	/// Activate the firewall.
	/// </summary>
	public void Activate()
	{
		active = true;
        lightArea.Activate();
		PlayParticles();

		for (int i = 0; i < victims.Count; i++)
		{
			SpammerFSM spammer = victims[i];
			spammer.DamageSpammer(myAttack, damagePerSecond, 
			                      (spammer.transform.position - myBounds.center).normalized);
		}

	}

	/// <summary>
	/// Deactivate the firewall
	/// </summary>
	public void Deactivate()
	{
		active = false;
        lightArea.Deactivate();
		StopParticles();
	}

	/// <summary>
	/// Play all particles
	/// </summary>
	void PlayParticles()
	{
		for(int i = 0; i < particles.Count; i++)
		{
			particles[i].Play();
		}
	}

	/// <summary>
	/// Stops the particles.
	/// </summary>
	void StopParticles()
	{
		for (int i = 0; i < particles.Count; i++)
		{
			particles[i].Stop();
		}
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.isTrigger)
		{
			Physics.IgnoreCollision(coll, myColl);
		}


		if (coll.CompareTag(GameTagManager.playerTag) && active)
		{
			player.DamagePlayer(damagePerSecond, myAttack);
		}
		else 
		{
			var spammer = coll.transform.root.GetComponent<SpammerFSM>();
			if (spammer != null && !coll.isTrigger)
			{
				if (!victims.Contains (spammer))
				{
					victims.Add (spammer);
					if (active)
					{
						GameTagManager.LogMessage("{0} is being set on fire", spammer);
						spammer.DamageSpammer(myAttack, damagePerSecond, 
						                      (spammer.transform.position - myBounds.center).normalized);
					}
				}
			}         

		}
	}

	void OnTriggerExit(Collider coll)
	{
		victims.Remove(coll.transform.root.GetComponent<SpammerFSM>());
	}
}
