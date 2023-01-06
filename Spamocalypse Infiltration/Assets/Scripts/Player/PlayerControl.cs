using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handles player movement.
/// </summary>

public class PlayerControl : MonoBehaviour {

	Transform myTransform;
	Rigidbody myBody;
	Vector3 northDir;
	public Transform neckBone;
	CapsuleCollider myCollider;
	NightVision nightVision;

	public float normalSpeed = 3;
	public float sneakSpeed = 1;
	public float runSpeed = 5;
	public float crouchSpeedMultiplier = 0.8f;

	[HideInInspector]
	public float currentSpeed;
	
	public int health = 100;

	AudioSource myAudio;
	public float runVolume = 1f;
	public float normalVolume = 0.5f;
	public AudioClip footsteps;
	public AudioClip sprinting;

	public GameObject compass;
	MeshRenderer compassRenderer;

	PlayerVoice playerVoice;

	CalculateLight myLightCalcuation;
	PlayerMap myMap;

	public float xSensitivity = 1f;

	public float ySensitivity = 1f;

	/// <summary>
	/// The mouse sensitivity. Has to be set high
	/// to account for Time.deltaTime
	/// </summary>
	public static float mouseSensitivity = 70f;

	[Tooltip("The maximum angle allowed for looking up")]
	public float xRotationMax = 270f;
    public float xRotationMin = 80f;

    bool canJump = true;
    bool jumping = false;
	public float jumpSpeed = 5f;
    float lastJumped;

    Vector3 jumpPosition;
    const float maxJumpTime = 1.5f;

	public Text healthbar;
	public Text breathBar;
	public Text objectiveText;

    bool canPause = true;
	public Canvas pauseCanvas;
	public GameObject playerCanvas;
	
	public DevConsole devConsole;

	public LevelManager manager;

	Inventory playerInventory;

	enum MoveType
	{
		normal,
		running,
		sneaking
	}

	MoveType currentMovement = MoveType.normal;
	bool canChangeMovement = true;
	bool canPlayerAttack = true;
	bool isCrouching;
	public float maxEndurance = 50f;
	float endurance;
	bool isExhausted;
	bool leaningLeft, leaningRight, revertFromLeft, revertFromRight;
    bool usingLadder;

	public static bool isMale;

	public float lightGemUpdateTime = 0.5f;
	float lightGemUpdated;

	float myXAxis, myZAxis;

	void Awake () 
	{
		myTransform = transform;
		endurance = maxEndurance;
        myAudio = GetComponent<AudioSource>();
        myBody = GetComponent<Rigidbody>();
        myLightCalcuation = GetComponent<CalculateLight>();
        myCollider = GetComponentInChildren<CapsuleCollider> ();
        nightVision = GetComponentInChildren<NightVision>();
        playerVoice = GetComponentInChildren<PlayerVoice>();
        myAudio.volume = 0.5f;
		ShowLightGem();

		float tmp = PlayerPrefs.GetFloat(GameTagManager.mouseSensitivity);
		if ( !tmp.Equals(0f) )
		{
			GameTagManager.LogMessage("mouse sensitivity is {0} and will become {1}", mouseSensitivity, tmp);
			mouseSensitivity = tmp;
		}

		tag = GameTagManager.playerTag;

		currentSpeed = normalSpeed;
		if (manager.north == LevelManager.NorthDirection.forwards)
		{
			northDir = Vector3.forward;
		}
		else if (manager.north == LevelManager.NorthDirection.backwards)
		{
			northDir = Vector3.back;
		}
		else if (manager.north == LevelManager.NorthDirection.left)
		{
			northDir = Vector3.left;
		}
		else
		{
			northDir = Vector3.right;
		}

		if (compass != null)
		{
			compassRenderer = compass.GetComponent<MeshRenderer>();
			compassRenderer.materials[1].EnableKeyword("_EMISSION");
		}

		ShowLightGem();
		myMap = GetComponent<PlayerMap>();
        myMap.control = this;
        DisplayObjectives();

		playerInventory = GetComponent<Inventory>();
		healthbar.text = string.Format("Health: {0}", health);
		Cursor.visible = false;
	}
	
