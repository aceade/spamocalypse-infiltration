using UnityEngine;

public interface ICalculateLight {

	/// <summary>
	/// Adds a specified light with a specific intensity.
	/// </summary>
	/// <param name="newLight">New light.</param>
	/// <param name="lightIntensity">Light intensity.</param>
	void AddLight(LightRadius newLight, float lightIntensity);

	/// <summary>
	/// Removes the specified light.
	/// </summary>
	/// <param name="oldLight">Old light.</param>
	void RemoveLight(LightRadius oldLight);

	/// <summary>
	/// Toggles whether or not the character is in the sun.
	/// </summary>
	/// <param name="inSun">If set to <c>true</c> in sun.</param>
	/// <param name = "sunIntensity">Intensity of the sun</param>
	void SetSun(bool inSun, float sunIntensity);

	/// <summary>
	/// Gets the transform of the character.
	/// </summary>
	/// <returns>The transform.</returns>
	Transform GetTransform();



}
