using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class GhoulScript : MonoBehaviour {

	[SerializeField] private Librarian librarian;
	[SerializeField] private MathFunctions universe;
	[SerializeField] private LibraryTranslator translator;
	[SerializeField] private Light pointLight;
	[SerializeField] private Text knowledgeMirror;
	[SerializeField] private int spawningChance;	//out of 100
	[SerializeField] private int maxTextLength;

	private string baseUrl = "https://libraryofbabel.info/book.cgi?";
	private string regexp="<div class = \"bookrealign\" id = \"real\"><PRE id = \"textblock\">[a-z.,\\s]*<\\/PRE><\\/div>";

	private string currentPage;
	private bool shouldRead;
	private Queue<string> knowledge;
	private const string baseKnowledge = "It's all just gibberish!";


	// Use this for initialization
	void Start () {
		checkSpawn();
		currentPage = "";
		shouldRead = false;
		pointLight.enabled = false;
		knowledge = new Queue<string>();

		InvokeRepeating("requestPageFromSite",3,20);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void checkSpawn(){
		int rand = Random.Range(0,100);
		if(rand >= spawningChance){
			this.gameObject.SetActive(false);
		}
	}

	public void respawn(){
		int rand = Random.Range(0,100);
		if(rand >= spawningChance){
			this.gameObject.SetActive(false);
		}else{
			this.gameObject.SetActive(true);
			//reset knowledge and mirror
			knowledge = new Queue<string>();
			knowledgeMirror.text = "";
		}
	}

	private void provideKnowledge(){
		if(knowledge.Count > 0){
			string s = knowledge.Dequeue();
			knowledge.Enqueue(s);
			showKnowledge(s);
		}else{
			showKnowledge(baseKnowledge);
		}
	}

	private void showKnowledge(string s){
		string text = s + "\n" + knowledgeMirror.text;
		knowledgeMirror.text = text;
		knowledgeMirror.CrossFadeAlpha(1f,0f,true);
		knowledgeMirror.CrossFadeAlpha(0f,5f,true);
	}

	void OnMouseOver(){
		pointLight.enabled = true;
	}

	void OnMouseExit(){
		pointLight.enabled = false;
	}

	void OnMouseDown(){
		provideKnowledge();
	}

	private void readPage(){
		string[] array = currentPage.Split(' ');
		print("Number of parts: " + array.Length);

		Queue<string> findings = new Queue<string>();
		filterShortStrings(findings,array);
	}

	private void filterShortStrings(Queue<string> findings,string[] text){
		foreach(string s in text){
			int length = s.Length;

			CommaAndPeriodInfo result = countCommasAndPeriods(s);
			int commaNumber = result.getCommaNumber();
			int periodNumber = result.getPeriodNumber();
			//every Comma extends the maxlength by 2 (because commas themselves translate to a conjunctor => their own number + 1 extra word for each comma)
			//every period extends the maclengt by 1 (have no effect on the length of the translation)
			if(length > 2 && length <= maxTextLength + (2 * commaNumber) + periodNumber){
				findings.Enqueue(s);
			}
		}
		print("The number of short strings is: " + findings.Count);
		if(findings.Count > 0){
			knowledge.Enqueue(translator.translateText(findings.Dequeue()));
			//print(translator.translateText(findings.Dequeue()));
		}
	}

	private CommaAndPeriodInfo countCommasAndPeriods(string s){
		char[] characters = s.ToCharArray();
		int periodCount = 0;
		int commaCount = 0;
		int totalCommas = 0;
		foreach(char c in characters){
			if(c == ','){
				if(commaCount < 2){
					commaCount++;
					totalCommas++;
				}
			}else{
				if(c == '.'){
					periodCount++;
				}
				commaCount = 0;
			}
		}
		CommaAndPeriodInfo capI = new CommaAndPeriodInfo();
		capI.setCommaNumber(totalCommas);
		capI.setPeriodNumber(periodCount);
		return capI;
	}

	public void requestPageFromSite(){
		if(shouldRead && this.gameObject.activeSelf){

			string url = generateUrl();

			StartCoroutine(waitForRequest(url,(www) => {
				currentPage = Parse(www.text);
				readPage();
			}));
		}
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
		//pick random book
		int wall = Random.Range(1,4);
		int shelf = Random.Range(1,5);
		int book = Random.Range(1,32);
		int page = Random.Range(1,410);

		//return testUrl;
		string volume = "";
		if(book < 10){
			volume += "0";
		}
		volume += book;

		return baseUrl + universe.getHexNumberBase36() + "-w" + wall + "-s" + shelf + "-v" + volume + ":" + page;
	}

	public void setShouldRead(bool b){
		shouldRead = b;
	}

	private class CommaAndPeriodInfo{
		private int periodNumber;
		private int commaNumber;

		public CommaAndPeriodInfo(){
			periodNumber = 0;
			commaNumber = 0;
		}

		public void setPeriodNumber(int i){
			periodNumber = i;
		}

		public void setCommaNumber(int i){
			commaNumber = i;
		}

		public int getPeriodNumber(){
			return periodNumber;
		}

		public int getCommaNumber(){
			return commaNumber;
		}
	}
}
