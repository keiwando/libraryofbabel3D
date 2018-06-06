using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Book: MonoBehaviour {

	public Librarian librarian { get; set; }

	public int Number { get; set; }
	public bool IsSelected { get; set; }

	public Shelf Shelf { get; set; }

	public string Title { 
		get { return title; }
		set { 
			title = value;
			titleLabel.text = title;
		} 
	}
	private string title;

	private Text titleLabel;

	//public Light bookLight;
	[SerializeField]
	private GameObject highlight;

	void Start () {
		
		titleLabel = GetComponentInChildren<Text>();
		titleLabel.text = "";

		//bookLight.enabled = false;
		IsSelected = false;
	}

	void OnMouseOver(){

		librarian.HoveringOver(this);
		//bookLight.enabled = true;
		highlight.SetActive(true);
	}

	void OnMouseExit(){
		
		librarian.HoveringOverEnded(this);
		//bookLight.enabled = false;
		highlight.SetActive(false);
	}

	void OnMouseUp(){

		if (!librarian.CanSelect())
			return;

		Shelf.BookSelected(this);
		IsSelected = true;
	}

	public void Deselect() {

		IsSelected = false;
		//bookLight.enabled = false;
		highlight.SetActive(false);
	} 
}
