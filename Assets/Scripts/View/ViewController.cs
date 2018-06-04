using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour {

	private Librarian librarian;

	[SerializeField]
	private ChoiceIndicator choiceIndicator;

	[SerializeField]
	private SearchMenu searchMenu;

	[SerializeField]
	private PageViewController pageViewController;

	[SerializeField]
	private SettingsScript settingsViewController;

	void Start () {
		librarian = GameObject.FindGameObjectWithTag("Librarian").GetComponent<Librarian>();
	}

	public void ShowChoiceIndicator() {
		choiceIndicator.gameObject.SetActive(true);
	}

	public void HideChoiceIndicator() {
		choiceIndicator.gameObject.SetActive(false);
	}

	public void ResetChoiceIndicator() {
		choiceIndicator.Reset();
	}

	/// <summary>
	/// Refreshes the choice indicator.
	/// </summary>
	/// <param name="wallIndex">Wall index (0 indexed)</param>
	/// <param name="shelfIndex">Shelf index (0 indexed)</param>
	/// <param name="bookIndex">Book index (0 indexed)</param>
	public void RefreshChoiceIndicator(int wallIndex, int shelfIndex, int bookIndex) {
		choiceIndicator.Refresh(wallIndex + 1, shelfIndex + 1, bookIndex + 1);
	}

	public void ShowSearchMenu() {
		searchMenu.Show();
	}

	public void HideSearchMenu() {
		searchMenu.Hide();
	}

	public void ShowSettings() {
		settingsViewController.Show();
	}

	public void HideSettings() {
		settingsViewController.Hide();
	}

	public void ShowPage() {
		pageViewController.Show();
	}

	public void HidePage() {
		pageViewController.Hide();
	}

	public void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion) {

		librarian.RequestPages(pages, onCompletion);
	}

	public void CloseAllMenus() {

		searchMenu.Hide();
		settingsViewController.Hide();
		pageViewController.Hide();
	}
}
