using UnityEngine;
using System.Collections;

public class OnlyIOS : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// Deactivate gameObject if not on IOS
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXEditor){
			this.gameObject.SetActive(false);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
