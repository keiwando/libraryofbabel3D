using System;
using System.Text;
using System.Collections.Generic;
using ScottGarland;

public class HexagonLocation {	

	public static readonly string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
	public static readonly int MaxNameLength = 3201; // since 29^3200 < m < 29^3201
	public static readonly BigInteger MaxValue = BigInteger.Pow(29, 3201) - 1;
	private static readonly BigInteger MaxValuePlusOne = MaxValue + 1;

	public string Name {
		get { 
			if (name == null) {
				name = value.ToString(36, Alphabet);
			}
			return name; 
		}
	}
	private string name; 

	public BigInteger Value {
		get {
			if (value == null) {
				value = new BigInteger(name, 36);
			}
			return value;
		}
	}
	private BigInteger value;

	public HexagonLocation(string name) {
		this.name = name;
		this.value = null;
	}

	public HexagonLocation(BigInteger value) {
		this.name = null;
		this.value = value; 
	}

	public static HexagonLocation RandomLocation() {

		var stringBuilder = new System.Text.StringBuilder();
		var rand = new System.Random();
		var characters = HexagonLocation.Alphabet.ToCharArray();

		var length = rand.Next(1, 3200);

		for (int i = 0; i < length; i++) {

			stringBuilder.Append(characters[rand.Next(0, characters.Length)]);
		}

		return new HexagonLocation(stringBuilder.ToString());
	}

	public HexagonLocation LocationWithOffset(int offset) {
		
		var newValue = Value + offset;
		if (newValue < 0) {
			var carry = 0 - newValue;
			newValue = MaxValue - carry + 1;
		} else if (newValue > MaxValue) {
			newValue = newValue % MaxValuePlusOne;
		}

		return new HexagonLocation(newValue);
	}

	public HexagonLocation LocationAbove() {
		return LocationWithOffset(24081899);	
	}

	public HexagonLocation LocationBelow() {
		return LocationWithOffset(-24081899);
	}
}


