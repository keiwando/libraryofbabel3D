using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class OnlineSearch : MonoBehaviour, ILibrarySearch {

	private const string BASE_URL = "https://libraryofbabel.info/search.cgi";

	//private readonly Regex textPattern = new Regex("<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform[0-9a-z.,'()\\s]*");

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

		//string choice = "exact match";

		/*string pattern = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform[0-9a-z.,'()\\s]*";
		string replacement = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform";
		string titlePattern = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*";
		string titleReplacement = "<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>";
		string text = "";*/
		//Regex regex = new Regex (pattern);
		Match res = pattern.Match (html);
		var text = res.Groups[2].Value;
		var title = res.Groups[1].Value;

		//text=Regex.Replace(text,replacement,"");
		text = bracketPattern.Replace(text, "");

		var parts = text.Split('\'');

		return new SearchResult() {
			Title = title,
			HexName = parts[1],
			WallNum = int.Parse(parts[3]),
			ShelfNum = int.Parse(parts[5]),
			BookNum = int.Parse(parts[7]),
			PageNum = int.Parse(parts[9])
		};

		//parseTitle
		//regex = new Regex(titlePattern);
		//res = regex.Match(html);
		//title = res.Groups[0].Value;

		//title = Regex.Replace(title,titleReplacement,"");

		//return information;
	}
}

