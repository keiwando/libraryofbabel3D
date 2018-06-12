using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroOrientation : MonoBehaviour {

	//private Queue<Vector3> filterDataQueue = new Queue<Vector3>();
	//[SerializeField]
	//private int filterLength = 3;

	public Quaternion Attitude {
		get { return attitude; }
	}
	private Quaternion attitude = Quaternion.identity;

	//private Quaternion originalOrientation;

	void Start () {
		//for(int i = 0; i < filterLength; i++)
		//	filterDataQueue.Enqueue(Input.acceleration); //filling the queue to requered length

		if (Settings.VREnabled) {
			Input.gyro.enabled = true;
			attitude = Input.gyro.attitude;
			Input.gyro.updateInterval = 0.0167f;
		}
	}

	void Update () {

		//attitude *= Quaternion.Euler(Input.gyro.rotationRateUnbiased * Time.deltaTime); 
		attitude *= Quaternion.Euler(Input.gyro.rotationRate);


		// = Input.gyro.attitude;

		/*Vector3 gyroAngles = gyro rotationalVelocity * dT;
		Vector3 accelAngles = Vector3.Cross(localUp, Input.acceleration accelerometer.sensedUp - localUp);
		Vector3 magnetoAngles = Vector3.Cross(localNorth, magnetometer.sensedNorth - localNorth);

		Vector3 estimatedUp = localUp + Vector3.Cross(angularDelta, localUp);
		Vector3 estimatedNorth = localNorth + Vector3.Cross(angularDelta, localNorth);

		float error = Vector3.Dot(estimatedUp, estimatedNorth) * 0.5f;

		localUp = Vector3.normalize(estimatedUp – error * estimatedNorth);
		localNorth = Vector3.normalize(estimatedNorth – error * estimatedUp);*/
	}
		
	/*void OnGUI() {

		var gyro = Input.gyro;

		GUI.Label(new Rect(500, 300, 200, 40), "Gyro rotation rate " + gyro.rotationRate);
		GUI.Label(new Rect(500, 350, 200, 40), "Gyro attitude" + gyro.attitude);
		GUI.Label(new Rect(500, 400, 200, 40), "Integr. attitude" + attitude);

	}*/
		

	/*public Vector3 LowPassAccelerometer() {

		if(filterLength <= 0)
			return Input.acceleration;
		filterDataQueue.Enqueue(Input.acceleration);
		filterDataQueue.Dequeue();

		Vector3 vFiltered= Vector3.zero;
		foreach(Vector3 v in filterDataQueue)
			vFiltered += v;
		vFiltered /= filterLength;
		return vFiltered;
	}*/
}
