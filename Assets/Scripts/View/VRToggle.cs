using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

public class VRToggle : MonoBehaviour {

	public Toggle toggle;

	void Start () {

		Input.gyro.enabled = Settings.VREnabled;
		Input.gyro.updateInterval = 0.0167f;
		toggle.isOn = Input.gyro.enabled;
	}

	public void VRToggled(bool val){
		
		Input.gyro.enabled = val;
		Settings.VREnabled = val;
	}
}
