﻿using UnityEngine;

public class ShowMouseSensitivity : MonoBehaviour {

	public static float speed = 70f;

	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * speed * Time.deltaTime);
	}
}
