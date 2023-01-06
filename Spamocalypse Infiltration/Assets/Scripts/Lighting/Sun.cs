using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tracks a list of objects and checks if they are in the sun or not.
/// </summary>

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour {

	public List<CalculateLight> lightees;
	public float maxRange = 20f;

	RaycastHit hit;
	Vector3 myForward;

	Light myLight;
	float myIntensity;

	// Use this for initialization
	void Start () 
	{
		myForward = transform.forward;
		myLight = GetComponent<Light>();
		myIntensity = myLight.intensity;
        GatherLightees();
		
	}

	// Runs on the Physics timestep - could use a CoRoutine or InvokeRepeating instead.
	void FixedUpdate () 
	{
		for (int i = 0; i< lightees.Count; i++)
		{
			Transform trans = lightees[i].GetTransform();
			if (Physics.Raycast(trans.position + (Vector3.up * 0.5f) - myForward * maxRange, myForward * maxRange, out hit))
			{
				if (hit.transform == trans)
				{
					if (!lightees[i].isInSun)
					{
						lightees[i].SetSun(true, myIntensity);
					}
				}
				else
				{
					if (lightees[i].isInSun)
					{
						lightees[i].SetSun(false, myIntensity);
					}
				}
			}
		}
	}

	public void GatherLightees()
	{
		lightees = new List<CalculateLight>();
		var tmp = GameObject.FindObjectsOfType<CalculateLight>();
		for (int i = 0; i < tmp.Length; i++)
		{
			lightees.Add(tmp[i]);
		}
	}

	public void SetBrightness(float newBrightness)
	{
		myIntensity = newBrightness;
	}

	public float GetIntensity()
	{
		return myIntensity;
	}
}
