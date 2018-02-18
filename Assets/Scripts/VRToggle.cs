using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

public class VRToggle : MonoBehaviour {

	public GameObject joystick;
	public Toggle toggle;

	private const string VR_KEY = "VR_ENABLED";

	void Start () {
		joystick.SetActive(false);

		Input.gyro.enabled = PlayerPrefs.GetInt(VR_KEY, 1) == 1;
		toggle.isOn = Input.gyro.enabled;
	}

	void Update () {
	
	}

	public void toggleVR(){

		Input.gyro.enabled = !Input.gyro.enabled;
		PlayerPrefs.SetInt(VR_KEY, Input.gyro.enabled ? 1 : 0);
		PlayerPrefs.Save();
	}

}
