using UnityEngine;
using System.Collections;

[RequireComponent(typeof(OnlineSearch))]
public class OnlineLibrary : MonoBehaviour, ILibrary {

	private OnlineSearch onlineSearch;

	private const string PAGE_REQ_BASE_URL = "https://libraryofbabel.info/book.cgi?";
	private string testUrl = "https://libraryofbabel.info/book.cgi?00000000000-w1-s5-v32:410";

	private const string PAGE_REGEXP = "<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";

	void Start() {
	
		onlineSearch = GetComponent<OnlineSearch>();
	}

	public void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion) {

		StartCoroutine(SendPageRequests(url1,(www) => {
			if(www.text == ""){
				pagetextField.text = "Offline";
			} else {
				pagetextField.text = Parse(www.text);
			}
			setVisible(true);
			onCompletion();
		}));
	} 

	private IEnumerator SendPageRequests(string url, System.Action<WWW> complete){
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

	private string PageRequestURL(PageLocation page) {
		//return testUrl;
		string volume = "";
		if(page.Book < 10){
			volume += "0";
		}
		volume += page.Book;

		return PAGE_REQ_BASE_URL + page.Hex.Name + "-w" + page.Wall + "-s" + page.Shelf + "-v" + volume + ":" + page.Page;
	}

	public ILibrarySearch GetSearch() {
		return onlineSearch;
	}


	

}

