using UnityEngine;
using System.Collections;

/// <summary>
/// Loot to pick up.
/// </summary>

public class Loot : PlayerInteraction {


	public enum TypeOfLoot
	{
		docs,
		gold
	}

	public int value;

	public TypeOfLoot myType;

	
	/// <summary>
	/// Interact with the player by adding to the player's inventory.
	/// </summary>
	protected override void Interact()
	{
		player.transform.root.GetComponent<Inventory>().AddLoot(myType, value);
        myColl.enabled = false;
        GameTagManager.DisableAllChildren(myTransform);
//		gameObject.SetActive (false);
	}
}
