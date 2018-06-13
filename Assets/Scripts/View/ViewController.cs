using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour {

	private Librarian librarian;

	[SerializeField]
	private ChoiceIndicator choiceIndicator;

	[SerializeField]
	private SearchViewController searchViewController;

	[SerializeField]
	private PageViewController pageViewController;

	[SerializeField]
	private SettingsViewController settingsViewController;

	[SerializeField]
	private DeathText deathText;

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
	/// <param name="wall">Wall number</param>
	/// <param name="shelf">Shelf number</param>
	/// <param name="book">Book number</param>
	public void RefreshChoiceIndicator(int wall, int shelf, int book) {
		choiceIndicator.Refresh(wall, shelf, book);
	}

	public void ShowSearchMenu(ILibrarySearch searcher) {
		searchViewController.Show(searcher);
	}

	public void HideSearchMenu() {
		searchViewController.Hide();
	}

	public void ShowSettings() {
		settingsViewController.Show();
	}

	public void HideSettings() {
		settingsViewController.Hide();
	}

	public void ShowBook(Book book) {
		pageViewController.Show(book);
	}

	public void ShowPage(PageLocation pageLocation, string title, string textToHighlight = "") {
		pageViewController.Show(pageLocation, title, textToHighlight);
		librarian.ShowingPage();
	}

	public void SetCurrentBookTitle(string title) {
		pageViewController.SetBookTitle(title);
	}

	public void HidePage() {
		pageViewController.Hide();
	}

	public void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion) {

		librarian.RequestPages(pages, onCompletion);
	}

	public void RequestTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {

		librarian.RequestTitle(page, onCompletion);
	}

	public void SetDeathText(string text) {
		deathText.SetText(text);
	}

	public void ActivateDeathText() {
		deathText.Activate();
	}

	public void CloseAllMenus() {

		searchViewController.Hide();
		settingsViewController.Hide();
		pageViewController.Hide();
		librarian.MenusClosed();
	}

	public void PostProcessingSettingUpdated() {
		librarian.PostProcessingSettingUpdated();
	}

	public HexagonLocation GetCurrentHexLocation() {
		return librarian.CurrentLocation;
	}

	public void GoToLocation(HexagonLocation location) {
		librarian.CurrentLocation = location;
	}

	public static ViewController Find() {
		return GameObject.FindGameObjectWithTag("ViewController").GetComponent<ViewController>();
	}
}
