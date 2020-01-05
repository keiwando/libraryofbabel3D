#if UNITY_IOS || UNITY_ANDROID
#define MOBILE
#endif


using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Shelf: MonoBehaviour {

	public Librarian librarian { get; set; }
	public int Number { get; set; }
	public bool IsSelected { get; set; }

	public Wall Wall { get; set; }
	public Book SelectedBook { get; set; }

	public ShelfLocation Location {
		get { 
			return new ShelfLocation() {
				Hex = Wall.Hex.location,
				Wall = Wall.Number,
				Shelf = Number
			};
		}
	}

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


	void Start () {

		IsSelected = false;
		SelectedBook = null;

		boxCollider = GetComponent<BoxCollider>();
	}

	public void GenerateBooks() {

		books = new Book[Universe.BOOKS_PER_SHELF];

		for (int i = 0; i < Universe.BOOKS_PER_SHELF; i++) {

			var offset = Random.Range(-bookDistanceVariance, bookDistanceVariance);
			var distance = i * bookDistance + offset;

			var newBookGO = Instantiate(firstBook.gameObject, transform, true) as GameObject;
			newBookGO.transform.localPosition = firstBook.transform.localPosition + new Vector3(distance, 0, 0);
			newBookGO.name = "Book " + (i + 1);

			var book = newBookGO.GetComponent<Book>();
			book.librarian = librarian;
			book.Number = i + 1;
			book.Shelf = this;

			books[i] = book;
		}

		firstBook.gameObject.SetActive(false);
	}

#if MOBILE
	void OnMouseUp() {
#else
	void OnHoverMouseUp(){
#endif
		
		if (!librarian.CanSelect())
			return;
		
		Wall.ShelfSelected(this);
		Select();
	}

#if MOBILE
	void OnMouseOver() {
#else
	void OnHover(){
#endif

		highlight.SetActive(true);
		librarian.HoveringOver(this);
	}
	
#if MOBILE
	void OnMouseExit() {
#else
	void OnHoverExit(){
#endif

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

	public void SetTitles(string[] titles) {
	
		for (int i = 0; i < Mathf.Min(titles.Length, books.Length); i++) {

			books[i].Title = titles[i];
		}

		if (SelectedBook != null) {
			ViewController.Find().SetCurrentBookTitle(SelectedBook.Title);
		}
	}


}
