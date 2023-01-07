using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tracks objects that are inside.
/// </summary>

[DisallowMultipleComponent]
[RequireComponent(typeof(Light))]
public class LightRadius : MonoBehaviour {

	Dictionary<Transform, ICalculateLight> lightObjects = new Dictionary<Transform, ICalculateLight>();

	Light myLight;
	float intensity;
	float range;

    Collider myCollider;

	RaycastHit hit;
	Vector3 myPosition;

	[Tooltip("If closer than this, add full intensity")]
	public float minDistance;

	delegate float calculateIntensity(Transform target);
	calculateIntensity MyAlgorithm;

	public float rate = 0.25f;
	WaitForSeconds delay;

	bool isProcessing;

    bool isActive;

	// Use this for initialization
	void Start () 
	{
		myLight = GetComponent<Light>();
        isActive = myLight.enabled;
		intensity = myLight.intensity;
		range = myLight.range;
		myPosition = transform.position;
        myCollider = GetComponent<Collider>();
        myCollider.enabled = isActive;
        myCollider.isTrigger = true;

		// Spot lights ignore range.
		if (myLight.type == LightType.Spot)
		{
			minDistance = myLight.range;
			MyAlgorithm = calculateSpotlightIntensity;
		}
		else
		{
			MyAlgorithm = calculatePointIntensity;
		}

		delay = new WaitForSeconds(rate);
	}

	void OnTriggerEnter(Collider coll)
	{
        var root = coll.transform.root;
		var lightObject = root.GetComponent<ICalculateLight>();

		if (lightObject != null && !lightObjects.ContainsKey(root) && isActive)
		{
			lightObjects.Add(root, lightObject);
			lightObject.AddLight(this, 0f);

			if (!isProcessing)
			{
				StartCoroutine(ProcessObjects());
			}
		}
	}

	/// <summary>
	/// Gets the light intensity.
	/// </summary>
	/// <returns>The light intensity.</returns>
	public float GetLightIntensity()
	{
		return intensity;
	}

	/// <summary>
	/// Returns the light attached to this.
	/// </summary>
	/// <returns>The light.</returns>
	public Light GetLight()
	{
		return myLight;
	}

	/// <summary>
	/// Processes the objects in the list.
	/// </summary>
	/// <returns>The objects.</returns>
	IEnumerator ProcessObjects()
	{
        isProcessing = true;
        while (isProcessing)
        {
            foreach (KeyValuePair<Transform, ICalculateLight> pair in lightObjects)
            {
                var root = pair.Key;
                float objIntensity = 0f;
                if (Physics.Raycast(myPosition + (Vector3.up * 0.5f), root.position +(Vector3.up * 0.5f) - myPosition, out hit, range))
                {
                    if (hit.transform.root == root)
                    {
                        objIntensity = MyAlgorithm(root);
                    }
                }
                lightObjects[root].AddLight(this, objIntensity);
            }
            yield return delay;
        }
		
	}

	void OnTriggerExit(Collider coll)
	{
		if (lightObjects.ContainsKey(coll.transform.root))
		{
			lightObjects[coll.transform.root].RemoveLight(this);
			lightObjects.Remove(coll.transform.root);
			if (lightObjects.Count <= 0)
			{
				StopCoroutine(ProcessObjects());
				isProcessing = false;
			}
		}
	}

	/// <summary>
	/// Calculates the spotlight intensity. These do not include any range check.
	/// </summary>
	/// <returns>The spotlight intensity.</returns>
	/// <param name="target">Target. Unused, but required</param>
	float calculateSpotlightIntensity(Transform target)
	{
		return intensity;
	}

	/// <summary>
	/// Calculates the point intensity by ignoring the y-axis.
	/// </summary>
	/// <returns>The point intensity.</returns>
	/// <param name="target">Target.</param>
	float calculatePointIntensity(Transform target) 
	{
		Vector3 position = target.position;
		position.y = myPosition.y;
		float distance = Vector3.Distance(myPosition, position);

		if (distance <= minDistance)
		{
			// if inside the minimum distance, don't attenuate.
			return intensity;
		}
		else if (distance >= range)
		{
			// if the distance happens to be greater than the light's range, ignore it.
			return 0;
		}
		else 
		{
			// otherwise, return the intensity as a proportion of the distance
			return intensity / (distance - minDistance);
		}

	}
    

    public override string ToString ()
    {
        return string.Format ("[LightRadius] {0} at {1}", name, myPosition);
    }

    /// <summary>
    /// Toggle the LightRadius.
    /// </summary>
    public void Toggle()
    {
        isActive = !isActive;
        if (isActive)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        if (myLight == null || myCollider == null)
        {
            myLight = GetComponent<Light>();
            myCollider = GetComponent<Collider>();
        }
        myLight.enabled = true;
        myCollider.enabled = true;
    }

    public void Deactivate()
    {
        myLight.enabled = false;
        myCollider.enabled = false;
        foreach (KeyValuePair<Transform, ICalculateLight> pair in lightObjects)
        {
            pair.Value.RemoveLight(this);
        }
        lightObjects.Clear();
    }
}
