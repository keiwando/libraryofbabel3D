using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {

	public HexagonGeneratorScript generator;
	public GameObject parent;
	private Vector3 inwardVector;
	public bool stairTrigger;
	// Use this for initialization
	void Start () {
		inwardVector = parent.transform.position - this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider collider){
		generator.triggerEntered(collider);
		//updateinwardVector
		inwardVector = parent.transform.position - this.transform.position;
	}

	void OnTriggerExit(Collider collider){
		if(!stairTrigger){
			generator.triggerLeft(collider,inwardVector);
		}else{
			generator.stairTriggerLeft(collider);
		}
	}
}
