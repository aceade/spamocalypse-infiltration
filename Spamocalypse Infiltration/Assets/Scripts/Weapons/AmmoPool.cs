using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ammo pool for projectiles.
/// </summary>

public class AmmoPool : MonoBehaviour {

	public GameObject ammoPrefab;

	public List<Projectile> ammoList = new List<Projectile>();

	public int maxAmmo;

	private int currentAmmo;

	// Use this for initialization
	void Start () 
	{
		if (ammoList.Count == 0)
		{
			Debug.LogWarningFormat("Ammo pool {0} has no ammo! Instantiating this has performance issues", name);
			if (ammoPrefab == null)
			{
				Debug.LogErrorFormat("You forgot to assign a prefab to the ammo pool {0}!", name);
			}
			else
			{
				InstantiatePool();
			}
		}
	}

	/// <summary>
	/// Instantiates the pool.
	/// </summary>
	/// <returns>The pool.</returns>
	public void InstantiatePool()
	{
		if (transform.childCount == 0)
		{
			ammoList.Clear();
		}

		for (int i = ammoList.Count; i < maxAmmo; i++)
		{
			GameObject newAmmoObject = GameObject.Instantiate(ammoPrefab);
			newAmmoObject.transform.SetParent(transform);
			Projectile newAmmo = newAmmoObject.GetComponent<Projectile>();
			newAmmo.name = ammoPrefab.name;
			ammoList.Add(newAmmo);
		}

		Debug.LogFormat("Ammo pool {0} has {1} pieces of ammo pooled", name, ammoList.Count);
	}

	/// <summary>
	/// Gets a projectile from the pool.
	/// </summary>
	/// <returns>The projectile.</returns>
	public Projectile GetProjectile()
	{
		Projectile result = ammoList[currentAmmo];
		currentAmmo++;
		if (currentAmmo >= ammoList.Count)
		{
			currentAmmo = 0;
		}

		return result;
	}

}
