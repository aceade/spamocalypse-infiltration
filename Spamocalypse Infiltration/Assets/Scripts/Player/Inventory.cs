using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// The player's inventory.
/// 
/// Uses slots, like in Thief. 
/// </summary>

public class Inventory : MonoBehaviour {

	public int maxPuppets = 20;
	public int puppetCount;
	[Tooltip("These are placed in the scene for use")]
	public AmmoPool puppets;
	
	public AmmoPool logicBombs;
	public int maxLogicBombs = 10;
	public int bombCount;
	
	/// <summary>
	/// The player's melee weapon.
	/// </summary>
	public Weapon melee;

	/// <summary>
	/// The SQLer.
	/// </summary>
	public Weapon sqler;

    NightVision sqlerNV;

	public int maxSqlerAmmo = 30;
	public int sqlerAmmo;

	Weapon currentWeapon;

	public Text weaponText;
	public Image weaponImage;

	public Text lootText;

	int totalLoot;

	enum Slot
	{
		melee,
		sqler,
		sockpuppets,
		logic,
		none
	}

	Slot currentSlot = Slot.none;

	[HideInInspector]
	public bool canChange = true;

	int healthPacks;

	WaitForSeconds changeDelay = new WaitForSeconds(1f);

	/// <summary>
	/// The player's viewpoint camera is used for attacking.
	/// </summary>
	public Transform firingCamera;

	[Tooltip("This manages the level")]
	public LevelManager manager;

	// Use this for initialization
	void Start () 
	{
		DeactivateWeapons();
		currentWeapon = sqler;

        (sqler as ProjectileWeapon).maxAmmo = maxSqlerAmmo;
        sqlerNV = sqler.GetComponentInChildren<NightVision>();

        // add the ammo from the previous level.
        AddAmmo(AmmoPickup.AmmoType.logic, PlayerPrefs.GetInt(GameTagManager.logicBombsPrefs));
        AddAmmo(AmmoPickup.AmmoType.sockpuppet, PlayerPrefs.GetInt(GameTagManager.sockpuppetPrefs));
        AddAmmo(AmmoPickup.AmmoType.sql, PlayerPrefs.GetInt(GameTagManager.sqlerAmmoPrefs));
	}

	/// <summary>
	/// Cycle up through the weapons
	/// </summary>
	/// <returns>The up.</returns>
	public IEnumerator SwitchUp()
	{
		canChange = false;
		if (currentSlot == Slot.sqler)
		{
			ChangeToMelee();
		}
		else if (currentSlot == Slot.melee)
		{
			ChangeToSockPuppets();
		}
		else if (currentSlot == Slot.sockpuppets)
		{
			ChangeToLogic();
		}
		else if (currentSlot == Slot.logic)
		{
			ChangeToRanged();
		}

		yield return changeDelay;
		canChange = true;
	}

    public void UseAimingCamera()
    {
        if (currentWeapon == sqler && currentSlot == Slot.sqler){
            currentWeapon.StartSecondaryAttack();
        }
    }

    public void ToggleNightVision()
    {
        sqlerNV.enabled = !sqlerNV.enabled;
    }

	/// <summary>
	/// Fires the current weapon.
	/// </summary>
	public void UseCurrentWeapon()
	{
		GameTagManager.LogMessage("Player is attacking with weapon in slot {0}", currentSlot);
		if (currentSlot == Slot.sqler || currentSlot == Slot.melee)
		{
			if (currentWeapon.IsAbleToAttack())
			{
				currentWeapon.StartAttack(firingCamera.position, firingCamera.forward);
				if (currentWeapon == sqler)
				{
					sqlerAmmo--;
					weaponText.text = string.Format("{0} Sqler Rounds", sqlerAmmo);
					if (sqlerAmmo <= 0)
					{
                        (sqler as Sqler).DisableCamera();
						StartCoroutine(SwitchUp());
					}
				}
			}
		}
		else if (currentSlot == Slot.sockpuppets || currentSlot == Slot.logic)
		{

			Projectile currentProjectile;
			if (currentSlot == Slot.sockpuppets)
			{
				currentProjectile = puppets.GetProjectile();
				puppetCount--;
				weaponText.text = string.Format("{0} Sockpuppets", puppetCount);
				if(puppetCount <= 0)
				{
					StartCoroutine(SwitchUp());
				}
			}
			else
			{
				currentProjectile = logicBombs.GetProjectile();
				if (bombCount > 0)
				{
					bombCount--;
					weaponText.text = string.Format("{0} Logic Bombs", bombCount);
				}
				else
				{
					StartCoroutine(SwitchUp());
				}
			}
			currentProjectile.Launch(firingCamera.position + firingCamera.forward, firingCamera.forward);
		}

	}

	void DeactivateWeapons ()
	{
		sqler.Deactivate ();
		melee.Deactivate ();
	}

	/// <summary>
	/// Changes to melee.
	/// </summary>
	public void ChangeToMelee()
	{
		currentSlot = Slot.melee;
		currentWeapon = melee;
		DeactivateWeapons ();
		melee.Activate();

		//TODO: make an image for this!
//        weaponImage.sprite = cuttersImage;
//		weaponImage.enabled = false;
//		weaponText.text = "Cable Cutters";
	}

	/// <summary>
	/// Changes to sock puppets.
	/// </summary>
	public void ChangeToSockPuppets()
	{
		DeactivateWeapons();
		if (puppetCount > 0)
		{
			currentSlot = Slot.sockpuppets;
			weaponText.text = string.Format("{0} Decoys", puppetCount);
		}
		else
		{
			StartCoroutine(SwitchUp());
		}
	}

