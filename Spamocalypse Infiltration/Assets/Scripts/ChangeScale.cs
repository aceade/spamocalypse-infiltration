using UnityEngine;

/// <summary>
/// Uniformly changes the scale of an object.
/// </summary>
public class ChangeScale : MonoBehaviour {

	bool isScaled;

	public void Scale(float scalingFactor)
	{
		if (isScaled)
		{
			isScaled = false;
			transform.localScale = Vector3.one;
			GameTagManager.LogMessage("{0} has a normal scale", this);

		}
		else
		{
			isScaled = true;
			transform.localScale = (Vector3.one * scalingFactor);
			GameTagManager.LogMessage("{0} has a scale of {1}", this, transform.localScale);
		}
	}

	public override string ToString ()
	{
		return name;
	}
}
