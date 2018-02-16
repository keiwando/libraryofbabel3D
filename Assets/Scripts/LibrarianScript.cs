using UnityEngine;
using System.Collections;
using System.IO;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
//using Parse;

public class LibrarianScript : Escapable {

	public MathFunctions universe;
	public FirstPersonController fpc;

	public SearchInterfaceScript search;
	private PageInterfaceScript pageInterface;
	public PageInterfaceScript singlePageInterface;
	[SerializeField] private PageInterfaceScript doublePageInterface;
	public ChoiceIndicatorScript choiceIndicator;

	public WallScript SelectedWall { get; set; }
	public ShelfScript SelectedShelf { get; set; }

	public int selectedStage;	//0 = nothing selected | 1 = wall selected | 2 = shelf selected | 3 = book selected | 4 = searchInterface
	private int selectedWall;
	private int selectedShelf;
	private int selectedBook;
	private int selectedPage;
	private string selectedTitle;
	private string[] titles;

	private bool cmdPressed;
	private bool escPressed;
	public bool backPressed;

	private Vector3 swipeStartPosition;
	private Vector3 swipeEndPosition;

	private int fallCount;
	private const int maxFallNum = 3;

	private string deathText;
	public DeathText deathTextObject;

	// Use this for initialization
	void Start () {
		cmdPressed = false;
		escPressed = false;
		backPressed = false;

		selectedStage = 0;
		selectedBook = -1;
		selectedShelf = -1;
		selectedWall = -1;
		selectedPage = 0;
		selectedTitle = "";

		titles = null;

		fallCount = 0;

		swipeStartPosition = swipeEndPosition = Vector3.zero;

		universe.generateRandomHexagonNumber();
		universe.setRandomHexagonNameInBase36();

		choosePageInterface();

		chooseDeathText();
	}
	
	// Update is called once per frame
	void Update () {
		keyPressHandling();
	}

	private void keyPressHandling(){
		if(cmdPressed && escPressed){
			Application.Quit();

		}
		if(Input.GetKeyDown(KeyCode.RightCommand) || Input.GetKeyDown(KeyCode.LeftCommand)){
			cmdPressed = true;
		}
		if(Input.GetKeyUp(KeyCode.RightCommand) || Input.GetKeyUp(KeyCode.LeftCommand)){
			cmdPressed = false;
		}
		if(Input.GetKeyUp(KeyCode.Escape)){
			escPressed = false;
		}
		if(Input.GetKeyDown(KeyCode.Escape)){
			EscapeClicked();
			//pageInterface.setVisible(false);
		}
		if(Input.GetKeyDown(KeyCode.M)){
			if(selectedStage != 4){
				
				showSearchInterface();
			}
		}
	}

	override public void EscapeClicked(){
		escPressed = true;
		choiceIndicator.setVisible(true);
	}

	private void swipeHandling(){
		if (Input.GetMouseButtonDown(0))    // swipe begins
		{
			swipeStartPosition = swipeEndPosition = Input.mousePosition;
			print("Swipe began");
		}
		if (Input.GetMouseButtonUp(0))    // swipe ends
		{
			swipeEndPosition = Input.mousePosition; //Camera.main.ScreenToWorldPoint(
			print("Swipe ended");
		}

		if (swipeStartPosition != swipeEndPosition && swipeStartPosition != Vector3.zero && swipeEndPosition != Vector3.zero)
		{
			float swipeDistance = Vector3.Distance(swipeStartPosition,swipeEndPosition);
			print("Screen.width: " + Screen.width + ", Distance: " + swipeDistance);

			if(swipeDistance >= Screen.width * 0.5){
				backSwipe();
			}

			swipeStartPosition = swipeEndPosition = Vector3.zero;
		}
	}

