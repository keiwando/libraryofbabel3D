using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageTextView : MonoBehaviour {

	public string Text {
		set { 
			smallText.text = value;
			scrollText.text = value;
		}
		get { 
			return smallText.text;
		}
	}

	[SerializeField]
	private Text smallText;
	[SerializeField]
	private GameObject scrollContainer;
	[SerializeField]
	private Text scrollText;

	void Start() {
		smallText.gameObject.SetActive(true);
		scrollContainer.SetActive(false);
	}

	void OnMouseDown() {

		smallText.gameObject.SetActive(!smallText.gameObject.activeSelf);
		scrollContainer.SetActive(!scrollContainer.activeSelf);
	}

}
