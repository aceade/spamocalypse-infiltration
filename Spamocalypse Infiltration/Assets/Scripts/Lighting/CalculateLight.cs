using UnityEngine;
using System.Collections.Generic;

public class CalculateLight : MonoBehaviour, ICalculateLight {

	public bool isInSun;

	private Transform myTransform;

	float totalIntensity, sunlightIntensity;

	[Tooltip("Set as true if you wish to clamp the maximum intensity to 1")]
	public bool clampLighting;

	[Tooltip("How often should this run")]
	public float rate = 0.2f;

	Dictionary<LightRadius, float> lights = new Dictionary<LightRadius, float>();

	// Use this for initialization
	void Start () {
		myTransform = transform;
		InvokeRepeating("CalculateLights", 0f, rate);
	}

	/// <summary>
	/// Calculates the intensity as the sum of all lights.
	/// </summary>
	void CalculateLights()
	{

		totalIntensity = 0;
		foreach (KeyValuePair<LightRadius, float> pair in lights)
		{
			totalIntensity += pair.Value;
		}
		if (isInSun)
		{
			totalIntensity += sunlightIntensity;
		}
		if (clampLighting)
		{
			totalIntensity = Mathf.Clamp(totalIntensity, 0f, 1f);
		}

	}

	/// <summary>
	/// Adds a specified light with a specific intensity.
	/// </summary>
	/// <param name="newLight">New light.</param>
	/// <param name="lightIntensity">Light intensity.</param>
	public void AddLight(LightRadius newLight, float lightIntensity)
	{
		lights[newLight] = lightIntensity;
	}

	/// <summary>
	/// Removes the specified light.
	/// </summary>
	/// <param name="oldLight">Old light.</param>
	public void RemoveLight(LightRadius oldLight)
	{
		if (lights.ContainsKey(oldLight))
		{
			lights.Remove(oldLight);
		}
	}

	/// <summary>
	/// Toggles whether or not the character is in the sun, and if so, sets the sun intensity.
	/// </summary>
	/// <param name="inSun">If set to <c>true</c> in sun.</param>
	/// <param name="sunIntensity">Sun intensity.</param>
	public void SetSun(bool inSun, float sunIntensity)
	{
		isInSun = inSun;
		if (inSun)
		{
			sunlightIntensity = sunIntensity;
		}
		else
		{
			sunlightIntensity = 0f;
		}
	}


	/// <summary>
	/// Gets the transform of the character.
	/// </summary>
	/// <returns>The transform.</returns>
	public Transform GetTransform()
	{
		return myTransform;
	}

	/// <summary>
	/// Gets the illumination of the character.
	/// </summary>
	/// <returns>The intensity.</returns>
	public float GetIllumination()
	{
		return totalIntensity;
	}

	/// <summary>
	/// Utility method that rounds the specified input to the specified number of decimal places.
	/// </summary>
	/// <returns>The to decimal.</returns>
	/// <param name="input">Input.</param>
	/// <param name="decimalPlaces">Decimal places.</param>
	static float RoundToDecimal(float input, int decimalPlaces)
	{
		float tmp = input * Mathf.Pow(10, decimalPlaces);
		tmp = Mathf.RoundToInt(tmp);
		tmp /= Mathf.Pow(10, decimalPlaces);
		return tmp;
	}
}
