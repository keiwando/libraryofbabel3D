using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class DeathText : MonoBehaviour {

	public static string Story = "";

	[SerializeField]
	private Text text;

	void Start () {

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
