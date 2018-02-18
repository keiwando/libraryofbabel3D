using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IOSControl : MonoBehaviour {

	public enum Location {
		Default, Menu, Book
	}

	public LibrarianScript librarian;

	// The UI button that replaces the missing esc/back button on IOS
	public Button defaultButton;
	public Button menuButton;
	public Button bookButton;

	// Use this for initialization
	void Start () {

		// disable self when not on IOS
		if(!PlatformHelper.IsPlatformIOS() && !PlatformHelper.IsPlatformEditor()) {
			
			defaultButton.gameObject.SetActive(false);
			//menuButton.gameObject.SetActive(false);
			//bookButton.gameObject.SetActive(false);

			this.gameObject.SetActive(false);
		}

		this.displayCorrectButton(Location.Default);
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}

	public void ButtonClicked(){
		// call the appropiate function
		print("Back button clicked.");
		// Loop over all gameobjects of type Escapable and call the EscapeClicked function
		object[] obj = GameObject.FindSceneObjectsOfType(typeof (Escapable));
		foreach (object o in obj)
		{
			((Escapable) o).EscapeClicked();
		}

		// go Default button
		displayCorrectButton(Location.Default);
	}

	public void displayCorrectButton(Location location){
		
		switch (location) {
		case Location.Default:
			defaultButton.gameObject.SetActive(true);
			//menuButton.gameObject.SetActive(false);
			//bookButton.gameObject.SetActive(false);
			break;
		case Location.Book:
			defaultButton.gameObject.SetActive(false);
			//menuButton.gameObject.SetActive(false);
			//bookButton.gameObject.SetActive(true);
			break;
		case Location.Menu:
			defaultButton.gameObject.SetActive(false);
			//menuButton.gameObject.SetActive(true);
			//bookButton.gameObject.SetActive(false);
			break;
		}
	}
}
