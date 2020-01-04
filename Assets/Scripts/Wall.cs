#if UNITY_IOS || UNITY_ANDROID
#define MOBILE
#endif

using UnityEngine;
using System.Collections;

public class Wall: MonoBehaviour {

	public Hexagon Hex { get; set; }
	
	public int Number { get; set; }
	public bool IsSelected { get; set; }

	public Shelf SelectedShelf { get; set; }

	[SerializeField]
	private GameObject highlight;

	private BoxCollider[] boxColliders;

	[SerializeField]
	private Shelf firstShelf;
	[SerializeField]
	private float ShelfDistance = 1f;

	private Librarian librarian;

	void Start () {
		
		this.boxColliders = GetComponents<BoxCollider>();

		SelectedShelf = null;
	}

	public void Setup(Librarian librarian) {

		this.librarian = librarian;

		GenerateShelves();

		GetComponent<CombineChildren>().Combine();
	}

#if MOBILE
	void OnMouseUp() {
#else
	void OnHoverMouseUp(){
#endif

		if (!librarian.CanSelect())
			return;

		librarian.WallSelected(this);
		Select();
	}

#if MOBILE
	void OnMouseOver() {
#else
	void OnHover() {
#endif

		if (!IsSelected && !librarian.IsSwipingCamera) {
			highlight.SetActive(true);
			librarian.HoveringOver(this);
		}
	}
	
#if MOBILE
	void OnMouseExit() {
#else
	void OnHoverExit() {
#endif
		
		highlight.SetActive(false);
		librarian.HoveringOverEnded(this);
	}

	private void GenerateShelves() {

		for (int i = 0; i < 5; i++) {

			var newShelfPos = firstShelf.transform.position;
			newShelfPos.y += i * ShelfDistance;

			var newShelfGO = Instantiate(firstShelf.gameObject, transform, true) as GameObject;
			newShelfGO.transform.position = newShelfPos;
			newShelfGO.name = "Shelf " + (i + 1);
			var shelf = newShelfGO.GetComponent<Shelf>();
			shelf.librarian = librarian;
			shelf.Wall = this;
			shelf.Number = i + 1;

			shelf.GenerateBooks();
		}

		firstShelf.GenerateBooks();
		firstShelf.gameObject.SetActive(false);
	}

	public void Select(){
		
		IsSelected = true;
		highlight.SetActive(false);

		foreach (var collider in boxColliders) {
			collider.enabled = false;
		}
	}

	public void Deselect(){

		if (SelectedShelf != null) {
			SelectedShelf.Deselect();
		}
		SelectedShelf = null;

		IsSelected = false;
		foreach (var collider in boxColliders) {
			collider.enabled = true;
		}
	}

	public void ShelfSelected(Shelf shelf) {

		if (SelectedShelf != null) {
			SelectedShelf.Deselect();
		} 
		SelectedShelf = shelf;
		librarian.ShelfSelected(shelf);
	}

	public void BookSelected(Book book) {

		librarian.BookSelected(book);
	}
}
