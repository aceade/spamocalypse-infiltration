using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for a weapon.
/// </summary>

public class Weapon : MonoBehaviour 
{	
	[Tooltip("How much damage does this do")]
	public int damage;

	[Tooltip("The rate of fire")]
	public float fireRate;

    public float secondaryRate = 0.2f;

	public AudioClip firingSound;

	protected Transform myTrans;

	protected WaitForSeconds primaryAttackCycle;
    protected WaitForSeconds secondaryAttackCycle;

	protected bool canAttack = false;

	RaycastHit hit;

	protected MeshRenderer myRenderer;

	protected Collider myCollider;

	protected AudioSource myAudio;

	protected Animator myAnimator;

	// Caching the layers for ease of reference.
	protected int playerLayer, spammerLayer;

	[Tooltip("What type of attack does this use?")]
	public GameTagManager.AttackMode myAttack;

	// Use this for initialization
	protected virtual void Start () 
	{
		myTrans = transform;
		playerLayer = LayerMask.NameToLayer("Player");
		spammerLayer = LayerMask.NameToLayer("Spammer");
		myRenderer = GetComponent<MeshRenderer>();
		myAudio = GetComponent<AudioSource>();
		myAnimator = GetComponent<Animator>();
		myCollider = GetComponent<Collider>();
		primaryAttackCycle = new WaitForSeconds(fireRate);
        secondaryAttackCycle = new WaitForSeconds(secondaryRate);
	}

	public bool IsAbleToAttack()
	{
		return canAttack;
	}

	/// <summary>
	/// Activate this weapon.
	/// </summary>
	public void Activate()
	{
		if (myRenderer != null)
		{
			myRenderer.enabled = true;
		}
		if (myCollider != null)
		{
			myCollider.enabled = true;
		}
		canAttack = true;
		gameObject.SetActive(true);
	}

	/// <summary>
	/// Deactivate this weapon.
	/// </summary>
	public void Deactivate()
	{
		if (myRenderer != null)
		{
			myRenderer.enabled = false;
		}
		if (myCollider != null)
		{
			myCollider.enabled = false;
		}
		canAttack = false;
	}

	/// <summary>
	/// Start attacking the specified target transform.
	/// </summary>
	/// <param name="target">Target.</param>
	public void StartAttack(Transform target)
	{
		if (!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
		}
		StartCoroutine(PrimaryAttack(target));
	}

    public void StartSecondaryAttack()
    {
        StartCoroutine(SecondaryAttack());
    }

	/// <summary>
	/// Start attacking from the current position in a specific direction.
	/// </summary>
	/// <param name="currentPosition">Current position.</param>
	/// <param name="attackDirection">Attack direction.</param>
	public void StartAttack(Vector3 currentPosition, Vector3 attackDirection)
	{
		if (!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
		}
		StartCoroutine(PrimaryAttack(currentPosition, attackDirection));
	}

	public virtual IEnumerator PrimaryAttack(Transform attackTarget)
	{
		yield return primaryAttackCycle;
	}

	public virtual IEnumerator PrimaryAttack(Vector3 position, Vector3 direction)
	{
		yield return primaryAttackCycle;
	}

    public virtual IEnumerator SecondaryAttack()
    {
        yield return primaryAttackCycle;
    }

	protected virtual void CheckHit(Transform hitTransform)
	{
		Transform collTrans = hitTransform.root;
		var damageScript = collTrans.GetComponent<IDamage>();
		if (damageScript != null)
		{
			damageScript.Damage(GameTagManager.AttackMode.spam, damage, collTrans.position - transform.position);
		}
	}

	/// <summary>
	/// Allows the user to attack again.
	/// </summary>
	protected virtual void ResetAttack()
	{
		canAttack = true;
	}
}
