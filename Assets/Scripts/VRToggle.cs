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
		if(Input.gyro.enabled){
			Input.gyro.enabled = false;
			joystick.SetActive(true);
		} else {
			Input.gyro.enabled = true;
			joystick.SetActive(false);
		}
	}

}
