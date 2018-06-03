using UnityEngine;
using System.Collections;

public class Wall : Escapable {

	public Librarian librarian;
	public int number;

	[SerializeField]
	private GameObject highlight;

	private bool activated = false;

	private BoxCollider[] boxColliders;

	[SerializeField]
	private Shelf firstShelf;
	[SerializeField]
	private float ShelfDistance = 1f;

	void Start () {
		
		this.boxColliders = GetComponents<BoxCollider>();

		GenerateShelves();

		GetComponent<CombineChildren>().Combine();
	}

	void Update () {
		if(activated){
			foreach (var col in boxColliders) {
				col.enabled = false;
			}

			//librarian.selectedStage = 1;
			backClick();
		} /*else if (!boxCollider.enabled) {
			boxCollider.enabled = true;
		}*/
	}

	void OnMouseDown(){
		
		if (librarian.isReadingBook() || librarian.isInMenu() || librarian.IsSwipingCamera) return;
		//EscapeClicked();
		//if(librarian.selectedStage == 0){
		print(number + 1);
		activated = true;
		librarian.selectedStage = 1;
		librarian.selectWall(number, this);
		//}
	}

	void OnMouseOver(){
		//if(librarian.selectedStage == 0){
		if (librarian.SelectedWall != this && !librarian.IsSwipingCamera) {
			//wallLight.enabled = true;
			highlight.SetActive(true);
			librarian.setWallIndicator(number + 1);
			librarian.updateIndicator();
		}
	}
	
	void OnMouseExit(){
		highlight.SetActive(false);
		if(librarian.selectedStage == 0){
			librarian.setWallIndicator(0);
			librarian.updateIndicator();
		}
	}

	private void GenerateShelves() {

		//firstShelf.GenerateBooks();

		for (int i = 1; i < 5; i++) {

			var newShelfPos = firstShelf.transform.position;
			newShelfPos.y += i * ShelfDistance;

			var newShelfGO = Instantiate(firstShelf.gameObject, transform, true) as GameObject;
			newShelfGO.transform.position = newShelfPos;
			newShelfGO.name = "Shelf " + (i + 1);
			var shelf = newShelfGO.GetComponent<Shelf>();
			shelf.GenerateBooks();
			shelf.shelfNumber = i + 1;
		}

		firstShelf.GenerateBooks();
	}

	public void backClick(){
		if(Input.anyKeyDown){
			if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)
			   ){
				EscapeClicked();
			}
		}
		librarian.backPressed = false;
	}

	public override void EscapeClicked (){
		if(activated){

			if(!librarian.backPressed){
				if(librarian.selectedStage == 1) librarian.selectedStage = 0;
				librarian.resetIndicator();
				activated = false;
			}
			librarian.backPressed = false;
		}
	}

	public void activate(){
		activated = true;
	}
	public void deactivate(){
		activated = false;
	}

	public void reset(){
		librarian.resetIndicator();
		activated = false;
	}
}
