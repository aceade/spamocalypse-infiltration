using UnityEngine;

/// <summary>
/// Ammo pickup.
/// </summary>

public class AmmoPickup : PlayerInteraction {

	[Tooltip("How much ammo should this give")]
	public int amountToCollect = 1;

	public enum AmmoType
	{
		sql,
		sockpuppet,
		logic,
		health
	}

	public AmmoType myType;

	/// <summary>
	/// Upon interacting with the player, add to their inventory
	/// </summary>
	protected override void Interact()
	{
		player.GetComponentInParent<Inventory>().AddAmmo(myType, amountToCollect);
        myColl.enabled = false;
		GameTagManager.DisableAllChildren(myTransform);
	}

}
