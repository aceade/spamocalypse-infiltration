using UnityEngine;
using System.Collections;

/// <summary>
/// Phisers just sit in one place and wait for somebody to come along. Then they cause an alarm.
/// </summary>

public class Phiser : MonoBehaviour {

	public LevelManager manager;
	
	void OnTriggerEnter(Collider coll)
	{
		manager.GeneralAlert(coll.transform);
	}
}
