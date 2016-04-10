using UnityEngine;
using System.Collections;

public class FallTriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit(Collider collider){
		print ("fallen");
		GameObject.Find("Librarian").GetComponent<LibrarianScript>().increaseFallCount();
	}
}
