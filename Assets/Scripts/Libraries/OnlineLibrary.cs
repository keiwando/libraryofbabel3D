using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[RequireComponent(typeof(OnlineSearch))]
public class OnlineLibrary : MonoBehaviour, ILibrary {

	private OnlineSearch onlineSearch;

	private const string PAGE_REQ_BASE_URL = "https://libraryofbabel.info/book.cgi?";
	//private string testUrl = "https://libraryofbabel.info/book.cgi?00000000000-w1-s5-v32:410";

	private readonly Regex pageRegex = new Regex("<PRE id = \"textblock\">([a-z.,\\s]*)<\\/PRE>");

	private const string TITLE_REQ_BASE_URL = "https://libraryofbabel.info/titler.cgi?";
	private readonly Regex titleRegex = new Regex("<PRE id = \"textblock\">([a-z.,\\s]*)<\\/PRE>");

	private static readonly string[] emptyTitles = Enumerable.Repeat("", Universe.BOOKS_PER_SHELF).ToArray();
	private static readonly char[] titleSplitArray = new char[] { ';' };

	void Start() {
	
		onlineSearch = GetComponent<OnlineSearch>();
	}

	public void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion) {

		StartCoroutine(SendPageRequests(pages, onCompletion));
	} 

	private IEnumerator SendPageRequests(PageLocation[] pages, OnPageRequestCompleted onCompletion){

		var results = new Dictionary<PageLocation, WWW>();

		foreach (var page in pages) {

			var www = new WWW(PageRequestURL(page));
			results[page] = www;
			yield return www;
		}

		var pageTexts = results.Keys.Select(delegate(PageLocation location) {
			var www = results[location];

			if (www.text == "") {
				Debug.Log("WWW Error: "+ www.error);
				return new Page() { Location = location, Text = "Offline" };
			} else {
				return new Page() { Location = location, Text = ParsePage(www.text) };
			}
		}).ToArray();

		onCompletion(pageTexts);
	}

	private string PageRequestURL(PageLocation page) {
		
		string volume = "";
		if(page.Book < 10){
			volume += "0";
		}
		volume += page.Book;

		return PAGE_REQ_BASE_URL + page.Hex.Name + "-w" + page.Wall + "-s" + page.Shelf + "-v" + volume + ":" + page.Page;
	}

	private string ParsePage(string html) {
		
		Match res = pageRegex.Match (html);
		return res.Groups[1].Value;
	}

	public void RequestBookTitles(ShelfLocation shelfLocation, OnTitleRequestCompleted onCompletion) {

		WWWForm form = new WWWForm();
		form.AddField("hex", shelfLocation.Hex.Name);
		form.AddField("wall", shelfLocation.Wall);
		form.AddField("shelf", shelfLocation.Shelf);

		print("FDS");

		StartCoroutine(WaitForRequest(TITLE_REQ_BASE_URL, form, (error, www) => {

			if (error) {
				onCompletion(emptyTitles);
			} else {
				onCompletion(www.text.Split(titleSplitArray));
			}
		}));
	}

	public void RequestBookTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {
	
		WWWForm form = new WWWForm();
		form.AddField("hex", page.Hex.Name);
		form.AddField("wall", page.Wall);
		form.AddField("shelf", page.Shelf);
		form.AddField("book", page.Book);

		StartCoroutine(WaitForRequest(TITLE_REQ_BASE_URL, form, (error, www) => {

			if (error) {
				onCompletion(emptyTitles);
			} else {
				onCompletion(www.text.Split(titleSplitArray));
			}
		}));
	}

	private IEnumerator WaitForRequest(string url, WWWForm form, System.Action<bool, WWW> onCompletion) {

		WWW www = new WWW(url,form);
		yield return www;

		if (www.error == null) {
			Debug.Log("WWW Ok!: " + www.text);
			Debug.Log("Pased " + ParseTitles(www.text));
			onCompletion(false, www);
		} else {
			Debug.Log("WWW Error: "+ www.error);
			onCompletion(true, www);
			//Debug.Log("WWW text:" + www.text);
		}
	}

	private string ParseTitles(string html) {

		Match res = titleRegex.Match (html);
		return res.Groups[1].Value;
	}

	public ILibrarySearch GetSearch() {
		return onlineSearch;
	}


	

}

