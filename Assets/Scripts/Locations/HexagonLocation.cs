using System;

public struct HexagonLocation {	

	public static readonly string ALPHABET = "abcdefghijklmnopqrstuvwxyz0123456789";
	public static readonly BigInteger MAX = BigInteger.Pow(ALPHABET.Length, 3260);

	public BigInteger Number {
		get { return hexNumber; }
	}
	private BigInteger hexNumber;

	public string Name {
		get { return name; }
	}
	private string name; 

	public HexagonLocation(BigInteger number, string name) {

		this.hexNumber = number;
		this.name = name;
	}

	public static HexagonLocation FromNumber(BigInteger number) {

		var name = HexagonLocation.NumberToName(number);

		return new HexagonLocation() {
			hexNumber = number,
			name = name
		};
	}

	public static HexagonLocation FromName(string name) {

		var number = HexagonLocation.NameToNumber(name);
		return new HexagonLocation() {
			hexNumber = number,
			name = name
		};
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
}


