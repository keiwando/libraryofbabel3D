using System;
using System.Text;
using System.Collections.Generic;

public struct HexagonLocation {	

	public static readonly string ALPHABET;
	public static readonly int MAX_LENGTH;

	/// <summary>
	/// If the name of the hexagon is longer than this threshold, then the 
	/// name above and below will be permutations of the current name.
	/// Otherwise, leading zeroes are added/removed from a name that is 
	/// shorter than the threshold.
	/// </summary>
	private static readonly int ABOVE_THRESHOLD;

	private static readonly Dictionary<char, int> char2Int;
	private static readonly Dictionary<int, char> int2Char;

	static HexagonLocation() {
		ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyz";

		char2Int = new Dictionary<char, int>();
		int2Char = new Dictionary<int, char>();

		char[] characters = ALPHABET.ToCharArray();

		for (int i = 0; i < characters.Length; i++) {

			char c = characters[i];
			char2Int[c] = i;
			int2Char[i] = c;
		}

		MAX_LENGTH = 3260;

		ABOVE_THRESHOLD = MAX_LENGTH / 2;
	}

	public string Name {
		get { return name; }
	}
	private string name; 

	public HexagonLocation(string name) {
		this.name = name;
	}

	public static HexagonLocation FromName(string name) {
		return new HexagonLocation(name);
	}

	public static BigInteger NameToNumber(string name) {

		return Universe.TextToNumber(name, ALPHABET);
	}

	public static string NumberToName(BigInteger number) {

		return Universe.NumberToText(number, ALPHABET);
	}

	public static HexagonLocation RandomLocation() {

		var stringBuilder = new System.Text.StringBuilder();
		var rand = new System.Random();
		var characters = HexagonLocation.ALPHABET.ToCharArray();

		var length = rand.Next(1, 3260);

		for (int i = 0; i < length; i++) {

			stringBuilder.Append(characters[rand.Next(0, characters.Length)]);
		}

		return HexagonLocation.FromName(stringBuilder.ToString());
	}

	public HexagonLocation LocationWithOffset(int offset) {
		
		return new HexagonLocation(LocationNameWithOffset(offset));
	}

	public HexagonLocation LocationAbove() {
		return new HexagonLocation(LocationNameAbove(Name));
	}

	public HexagonLocation LocationBelow() {
		return new HexagonLocation(LocationNameBelow(Name));
	}

	private string LocationNameWithOffset(int offset) {

		return offset >= 0 ? Add(Name, offset) : Subtract(Name, offset);
	}

	private string Add(string location, int offset) {

		var sb = new StringBuilder(location);

		int carry = 0;
		int b = ALPHABET.Length;

		int i = sb.Length - 1;

		while (offset != 0 || carry != 0) {

			var c2I = char2Int;
			var i2C = int2Char;		

			char c;
			if (i < 0) {
				c = i2C[0];
				sb.Insert(0, c);
			} else {
				c = sb[i];
			}     

			int x = c2I[c];

			int incr = offset % b + carry;
			carry = (x + incr) / b;

			offset = offset / b;

			int indx = (x + incr) % b;

			sb[Math.Max(0, i)] = i2C[indx];

			i--;
		}

		var s = LimitToNCharacters(sb.ToString(), MAX_LENGTH);
		return RemoveLeadingZeroes(s);
	}

	private string Subtract(string location, int offset) {

		var sb = new StringBuilder(location);

		int carry = 0;
		int b = ALPHABET.Length;

		int i = sb.Length - 1;

		while (offset != 0 || carry != 0 && sb.Length < MAX_LENGTH + 1) {

			var c2I = char2Int;
			var i2C = int2Char;		

			char c;
			if (i < 0) {
				c = i2C[0];
				//sb.Remove(0, 0);
				sb.Insert(0, c);
			} else {
				c = sb[i];
			}     

			int x = c2I[c];

			int incr = offset % b + carry;
			carry = (x + incr) / b;

			offset = offset / b;

			int indx = (x + incr) % b;
			if (indx < 0) {
				indx += b;
				carry -= 1;
			}

			sb[Math.Max(0, i)] = i2C[indx];

			i--;
		}

		var s = LimitToNCharacters(sb.ToString(), MAX_LENGTH);
		return RemoveLeadingZeroes(s);
	}

	private string LocationNameAbove(string s) {

		if (s.Length > ABOVE_THRESHOLD) {
			return LocationNameAboveShift(s);
		} else {
			return LocationNameAbovePad(s);
		}
	}

	private string LocationNameBelow(string s) {

		if (s.Length > ABOVE_THRESHOLD) {
			return LocationNameBelowShift(s);
		} else {
			return LocationNameBelowPad(s);
		}
	}

	private string LocationNameAboveShift(string s) {

		var first = s[0];
		return new StringBuilder(s)
			.Remove(0, 1)
			.Append(first)
			.ToString();
	}

	private string LocationNameBelowShift(string s) {

		var last = s[s.Length - 1];
		return new StringBuilder(s)
			.Remove(s.Length - 1, 1)
			.Insert(0, last)
			.ToString();
	}

	private string LocationNameAbovePad(string s) {

		char zero = int2Char[0];

		if (s[0] == zero && s.Length == ABOVE_THRESHOLD) {
			return RemoveLeadingZeroes(s);
		}
		if (s.Length == ABOVE_THRESHOLD) {
			
			if (s[0] == zero)
				return RemoveLeadingZeroes(s);
			else
				return s;
		}

		return LimitToNCharacters(new StringBuilder(s)
			.Insert(0, zero)
			.ToString(), ABOVE_THRESHOLD);
	}

	private string LocationNameBelowPad(string s) {
	
		char zero = int2Char[0];

		if (s[0] == zero && s.Length == 1 || (s[0] != zero && s.Length < ABOVE_THRESHOLD)) {
			return new StringBuilder(s)
				.Insert(0, new String(zero, ABOVE_THRESHOLD - s.Length))
				.ToString();
		}

		if (s[0] == zero)
			return new StringBuilder(s).Remove(0, 1).ToString();

		return s;
	}

	private string LimitToNCharacters(string s, int maxLength) {

		if (s.Length > maxLength) {
			return s.Substring(s.Length - maxLength, maxLength);
		}
		return s;
	}

	private string RemoveLeadingZeroes(string s) {

		char zero = int2Char[0];

		for (int i = 0; i < s.Length - 1; i++) {
			if (s[i] != zero) {
				if (i == 0)
					return s;
				else 
					return s.Substring(i, s.Length - 1 - i);
			}
		}

		return zero.ToString();
	}
}


