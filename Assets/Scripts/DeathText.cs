using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class DeathText : MonoBehaviour {

	// Use this for initialization
	public Text text;

	void Start () {
		foreach(Transform child in transform){
			child.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void activate(){
		foreach(Transform child in transform){
			child.gameObject.SetActive(true);
		}
	}

	public void setText(string t){
		text.text = t;
	}
}
