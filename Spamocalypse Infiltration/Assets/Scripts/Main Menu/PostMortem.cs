using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a post mortem screen so the player knows how they did.
/// </summary>
[ExecuteInEditMode]
public class PostMortem : MonoBehaviour {

    [Tooltip("The stats/credits panel")]
    public Transform panel;

    [Tooltip("How fast should the stats and credits play?")]
    public float speed = 2f;

    [Tooltip("The Text component that will display the text")]
    public Text statsText;

    public float maxHeight = 900f;

    int numberOfDeaths, numberOfKills, numberOfDecoysUsed, totalLootGathered;

	// Use this for initialization
	void Start ()
    {
        numberOfDeaths = PlayerPrefs.GetInt(GameTagManager.numberOfDeathsPrefs, 0);
        numberOfKills = PlayerPrefs.GetInt(GameTagManager.numberOfKillsPrefs, 0);
        numberOfDecoysUsed = PlayerPrefs.GetInt(GameTagManager.numberOfDetectionPrefs, 0);
        totalLootGathered = PlayerPrefs.GetInt(GameTagManager.numberOfLootPrefs, 0);

	    statsText.text = FormatStats();
	}

    /// <summary>
    /// Format the stats.
    /// </summary>
    /// <returns>A formatted string containing the player's total stats.</returns>
    string FormatStats()
    {
        return string.Format("Game Stats:\n\nPlayer Deaths: {0}\nSpammers Killed: {1}\nTimes Player Spotted: {2}\nLoot Acquired: {3}", numberOfDeaths, numberOfKills, numberOfDecoysUsed, totalLootGathered);
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetKey(KeyCode.Escape) || panel.transform.localPosition.y > maxHeight)
        {
            SkipStats();
        }
        panel.Translate(Vector3.up * speed * Time.deltaTime);
	}

    /// <summary>
    /// Skips the stats.
    /// </summary>
    public void SkipStats()
    {
        GameTagManager.ChangeLevel("Main Menu");
    }
}
