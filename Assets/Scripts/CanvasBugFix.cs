using UnityEngine;
using System.Collections;

public class CanvasBugFix : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Set the canvas rect pivot
		var rectTransform = GetComponent<RectTransform>();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
		rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
