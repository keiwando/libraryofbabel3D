using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[RequireComponent(typeof(LibraryTranslator))]
public class GhoulScript : MonoBehaviour {
	
	[SerializeField] private Light pointLight;
	[SerializeField] private Text knowledgeMirror;
	[SerializeField] private int spawningChance;	//out of 100
	[SerializeField] private int maxTextLength;

	private LibraryTranslator translator;
	private Librarian librarian;
	private Hexagon hexagon;

	private Queue<string> knowledge;
	private const string baseKnowledge = "It's all just gibberish!";

	private const float pageReadingInterval = 15.0f;
	private bool shouldRead {
		get { return librarian.CurrentHexagon == hexagon; }
	}

	[SerializeField]
	private Material defaultMaterial;
	[SerializeField]
	private Material highlightMaterial;

	private SkinnedMeshRenderer meshRenderer;

	void Start () {

		translator = GetComponent<LibraryTranslator>();
		librarian = Librarian.Find();
		hexagon = transform.parent.GetComponent<Hexagon>();

		meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		SetMaterial(defaultMaterial);

		CheckSpawn();
		pointLight.enabled = false;
		knowledge = new Queue<string>();
	}

	private void SetMaterial(Material material) {
		meshRenderer.material = material;
	}

	private void CheckSpawn() {

		int rand = Random.Range(0,100);

		if (rand >= spawningChance) {
			this.gameObject.SetActive(false);
			StopAllCoroutines();
		} else {
			this.gameObject.SetActive(true);
			StopAllCoroutines();
			StartCoroutine(ReadPage(pageReadingInterval));
		}
	}

	public void Respawn(){
		
		CheckSpawn();

		knowledge = new Queue<string>();
		knowledgeMirror.text = "";
	}

	private void ProvideKnowledge(){
		
		if (knowledge.Count > 0) {
			
			string s = knowledge.Dequeue();
			knowledge.Enqueue(s);
			ShowKnowledge(s);
		}else{
			ShowKnowledge(baseKnowledge);
		}
	}

	private void ShowKnowledge(string s){
		
		string text = s + "\n" + knowledgeMirror.text;
		knowledgeMirror.text = text;
		knowledgeMirror.CrossFadeAlpha(1f, 0f, true);
		knowledgeMirror.CrossFadeAlpha(0f, 5f, true);
	}

	void OnMouseOver(){

		SetMaterial(highlightMaterial);
	}

	void OnMouseExit(){
		
		SetMaterial(defaultMaterial);
	}

	void OnMouseDown(){

		ProvideKnowledge();
	}

	private IEnumerator ReadPage(float secondDifference) {

		while (true) {

			yield return new WaitForSeconds(secondDifference);
			
			if (shouldRead) {
				ReadPage();
			}
		}
	}

	private void ReadPage() {

		// Pick random page
		int wall = Random.Range(1, Universe.WALLS_PER_ROOM);
		int shelf = Random.Range(1, Universe.SHELVES_PER_WALL);
		int book = Random.Range(1, Universe.BOOKS_PER_SHELF);
		int page = Random.Range(1, Universe.PAGES_PER_BOOK);

		var pageLocation = new PageLocation() {
			Hex = hexagon.location,
			Wall = wall,
			Shelf = shelf,
			Book = book,
			Page = page
		};

		librarian.RequestPages(new PageLocation[]{ pageLocation }, delegate(Page[] pages) {

			var splitPage = pages[0].Text.Split(' ');
			FilterShortStrings(splitPage);

		});
	}

	private void FilterShortStrings(string[] text) {

		var findings = new Queue<string>();
		
		foreach (string s in text) {
			
			int length = s.Length;

			CommaAndPeriodInfo result = CountCommasAndPeriods(s);
			int commaNumber = result.getCommaNumber();
			int periodNumber = result.getPeriodNumber();
			//every Comma extends the maxlength by 2 (because commas themselves translate to a conjunctor => their own number + 1 extra word for each comma)
			//every period extends the maclengt by 1 (have no effect on the length of the translation)
			if (length > 2 && length <= maxTextLength + (2 * commaNumber) + periodNumber) {
				findings.Enqueue(s);
			}
		}

		if (findings.Count > 0) {
			knowledge.Enqueue(translator.translateText(findings.Dequeue()));
		}
	}

	private CommaAndPeriodInfo CountCommasAndPeriods(string s){
		
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

	private class CommaAndPeriodInfo {
		private int periodNumber;
		private int commaNumber;

		public CommaAndPeriodInfo() {
			periodNumber = 0;
			commaNumber = 0;
		}

		public void setPeriodNumber(int i) {
			periodNumber = i;
		}

		public void setCommaNumber(int i) {
			commaNumber = i;
		}

		public int getPeriodNumber() {
			return periodNumber;
		}

		public int getCommaNumber() {
			return commaNumber;
		}
	}
}
