using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextMeshFit : MonoBehaviour {

	private TextMesh textMesh;

	void Start () {

		textMesh = GetComponent<TextMesh>();
		Refresh();
	}
	
	public void Refresh() {


	}
}
