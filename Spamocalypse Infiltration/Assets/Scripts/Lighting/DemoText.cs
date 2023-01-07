using UnityEngine;
using UnityEngine.UI;

public class DemoText : MonoBehaviour {

	[Tooltip("This one shows the player's current light intensity")]
	public Text lightText;

	[Tooltip("This one shows you if the player is in the sun")]
	public Text sunText;

	public CalculateLight player;
	
	// Update is called once per frame
	void Update () 
	{
		sunText.text = string.Format("Player is {0} in the sun", player.isInSun ? "" : "not ");
		lightText.text = string.Format("Player illumination is {0}", player.GetIllumination());
	}
}
