using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SearchViewController : MonoBehaviour {

	private ILibrarySearch search;

	[SerializeField]
	private InputField searchInput;
	[SerializeField]
	private InputField hexNameField;

	[SerializeField]
	private Toggle exactMatchToggle;

	[SerializeField]
	private Button goButton;
	[SerializeField]
	private Button searchButton;
	[SerializeField]
	private Button settingsButton;

	//[SerializeField] private Button bugButton;
	[SerializeField]
	private Text wallNumLabel;
	[SerializeField]
	private Text shelfNumLabel;
	[SerializeField]
	private Text bookNumLabel;
	[SerializeField]
	private Text pageNumLabel;

	private string foundTitle = "";

	private ViewController viewController;

	private Regex validSearchCharactersFilter = new Regex(string.Format("[^{0}]", Universe.Alphabet), RegexOptions.IgnoreCase);
	private Regex validHexNameCharactersFilter = new Regex(string.Format("[^{0}]", HexagonLocation.Alphabet), RegexOptions.IgnoreCase);

	void Start () {

		searchInput.onValueChanged.AddListener(delegate(string arg0) {

			foundTitle = "";

			if (arg0 == "\n") {
				Search();
			} else {
				searchInput.text = FilterValidSearchCharacters(arg0);	
			}
		});

		hexNameField.onValueChanged.AddListener(delegate(string arg0) {

			foundTitle = "";
			hexNameField.text = FilterValidHexCharacters(arg0);
		});

		goButton.onClick.AddListener(delegate {
			GoToSelection();
		});
		searchButton.onClick.AddListener(delegate {
			Search();	
		});
		settingsButton.onClick.AddListener(delegate {
			viewController.ShowSettings();
		});
	}

	public void Show(ILibrarySearch search = null) {

		viewController = ViewController.Find();

		this.search = search;
		foundTitle = "";

		gameObject.SetActive(true);
		ResetLabels();
	}

	public void Hide() {

		gameObject.SetActive(false);
	}

	public void Search(){

		this.search.Search(searchInput.text, exactMatchToggle.isOn, delegate(SearchResult result) {

			hexNameField.text = result.Hex.Name;
			wallNumLabel.text = result.Wall.ToString();
			shelfNumLabel.text = result.Shelf.ToString();
			bookNumLabel.text = result.Book.ToString();
			pageNumLabel.text = result.Page.ToString();

			foundTitle = result.Title;
			print("Found title: " + foundTitle);
		});
	}

	private string FilterValidSearchCharacters(string text) {

		return validSearchCharactersFilter.Replace(text, "");
	}

	private string FilterValidHexCharacters(string text) {

		return validHexNameCharactersFilter.Replace(text.ToLower(), "");
	}

	public void GoToSelection() {

		var name = FilterValidHexCharacters(hexNameField.text);
		var newLocation = new HexagonLocation(name);

		viewController.GoToLocation(newLocation);

		int wall = int.Parse(wallNumLabel.text);
		int shelf = int.Parse(shelfNumLabel.text);
		int book = int.Parse(bookNumLabel.text);
		int page = int.Parse(pageNumLabel.text);

		if (wall != 0 && shelf != 0 && book != 0 && page != 0) {
			// Show the selected page

			var pageLocation = new PageLocation() {
				Hex = newLocation,
				Wall = wall,
				Shelf = shelf,
				Book = book,
				Page = page
			};

			viewController.ShowPage(pageLocation, foundTitle, searchInput.text);
			Hide();
		} else {
			
			viewController.CloseAllMenus();
		}
	}

	private void ResetLabels(){
		
		wallNumLabel.text = "0";
		shelfNumLabel.text = "0";
		bookNumLabel.text = "0";
		pageNumLabel.text = "0";

		hexNameField.text = viewController.GetCurrentHexLocation().Name;

		searchInput.text = "";
	}
}
