using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour {

	[SerializeField]
	private ChoiceIndicator choiceIndicator;

	[SerializeField]
	private SearchMenu searchMenu;

	void Start () {
		
	}

	public void ShowChoiceIndicator() {
		choiceIndicator.gameObject.SetActive(true);
	}

	public void HideChoiceIndicator() {
		choiceIndicator.gameObject.SetActive(false);
	}

	public void ShowSearchMenu() {
		searchMenu.Show();
	}

	public void HideSearchMenu() {
		searchMenu.Hide();
	}

	public void ShowSettings() {

	}

	public void HideSettings() {
		
	}

	public void ShowPage() {
	
	}

	public void HidePage() {
		
	}


}
