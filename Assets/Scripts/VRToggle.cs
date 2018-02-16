using UnityEngine;
using System.Collections;

public class VRToggle : MonoBehaviour {

	public GameObject joystick;

	void Start () {
		joystick.SetActive(false);
	}

	void Update () {
	
	}

	public void toggleVR(){

		Input.gyro.enabled = !Input.gyro.enabled;

		joystick.SetActive(!joystick.activeSelf);
	}

}
