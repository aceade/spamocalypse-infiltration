using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Player map.
/// </summary>

public class PlayerMap : MonoBehaviour 
{
	
	public Button prevMapButton;
	public Button nextMapButton;

	public List<Sprite> mapImages;
	private int currentMapImage;

	public GameObject mapCanvas;
	public Image map;

    [HideInInspector]
    public PlayerControl control;

    bool isPolling;

	void Start()
	{
		if (mapImages.Count <= 1)
		{
			nextMapButton.gameObject.SetActive(false);
			prevMapButton.gameObject.SetActive(false);
		}
		map.sprite = mapImages[0];
	}

	public void ActivateMap()
	{
        GameTagManager.LogMessage("Showing map");
		mapCanvas.SetActive(true);
        isPolling = true;
        StartCoroutine(PollForInput());
	}

	public void DeactivateMap()
	{
		if (mapCanvas == null)
		{
			mapCanvas = GameObject.Find("UI").transform.Find("Map Canvas").gameObject;
			GameTagManager.LogMessage("mapCanvas was unassigned. It is now {0}", mapCanvas);
		}
		mapCanvas.SetActive(false);
        StopCoroutine(PollForInput());
	}

	public void ShowNextImage()
	{
		CheckAssignments();

		currentMapImage++;
		if (currentMapImage == mapImages.Count)
		{
			currentMapImage = mapImages.Count - 1;
			nextMapButton.enabled = false;
		}
		if (mapImages.Count > 1)
		{
			prevMapButton.enabled = true;
		}
		map.sprite = mapImages[currentMapImage];

	}

	public void ShowPreviousImage()
	{
		CheckAssignments();
		currentMapImage--;
		if (currentMapImage <= 0)
		{
			currentMapImage = 0;
			prevMapButton.enabled = false;
		}
		if (mapImages.Count > 1)
		{
			nextMapButton.enabled = true;
		}

		map.sprite = mapImages[currentMapImage];
	}

	/// <summary>
	/// Checks that the required objects are assigned.
	/// </summary>
	void CheckAssignments()
	{
        if (mapCanvas == null)
        {
         mapCanvas = GameObject.Find("Map Canvas");
        }

		if (nextMapButton == null)
		{
			nextMapButton = GameObject.Find ("Next Image").GetComponent<Button>();
			GameTagManager.LogMessage("Next map button was null! It is now {0}", nextMapButton);
		}
		if (prevMapButton == null)
		{
			prevMapButton = GameObject.Find("Previous Image").GetComponent<Button>();
			GameTagManager.LogMessage("Previous map button was null! It is now {0}", prevMapButton);
		}

		if (map == null)
		{      
			map = GameObject.Find("Map").GetComponent<Image>();
			GameTagManager.LogMessage("Map image was null! It is now {0}", map);
		}

	}

    public void AssignPublicVariables()
    {
        CheckAssignments();
    }

    IEnumerator PollForInput()
    {
        while (isPolling)
        {
            yield return null;
            if (Input.GetButtonDown ("Map") || Input.GetButtonDown ("Pause"))
            {
                control.HideMap();
                isPolling = false;
            }

        }
    }

}
