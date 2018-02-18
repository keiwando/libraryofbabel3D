﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SearchInterfaceScript : Escapable {

	public MathFunctions universe;
	public LibrarianScript librarian;
	public PageInterfaceScript pageInterface;
	public Canvas canvas;
	public InputField searchInput;
	public InputField hexNumberField;
	public Toggle searchToggle;
	public Toggle searchWithEnglishWordsToggle;
	public SettingsScript settings;

	public IOSControl iosControl;

	[SerializeField] private Button bugButton;

	public Text wallnumber;
	public Text shelfNumber;
	public Text bookNumber;
	public Text pageNumber;
	private string title;

	private long[] hexNumber;
	private int[] roomposition;

	private string url = "https://libraryofbabel.info/search.cgi";
	private string foundHexagon = "";
	//private string searchedString = "";
	private const string alphabet = "abcdefghijklmnopqrstuvwxyw,. ";

	// Use this for initialization
	void Start () {
		title = "";
		canvas.enabled = false;
		if(Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.OSXEditor){
			//remove bugButton
			print(Application.platform);
			bugButton.enabled = false;
		}
		if(Application.platform == RuntimePlatform.WindowsPlayer){
			bugButton.enabled = false;
		}

		searchInput.onValueChanged.AddListener(delegate(string arg0) {
			FilterValidCharacters(arg0);
		});
	}
	
	// Update is called once per frame
	void Update () {
		BackClick();
	}

	public void Search(){

		RequestSearchFromSite();
	}

	private string FillPageRandomly(string text){
		
		string newText = " ";
		if(text.Length < 3200){
			int missingCharacters = 3200 - text.Length;
			int charactersBefore = Random.Range(0,missingCharacters);
			int charactersAfter = missingCharacters - charactersBefore - 1;
			string characterSet = universe.getAlphabet();
			char[] alphabetArray = characterSet.ToCharArray();
			//add random characters before
			for(int i = 0; i < charactersBefore; i++){
				int index = Random.Range(0,alphabetArray.Length - 1);
				newText += alphabetArray[index];
			}
			//add search text
			newText += text;
			//add random characters after
			for(int j = 0; j < charactersAfter; j++){
				int index = Random.Range(0,alphabetArray.Length - 1);
				newText += alphabetArray[index];
			}
			return newText;
			
		}else{
			return text;
		}

		return newText;
	}

	private void RequestSearchFromSite(){

		string text = Regex.Replace(searchInput.text, string.Format("[^{0}]", alphabet), "", RegexOptions.IgnoreCase);
		if (text == "") return;

		if(searchToggle.isOn){
			//exact search
			text = FillPageBlank(text);
		}else if(!searchToggle.isOn){
			text = FillPageRandomly(text);
		}

		WWWForm form = new WWWForm();
		form.AddField("find",text);
		form.AddField("method","x");

		StartCoroutine(WaitForRequest(url,form,(www) => {
			string[] info = Parse(www.text);
			foundHexagon = info[1];
			hexNumberField.text = foundHexagon;
			wallnumber.text = info[3];
			shelfNumber.text = info[5];
			bookNumber.text = info[7];
			pageNumber.text = info[9];
		}));

	}

	private string[] Parse(string html){

		string choice = "exact match";

		string pattern = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform[0-9a-z.,'()\\s]*";
		string replacement = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform";
		string titlePattern = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*";
		string titleReplacement = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>";
		string text = "";
		Regex regex = new Regex (pattern);
		Match res = regex.Match (html);
		text = res.Groups [0].Value;

		text=Regex.Replace(text,replacement,"");
		text=Regex.Replace(text,"[()]","");

		string[] information = text.Split('\'');

		//parseTitle
		regex = new Regex(titlePattern);
		res = regex.Match(html);
		title = res.Groups[0].Value;

		title = Regex.Replace(title,titleReplacement,"");

		return information;
	}

	private IEnumerator WaitForRequest(string url, WWWForm form,System.Action<WWW> complete){
		WWW www = new WWW(url,form);
		yield return www;
		complete(www);
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: " + www.error);
		}
	}

	private string FillPageBlank(string text){
		string newText = text;
		if(text.Length < 3200){
			int missingCharacters = 3200 - text.Length;
			for(int i = 0; i < missingCharacters; i++){
				newText += " ";
			}
		}

		return newText;
	}

	private void FilterValidCharacters(string text) {

		var pattern = string.Format("[^{0}]", alphabet);
		searchInput.text = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase);
	}

	public void removeBug(){
		string s = hexNumberField.text;
		//shorten text to half of its length
		int newLength = s.Length / 2;
		print("oldLength: " + s.Length);
		print("newLength: " + newLength);
		s = s.Substring(0,newLength);
		hexNumberField.text = s;
	}

	public bool goToHexagon(){
		print("went to hex");

		universe.setHexNumberInBase36(hexNumberField.text);
		setVisible(false);
		return true;
	}

	public void goToPage(){
		if(goToHexagon() == true){
			
			pageInterface = librarian.getPageInterface();

			if((int.Parse(wallnumber.text) != 0) && (int.Parse(shelfNumber.text) != 0) && (int.Parse(bookNumber.text) != 0) && (int.Parse(pageNumber.text) != 0)){
				setVisible(false);
				librarian.selectWall(int.Parse(wallnumber.text)-1);
				librarian.selectShelf(int.Parse(shelfNumber.text)-1);
				librarian.selectBook(int.Parse(bookNumber.text)-1);
				librarian.setSelectedPage(int.Parse(pageNumber.text)-1);
				librarian.selectedStage = 3;

				//make sure double page loads correctly
				int selectedPage = librarian.getSelectedPage();
				if((selectedPage + 1) % 2 != 0){
					librarian.setSelectedPage(selectedPage - 1);
				}

				pageInterface.setVisible(true);
				pageInterface.setPositionIndication(PositionToString());
				pageInterface.setTitle(title);

				pageInterface.requestPageFromSite(delegate {
					pageInterface.HighlightPatternOnPages(searchInput.text);	
				});

			}else{
				librarian.lockMouseUnlockCamera();
				librarian.selectedStage = 0;
				print (hexNumberField.text);
			}
		}
	}

	private void ResetNumbers(){
		wallnumber.text = "0";
		shelfNumber.text = "0";
		bookNumber.text = "0";
		pageNumber.text = "0";

		hexNumberField.text = universe.getHexNumberBase36();

		searchInput.text = "";
	}

	public void setVisible(bool v){
		if(v){
			canvas.enabled = true;
			ResetNumbers();
			//Update Canvas
			Canvas.ForceUpdateCanvases();

			// tell the IOSControl
			if (iosControl != null) {

				iosControl.displayCorrectButton(IOSControl.Location.Menu);
			}
		}else{
			canvas.enabled = false;
		}
	}

	private void BackClick(){
		if(canvas.enabled){
			if(Input.GetKeyDown(KeyCode.Escape)){
				EscapeClicked();
			}
		}
	}

	public override void EscapeClicked (){
		
		canvas.enabled = false;
		librarian.lockMouseUnlockCamera();
		librarian.selectedStage = 0;
		librarian.setIndicatorVisible(true);
		settings.setVisible(false);
	}

	public bool isVisible() {
		return canvas.enabled;
	}

	private string PositionToString(){
		return "W:"+wallnumber.text + " S:" + shelfNumber.text + " B:" + bookNumber.text;
	}

}