	/// <summary>
	/// Changes to logic bombs.
	/// </summary>
	public void ChangeToLogic()
	{
		DeactivateWeapons();
		if (bombCount > 0)
		{
			currentSlot = Slot.logic;
			weaponText.text = string.Format("{0} Logic Bombs", bombCount);
		}
		else
		{
			StartCoroutine(SwitchUp());
		}
	}

	/// <summary>
	/// Changes to ranged weapon.
	/// </summary>
	public void ChangeToRanged()
	{
		currentSlot = Slot.sqler;
		currentWeapon = sqler;
		melee.Deactivate();
		sqler.Activate();

		if (sqlerAmmo > 0 )
		{
			sqler.Activate();
			currentWeapon = sqler;
//			weaponImage.sprite = sqlerAmmoImage;
			weaponText.text = string.Format("{0} SQL Injections", sqlerAmmo);
		}
		else
		{
			StartCoroutine(SwitchUp());
		}
	}

	/// <summary>
	/// Clears the current weapon.
	/// </summary>
	public void ClearWeapon()
	{
		DeactivateWeapons();
		currentSlot = Slot.none;
		weaponText.text = "";
	}

	/// <summary>
	/// Adds the specified ammo type to the player's inventory.
	/// </summary>
	/// <param name="type">Type of ammo.</param>
	/// <param name="amountOfAmmo">Amount of ammo to add</param>
	public void AddAmmo(AmmoPickup.AmmoType type, int amountOfAmmo)
	{
//		GameManager.LogMessage("Adding ammo of type {0} to the player's inventory", type);
		switch (type)
		{
		case AmmoPickup.AmmoType.sockpuppet:
			puppetCount+= amountOfAmmo;
			if (puppetCount > maxPuppets)
			{
				puppetCount = maxPuppets;
			}
			if (currentSlot == Slot.sockpuppets)
			{
				weaponText.text = string.Format("{0} Decoys", puppetCount);
			}
			break;
		case AmmoPickup.AmmoType.sql:
			sqlerAmmo+= amountOfAmmo;
            (sqler as Sqler).EnableCamera();
			if (sqlerAmmo > maxSqlerAmmo)
			{
				sqlerAmmo = maxSqlerAmmo;
			}
			if (currentSlot == Slot.sqler)
			{
				weaponText.text = string.Format("{0} SQL Injections", sqlerAmmo);
			}
			break;
		case AmmoPickup.AmmoType.logic:
			bombCount+= amountOfAmmo;
			if (bombCount > maxLogicBombs)
			{
				bombCount = maxLogicBombs;
			}
			if (currentSlot == Slot.logic)
			{
				weaponText.text = string.Format("{0} Logic Bombs", bombCount);
			}
			break;
		case AmmoPickup.AmmoType.health:
			healthPacks+= amountOfAmmo;
			break;
		}
	}

	/// <summary>
	/// Adds a specific type of loot with a specific value.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="lootValue">Loot value.</param>
	public void AddLoot(Loot.TypeOfLoot type, int lootValue)
	{
		totalLoot += lootValue;
		lootText.canvasRenderer.SetAlpha(1.0f);
		lootText.text = string.Format("{0} : {1}\nTotal: {2}", type, lootValue, totalLoot);
		StartCoroutine(ShowLoot());
	}

	/// <summary>
	/// Shows the loot amount to the player.
	/// </summary>
	/// <returns>The loot.</returns>
	IEnumerator ShowLoot()
	{
		manager.CheckLootObtained(totalLoot);
		yield return new WaitForSeconds(1.5f);
		while (lootText.canvasRenderer.GetAlpha() >= 0f)
		{
			yield return new WaitForSeconds(0.2f);
			lootText.canvasRenderer.SetAlpha(lootText.canvasRenderer.GetAlpha() - 0.1f);
		}
	}

	/// <summary>
	/// Gets the ammo count.
	/// </summary>
	public int[] GetAmmoCount()
	{ 
		var ammo = new int[3];
		ammo[0] = sqlerAmmo;
		ammo[1] = puppetCount;
		ammo[2] = bombCount;
        return ammo;
	}

    /// <summary>
    /// Gets the amount of SQLer ammo the player has.
    /// </summary>
    /// <returns>The sqler ammo.</returns>
    public int GetSqlerAmmo()
    {
        return sqlerAmmo;
    }

    /// <summary>
    /// Gets the number of sockpuppets the player has.
    /// </summary>
    /// <returns>The puppet count.</returns>
    public int GetPuppetCount()
    {
        return puppetCount;
    }

    /// <summary>
    /// Gets the player's logic bomb count.
    /// </summary>
    /// <returns>The bomb count.</returns>
    public int GetBombCount()
    {
        return bombCount;
    }

	/// <summary>
	/// Sets the ammo count after loading the game.
	/// </summary>
	/// <param name="newSqlerAmmo">New sqler ammo.</param>
	/// <param name="newPuppetCount">New puppet count.</param>
	/// <param name="newBombCount">New bomb count.</param>
	public void SetAmmoCount(int newSqlerAmmo, int newPuppetCount, int newBombCount)
	{
		sqlerAmmo = newSqlerAmmo;
		puppetCount = newPuppetCount;
		bombCount = newBombCount;
	}

	public int GetLootCount()
	{
		return totalLoot;
	}

    public void AssignPublicVariables()
    {
        puppets = GameObject.Find("Sockpuppets Pool").GetComponent<AmmoPool>();
        logicBombs = GameObject.Find("Logic Bombs Pool").GetComponent<AmmoPool>();
        sqler = GameObject.Find("SQLer").GetComponent<Weapon>();
    }
}
