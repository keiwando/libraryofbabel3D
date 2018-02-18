using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;

public class PageInterfaceScript : Escapable {

	public LibrarianScript librarian;
	[SerializeField] private Camera camera;

	public MathFunctions universeMath;
	public Text pagetextField;
	public Text title;
	public Text position;
	public InputField inputField;
	public Canvas canvas;
	public GameObject loadingIndicator;

	public IOSControl iosControl;

	public TouchScreenKeyboard keyboard;

	private bool shouldRequestPage;

	//Double Interface Extras
	[SerializeField] private bool doubleInterface;
	[SerializeField] private Text pageText1;

	private string baseUrl = "https://libraryofbabel.info/book.cgi?";
	private string testUrl = "https://libraryofbabel.info/book.cgi?00000000000-w1-s5-v32:410";

	private string regexp="<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";
	private const string alphabet = "abcdefghijklmnopqrstuvwxyz,. ";

	// Use this for initialization
	void Start () {
		canvas.enabled = false;
		shouldRequestPage = false;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(Input.GetKeyDown(KeyCode.R)){
			requestRandomPage();
		}
		*/
		if(Input.GetKeyDown(KeyCode.Return)){
			if(inputField.text != string.Empty){
				goToSelectedPageAndUpdate();
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			EscapeClicked();	
		}
	}

	void FixedUpdate(){
		if(shouldRequestPage){
			if(true){
				requestPageFromSite(null);
			}else{
				print ("offline page Request");
				requestPage();
			}
		}
	}

	public override void EscapeClicked () {
		
		if(librarian.selectedStage == 3){
			librarian.selectedStage = 0;
			librarian.setSelectedPage(0);
			this.setVisible(false);
			librarian.lockMouseUnlockCamera();
		}
		#if MOBILE_INPUT
		this.setVisible(false);
		#endif
	}


	public void startPageRequestWithLoading(){
		if(librarian.getSelectedWall() != 0){
			loadingIndicator.GetComponent<SpriteRenderer>().enabled = true;
		}
		shouldRequestPage = true;
	}

	public void requestPage(){
		shouldRequestPage = false;

		pagetextField.text = librarian.requestPage();

		setVisible(true);

		if(librarian.getSelectedWall() != 0){
			loadingIndicator.GetComponent<SpriteRenderer>().enabled = false;
		}
		updateInputField();
		GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
	}

	public void requestPageFromSite(Action onCompletion){
		if(doubleInterface){
			requestDoublePage(onCompletion);
		}else{
			shouldRequestPage = false;

			//setVisible(true);

			string url = generateUrl();

			StartCoroutine(waitForRequest(url,(www) => {
				if(www.text == ""){
					pagetextField.text = "Offline";
				} else {
					pagetextField.text = Parse(www.text);
				}
				setVisible(true);
			}));

			if(librarian.getSelectedWall() != 0){
				loadingIndicator.GetComponent<SpriteRenderer>().enabled = false;
			}
			updateInputField();
			GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
		}
	}

	private void requestDoublePage(Action onCompletion){
		shouldRequestPage = false;
		if (onCompletion == null) {
			onCompletion = delegate {};
		}

		string url1 = generateUrl();

		if (librarian.getSelectedPage() == 0) {
			title.gameObject.SetActive(true);
			position.gameObject.SetActive(true);

			StartCoroutine(waitForRequest(url1,(www) => {
				if(www.text == ""){
					pagetextField.text = "Offline";
				} else {
					pagetextField.text = Parse(www.text);
				}
				setVisible(true);
				onCompletion();
			}));

			pageText1.text = "";

		} else if (librarian.getSelectedPage() == 409) {
			title.gameObject.SetActive(false);
			position.gameObject.SetActive(false);

			StartCoroutine(waitForRequest(url1,(www) => {
				if(www.text == ""){
					pageText1.text = "Offline";
				} else {
					pageText1.text = Parse(www.text);
				}
				pagetextField.text = "";
				setVisible(true);
				onCompletion();
			}));
		} else {	//firstPage
			title.gameObject.SetActive(false);
			position.gameObject.SetActive(false);

			StartCoroutine(waitForRequest(url1,(www) => {
				if(www.text == ""){
					pageText1.text = "Offline";
				} else {
					pageText1.text = Parse(www.text);
				}
				onCompletion();
				//setVisible(true);
			}));

			string url2 = generateNextPageUrl();

			StartCoroutine(waitForRequest(url2,(www) => {
				if(www.text == ""){
					pagetextField.text = "Offline";
				} else {
					pagetextField.text = Parse(www.text);
				}
				onCompletion();
				setVisible(true);
			}));
		}

		if (librarian.getSelectedWall() != 0) {
			loadingIndicator.GetComponent<SpriteRenderer>().enabled = false;
		}

		updateInputField();
		GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
	}

	private IEnumerator waitForRequest(string url,System.Action<WWW> complete){
		WWW www = new WWW(url);
		yield return www;
		complete(www);
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}

	private string Parse(string html){
		string text = "";
		Regex regex = new Regex (regexp);
		Match res = regex.Match (html);
		text = res.Groups [0].Value;
		text=Regex.Replace(text,"<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">\n","");
		text=Regex.Replace(text,"</PRE></div>","");
		return text;
	}

	private string generateUrl(){
		//return testUrl;
		string volume = "";
		if((librarian.getSelectedBook() + 1) < 10){
			volume += "0";
		}
		volume += (librarian.getSelectedBook() + 1);

		return baseUrl + universeMath.getHexNumberBase36() + "-w" + (librarian.getSelectedWall() + 1) + "-s" + (librarian.getSelectedShelf() + 1) + "-v" + volume + ":" + (librarian.getSelectedPage() + 1);
	}

	private string generateNextPageUrl(){
		//return testUrl;
		string volume = "";
		if((librarian.getSelectedBook() + 1) < 10){
			volume += "0";
		}
		volume += (librarian.getSelectedBook() + 1);

		return baseUrl + universeMath.getHexNumberBase36() + "-w" + (librarian.getSelectedWall() + 1) + "-s" + (librarian.getSelectedShelf() + 1) + "-v" + volume + ":" + (librarian.getSelectedPage() + 2);
	}

	private void updatePage(){
		pagetextField.text = librarian.requestPage();
		GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
	}

	private void updatePageFromSite(){
		if(!doubleInterface){
			string url = generateUrl();
			
			StartCoroutine(waitForRequest(url,(www) => {
				pagetextField.text = Parse(www.text);
				GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
			}));
		}else{
			GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
			requestDoublePage(null);
		}
	}

	public void setVisible(bool v){
		if(v){
			canvas.enabled = true;
			//Update Canvas
			Canvas.ForceUpdateCanvases();

			// Tell IOSControl
			if (iosControl != null) {

				iosControl.displayCorrectButton(IOSControl.Location.Book);
			}
		}else{
			canvas.enabled = false;
		}
	}

	public bool isVisible() {

		return canvas.enabled;
	}

	public void nextPage(){
		if(librarian.getSelectedPage() <= 408){
			librarian.setSelectedPage(librarian.getSelectedPage() +1);

			if(doubleInterface && librarian.getSelectedPage() <= 408 && librarian.getSelectedPage() != 1){
				librarian.setSelectedPage(librarian.getSelectedPage() +1);
			}
	
			updatePageFromSite();
			//GameObject.Find("SoundController").GetComponent<SoundController>().pageFlip();
		}
		updateInputField();
	}

	private void updateInputField(){
		inputField.text = (librarian.getSelectedPage() + 1).ToString();
	}

	public void previousPage(){
		if(librarian.getSelectedPage() > 0){
			librarian.setSelectedPage(librarian.getSelectedPage() -1);

			if(doubleInterface && librarian.getSelectedPage() > 0){
				librarian.setSelectedPage(librarian.getSelectedPage() -1);
			}

			updatePageFromSite();
		}
		updateInputField();
	}

	private void goToSelectedPage(){
		int page = 0;
		int.TryParse(inputField.text,out page);
		if(page != 0){
			page --;
			if(page >= 0 && page <= 409){
				if(!doubleInterface || page == 0 || page == 409){
					librarian.setSelectedPage(page);
				}else{
					if((page + 1) % 2 == 0){
						librarian.setSelectedPage(page);
					}else{
						librarian.setSelectedPage(page - 1);
					}
				}
			}else{
				updateInputField();
			}
		}else{
			//input was not correct
			updateInputField();
		}
	}

	public void goToSelectedPageAndUpdate(){
		goToSelectedPage();
		updatePageFromSite();
	}

	public void HighlightPatternOnPages(string pattern) {

		// Make sure the string only contains valid characters
		pattern = pattern.ToLower();
		pattern = Regex.Replace(pattern, string.Format("[^{0}]", alphabet), "");
		if (pattern == "") return;

		pageText1.text = HighlightPatternInText(pattern, pageText1.text);
		pagetextField.text = HighlightPatternInText(pattern, pagetextField.text);
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

	public void setPositionIndication(string t){
		position.text = t;
	}

	public void setTitle(string t){
		title.text = t;
	}

	private void testPageAlgorithms(){	
		print("pagebefore:  " + pagetextField.text);
		string s = universeMath.getPageFromData(universeMath.turnPageIntoData(pagetextField.text));
		print ("pageAfter:  " + s);
	}	
}
