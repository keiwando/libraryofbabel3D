using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;

public class PageViewController: MonoBehaviour {

	[SerializeField] 
	private Text pageTextLeft;
	[SerializeField]
	private Text pageTextRight;
	[SerializeField]
	private Text titleLabel;
	[SerializeField]
	private Text positionLabel;
	[SerializeField]
	private InputField pageNumberInput;
	[SerializeField]
	private Button nextPageButton;
	[SerializeField]
	private Button previousPageButton;

	[SerializeField]
	private GameObject pageView;

	// In the case of two pages the first one of the two
	private PageLocation currentPageLocation;
	private string bookTitle = "";

	/*private string baseUrl = "https://libraryofbabel.info/book.cgi?";
	private string testUrl = "https://libraryofbabel.info/book.cgi?00000000000-w1-s5-v32:410";

	private string regexp="<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";
	private const string alphabet = "abcdefghijklmnopqrstuvwxyz,. ";*/

	private ViewController viewController;
	private SoundController soundController;

	void Start() {
		viewController = ViewController.Find();
		soundController = SoundController.Find();

		nextPageButton.onClick.AddListener(delegate {
			ShowNextPages();
		});

		previousPageButton.onClick.AddListener(delegate {
			ShowPreviousPages();
		});
	}

	void Update() {
		
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (pageNumberInput.text != string.Empty) {
				GoToSelectedPageAndUpdate();
			}
		}
	}

	public void Show(PageLocation pageLocation, string title, string textToHighlight = "") {

		viewController = ViewController.Find();
		this.gameObject.SetActive(true);
		currentPageLocation = pageLocation;

		if (string.IsNullOrEmpty(title)) {
			viewController.RequestTitle(pageLocation, delegate(string[] titles) {

				if (titles.Length == 0) return;

				bookTitle = titles[0];
				if (currentPageLocation.Page == 1) {
					titleLabel.text = bookTitle;
				}
			});
		}

		ShowCurrentPages(textToHighlight);
	}

	public void Show(Book book) {

		viewController = ViewController.Find();
		this.gameObject.SetActive(true);

		currentPageLocation = new PageLocation() {
			Hex = book.Shelf.Wall.Hex.location,
			Wall = book.Shelf.Wall.Number,
			Shelf = book.Shelf.Number,
			Book = book.Number,
			Page = 1
		};

		ShowCurrentPages();
	}

	public void Hide() {
		this.gameObject.SetActive(false);
		pageView.gameObject.SetActive(false);
	}

	private void ShowCurrentPages(string textToHighlight = "") {

		SetTitle("");
		SetPositionIndication("");
		//pageTextLeft.text = "";
		//pageTextRight.text = "";

		var pageLocations = new List<PageLocation>() { currentPageLocation };

		int pageNum = currentPageLocation.Page;

		if (pageNum > 1 && pageNum < 410) {
			if (pageNum % 2 == 0) {
				// Current page shows on the left
				pageLocations.Add(currentPageLocation.NextPage());
			} else {
				// current page shows on the right
				pageLocations.Add(currentPageLocation.PreviousPage());
			}
		}

		viewController.RequestPages(pageLocations.ToArray(), delegate(Page[] pages) {

			foreach (var page in pages) {
				if (page.Location.Page % 2 == 0) {
					pageTextLeft.text = page.Text;
				} else {
					pageTextRight.text = page.Text;
				}
			}

			if (pages[0].Location.Page == 1) {
				// Show the title and book location
				SetTitle(bookTitle);
				SetPositionIndication(currentPageLocation);
				pageTextLeft.text = "";
			} else if (pages[0].Location.Page == Universe.PAGES_PER_BOOK) {
				pageTextRight.text = "";
			} else {
				SetTitle("");
				SetPositionIndication("");
			}

			if (textToHighlight != "") {
				print("Trying to highlight " + textToHighlight);
				HighlightPatternOnPages(textToHighlight);
			}

			pageNumberInput.text = pageNum.ToString();
			soundController.PageFlip();

			pageView.gameObject.SetActive(true);
		});
	}

	public void ShowNextPages() {

		currentPageLocation = currentPageLocation.Page == 1 ? currentPageLocation.NextPage() : currentPageLocation.NextPage().NextPage();
		pageNumberInput.text = currentPageLocation.Page.ToString();
		ShowCurrentPages();
	}

	public void ShowPreviousPages() {

		currentPageLocation = currentPageLocation.Page == 2 ? currentPageLocation.PreviousPage() : currentPageLocation.PreviousPage().PreviousPage();
		pageNumberInput.text = currentPageLocation.Page.ToString();
		ShowCurrentPages();
	}

	public void GoToSelectedPageAndUpdate(){

		int pageNum = int.Parse(pageNumberInput.text);
		pageNum = Math.Max(PageLocation.MinPage, Math.Min(PageLocation.MaxPage, pageNum));

		if (pageNum > 1) {
			pageNum -= pageNum % 2;
		}

		if (currentPageLocation.Page != pageNum) {
			currentPageLocation.Page = pageNum;
		}

		ShowCurrentPages();
	}

	public void HighlightPatternOnPages(string pattern) {

		// Make sure the string only contains valid characters
		pattern = pattern.ToLower();
		pattern = Regex.Replace(pattern, string.Format("[^{0}]", Universe.Alphabet), "");
		if (pattern == "") return;

		pageTextLeft.text = HighlightPatternInText(pattern, pageTextLeft.text);
		pageTextRight.text = HighlightPatternInText(pattern, pageTextRight.text);
	}

	/// <summary>
	/// Highlights the given pattern in the page text and returns the resulting string.
	/// Highlighting is done by using the Unity Rich Text <color> tag.
	/// </summary>
	/// <returns>The text with the pattern highlighted in red</returns>
	/// <param name="pattern">The pattern to be highlighted</param>
	/// <param name="text">The text that contains the pattern</param>
	private string HighlightPatternInText(string pattern, string text) {

		var highlight = "<color=#800000ff>";
		var tagEnd = "</color>";
		var replacement = string.Format("{0}{1}{2}", highlight, pattern, tagEnd);

		pattern = Regex.Replace(pattern, @"\.", "\\.");

		text = Regex.Replace(text, @"\t|\n|\r", "");

		text = Regex.Replace(text, pattern, replacement, RegexOptions.Singleline);

		var lines = new List<string>();

		int startIndex = 0;
		for (int i = 0; i < 40; i++) { // There are 40 lines per page
			bool inTag = false;
			int length = 0;
			int c = 0;
			while (c < 80) { // There are 80 characters per line
				
				switch (text[startIndex + length]) {

				case '<': inTag = true; break;
				case '>': inTag = false; break;
				default: break;
				}

				c += inTag ? 0 : 1; 
				length += 1;
			}
			lines.Add(text.Substring(startIndex, length));
			startIndex += length;
		}

		return string.Join("\n", lines.ToArray());
	}

	private void SetPositionIndication(PageLocation page) {

		SetPositionIndication(string.Format("W:{0} S:{1} B:{2}", page.Wall, page.Shelf, page.Book));
	}

	private void SetPositionIndication(string t){
		positionLabel.text = t;
	}

	public void SetTitle(string t){
		titleLabel.text = t;
	}
}