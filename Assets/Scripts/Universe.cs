using UnityEngine;
using System.Collections;
using System.Text;

public class Universe : MonoBehaviour {

	public static readonly int WALLS_PER_ROOM = 4;
	public static readonly int SHELVES_PER_WALL = 5;
	public static readonly int BOOKS_PER_SHELF = 32;
	public static readonly int PAGES_PER_BOOK = 410;
	public static readonly int LINES_PER_PAGE = 40;
	public static readonly int CHARACTERS_PER_LINE = 80;

	public static Universe Shared {
		get { return shared; }
	}
	private static Universe shared;

	public LibrarySettings Settings {
		get { return settings; }
	}
	private LibrarySettings settings;

	[SerializeField]
	private OnlineLibrary onlineLibrary;

	void Awake() {
		if (shared == null) {
			Universe.shared = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this.gameObject);
		}
	}

	void Start() {

		settings = new LibrarySettings() { 
			Alphabet = "abcdefghijklmnopqrstuvwxyz,. "
		};
	}
	
	public string FillPageRandomly(string text){

		var charactersPerPage = LINES_PER_PAGE * CHARACTERS_PER_LINE;

		int missingCharacters = charactersPerPage - text.Length;

		if (missingCharacters < 0) {
			return text.Substring(0, charactersPerPage);
		} else if (missingCharacters == 0) {
			return text;
		}

		var stringBuilder = new StringBuilder();

		int charactersBefore = Random.Range(0, missingCharacters);
		int charactersAfter = missingCharacters - charactersBefore - 1;

		char[] alphabetArray = settings.Alphabet.ToCharArray();

		//add random characters before
		for(int i = 0; i < charactersBefore; i++){
			int index = Random.Range(0,alphabetArray.Length - 1);
			stringBuilder.Append(alphabetArray[index]);
		}

		//add search text
		stringBuilder.Append(text);

		//add random characters after
		for(int j = 0; j < charactersAfter; j++){
			int index = Random.Range(0,alphabetArray.Length - 1);
			stringBuilder.Append(alphabetArray[index]);
		}

		return stringBuilder.ToString();
	}

	public string FillPageBlank(string text){
		string newText = text;
		if(text.Length < 3200){
			int missingCharacters = 3200 - text.Length;
			for(int i = 0; i < missingCharacters; i++){
				newText += " ";
			}
		}

		return newText;
	}
}

