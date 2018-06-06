using UnityEngine;
using System.Collections;

public class FallTriggerScript : MonoBehaviour {

	private Librarian librarian;

	void Start () {
		librarian = Librarian.Find();
	}

	void OnTriggerExit(Collider collider){
		print ("fallen");
		librarian.IncreaseFallCount();
	}
}
