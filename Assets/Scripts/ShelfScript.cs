using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ShelfScript : Escapable {

	public LibrarianScript librarian;
	public Light shelfLight1;
	public Light shelfLight2;
	public int shelfNumber;

	private Light[] lights;

	[SerializeField]
	private BookScript firstBook;
	[SerializeField]
	private float bookDistance;
	[SerializeField]
	private float bookDistanceVariance = 0f;

	private bool activated;
	private bool pointedAt;

	private string baseUrl = "https://libraryofbabel.info/titler.cgi?";

	private string regexp="<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";

	// Use this for initialization
	void Start () {
		activated = false;
		lights = new Light[2];
		lights[0] = shelfLight1;
		lights[1] = shelfLight2;
		for(int i = 0; i < lights.Length; i++){
			lights[i].enabled = false;
		}

		//turn shelf number into index
		shelfNumber--;
	}
	
	// Update is called once per frame
	void Update () {
		if(activated){
			GetComponent<BoxCollider>().enabled = false;
			//librarian.selectedStage = 2;
			backClick();
		}else{
			GetComponent<BoxCollider>().enabled = true;
		}
	}

	public void GenerateBooks() {

		for (int i = 1; i < 32; i++) {

			var offset = Random.Range(-bookDistanceVariance, bookDistanceVariance);
			var distance = i * bookDistance + offset;

			var newBookGO = Instantiate(firstBook.gameObject, transform, true) as GameObject;
			newBookGO.transform.localPosition = firstBook.transform.localPosition + new Vector3(distance, 0, 0);
			newBookGO.name = "Book " + (i + 1);

			var book = newBookGO.GetComponent<BookScript>();
			book.bookNumber = i + 1;
		}
	}

	private void requestBookTitles(){
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

	private void setBookTitles(string[] titles){
		librarian.setTitles(titles);
		foreach(Transform child in transform){
			if(child.gameObject.GetComponent<BookScript>() != null){
				BookScript book = child.gameObject.GetComponent<BookScript>();
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
	}

	void OnMouseDown(){
		//if(librarian.selectedStage == 1){
		if (librarian.isReadingBook() || librarian.isInMenu() || librarian.IsSwipingCamera) return;
			
		print (shelfNumber + 1);
		activated = true;
		librarian.selectedStage = 2;
		librarian.selectShelf(shelfNumber, this);

		activateAllBooks();

		librarian.setShelfIndicator(shelfNumber + 1);
		librarian.updateIndicator();
		//}
	}

	void OnMouseOver(){
		//if(librarian.selectedStage == 1){
		if (librarian.SelectedShelf != this) {
			for(int i = 0; i < lights.Length; i++){
				lights[i].enabled = true;
			}
			librarian.setWallIndicator(librarian.getSelectedWall() + 1);
			librarian.setShelfIndicator(shelfNumber + 1);
			librarian.updateIndicator();
		}
	}
	
	void OnMouseExit(){
		for(int i = 0; i < lights.Length; i++){
			lights[i].enabled = false;
		}
		if(librarian.selectedStage == 1){
			librarian.setWallIndicator(librarian.getSelectedWall() + 1);
			librarian.setShelfIndicator(0);
			librarian.updateIndicator();
		}
	}

	public void backClick(){
		if(Input.anyKeyDown){
			if(Input.GetKeyDown(KeyCode.Backspace)){
				//go back one step
				if(!librarian.backPressed){
					if(librarian.selectedStage == 2) librarian.selectedStage = 1;
					gameObject.GetComponentInParent<WallScript>().activate();
					activated = false;

					librarian.setShelfIndicator(0);
					//deactivateBooks
					deactivateAllBooks();
				}
				librarian.backPressed = true;
			}else if(Input.GetKeyDown(KeyCode.Escape)){
				EscapeClicked();
			}
		}
	}

	public override void EscapeClicked (){
		
		if(activated){
			//go back to wall choice
			if(librarian.selectedStage == 2){
				librarian.selectedStage = 0;
				gameObject.GetComponentInParent<WallScript>().deactivate();
				activated = false;

				librarian.resetIndicator();

				deactivateAllBooks();
			}
		}
	}

	public void activate(){
		activated = true;
	}

	public void deactivate(){
		activated = false;
	}

	public void reset(){
		librarian.selectedStage = 0;
		gameObject.GetComponentInParent<WallScript>().deactivate();
		activated = false;

		librarian.resetIndicator();

		deactivateAllBooks();
	}

	private void activateAllBooks(){
		requestBookTitles();

		foreach(Transform child in transform){
			if(child.gameObject.GetComponent<BookScript>() != null){
				BookScript book = child.gameObject.GetComponent<BookScript>();
				child.gameObject.SetActive(true);
				book.removeLightBug();
			}
		}
	}

	private void deactivateAllBooks(){
		foreach(Transform child in transform){
			if(child.gameObject.GetComponent<BookScript>() != null){
				child.gameObject.SetActive(false);
			}
		}
	}
}
