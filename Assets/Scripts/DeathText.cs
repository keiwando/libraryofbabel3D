using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

[RequireComponent(typeof(Text))]
public class DeathText : MonoBehaviour {


	private Text text;

	void Start () {

		text = GetComponent<Text>();

		foreach (Transform child in transform){
			child.gameObject.SetActive(false);
		}
	}

	public void Activate(){
		foreach(Transform child in transform){
			child.gameObject.SetActive(true);
		}
	}

	public void SetText(string t){

		if (text == null)
			return;

		text.text = t;
	}
}
