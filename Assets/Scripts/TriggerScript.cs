using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {

	private LibraryMovementSimulator movementSimulator;
	private Vector3 inwardVector;
	public bool stairTrigger;

	private Collider col;
	// Use this for initialization
	void Start () {
		inwardVector = transform.parent.transform.position - this.transform.position;

		movementSimulator = LibraryMovementSimulator.Find();

		this.col = GetComponent<Collider>();
	}

	void OnTriggerEnter(Collider collider){
		movementSimulator.TriggerEntered(collider);
		//updateinwardVector
		inwardVector = transform.parent.transform.position - this.transform.position;
	}

	void OnTriggerExit(Collider collider){
		
		if(!stairTrigger){
			movementSimulator.TriggerLeft(col, inwardVector);
		}else{
			movementSimulator.StairTriggerLeft(col);
		}
	}
}
