﻿using UnityEngine;
using System.Collections;

public class FireScript : MonoBehaviour {

	[SerializeField] private float zVertDif;
	[SerializeField] private float xDiagDif;
	[SerializeField] private float zDiagDif;
	[SerializeField] private float baseHeight;

	[SerializeField] private int radius;

	[SerializeField] GameObject fire;

	// Use this for initialization
	void Start () {
		spawnAtRandomPosition();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void spawnAtRandomPosition(){
		int direction = Random.Range(0,5);
		int factor = Random.Range(-radius,radius);
		int height = Random.Range(-radius,radius);
		print("HEIGHT: " + height);
		moveFire(direction,factor);
		changeHeight(height);
	}

	private void changeHeight(int height){
		Vector3 newPosition = fire.transform.position;
		newPosition.y += (baseHeight * height);
		fire.transform.position = newPosition;
	}

	private void moveFire(int direction, int factor){
		float moveX = 0;	//left-right
		float moveZ = 0;	//up-down
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

		Vector3 newPosition = fire.transform.position;
		newPosition.x += (moveX * factor);
		newPosition.z += (moveZ * factor);
		fire.transform.position = newPosition;
	}
}
