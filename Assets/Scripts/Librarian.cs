using UnityEngine;
using System.Collections;
using System.IO;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(FirstPersonController))]
public class Librarian : MonoBehaviour {

	private enum Selection {
		None = 0,
		Wall,
		Shelf, 
		Book,
		Search,
		Settings
	}

	private FirstPersonController fpc;

	[SerializeField]
	private ViewController viewController;

	private Selection selection;

	[SerializeField]
	private Hexagon currentHexagon;
	private Wall selectedWall;

	private Vector3 swipeStartPosition;
	private Vector3 swipeEndPosition;
	public bool IsSwipingCamera {
		get { return isSwipingCamera; }
	}
	private bool isSwipingCamera = false;
	private bool touchExistsInSwipeArea = false;

	private int fallCount;
	private const int maxFallNum = 3;

	private string deathText;
	public DeathText deathTextObject;

	void Start () {

		fpc = GetComponent<FirstPersonController>();

		selection = Selection.None;

		fallCount = 0;

		swipeStartPosition = swipeEndPosition = Vector3.zero;

		//universe.generateRandomHexagonNumber();
		//universe.setRandomHexagonNameInBase36();

		ChooseDeathText();
	}

	void Update () {
		KeyPressHandling();

		if (PlatformHelper.IsPlatformMobile() || PlatformHelper.IsPlatformEditor()) {
			SwipeHandling();
		}
	}

	private void KeyPressHandling(){
		
		if(Input.GetKeyDown(KeyCode.Escape)){
			EscapeClicked();
		}

		if(Input.GetKeyDown(KeyCode.M)){
			if(selection != Selection.Search){
				
				ShowSearchInterface();
			}
		}
	}

	public void EscapeClicked(){
		
		viewController.ShowChoiceIndicator();
		viewController.CloseAllMenus();

		DeselectAll();

		LockMouseUnlockCamera();
	}

	public void RequestPages(PageLocation[] pages, OnPageRequestCompleted onComplete) {
		
	} 

	private Touch[] FilterSwipeValidTouches(Touch[] touches) {

		return touches.Where(t => t.position.magnitude > Screen.width / 2.5 && t.position.y > Screen.height * 0.2).ToArray();
	}

	private void RotateCamera(Vector2 rotateVector, float rotationSpeed){
		
		fpc.transform.Rotate(rotateVector.y * -rotationSpeed, rotateVector.x * rotationSpeed, 0, Space.Self);
		float z = fpc.transform.eulerAngles.z;
		fpc.transform.Rotate(0, 0, -z);
	}

	public void ShowSearchInterface(){
		
		viewController.ShowSearchMenu();
		
		LockCameraUnlockMouse();
		selection = Selection.Search;
	}

	public void LockCameraUnlockMouse(){
		fpc.setLocked(true);
	}

	public void LockMouseUnlockCamera(){
		fpc.setLocked(false);
	}

	public void IncreaseFallCount(){
		fallCount++;
		if(fallCount > maxFallNum){
			if(!Application.isMobilePlatform && !Application.platform != RuntimePlatform.WebGLPlayer){
				Application.Quit();		// Quit the Application on Standalone builds
			} else {
				deathTextObject.activate();
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);				// Respawn on mobile
			}
		}
	}

	private void ChooseDeathText() {

		string[] sentences = ReadTextFile();
		int pickedSentenceCount = 1;

		int startIndex = Random.Range(0,sentences.Length - pickedSentenceCount -1);

		deathText = "";

		for(int i = startIndex; i < startIndex + pickedSentenceCount; i++){
			deathText += sentences[i] + ". ";
		}

		deathTextObject.setText(deathText);
	}

	private string[] ReadTextFile() {
		
		TextAsset txt = (TextAsset)Resources.Load("LibraryofBabel", typeof(TextAsset));
		string content = txt.text;

		//divide string into sentences and return array
		return content.Split(new char[] {'.',';'});
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

	/// <summary>
	/// Deselects the currently selected wall, shelf and book
	/// </summary>
	public void DeselectAll() {

		if (selectedWall != null) {
			selectedWall.Deselect();
		}
		selectedWall = null;
		selection = Selection.None;
		viewController.ResetChoiceIndicator();
	}

	public void WallSelected(Wall wall) {
	
		if (selectedWall != null) {
			selectedWall.Deselect();	
		}

		selectedWall = wall;
	}

	public void BookSelected(Book book) {
		// TODO: Open and Show first page of the book
	}

	private bool CannotSelect() {
		return IsReadingBook() || IsInMenu() || isSwipingCamera;
	}

	public bool CanSelect() {
		return !CannotSelect();
	}

	public void HoveringOver(Wall wall) {
		viewController.RefreshChoiceIndicator(wall.Number, -1, -1);
	}

	public void HoveringOver(Shelf shelf) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, shelf.Number, -1);
	}

	public void HoveringOver(Book book) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, selectedWall.SelectedShelf.Number, book.Number);
	}

	public void HoveringOverEnded(Wall wall) {
		viewController.ResetChoiceIndicator();
	}

	public void HoveringOverEnded(Shelf shelf) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, -1, -1);
	}

	public void HoveringOverEnded(Book book) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, selectedWall.SelectedShelf.Number, -1);
	}

	public bool IsReadingBook() {
		return selection == Selection.Book;
	}
					
	private bool IsInMenu() {
		return selection == Selection.Search || selection == Selection.Settings;
	}

	/*public void resetIndicator(){
		pageInterface.setPositionIndication(choiceIndicator.toString());
		pageInterface.setTitle(selectedTitle);
		choiceIndicator.reset();
	}

	public void setTitle(string t){
		selectedTitle = t;
		pageInterface.setTitle(t);
	}

	public void updateTitle(){
		if(titles != null && selectedBook >= 0 && selectedBook < 32){
			pageInterface.setTitle(titles[selectedBook]);
		}
	}*/	

	private void SwipeHandling(){

		var touches = FilterSwipeValidTouches(Input.touches);

		if (touches.Length != 1) {	// swipe ends
			isSwipingCamera = false;
			touchExistsInSwipeArea = false;
			return;
		}

		if (!touchExistsInSwipeArea) {	// swipe begins

			swipeStartPosition = swipeEndPosition = touches[0].position;
			touchExistsInSwipeArea = true;
			return;
		}

		swipeEndPosition = touches[0].position;

		if (swipeStartPosition != swipeEndPosition) {

			if (swipeStartPosition.magnitude < Screen.width / 2.5 || swipeStartPosition.y < Screen.height * 0.2) return;

			float swipeDistance = Vector3.Distance(swipeStartPosition,swipeEndPosition);

			if(swipeDistance >= 0.1){

				isSwipingCamera = true;
				DeselectAll();

				var newPos = swipeEndPosition - swipeStartPosition;
				var xAngle = newPos.x * 180.0f / Screen.width;
				var yAngle = newPos.y * 90.0f / Screen.height;

				var invert = PlayerPrefs.GetInt(SettingsScript.INVERTCAM_KEY, 0) == 1 ? -1 : 1;
				var rotateVector = new Vector2(invert * xAngle, invert * yAngle);

				var fromRotation = fpc.transform.rotation;
				RotateCamera(rotateVector, 1f);
				var toRotation = fpc.transform.rotation;

				fpc.VROffset *= Quaternion.Inverse(fromRotation) * toRotation;

				swipeStartPosition = swipeEndPosition;
			}
		}
	}
}
