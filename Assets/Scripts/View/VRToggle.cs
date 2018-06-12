using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

public class VRToggle : MonoBehaviour {

	public GameObject joystick;
	public Toggle toggle;

	//private const string VR_KEY = "VR_ENABLED";

	void Start () {

		joystick.SetActive(false);

		Input.gyro.enabled = Settings.VREnabled;
		Input.gyro.updateInterval = 0.0167f;
		toggle.isOn = Input.gyro.enabled;
	}

	public void VRToggled(bool val){
		
		Input.gyro.enabled = val;
		Settings.VREnabled = val;
	}

}
