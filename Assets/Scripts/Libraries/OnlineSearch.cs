using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class OnlineSearch : MonoBehaviour, ILibrarySearch {

	private const string BASE_URL = "https://libraryofbabel.info/search.cgi";

	private Regex pattern = new Regex("Title: <b>([a-z.,\\s]*)</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform([0-9a-z.,'()\\s]*)");
	private Regex bracketPattern = new Regex("[()]");

	public void Search(string text, bool exactMatch, OnSearchCompleted onCompleted){

		text = Regex.Replace(text, string.Format("[^{0}]", Universe.Alphabet), "", RegexOptions.IgnoreCase);
		if (text == "") return;

		text = exactMatch ? Universe.FillPageBlank(text) : Universe.FillPageRandomly(text);

		WWWForm form = new WWWForm();
		form.AddField("find",text);
		form.AddField("method","x");

		StartCoroutine(WaitForRequest(BASE_URL, form, (www, error) => {

			if (error != SearchError.None) 
				return;
			
			onCompleted(Parse(www.text));
		}));
	}

	private IEnumerator WaitForRequest(string url, WWWForm form, System.Action<WWW, SearchError> complete){
		WWW www = new WWW(url,form);
		yield return www;

		// check for errors
		if (www.error == null) {

			complete(www, SearchError.None);
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			complete(www, SearchError.Offline);
			Debug.Log("WWW Error: " + www.error);
		}
	}

	private SearchResult Parse(string html){

		Match res = pattern.Match (html);
		var text = res.Groups[2].Value;
		var title = res.Groups[1].Value;

		text = bracketPattern.Replace(text, "");

		var parts = text.Split('\'');

		return new SearchResult() {
			Title = title,
			Hex = new HexagonLocation(parts[1]),
			Wall = int.Parse(parts[3]),
			Shelf = int.Parse(parts[5]),
			Book = int.Parse(parts[7]),
			Page = int.Parse(parts[9])
		};
	}
}

