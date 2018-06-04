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
	public static int CHARACTERS_PER_PAGE {
		get { return LINES_PER_PAGE * CHARACTERS_PER_LINE; }
	} 

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
	private OfflineLibrary offlineLibrary;

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
			Alphabet = "abcdefghijklmnopqrstuvwxyz,. ",
			Offline = false
		};

		offlineLibrary = new OfflineLibrary();
	}



	/// <summary>
	/// Creates a page containing the specified text at a random position and
	/// is otherwise filled with random characters of the Universe's alphabet
	/// </summary>
	public string FillPageRandomly(string text){

		var charactersPerPage = CHARACTERS_PER_PAGE;

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

	/// <summary>
	/// Creates a page that begins with the given text and is otherwise filled
	/// with spaces
	/// </summary>
	public string FillPageBlank(string text){

		var stringBuilder = new StringBuilder(text);
		int pageLength = CHARACTERS_PER_PAGE;
		if(text.Length < pageLength){
			int missingCharacters = pageLength - text.Length;
			for(int i = 0; i < missingCharacters; i++){
				stringBuilder.Append(" ");
			}
		}

		return stringBuilder.ToString();
	}

	/// <summary>
	/// Converts a string consisting of characters of the Universe's alphabet to an equivalent number (base conversion).
	/// </summary>
	public BigInteger TextToNumber(string text) {

		return Universe.ArbitraryToDecimalSystem(text, settings.Alphabet);
	}

	/// <summary>
	/// Converts a number to an equivalent string consisting of cahracters of the Universe's alphabet (base conversion).
	/// </summary>
	public string NumberToText(BigInteger number) {

		return Universe.DecimalToArbitrarySystem(number, settings.Alphabet);
	}

	/// <summary>
	/// Performs base conversion between a number in decimal representation and a string consisting of 
	/// characters of the given characterset.
	/// </summary>
	private static string DecimalToArbitrarySystem(BigInteger decimalNumber, string characterset) {
		
		string digits = characterset;
		int radix = characterset.Length;

		if (radix < 2 || radix > digits.Length)
			throw new System.ArgumentException("The radix must be >= 2 and <= " + digits.Length.ToString());

		if (decimalNumber == 0)
			return "0";

		var result = new StringBuilder();
		BigInteger currentNumber = BigInteger.Abs(decimalNumber);

		while (currentNumber != 0) {

			int remainder = BigInteger.ToInt32(BigInteger.Modulus(currentNumber, radix));
			result.Insert(0, digits[remainder]);

			currentNumber = currentNumber / radix;
		}
			
		if (decimalNumber < 0) {
			
			result.Insert(0, "-");
		}

		return result.ToString();
	}

	/// <summary>
	/// Performs base conversion between a string consisting of 
	/// characters of the given characterset and a number in decimal representation.
	/// </summary>
	private static BigInteger ArbitraryToDecimalSystem(string number, string characterset) {
		
		string digits = characterset;
		int radix = characterset.Length;

		if (radix < 2 || radix > digits.Length) {
			throw new System.ArgumentException("The radix must be >= 2 and <= " +
				digits.Length.ToString());
		}
			
		if (string.IsNullOrEmpty(number))
			return 0;

		// Make sure the arbitrary numeral system number is in lower case
		number = number.ToLowerInvariant();

		BigInteger result = 0;
		BigInteger multiplier = 1;

		for (int i = number.Length - 1; i >= 0; i--) {
			
			char c = number[i];
			if (i == 0 && c == '-') {
				
				// This is the negative sign symbol
				result = -result;
				break;
			}

			int digit = digits.IndexOf(c);
			if (digit == -1) {
				throw new System.ArgumentException(
					"Invalid character in the arbitrary numeral system number",
					"number");
			}

			result += digit * multiplier;
			multiplier *= radix;
		}

		return result;
	}
}

