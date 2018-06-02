using UnityEngine;
using System.Collections;

public class WallScript : Escapable {

	public LibrarianScript librarian;
	public Light wallLight;
	public int wallNumber;

	private bool activated;

	private BoxCollider boxCollider;

	public ShelfScript firstShelf;
	public float ShelfDistance = 1f;

	// Use this for initialization
	void Start () {
		this.boxCollider = GetComponent<BoxCollider>();
		removeLightBug();

		GenerateShelves();

		GetComponent<CombineChildren>().Combine();

		activated = false;

		//turn Wallnumber into index
		wallNumber--;
	}

	void Update () {
		if(activated){
			boxCollider.enabled = false;
			//librarian.selectedStage = 1;
			backClick();
		} else if (!boxCollider.enabled) {
			boxCollider.enabled = true;
		}
	}

	void OnMouseDown(){
		if (librarian.isReadingBook() || librarian.isInMenu() || librarian.IsSwipingCamera) return;
		//EscapeClicked();
		//if(librarian.selectedStage == 0){
		print(wallNumber + 1);
		activated = true;
		librarian.selectedStage = 1;
		librarian.selectWall(wallNumber, this);
		//}
	}

	void OnMouseOver(){
		//if(librarian.selectedStage == 0){
		if (librarian.SelectedWall != this && !librarian.IsSwipingCamera) {
			wallLight.enabled = true;
			librarian.setWallIndicator(wallNumber + 1);
			librarian.updateIndicator();
		}
	}
	
	void OnMouseExit(){
		wallLight.enabled = false;
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
			var shelf = newShelfGO.GetComponent<ShelfScript>();
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

	public void removeLightBug(){
		Vector3 newPos = wallLight.transform.position;
		wallLight.enabled = true;
		newPos.x ++;
		wallLight.transform.position = newPos;
		newPos.x --;
		wallLight.transform.position = newPos;
		wallLight.enabled = false;
	}
}
