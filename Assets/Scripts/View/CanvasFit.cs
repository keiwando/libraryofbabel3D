using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFit : MonoBehaviour {

	public float refAspectWidth = 16f;
	public float refAspectHeight = 9f;

	void Start () {

		var canvasScaler = GetComponent<CanvasScaler>();

		var screenAspect = (float)Screen.width / Screen.height;
		var refAspect = refAspectWidth / refAspectHeight;


		if (screenAspect > refAspect) {
			// Screen is too wide -> match height
			canvasScaler.matchWidthOrHeight = 1f;
		} else {
			// Screen is too tall -> match width
			canvasScaler.matchWidthOrHeight = 0f;
		}
	}
	

}
