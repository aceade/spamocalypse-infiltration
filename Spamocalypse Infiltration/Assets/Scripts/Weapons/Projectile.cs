using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for fire-and-forget Projectiles.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour {

	public float launchVelocity;

    protected Vector3 myLaunchPosition;

	public ForceMode launchType;

	Rigidbody myBody;

	Transform myTransform;

	Renderer myRenderer;

	Collider myCollider;

	protected AudioSource myAudio;

	public List<AudioClip> hitNoises;

	// Use this for initialization
	protected void Start () {
		myTransform = transform;
		myRenderer = GetComponent<Renderer>();
		myBody = GetComponent<Rigidbody>();
		myCollider = GetComponent<Collider>();
		myAudio = GetComponent<AudioSource>();
		Sleep();
	}

	/// <summary>
	/// Launch from the specified launchPosition and in the specified launchDirection.
	/// </summary>
	/// <param name="launchPosition">Launch position.</param>
	/// <param name="launchDirection">Launch direction.</param>
	public void Launch(Vector3 launchPosition, Vector3 launchDirection)
	{
		myTransform.position = launchPosition;
		myTransform.forward = launchDirection;
        myLaunchPosition = launchPosition;
        myBody.isKinematic = false;
		myRenderer.enabled = true;
		myBody.AddForce(launchDirection * launchVelocity, launchType);
		myCollider.enabled = true;
	}

	/// <summary>
	/// Disables the renderer, the rigidbody and the collider.
	/// </summary>
	public void Sleep()
	{
		myCollider.enabled = false;
		myRenderer.enabled = false;
        myBody.velocity = Vector3.zero;
	}

	/// <summary>
	/// Plays the hit effects.
	/// </summary>
	/// <returns>The hit effects.</returns>
	protected virtual void PlayHitEffects(Collider coll)
	{
        // noises go here, as they are common
        myAudio.clip = hitNoises[Random.Range(0, hitNoises.Count)];
        myAudio.Play();
		Sleep();
	}

	void OnCollisionEnter(Collision coll)
	{
		PlayHitEffects(coll.collider);
	}
}
