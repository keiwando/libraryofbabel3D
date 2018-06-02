using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {

	public HexagonGeneratorScript generator;
	public GameObject parent;
	private Vector3 inwardVector;
	public bool stairTrigger;

	private Collider col;
	// Use this for initialization
	void Start () {
		inwardVector = parent.transform.position - this.transform.position;

		this.col = GetComponent<Collider>();
	}

	void OnTriggerEnter(Collider collider){
		generator.TriggerEntered(collider);
		//updateinwardVector
		inwardVector = parent.transform.position - this.transform.position;
	}

	void OnTriggerExit(Collider collider){
		if(!stairTrigger){
			generator.TriggerLeft(col, inwardVector);
		}else{
			generator.StairTriggerLeft(col);
		}
	}
}
