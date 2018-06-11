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

			if (title == "") {
				
				canvas.gameObject.SetActive(false);
			} else {

				canvas.gameObject.SetActive(true);
				titleLabel.text = title;

				if (titleHideRoutine != null) {
					StopCoroutine(titleHideRoutine);
				}
				StartCoroutine(HideTitleAfterSeconds(hideCanvasTime));
			}
		} 
	}
	private string title;
	private Coroutine titleHideRoutine;
	private float hideCanvasTime = 40f;

	[SerializeField]
	private GameObject highlight;
	[SerializeField]
	private Canvas canvas;
	[SerializeField]
	private Text titleLabel;


	void Start () {

		Title = "";

		IsSelected = false;
	}

	void OnMouseOver(){

		librarian.HoveringOver(this);
		highlight.SetActive(true);
	}

	void OnMouseExit(){
		
		librarian.HoveringOverEnded(this);
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
		highlight.SetActive(false);
	} 

	private IEnumerator HideTitleAfterSeconds(float seconds) {

		yield return new WaitForSecondsRealtime(seconds);
		canvas.gameObject.SetActive(false);
	}
}
