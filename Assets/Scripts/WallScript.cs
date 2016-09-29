using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	public LibrarianScript librarian;
	public Light wallLight;
	public int wallNumber;

	private bool activated;

	// Use this for initialization
	void Start () {
		removeLightBug();

		activated = false;

		//turn Wallnumber into index
		wallNumber--;
	}
	
	// Update is called once per frame
	void Update () {
		if(activated){
			GetComponent<BoxCollider>().enabled = false;
			//librarian.selectedStage = 1;
			backClick();
		}else{
			GetComponent<BoxCollider>().enabled = true;
		}
	}

	void OnMouseDown(){
		if(librarian.selectedStage == 0){
			print(wallNumber + 1);
			activated = true;
			librarian.selectedStage = 1;
			librarian.selectWall(wallNumber);
		}
	}

	void OnMouseOver(){
		if(librarian.selectedStage == 0){
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

	public void backClick(){
		if(Input.anyKeyDown){
			if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)
			   ){
				if(!librarian.backPressed){
					if(librarian.selectedStage == 1) librarian.selectedStage = 0;
					librarian.resetIndicator();
					activated = false;
				}
			}
		}
		librarian.backPressed = false;
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
