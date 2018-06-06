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

	public HexagonLocation CurrentLocation {
		get { return currentHexagon.location; }
		set { currentHexagon.location = value; }
	}

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

	void Start () {

		fpc = GetComponent<FirstPersonController>();

		selection = Selection.None;

		fallCount = 0;

		swipeStartPosition = swipeEndPosition = Vector3.zero;

		//universe.generateRandomHexagonNumber();
		//universe.setRandomHexagonNameInBase36();
		CurrentLocation = HexagonLocation.RandomLocation();

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

		Universe.Shared.RequestPages(pages, onComplete);
	} 

	public void RequestTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {

		Universe.Shared.RequestTitle(page, onCompletion);
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
			if(!Application.isMobilePlatform && Application.platform != RuntimePlatform.WebGLPlayer){
				Application.Quit();		// Quit the Application on Standalone builds
			} else {
				viewController.ActivateDeathText();
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

		viewController.SetDeathText(deathText);
	}

	private string[] ReadTextFile() {
		
		TextAsset txt = (TextAsset)Resources.Load("LibraryofBabel", typeof(TextAsset));
		string content = txt.text;

		//divide string into sentences and return array
		return content.Split(new char[] {'.',';'});
	}

	public void movedToNextRoom(){
		//increase hex number by 1
		//universe.addToHexNumber36(1);
		currentHexagon.location = currentHexagon.NextHexLocation();
	}

	public void movedToPreviousRoom(){
		//decrease hex number by 0
		//universe.addToHexNumber36(-1);
		currentHexagon.location = currentHexagon.PrevHexLocation();
	}

	public void movedToRoomAbove(){
		//increase each part of hex number by 66666
		//universe.addToAllHexNumbers36(66666);
		currentHexagon.location = currentHexagon.AboveLocation();
	}

	public void movedToRoomBelow(){
		//subtract each part of hex number by 66666
		//universe.addToAllHexNumbers36(-66666);
		currentHexagon.location = currentHexagon.BelowLocation();
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
		viewController.ShowBook(book);
	}

	public void PageSelected(PageLocation pageLocation) {
		viewController.ShowPage(pageLocation, "", "");
	}

	private bool CannotSelect() {
		return IsReadingBook() || IsInMenu() || isSwipingCamera;
	}

	public bool CanSelect() {
		return !CannotSelect();
	}

	public void HoveringOver(Wall wall) {
		viewController.RefreshChoiceIndicator(wall.Number, 0, 0);
	}

	public void HoveringOver(Shelf shelf) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, shelf.Number, 0);
	}

	public void HoveringOver(Book book) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, selectedWall.SelectedShelf.Number, book.Number);
	}

	public void HoveringOverEnded(Wall wall) {
		viewController.ResetChoiceIndicator();
	}

	public void HoveringOverEnded(Shelf shelf) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, 0, 0);
	}

	public void HoveringOverEnded(Book book) {
		viewController.RefreshChoiceIndicator(selectedWall.Number, selectedWall.SelectedShelf.Number, 0);
	}

	public bool IsReadingBook() {
		return selection == Selection.Book;
	}
					
	private bool IsInMenu() {
		return selection == Selection.Search || selection == Selection.Settings;
	}

	public void MenusClosed() {

		LockCameraUnlockMouse();
	}

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

				var invert = Settings.ShouldInvertCamera ? -1 : 1;
				var rotateVector = new Vector2(invert * xAngle, invert * yAngle);

				var fromRotation = fpc.transform.rotation;
				RotateCamera(rotateVector, 1f);
				var toRotation = fpc.transform.rotation;

				fpc.VROffset *= Quaternion.Inverse(fromRotation) * toRotation;

				swipeStartPosition = swipeEndPosition;
			}
		}
	}

	public static Librarian Find() {
		return GameObject.FindGameObjectWithTag("Librarian").GetComponent<Librarian>();
	}
}
