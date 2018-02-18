using UnityEngine;
using System.Collections;

public class OnlyVisibleOnMobile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(!PlatformHelper.IsPlatformMobile()){
			gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
