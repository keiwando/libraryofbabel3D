using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	[SerializeField] private float zVertDif;
	[SerializeField] private float xDiagDif;
	[SerializeField] private float zDiagDif;
	[SerializeField] private float baseHeight;

	[SerializeField] private int radius;

	void Start () {
		transform.GetChild(0).gameObject.SetActive(true);
		SpawnAtRandomPosition();
	}

	private void SpawnAtRandomPosition(){
		int direction = Random.Range(0,5);
		int factor = Random.Range(-radius,radius);
		int height = Random.Range(-radius,radius);
		
		MoveFire(direction,factor);
		ChangeHeight(height);
	}

	private void ChangeHeight(int height){
		Vector3 newPosition = gameObject.transform.position;
		newPosition.y += (baseHeight * height);
		gameObject.transform.position = newPosition;
	}

	private void MoveFire(int direction, int factor){
		float moveX = 0;
		float moveZ = 0;
		switch(direction){
		case 0:	moveX = 0;
			moveZ = zVertDif;
			break;
		case 1: 
			moveX = xDiagDif;
			moveZ = zDiagDif;
			break;
		case 2:
			moveX = xDiagDif;
			moveZ = -zDiagDif;
			break;
		case 3:
			moveX = 0;
			moveZ = -zVertDif;
			break;
		case 4:
			moveX = -xDiagDif;
			moveZ = -zDiagDif;
			break;
		case 5:
			moveX = -xDiagDif;
			moveZ = zDiagDif;
			break;
		}

		Vector3 newPosition = gameObject.transform.position;
		newPosition.x += (moveX * factor);
		newPosition.z += (moveZ * factor);
		gameObject.transform.position = newPosition;
	}
}