	// Update is called once per frame;
	// only use for input and movement
	void Update () 
	{
		if (!GameTagManager.isPaused)
		{
			UnityEngine.Profiling.Profiler.BeginSample("PlayerControl.Update");
			
            myXAxis = Input.GetAxis("Horizontal");
            myZAxis = Input.GetAxis("Vertical");
            myTransform.Translate(Vector3.right * myXAxis * currentSpeed * Time.deltaTime);

            // The player can climb ladders if not exhausted
            if (usingLadder && !isExhausted)
            {
                myTransform.Translate(Vector3.up * myZAxis * currentSpeed * Time.deltaTime);
            }
            else
            {
                myTransform.Translate(Vector3.forward * myZAxis * currentSpeed * Time.deltaTime);
            }

            // if not too tired and not crouching, jump
            if (canJump && !jumping)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
            
			// would be forward, but Blender and Unity have different axes
			compass.transform.forward = northDir;
            float xAngle = Vector3.Angle(myTransform.forward, neckBone.forward);

			// workaround for an issue with Input.GetAxis on Linux
			if (Input.GetButton ("Rotate Left")) 
			{
				myTransform.Rotate(Vector3.up * -mouseSensitivity * Time.deltaTime);
			}
			if (Input.GetButton ("Rotate Right")) 
			{
				myTransform.Rotate(Vector3.up * mouseSensitivity * Time.deltaTime);
			}
			if (Input.GetButton ("Rotate Up")) 
			{
				neckBone.Rotate(Vector3.right * -mouseSensitivity * Time.deltaTime);
			}
            if (Input.GetButton ("Rotate Down")) 
			{
				neckBone.Rotate(Vector3.right * mouseSensitivity * Time.deltaTime);
			}

			// crouching
			if (Input.GetButtonDown("Crouch"))
			{
				Crouch();
			}

            if (!jumping)
            {
                leaningLeft = Input.GetButton("Lean Left");
                leaningRight = Input.GetButton("Lean Right");
                LeanPlayer();
            }
			

			// rotate around the y-axis using the mouse - x-axis to come later
			myTransform.Rotate(0f, Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, 0f);

			// rotating up and down: limit it to the camera only
			// and change to the neck bone once animation is done
            neckBone.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * -mouseSensitivity * Time.deltaTime);
            ClampRotation();
			
			
			if (Input.GetButtonDown ("Fire1") && canPlayerAttack )
			{
				playerInventory.UseCurrentWeapon ();
			}

			if (Input.GetButtonDown ("Map"))
			{
				ShowMap();
			}

			if (Input.GetButtonDown("Night Vision"))
			{
				ToggleNightVision();
			}

			// if the movement input axes are 0, then the player isn't moving
			// stop the audio if this is true
			if (myXAxis == 0 && myZAxis == 0)
			{
				if (myAudio.isPlaying)
				{
					Invoke("StopMovementSound", 0.2f);
				}
				if (endurance < maxEndurance)
				{
					endurance += Time.deltaTime;
					breathBar.text = string.Format("Breath: {0}", (endurance / maxEndurance).ToString("#.##") );

					if (playerVoice.isBreathing && endurance > 10)
					{
						playerVoice.StopBreathing();
					}
				}
			}
			else
			{
				ShowLightGem();

				if (endurance > 0)
				{
					endurance -= ((currentMovement == MoveType.running ? 5f : 0f) * Time.deltaTime);
					breathBar.text = string.Format("Breath: {0}",(endurance / maxEndurance).ToString("#.##"));
					if (!myAudio.isPlaying && currentMovement != MoveType.sneaking)
					{
						Invoke("ChangeMovementSound", 0.3f);
					}
					if (endurance < 10)
					{
						playerVoice.PlayBreathing();
					}
				}
				else if (endurance <= 0f && !isExhausted)
				{
					StartCoroutine(ToggleExhaustion());
				}
					
			}

			// enable sneaking
			if (Input.GetButtonDown("Sneak") && canChangeMovement )
			{
				if (currentMovement == MoveType.normal)
				{
					ChangeMoveType(MoveType.sneaking);
				}
				else
				{
					ChangeMoveType(MoveType.normal);
				}
				
				canChangeMovement = false;
				Invoke("AllowMovementChange", 0.1f);
			}
			
			// enable sprinting
			if (Input.GetButtonDown("Sprint") && canChangeMovement && !isCrouching)
			{
				ChangeMoveType(MoveType.running);
				canChangeMovement = false;
				Invoke("AllowMovementChange", 0.1f);
			}
			
			if (Input.GetButtonDown ("Pause") && canPause)
			{
				PauseGame();
			}

			// show the Dev Console. Hide in production...or convert to sillyness only.
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				devConsole.Show();
			}