	private void backSwipe(){
		choiceIndicator.setVisible(true);
		pageInterface.setVisible(false);
		search.setVisible(false);
		selectedStage = 0;

		float colliderRadius = 40;
		GameObject currentObject;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, colliderRadius);
		for(int i = 0; i < hitColliders.Length; i++) {
			currentObject = hitColliders[i].gameObject;

			if (currentObject.GetComponent<WallScript>() != null){
				currentObject.GetComponent<WallScript>().reset();
			}else if (currentObject.GetComponent<ShelfScript>() != null){
				currentObject.GetComponent<ShelfScript>().reset();
			}else if (currentObject.GetComponent<BookScript>() != null){
				currentObject.GetComponent<BookScript>().reset();
			}
		}
	}

	public void showSearchInterface(){
		choiceIndicator.setVisible(false);
		search.setVisible(true);
		pageInterface.setVisible(false);
		lockCameraUnlockMouse();
		selectedStage = 4;
	}

	private void choosePageInterface(){
		if(true){
			//print("WebGL");
			pageInterface = doublePageInterface;
		}else{
			pageInterface = singlePageInterface;
		}
	}

	public void lockCameraUnlockMouse(){
		fpc.setLocked(true);
	}

	public void lockMouseUnlockCamera(){
		fpc.setLocked(false);
	}

	public void increaseFallCount(){
		fallCount++;
		if(fallCount > maxFallNum){
			if(!Application.isMobilePlatform){
				Application.Quit();		// Quit the Application on Standalone builds
			} else {
				deathTextObject.activate();
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);				// Respawn on mobile
			}
		}
	}

	private void chooseDeathText(){

		string[] sentences = readTextFile("FILEPATH MISSING");
		int pickedSentenceCount = 1;

		int startIndex = Random.Range(0,sentences.Length - pickedSentenceCount -1);

		deathText = "";

		for(int i = startIndex; i < startIndex + pickedSentenceCount; i++){
			deathText += sentences[i] + ". ";
		}

		deathTextObject.setText(deathText);
	}

	private string[] readTextFile(string file_path)
	{
		
		TextAsset txt = (TextAsset)Resources.Load("LibraryofBabel", typeof(TextAsset));
		string content = txt.text;

		//divide string into sentences and return array
		return content.Split(new char[] {'.',';'});
	}

	public string requestPage(){
		//createRoomPos in Mathfunctions
		int roomPosition = universe.calcRoomPosNumber(selectedWall,selectedShelf,selectedBook,selectedPage);
		print ("Roomposition: " + roomPosition);
		//run the algorithm
		universe.algorithm(roomPosition);

		return universe.getPageFromData();
	}

	public void movedToNextRoom(){
		//increase hex number by 1
		universe.addToHexNumber36(1);
	}

	public void movedToPreviousRoom(){
		//decrease hex number by 0
		universe.addToHexNumber36(-1);
	}

	public void movedToRoomAbove(){
		//increase each part of hex number by 66666
		universe.addToAllHexNumbers36(66666);
	}

	public void movedToRoomBelow(){
		//subtract each part of hex number by 66666
		universe.addToAllHexNumbers36(-66666);
	}

	public void selectWall(int w, WallScript wall) {

		if (SelectedWall != null) {
			SelectedWall.deactivate();	
		}

		selectedWall = w;
		SelectedWall = wall;
	}

	public void selectWall(int w){
		selectedWall = w;
	}

	public void selectShelf(int s, ShelfScript shelf) {

		if (SelectedShelf != null) {
			SelectedShelf.deactivate();
		}

		selectedShelf = s;
		SelectedShelf = shelf;
	}

	public void selectShelf(int s){
		selectedShelf = s;
	}

	public void selectBook(int b){
		selectedBook = b;
	}

	public void setSelectedPage(int p){
		selectedPage = p;
	}

	public int getSelectedPage(){
		return selectedPage;
	}

	public int getSelectedWall(){
		return selectedWall;
	}

	public int getSelectedShelf(){
		return selectedShelf;
	}

	public int getSelectedBook(){
		return selectedBook;
	}

	public bool isReadingBook() {
		return pageInterface.isVisible();
	}

	public void setRoomPosition(int[] roomposition){
		//roomposition[0] equals the page (between 0-409
		selectedPage = roomposition[1];
		//roomposition[1] equals the book number
		selectedBook = roomposition[2];
		//roomposition[2] equals the shelf number
		selectedShelf = roomposition[3];
		//roomposition[3] equals the wall number
		selectedWall = roomposition[0];
	}

	public void setWallIndicator(int i){
		choiceIndicator.wall = i;
	}

	public void setShelfIndicator(int i){
		choiceIndicator.shelf = i;
	}

	public void setBookIndicator(int i){
		choiceIndicator.book = i;
	}

	public void updateIndicator(){
		choiceIndicator.updateText();
	}

	public void resetIndicator(){
		pageInterface.setPositionIndication(choiceIndicator.toString());
		pageInterface.setTitle(selectedTitle);
		choiceIndicator.reset();
	}

	public void setTitle(string t){
		selectedTitle = t;
		pageInterface.setTitle(t);
	}

	public void setTitles(string[] t){
		titles = t;
		updateTitle();
	}

	public void updateTitle(){
		if(titles != null && selectedBook >= 0 && selectedBook < 32){
			pageInterface.setTitle(titles[selectedBook]);
		}
	}

	public void setIndicatorVisible(bool b){
		choiceIndicator.setVisible(b);
	}

	public PageInterfaceScript getPageInterface(){
		return pageInterface;
	}
		
}
