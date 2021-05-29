#if UNITY_IOS || UNITY_ANDROID
#define MOBILE
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Keiwando.Lob;
using ScottGarland;

[RequireComponent(typeof(LibraryTranslator))]
public class GhoulScript : MonoBehaviour {
	
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

	private static Math.LCGParameters respawnLCG;

	[SerializeField]
	private Material defaultMaterial;
	[SerializeField]
	private Material highlightMaterial;

	private SkinnedMeshRenderer meshRenderer;

	static GhoulScript() {
		respawnLCG.m = BigInteger.Pow(2, 64);
		respawnLCG.a = new BigInteger(6364136223846793005);
		respawnLCG.c = new BigInteger(1442695040888963407);
		respawnLCG.aInverse = Math.extendedGcd(respawnLCG.a, respawnLCG.m).x;
	}

	void Start () {

		translator = GetComponent<LibraryTranslator>();
		librarian = Librarian.Find();
		hexagon = transform.parent.GetComponent<Hexagon>();
		Assert.IsNotNull(hexagon);
		Assert.IsNotNull(librarian);

		meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		SetMaterial(defaultMaterial);
		
		if (hexagon.location != null) {
			CheckSpawn();
		} else {
			this.gameObject.SetActive(false);
		}
		
		knowledge = new Queue<string>();
	}

	private void SetMaterial(Material material) {
		if (material != null) {
			meshRenderer.material = material;
		}
	}

	private void CheckSpawn() {

		Assert.IsNotNull(hexagon.location);

		// var locationData = hexagon.location.Value.GetDigitsData();
		// var locationDataHash = ((IStructuralEquatable)locationData).GetHashCode(EqualityComparer<System.UInt32>.Default);
		// bool spawnHere = locationDataHash % 100 < spawningChance;

		bool spawnHere = Math.LCG(hexagon.location.Value, respawnLCG) % 100 < spawningChance;

		if (!spawnHere) {
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

		// TODO: Keep the knowledge after respawning
		knowledge = new Queue<string>();
		if (knowledgeMirror != null) {
			knowledgeMirror.text = "";
		}
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

#if MOBILE
	void OnMouseOver() {
#else
	void OnHover(){
#endif

		SetMaterial(highlightMaterial);
	}

#if MOBILE
	void OnMouseExit() {
#else
	void OnHoverExit() {
#endif
		
		SetMaterial(defaultMaterial);
	}

#if MOBILE
	void OnMouseUp() {
#else
	void OnHoverMouseUp() {
#endif

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
