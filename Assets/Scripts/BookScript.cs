using UnityEngine;
using System.Collections;

public class BookScript : MonoBehaviour {

	public Light bookLight;

	private LibrarianScript librarian;
	private PageInterfaceScript page;
	private TextMesh booktitle;
	private string title;
	private bool activated;
	public int bookNumber;

	private bool checkForDisabling = false;

	// Use this for initialization
	void Start () {
		librarian = GameObject.Find("Librarian").GetComponent<LibrarianScript>();
		page = librarian.getPageInterface();
		booktitle = GetComponentInChildren<TextMesh>();
		booktitle.text = "";
		setupTextMesh();
		bookLight.enabled = false;
		//test
		//bookLight.gameObject.SetActive(false);
		activated = false;

		removeLightBug();

		//turn booknumber into index
		bookNumber--;

		//TEST
		this.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		//wait for combine children to finish its work then deactivate object
		if(this.transform.parent.transform.parent.gameObject.GetComponent<CombineChildren>().getCompleted()){
			this.gameObject.SetActive(false);
			page = librarian.getPageInterface();
		}else{
			checkForDisabling = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(activated){
			backClick();
		}
		if(checkForDisabling){
			if(this.transform.parent.transform.parent.gameObject.GetComponent<CombineChildren>().getCompleted()){
				this.gameObject.SetActive(false);
				page = librarian.getPageInterface();
				checkForDisabling = false;
			}
		}
	}

	void OnMouseOver(){
		if(librarian.selectedStage == 2){
			bookLight.enabled = true;
		
			librarian.setWallIndicator(librarian.getSelectedWall() + 1);
			librarian.setShelfIndicator(librarian.getSelectedShelf() + 1);
			librarian.setBookIndicator(bookNumber + 1);
			librarian.updateIndicator();
		}
	}

	void OnMouseExit(){
		bookLight.enabled = false;
		if(librarian.selectedStage == 2){
			librarian.setWallIndicator(librarian.getSelectedWall() + 1);
			librarian.setShelfIndicator(librarian.getSelectedShelf() + 1);
			librarian.setBookIndicator(0);
			librarian.updateIndicator();
		}
	}

	void OnMouseDown(){
		print(bookNumber + 1);
		if(librarian.selectedStage == 2){
			activated = true;
			librarian.selectedStage = 3;
			librarian.selectBook(bookNumber);
			librarian.setTitle(booktitle.text);
			librarian.updateTitle();
			librarian.resetIndicator();

			librarian.lockCameraUnlockMouse();


			//show Page
			if(page == null){
				page = librarian.getPageInterface();
			}
			page.startPageRequestWithLoading();
			//page.setVisible(true);
			//page.requestPage();
		}
	}


	public void backClick(){
		if(Input.anyKeyDown){
			if(Input.GetKeyDown(KeyCode.Escape)){
				gameObject.GetComponentInParent<ShelfScript>().deactivate();
				gameObject.GetComponentInParent<ShelfScript>().GetComponentInParent<WallScript>().deactivate();
				print ("back");
				activated = false;

				librarian.lockMouseUnlockCamera();
				librarian.setBookIndicator(0);
				librarian.resetIndicator();

				//remove PageInterface
				page.setVisible(false);

				GameObject.Find("SoundController").GetComponent<SoundController>().bookClose();

				//deactivate book
				//this.gameObject.SetActive(false);
			}
		}
	}

	public void removeLightBug(){
		Vector3 newPos = bookLight.transform.position;
		bookLight.enabled = true;
		newPos.x ++;
		bookLight.transform.position = newPos;
		newPos.x --;
		bookLight.transform.position = newPos;
		bookLight.enabled = false;
	}

	public void setTitle(string t){
		booktitle.text = t;
		title = t;
	}

	public string getTitle(){
		return title;
	}

	private void setupTextMesh(){
		booktitle.fontSize = 13;
		booktitle.alignment = TextAlignment.Center;
		booktitle.color = Color.black;
		booktitle.anchor = TextAnchor.MiddleCenter;
		Vector3 position = booktitle.transform.position;
		position.y += 0.25f;
		position.x += 0.006f;
		booktitle.transform.position = position;
		GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	} 
}
