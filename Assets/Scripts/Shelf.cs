using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Shelf: MonoBehaviour {

	public Librarian librarian { get; set; }
	public int Number { get; set; }
	public bool IsSelected { get; set; }

	public Wall Wall { get; set; }
	public Book SelectedBook { get; set; }

	[SerializeField]
	private GameObject highlight;

	private BoxCollider boxCollider;

	[SerializeField]
	private Book firstBook;
	[SerializeField]
	private float bookDistance;
	[SerializeField]
	private float bookDistanceVariance = 0f;

	private Book[] books;

	private string baseUrl = "https://libraryofbabel.info/titler.cgi?";

	private string regexp="<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";

	// Use this for initialization
	void Start () {

		IsSelected = false;
		SelectedBook = null;

		boxCollider = GetComponent<BoxCollider>();
	}

	public void GenerateBooks() {

		books[0] = firstBook;

		for (int i = 1; i < Universe.BOOKS_PER_SHELF; i++) {

			var offset = Random.Range(-bookDistanceVariance, bookDistanceVariance);
			var distance = i * bookDistance + offset;

			var newBookGO = Instantiate(firstBook.gameObject, transform, true) as GameObject;
			newBookGO.transform.localPosition = firstBook.transform.localPosition + new Vector3(distance, 0, 0);
			newBookGO.name = "Book " + (i + 1);

			var book = newBookGO.GetComponent<Book>();
			//book.librarian = librarian;
			book.Number = i;

			books[i] = book;
		}
	}

	/*private void RequestBookTitles(){
		string url = baseUrl;
		print(url);

		WWWForm form = new WWWForm();
		form.AddField("hex",GameObject.Find("Mathematics").GetComponent<MathFunctions>().getHexNumberBase36());
		form.AddField("wall",(librarian.getSelectedWall() + 1));
		form.AddField("shelf",(librarian.getSelectedShelf() + 1));

		StartCoroutine(WaitForRequest(url,form,(www) => {
			if (www.error == null || (www.text != null && www.text.Contains(";"))) {
				setBookTitles(www.text.Split(';'));	
			} else {
				
				string[] emptyTitles = new string[32];
				for(int i = 0; i < 32; i++){
					emptyTitles[i] = "";
				}
				setBookTitles(emptyTitles);
			}
		}));
	}

	private void SetBookTitles(string[] titles){
		librarian.setTitles(titles);
		foreach(Transform child in transform){
			if(child.gameObject.GetComponent<Book>() != null){
				Book book = child.gameObject.GetComponent<Book>();
				book.setTitle(titles[book.bookNumber]);
			}
		}
	}



	private IEnumerator WaitForRequest(string url, WWWForm form, System.Action<WWW> complete){
		WWW www = new WWW(url,form);
		yield return www;
		complete(www);
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
			Debug.Log("WWW text:" + www.text);
		}
	}

	private string generateUrl(){
		return baseUrl + GameObject.Find("Mathematics").GetComponent<MathFunctions>().getHexNumberBase36() + "-w" + (librarian.getSelectedWall() + 1) + "-s" + (librarian.getSelectedShelf() + 1);
	}*/

	void OnMouseDown(){
		//if(librarian.selectedStage == 1){
		//if (librarian.IsReadingBook() || librarian.isInMenu() || librarian.IsSwipingCamera) return;
		if (!librarian.CanSelect())
			return;
		

		Wall.ShelfSelected(this);
		Select();
	}

	void OnMouseOver(){
		//if(librarian.selectedStage == 1){

		highlight.SetActive(true);
		librarian.HoveringOver(this);
	}
	
	void OnMouseExit(){

		highlight.SetActive(false);
		librarian.HoveringOverEnded(this);
	}

	public void Select(){
		IsSelected = true;
		boxCollider.enabled = false;
	}

	public void Deselect(){
		
		IsSelected = false;
		if (SelectedBook != null) {
			SelectedBook.Deselect();
		}
		SelectedBook = null;

		boxCollider.enabled = true;
	}

	public void BookSelected(Book book){
	
		SelectedBook = book;
		Wall.BookSelected(book);
	}
}