            if (Input.GetButtonDown("Fire2"))
            {
                playerInventory.UseAimingCamera();
            }

			// change weapons
			if (Input.GetButton("Cycle Weapons"))
			{
				playerInventory.StartCoroutine(playerInventory.SwitchUp());
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				playerInventory.ChangeToRanged();
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				playerInventory.ChangeToMelee();
			}
			else if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				playerInventory.ChangeToSockPuppets();
			}
			else if(Input.GetKeyDown(KeyCode.Alpha4))
			{
				playerInventory.ChangeToLogic();
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				playerInventory.ClearWeapon();
			}
			UnityEngine.Profiling.Profiler.EndSample();
		}

	}

    /// <summary>
    /// Clamps the rotation.
    /// </summary>
    void ClampRotation()
    {
        Vector3 eulerAngles = neckBone.eulerAngles;
        float dot = Vector3.Dot(neckBone.forward, Vector3.up);

        if (dot > 0)
        {
            if (eulerAngles.x < 280f)
            {
                neckBone.Rotate(Vector3.right * 2f);
            }
        }
        else
        {
            if (eulerAngles.x > 70f)
            {
                neckBone.Rotate(Vector3.left * 2f);
            }
        }

    }


	/// <summary>
	/// Shows the light gem.
	/// </summary>
	void ShowLightGem()
	{
		lightGemUpdated += Time.deltaTime;
		if (lightGemUpdated >= lightGemUpdateTime)
		{
			lightGemUpdated = 0f;

			float myIllumination = myLightCalcuation.GetIllumination();

			if (myIllumination >= 0.8f)
			{
				compassRenderer.materials[1].SetColor("_EmissionColor", Color.yellow * 0.8f);
			}
			else if (myIllumination >= 0.6f && myIllumination < 0.8f)
			{
				compassRenderer.materials[1].SetColor("_EmissionColor", Color.yellow * 0.6f);
			}
			else if (myIllumination >= 0.4f && myIllumination < 0.6f)
			{
				compassRenderer.materials[1].SetColor("_EmissionColor", Color.yellow * 0.4f);
			}
			else if (myIllumination >= 0.2f && myIllumination < 0.4f)
			{
				compassRenderer.materials[1].SetColor("_EmissionColor", Color.yellow * 0.2f);
			}
			else
			{
				compassRenderer.materials[1].SetColor("_EmissionColor", Color.yellow * 0f);
			}
		}

	}

	/// <summary>
	/// Allows the player to jump. If beside an obstacle that is low enough,
	/// the player will climb
	/// </summary>
	public void Jump()
	{
        jumping = true;
        lastJumped = Time.time;
        myBody.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
	}

	/// <summary>
	/// Changes the player's movement type.
	/// </summary>
	/// <param name="newMoveType">New move type.</param>
	void ChangeMoveType(MoveType newMoveType)
	{
		if (currentMovement != newMoveType)
		{
			currentMovement = newMoveType;
			if (currentMovement == MoveType.normal)
			{
                currentSpeed = normalSpeed;
			}

			if (currentMovement == MoveType.running)
			{
				if (!isExhausted)
				{
					currentSpeed = runSpeed;
				}
				else
				{
					ChangeMoveType(MoveType.normal);
				}
			}

			if (currentMovement == MoveType.sneaking)
			{
				currentSpeed = sneakSpeed;
			}
            Invoke("ChangeMovementSound", 0.5f);
		}
	}

    void ChangeMovementSound()
    {
        switch (currentMovement)
        {
            case MoveType.running:
                
                myAudio.volume = runVolume;
                myAudio.clip = sprinting;
                if (!myAudio.isPlaying && (Mathf.Abs(myXAxis) > 0 || Mathf.Abs(myZAxis) > 0))
                {
                    myAudio.Play();
                }
                break;
            case MoveType.normal:
                myAudio.volume = normalVolume;
                myAudio.clip = footsteps;
                if (!myAudio.isPlaying && (Mathf.Abs(myXAxis) > 0 || Mathf.Abs(myZAxis) > 0))
                {
                    myAudio.Play();
                }
                break;
            case MoveType.sneaking: 
                StopMovementSound();
                break;
        }
    }

    void StopMovementSound()
    {
        myAudio.Stop();
    }

	/// <summary>
	/// Allows the movement type to be changed.
	/// </summary>
	void AllowMovementChange()
	{
		canChangeMovement = true;
	}

	/// <summary>
	/// Allows the player to crouch.
	/// </summary>
	void Crouch()
	{
		if (isCrouching)
		{
			isCrouching = false;
			myCollider.height = 2f;
			myTransform.Translate(Vector3.up * 0.8f);
			Camera.main.transform.Translate(Vector3.up * 0.2f);
			currentSpeed /= crouchSpeedMultiplier;
            canJump = true;
		}
		else
		{
			if (currentMovement == MoveType.running)
			{
				ChangeMoveType(MoveType.normal);
			}

			isCrouching = true;
			myCollider.height = 1f;
			Camera.main.transform.Translate(Vector3.down * 0.2f);
			currentSpeed *= crouchSpeedMultiplier;
            canJump = false;
		}
	}

	/// <summary>
	/// Damages the player.
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="type">Type.</param>
	public void DamagePlayer(int damage, GameTagManager.AttackMode type)
	{
		GameTagManager.LogMessage("Player took {0} points of {1} damage", damage, type);
		health -= Mathf.RoundToInt(damage * GameTagManager.GetDifficultyMultiplier());
		healthbar.text = string.Format("Health: {0}", health);

		if (health <= 0)
		{
			healthbar.text = string.Format("You died from a {0} attack", type);
			manager.FailMission(true);
		}
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void PauseGame()
	{
		pauseCanvas.gameObject.SetActive(true);
		playerCanvas.gameObject.SetActive(false);
		canPlayerAttack = false;
        canPause = false;
		myAudio.Stop();
		manager.Pause();
        DisplayObjectives();
	}

	/// <summary>
	/// Resumes the game. Called from a button.
	/// </summary>
	public void ResumeGame()
	{
		pauseCanvas.gameObject.SetActive(false);
		playerCanvas.gameObject.SetActive(true);
		manager.Resume();
		StartCoroutine(DelayResume());
	}

	/// <summary>
	/// Prevents the player from accidentally attacking just after pausing.
	/// </summary>
	/// <returns>The resume.</returns>
	IEnumerator DelayResume()
	{
		yield return new WaitForSeconds(0.5f);
		canPlayerAttack = true;
        canPause = true;
	}

	/// <summary>
	/// Allows the player to return to the main menu
	/// </summary>
	public void Exit()
	{
		GameTagManager.LogMessage ("Player is exiting");
		Time.timeScale = 1f;
		GameTagManager.ChangeLevel("Main Menu");
	}

	void OnDisable()
	{
		myAudio.Stop();
	}

	public void DisplayObjectives()
	{
		objectiveText.text = string.Empty;
		for (int i = 0; i < manager.objectives.Count; i++)
		{
			string text = string.Concat(manager.objectives[i].ToString(), "\n");
			objectiveText.text += text;
		}
	}

	public void Save()
	{
		GameTagManager.LogMessage("Player is saving");
		manager.SaveGame(health);
		StartCoroutine(WaitForSave());
	}

    /// <summary>
    /// Disable buttons while waiting to save.
    /// </summary>
    /// <returns>The for save.</returns>
	IEnumerator WaitForSave()
	{
		var buttons = pauseCanvas.GetComponentsInChildren<Button>();
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = false;
		}
		while (GameTagManager.isSaving)
		{
			yield return new WaitForSeconds(0.1f);
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = true;
		}
	}

	public void Restart()
	{
		manager.RestartLevel();
	}

	/// <summary>
	/// Shows the map.
	/// </summary>
	/// <returns>The map.</returns>
	public void ShowMap()
	{
        myAudio.Stop();
		GameTagManager.PauseGame();
		playerCanvas.SetActive(false);
		GameTagManager.LogMessage("Showing Map: Player Map is {0}", myMap);
		myMap.ActivateMap();
	}

	/// <summary>
	/// Increments the map image.
	/// </summary>
	public void IncrementMap()
	{
		myMap.ShowNextImage();
	}

	/// <summary>
	/// Decrements the map image.
	/// </summary>
	public void DecrementMap()
	{
		myMap.ShowPreviousImage();
	}

	/// <summary>
	/// Hides the map.
	/// </summary>
	/// <returns>The map.</returns>
	public void HideMap()
	{
		// this is a hacky workaround for the Canvas apparently being unassigned
		if (playerCanvas == null)
		{
			playerCanvas = GameObject.Find("UI").transform.Find("Player Canvas").gameObject;
			GameTagManager.LogMessage("playerCanvas was unassigned. It is now {0}", playerCanvas);
		}
		if (myMap == null)
		{
			GameTagManager.LogMessage("PlayerMap is null?!?!? Should be {0}", gameObject.GetComponent<PlayerMap>());
			myMap = GetComponent<PlayerMap>();
		}
		myMap.DeactivateMap();
        ResumeGame();
	}

    void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Walkable") || coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            StartCoroutine(CheckIfJumping(myTransform.position));
        }
    }

    IEnumerator CheckIfJumping(Vector3 originalPosition)
    {
        yield return new WaitForSeconds(1f);
        if (myTransform.position.y - originalPosition.y > 1f && !jumping)
        {
            GameTagManager.LogMessage("Player is actually jumping!");
            jumpPosition = originalPosition;
            jumping = true;
            lastJumped = Time.time - 1f;
        }
    }

	void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.layer == LayerMask.NameToLayer("Walkable") || coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
            if (jumping)
            {
                jumping = false;
                float jumpTime = Time.time - lastJumped;
                lastJumped = Time.time;
                GameTagManager.LogMessage("Player landed on {0}. Jump time was {1}", coll.gameObject, jumpTime);
                if (jumpTime > 1f && Mathf.Abs(jumpPosition.y - myTransform.position.y) > 1f)
                {
                    // play footstep noises if they jump and land.
                    if (!myAudio.isPlaying)
                    {
                        myAudio.Play();
                    }
//                    if (jumpTime >= maxJumpTime)
//                    {
//                        DamagePlayer(Mathf.RoundToInt(10 * jumpTime), GameManager.AttackMode.melee);
//                    }
                }
            }

		}
	}

	void ToggleNightVision()
	{
		nightVision.enabled = !nightVision.enabled;
        playerInventory.ToggleNightVision();
	}

	/// <summary>
	/// Lean the player
	/// </summary>
	void LeanPlayer()
	{
		if (leaningLeft)
		{
            canJump = false;
			revertFromLeft = true;
			revertFromRight = false;
			if (Vector3.Angle(neckBone.up, Vector3.up) <= 20f)
			{
				myTransform.Rotate(0f, 0f, 20f * Time.deltaTime);
			}

		}
		else if (leaningRight) 
		{
            canJump = false;
			revertFromLeft = false;
			revertFromRight = true;
			if (Vector3.Angle(neckBone.up, Vector3.up) <= 20f)
			{
				myTransform.Rotate(0f, 0f, -20f * Time.deltaTime);
			}
		}
		else if (revertFromLeft)
		{
			canJump = false;
			myTransform.Rotate(0f, 0f, -20f * Time.deltaTime);
			if (Vector3.Angle(neckBone.right, Vector3.up) >= 90f)
			{
				revertFromLeft = false;
                canJump = true;
			}
		}
		else if (revertFromRight)
		{
			canJump = false;
			myTransform.Rotate(0f, 0f, 20f * Time.deltaTime);
			if (Vector3.Angle(neckBone.right, Vector3.up) <= 90f)
			{
				revertFromRight = false;
                canJump = true;
			}
		}
	}

    public void ToggleClimbing(bool isClimbing)
    {
        usingLadder = isClimbing;
        myBody.useGravity = !isClimbing;
    }

	/// <summary>
	/// When the player is exhausted, they can't move for 2-3 seconds.
	/// </summary>
	/// <returns>The exhaustion.</returns>
	IEnumerator ToggleExhaustion()
	{
		isExhausted = true;
        currentSpeed = sneakSpeed * crouchSpeedMultiplier;
		canPlayerAttack = false;
        canJump = false;
		// play noise here
		myAudio.Stop();

		// Wait 2-3 seconds to catch the player's breath
		yield return new WaitForSeconds(1f);
		isExhausted = false;
		canPlayerAttack = true;
        ChangeMoveType(MoveType.normal);
        canJump = true;

	}

    public void AssignPublicVariables()
    {
        breathBar = GameObject.Find("Player Breath").GetComponent<Text>();
        healthbar = GameObject.Find("Player Health").GetComponent<Text>();
        pauseCanvas = GameObject.Find("Pause Canvas").GetComponent<Canvas>();
        objectiveText = GameObject.Find("Objectives Text").GetComponent<Text>();
        devConsole = GetComponentInChildren<DevConsole>();
        manager = FindObjectOfType<LevelManager>();
    }

}
