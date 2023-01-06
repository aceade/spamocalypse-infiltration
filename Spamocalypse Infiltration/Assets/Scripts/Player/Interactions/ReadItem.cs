using UnityEngine;
using System.Collections;

/// <summary>
/// Allows the player to read an item.
/// </summary>

public class ReadItem : PlayerInteraction {
	
	/// <summary>
	/// The Image that holds the pages
	/// </summary>
	public GameObject pages;

    bool isPolling;
    bool canInteract = true;

	protected override void Interact()
	{
        if (canInteract)
        {
            FreezePlayer();
            GameTagManager.PauseGame();
            pages.SetActive (true);
            isPolling = true;
            canInteract = false;
            StartCoroutine(PollForInput());
        }
		
	}

	public void Hide()
	{
		pages.SetActive(false);
		UnfreezePlayer();
		player.GetComponentInParent<PlayerControl>().ResumeGame();
        StartCoroutine(CanInteractAgain());
	}

    IEnumerator CanInteractAgain()
    {
        yield return new WaitForSeconds(1f);
        canInteract = true;
    }

    /// <summary>
    /// Polls for input so player can close.
    /// </summary>
    /// <returns>The for input.</returns>
    IEnumerator PollForInput()
    {
        while (isPolling)
        {
            yield return null;
            if (Input.GetButtonDown ("Use") || Input.GetButtonDown("Pause"))
            {
                GameTagManager.LogMessage("ReadItem is hiding");
                Hide();
                isPolling = false;
            }
        }
    }
}
