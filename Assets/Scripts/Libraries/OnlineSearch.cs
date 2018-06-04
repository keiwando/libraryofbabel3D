using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class OnlineSearch : MonoBehaviour, ILibrarySearch {

	private const string BASE_URL = "https://libraryofbabel.info/search.cgi";

	//private readonly Regex textPattern = new Regex("<h3>" + choice + ":</h3><PRE class = \"textsearch\" style = \"text-align: left\">Title: <b>[a-z.,\\s]*</b> Page: <b>[0-9.,\\s]*</b><br>Location: <a class = \"intext\" style = \"cursor:pointer\" title = \"\" onclick = \"postform[0-9a-z.,'()\\s]*");
	
	public void Search(string text, bool exactMatch, OnSearchCompleted onCompleted){

		//string text = Regex.Replace(searchInput.text, string.Format("[^{0}]", alphabet), "", RegexOptions.IgnoreCase);
		if (text == "") return;

		var universe = Universe.Shared;

		text = exactMatch ? universe.FillPageBlank(text) : universe.FillPageRandomly(text);

		WWWForm form = new WWWForm();
		form.AddField("find",text);
		form.AddField("method","x");

		StartCoroutine(WaitForRequest(BASE_URL, form, (www, error) => {

			string[] info = Parse(www.text);
			/*foundHexagon = info[1];
			hexNumberField.text = foundHexagon;
			wallnumber.text = info[3];
			shelfNumber.text = info[5];
			bookNumber.text = info[7];
			pageNumber.text = info[9];*/
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
		//title = res.Groups[0].Value;

		//title = Regex.Replace(title,titleReplacement,"");

		return information;
	}
}

