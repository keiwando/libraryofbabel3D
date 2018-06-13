using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class PageViewController: MonoBehaviour {

	[SerializeField]
	private PageTextView leftPage;
	[SerializeField]
	private PageTextView rightPage;

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

	private const int scrollLineLength = 80;

	private readonly Regex newlineRegex = new Regex(@"\t|\n|\r");
	private readonly Regex filterNonAlphabet = new Regex(string.Format("[^{0}]", Universe.Alphabet));

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

				if (titles.Length == 0)
					return;

				bookTitle = titles[0];
				if (currentPageLocation.Page == 1) {
					SetVisibleTitle(bookTitle);
				}
			});
		} else {
			bookTitle = title;
		}

		ShowCurrentPages(textToHighlight);
	}

	public void Show(Book book) {

		viewController = ViewController.Find();
		this.gameObject.SetActive(true);
		this.bookTitle = book.Title;

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

		if (gameObject.activeSelf) {
			soundController.BookClose();
		}

		this.gameObject.SetActive(false);
		pageView.gameObject.SetActive(false);
	}

	private void ShowCurrentPages(string textToHighlight = "") {

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

			if (pageNum != currentPageLocation.Page) 
				return;

			pageNumberInput.text = currentPageLocation.Page.ToString();

			textToHighlight = textToHighlight.ToLower();
			textToHighlight = filterNonAlphabet.Replace(textToHighlight, "");

			foreach (var page in pages) {
				if (page.Location.Page % 2 == 0) {
					leftPage.Text = HighlightPatternInText(textToHighlight, page.Text);
				} else {
					rightPage.Text = HighlightPatternInText(textToHighlight, page.Text);
				}
			}

			if (pages[0].Location.Page == 1) {
				// Show the title and book location
				SetVisibleTitle(bookTitle);
				SetPositionIndication(currentPageLocation);
				leftPage.Text = "";

			} else if (pages[0].Location.Page == Universe.PAGES_PER_BOOK) {
				//pageTextRight.text = "";
				rightPage.Text = "";

			} else {
				SetVisibleTitle("");
				SetPositionIndication("");
			}

			soundController.PageFlip();

			pageView.gameObject.SetActive(true);
		});
	}

	public void ShowNextPages() {

		if (currentPageLocation.Page == Universe.PAGES_PER_BOOK)
			return;

		currentPageLocation = currentPageLocation.Page == 1 ? currentPageLocation.NextPage() : currentPageLocation.NextPage().NextPage();
		pageNumberInput.text = currentPageLocation.Page.ToString();
		//leftPage.Text = " ";
		//rightPage.Text = " ";
		ShowCurrentPages();
	}

	public void ShowPreviousPages() {

		if (currentPageLocation.Page == 1)
			return;

		currentPageLocation = currentPageLocation.Page == 2 ? currentPageLocation.PreviousPage() : currentPageLocation.PreviousPage().PreviousPage();
		pageNumberInput.text = currentPageLocation.Page.ToString();
		//leftPage.Text = " ";
		//rightPage.Text = " ";
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

	private string HighlightPatternInText(string pattern, string text) {

		var startTag = "<color=#800000ff>";
		var endTag = "</color>";

		pattern = Regex.Replace(pattern, @"\t|\n|\r", "");
		text = newlineRegex.Replace(text, "");

		if (pattern == "")
			return text;

		var highlight = Regex.Replace(text, pattern, string.Format("{0}{1}{2}", startTag, pattern, endTag));

		return highlight;
	}

	private void SetPositionIndication(PageLocation page) {

		SetPositionIndication(string.Format("W:{0} S:{1} B:{2}", page.Wall, page.Shelf, page.Book));
	}

	private void SetPositionIndication(string t){
		positionLabel.text = t;

		positionLabel.gameObject.SetActive(t != "");
	}

	private void SetVisibleTitle(string t){
		titleLabel.text = t;

		titleLabel.gameObject.SetActive(t != "");
	}

	public void SetBookTitle(string title) {
		
		this.bookTitle = title;

		if (pageNumberInput.text != "" && int.Parse(pageNumberInput.text) == 1) {
			SetVisibleTitle(title);
		}
	}
}