using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PageInterfaceScript : MonoBehaviour {

	public LibrarianScript librarian;
	[SerializeField] private Camera camera;

	public MathFunctions universeMath;
	public Text pagetextField;
	public Text title;
	public Text position;
	public InputField inputField;
	public Canvas canvas;
	public GameObject loadingIndicator;

	public TouchScreenKeyboard keyboard;

	private bool shouldRequestPage;

	//Double Interface Extras
	[SerializeField] private bool doubleInterface;
	[SerializeField] private Text pageText1;

	private string baseUrl = "https://libraryofbabel.info/book.cgi?";
	private string testUrl = "https://libraryofbabel.info/book.cgi?00000000000-w1-s5-v32:410";

	private string regexp="<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";

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
	}

	void FixedUpdate(){
		if(shouldRequestPage){
			if(true){
				requestPageFromSite();
			}else{
				print ("offline page Request");
				requestPage();
			}
		}
	}


	/*
	public void requestRandomPage(){
		universeMath.generateRandomPage();
		pagetextField.text = universeMath.getPageFromData();
	}
	*/
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

	public void requestPageFromSite(){
		if(doubleInterface){
			requestDoublePage();
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

	private void requestDoublePage(){
		shouldRequestPage = false;

		string url1 = generateUrl();

		if(librarian.getSelectedPage() == 0){
			title.gameObject.SetActive(true);
			position.gameObject.SetActive(true);

			StartCoroutine(waitForRequest(url1,(www) => {
				if(www.text == ""){
					pagetextField.text = "Offline";
				} else {
					pagetextField.text = Parse(www.text);
				}
				setVisible(true);
			}));

			pageText1.text = "";

		}else if(librarian.getSelectedPage() == 409){
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
			}));
		}else{	//firstPage
			title.gameObject.SetActive(false);
			position.gameObject.SetActive(false);

			StartCoroutine(waitForRequest(url1,(www) => {
				if(www.text == ""){
					pageText1.text = "Offline";
				} else {
					pageText1.text = Parse(www.text);
				}
				//setVisible(true);
			}));

			string url2 = generateNextPageUrl();

			StartCoroutine(waitForRequest(url2,(www) => {
				if(www.text == ""){
					pagetextField.text = "Offline";
				} else {
					pagetextField.text = Parse(www.text);
				}
				setVisible(true);
			}));
		}

		if(librarian.getSelectedWall() != 0){
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
			Debug.Log("WWW Ok!: " + www.data);
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
			requestDoublePage();
		}
	}

	public void setVisible(bool v){
		if(v){
			canvas.enabled = true;
			//Update Canvas
			Canvas.ForceUpdateCanvases();
		}else{
			canvas.enabled = false;
		}
	}

	public void nextPage(){
		if(librarian.getSelectedPage() <= 408){
			librarian.setSelectedPage(librarian.getSelectedPage() +1);

			if(doubleInterface && librarian.getSelectedPage() <= 408 && librarian.getSelectedPage() != 1){
				librarian.setSelectedPage(librarian.getSelectedPage() +1);
			}
			/*
			if(PlayerPrefs.GetInt("CONNECTED") == 0 && Application.platform != RuntimePlatform.OSXWebPlayer
			   && Application.platform != RuntimePlatform.WindowsWebPlayer){
				updatePage();
			}else{
			*/
				updatePageFromSite();
			//}
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
			/*
			if(PlayerPrefs.GetInt("CONNECTED") == 0 && Application.platform != RuntimePlatform.OSXWebPlayer
			   && Application.platform != RuntimePlatform.WindowsWebPlayer){
				updatePage();
			}else{
			*/
				updatePageFromSite();
			//}
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
