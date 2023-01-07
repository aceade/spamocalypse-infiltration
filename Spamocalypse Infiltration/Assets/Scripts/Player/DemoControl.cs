using UnityEngine;
using System.Collections;

/// <summary>
/// Used to move the player object through or around an incomplete level.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class DemoControl : MonoBehaviour
{

	private Transform myTransform;
	private Rigidbody myRigidbody;

	public bool isGrounded = true;

	Vector3 myPosition, lastGroundedPosition;

	float myXAxis, myYAxis, myZAxis;

	public float speed = 5f;

	public float arrowLookSpeed = 10f;

	public float mouseSensitivity = 70f;

	public Camera myCamera;

	// Use this for initialization
	void Start ()
	{
		myTransform = transform;
		myRigidbody = GetComponent<Rigidbody> ();
		myPosition = transform.position;
		lastGroundedPosition = myPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
		myPosition = myTransform.position;

		// get the value of the axes
		myXAxis = Input.GetAxis ("Horizontal");
		myZAxis = Input.GetAxis ("Vertical");

		// use them to move
		myTransform.Translate (Vector3.forward * myZAxis * speed * Time.deltaTime);
		myTransform.Translate (Vector3.right * myXAxis * speed * Time.deltaTime);

		// up-down, if not grounded
		if (isGrounded) 
		{
			// press space to get off the ground
			if (Input.GetButton ("Jump")) 
			{
				LeaveGround ();
			} 
		}
		else 
		{
			if (Input.GetKey (KeyCode.Q)) 
			{
				myTransform.Translate (Vector3.down * speed * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.E) )
			{
				myTransform.Translate (Vector3.up * speed * Time.deltaTime);
			}

			if (Input.GetButton("Jump"))
			{
				LeaveGround();
			}
		}


		// workaround for an issue with Input.GetAxis on Linux
		if (Input.GetButton ("Rotate Left")) 
		{
			myTransform.Rotate(Vector3.up * -arrowLookSpeed * Time.deltaTime);
		}
		if (Input.GetButton ("Rotate Right")) 
		{
			myTransform.Rotate(Vector3.up * arrowLookSpeed * Time.deltaTime);
		}
		if (Input.GetButton ("Rotate Up")) 
		{
			myCamera.transform.Rotate(Vector3.right * arrowLookSpeed * Time.deltaTime);
		}
		if (Input.GetButton ("Rotate Down")) 
		{
			myCamera.transform.Rotate(Vector3.right * -arrowLookSpeed * Time.deltaTime);
		}
		
		
		// rotate around the y-axis using the mouse - x-axis to come later
		myTransform.Rotate(0f, Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, 0f);
		myCamera.transform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.R))
		{
			ResetPosition ();
		}

	}

	/// <summary>
	/// Resets the player's position to their last grounded position.
	/// </summary>
	/// <returns>The position.</returns>
	void ResetPosition ()
	{
		Debug.Log(string.Format("Resetting to {0}", lastGroundedPosition));
		myTransform.position = lastGroundedPosition;
		isGrounded = true;
		myRigidbody.useGravity = true;
	}

	/// <summary>
	/// Leaves the ground.
	/// </summary>
	/// <returns>The ground.</returns>
	void LeaveGround ()
	{
		Debug.Log("Leaving the ground");
		lastGroundedPosition = myPosition;
		isGrounded = false;
		myRigidbody.useGravity = false;
	}
}
