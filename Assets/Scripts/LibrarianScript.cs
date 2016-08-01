using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
//using Parse;

public class LibrarianScript : MonoBehaviour {

	public MathFunctions universe;
	public FirstPersonController fpc;

	public SearchInterfaceScript search;
	private PageInterfaceScript pageInterface;
	public PageInterfaceScript singlePageInterface;
	[SerializeField] private PageInterfaceScript doublePageInterface;
	public ChoiceIndicatorScript choiceIndicator;

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

	private int fallCount;
	private const int maxFallNum = 3;

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

		universe.generateRandomHexagonNumber();
		universe.setRandomHexagonNameInBase36();

		choosePageInterface();
		//universe.testStringToHexNumberMethods();

		//ParseAnalytics.TrackAppOpenedAsync ();
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
			escPressed = true;
			choiceIndicator.setVisible(true);
		}
		if(Input.GetKeyDown(KeyCode.M)){
			if(selectedStage != 4){
				/*
				choiceIndicator.setVisible(false);
				search.setVisible(true);
				pageInterface.setVisible(false);
				lockCameraUnlockMouse();
				selectedStage = 4;
				*/
				showSearchInterface();
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
			print("WebGL");
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
			Application.Quit();
		}
	}

	public string requestPage(){
		//createRoomPos in Mathfunctions
		int roomPosition = universe.calcRoomPosNumber(selectedWall,selectedShelf,selectedBook,selectedPage);
		print ("Roomposition: " + roomPosition);
		//run the algorithm
		universe.algorithm(roomPosition);

		//ParseAnalytics.TrackEventAsync("PAGE REQUESTED");

		return universe.getPageFromData();
	}

	public void movedToNextRoom(){
		//increase hex number by 1
		/*
		if(PlayerPrefs.GetInt("CONNECTED") == 0 && Application.platform != RuntimePlatform.OSXWebPlayer
		   && Application.platform != RuntimePlatform.WindowsWebPlayer){
			universe.addToHexNumber(1);
		}else{
		*/
			universe.addToHexNumber36(1);
		//}

	}

	public void movedToPreviousRoom(){
		//decrease hex number by 0
		/*
		if(PlayerPrefs.GetInt("CONNECTED") == 0 && Application.platform != RuntimePlatform.OSXWebPlayer
		   && Application.platform != RuntimePlatform.WindowsWebPlayer){
			universe.addToHexNumber(-1);
		}else{
		*/
			universe.addToHexNumber36(-1);
		//}
	}

	public void movedToRoomAbove(){
		//increase each part of hex number by 66666
		/*
		if(PlayerPrefs.GetInt("CONNECTED") == 0 && Application.platform != RuntimePlatform.OSXWebPlayer
		   && Application.platform != RuntimePlatform.WindowsWebPlayer){
			universe.addToAllHexNumbers(66666);
		}else{
		*/
			universe.addToAllHexNumbers36(66666);
		//}
	}

	public void movedToRoomBelow(){
		//subtract each part of hex number by 66666
		/*
		if(PlayerPrefs.GetInt("CONNECTED") == 0 && Application.platform != RuntimePlatform.OSXWebPlayer
		   && Application.platform != RuntimePlatform.WindowsWebPlayer){
			universe.addToAllHexNumbers(-66666);
		}else{
		*/
			universe.addToAllHexNumbers36(-66666);
		//}
	}

	public void selectWall(int w){
		selectedWall = w;
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
