using UnityEngine;
using System.Collections;

public class GyroView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, 0); 
	}

	void FixedUpdate() {
		transform.Rotate(Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, 0); 
	}
}
