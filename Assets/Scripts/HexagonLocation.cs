using System;

public struct HexagonLocation {	

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

	public HexagonLocation(BigInteger number) {

		var name = HexagonLocation.NumberToName(number);
		this(number, name);
	}

	public HexagonLocation(string name) {

		var number = HexagonLocation.NameToNumber(name);
	}

	public static BigInteger NameToNumber(string name) {

		return Universe.Shared.TextToNumber(name);
	}

	public static string NumberToName(BigInteger number) {

		return Universe.Shared.NumberToText(number);
	}
}


