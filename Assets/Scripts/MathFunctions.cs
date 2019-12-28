using UnityEngine;
using System.Collections;

public class MathFunctions : MonoBehaviour {

	//private Random random;

	const string BOOKCHARACTERSET = "abcdefghijklmnopqrstuvwxyz,. ";
	const string HEXCHARACTERSET = "0123456789abcdefghijklmnopqrstuvwxyz";
	const int PARTSTRINGLENGTH = 10;

	private long[] hexagonNumber;
	private long[] pageData;
	private int[] roomposition;		//only saved to by inverted algorithm  => SEARCH
	private const int LongNumber = 320;
	private const long MAXLONG = 420707233300200;
	private const long LASTLONG = 420707229868791;

	private string hexNumberInBase36;
	private const long MAXLONGFORBASE36 = 101559956668415; // 101559956668415

	//long.maxValue = 9223372036854775807
	//maxlong = 		  420707233300200
	//max Room Position number = 409|31|4|3     //pageNumber|BookNumber|ShelfNumber|WallNumber
	/*
	private int pageNumber;
	private int bookNumber;
	private int shelfNumber;
	private int wallNumber;
	*/

	//lcg
	private const long MOD = 420707233300201;
	private const long B = 27;
	private const long A = 30;

	private const long AINVERSE = 406683658856861;

	// Use this for initialization
	void Start () {
		//random = new Random();
		hexagonNumber = new long[LongNumber];
		roomposition = new int[4];
		pageData = new long[LongNumber];


		//testConversionPrints();
		//testTrailingZeroRemove();
		//test36ConversionMethods();
	}

	private void testConversionPrints() {

		string numAsString = "000000000";

		long number = ArbitraryToDecimalSystem(numAsString,36,HEXCHARACTERSET);
		print("String: " + numAsString + " as number: " + number);
		print("Number: " + number + " -> String: " + DecimalToArbitrarySystem(number,36,HEXCHARACTERSET) + "\n");

		number = 904;
		numAsString = DecimalToArbitrarySystem(number,36,HEXCHARACTERSET);
		print("Number: " + number + " -> String: " + numAsString);
		print("String: " + numAsString + " -> Number: " + ArbitraryToDecimalSystem(numAsString,36,HEXCHARACTERSET) + "\n");

	}

	private string addMissingCharacters(string s,int wantedLength,char fill){
		if(s.Length < wantedLength){
			int missingNumber = wantedLength - s.Length;
			string newstr = "";
			for(int i = 0; i < missingNumber; i++){
				newstr += fill;
			}
			newstr += s;
			return newstr;
		}
		return s;
	}

	public void generateRandomHexagonNumber(){
		//create the first 320 longs
		for(int i = 0; i <= hexagonNumber.Length - 1; i++){
			long current = (long)Random.Range(0,1000000000) * 1000000 + (long)Random.Range(0,1000000);
			current = current % MAXLONG;
			hexagonNumber[i] = current;
		}

	}

	public string generateRandomHexagonNameInBase36(){
		int length = Random.Range(1,3260);
		string s = "";
		for(int i = 0; i < length; i++){
			s += HEXCHARACTERSET[Random.Range(0,HEXCHARACTERSET.Length)];
		}
		//print(s);
		//print ("hexname length: " + s.Length);
		return s;
	}

	public void setRandomHexagonNameInBase36(){
		hexNumberInBase36 = generateRandomHexagonNameInBase36();
	}

	public void setHexNumberInBase36(string hexnum){
		hexNumberInBase36 = hexnum;
	}


	public long[] turnHexNumber36To10(){
		int length;
		int rest;
		int startIndex = 0;
		int increment = 0;
		long[] number;

		rest = hexNumberInBase36.Length % 9;

		if(rest == 0){
			length = hexNumberInBase36.Length / 9;
			length ++;
			// test
			length++;
		}else{
			length = hexNumberInBase36.Length / 9;
			length += 2;
		}

		number = new long[length];

		if(rest > 0){
			number[0] = ArbitraryToDecimalSystem(hexNumberInBase36.Substring(startIndex,rest),36,HEXCHARACTERSET);
			//print("Rest: " + number[0]);
			increment = 1;
			number[length-1] = 1;	//last long in the array indicates if there was a rest or not
		}else{
			number[length-1] = 0;
		}

		//print("increment: " + increment);

		for(int i = 0; i < number.Length - 2; i++){
			startIndex = rest + (i * 9);			//because each long takes 9 characters		=> 	substring begin at: 0,9,18,27...

			number[i+increment] = ArbitraryToDecimalSystem(hexNumberInBase36.Substring(startIndex,9),36,HEXCHARACTERSET);

			number[i] = number[i] % (MAXLONGFORBASE36 + 1);
		}

		// Test always return long[] of size 363
		long[] result = new long[363];
		//print("Number length: " + number.Length );
		//print("String length: " + hexNumberInBase36.Length);
		int difference = result.Length - number.Length - 1 < 0 ? 0: result.Length - number.Length;
		//print("difference: " + difference);
		if (difference < 0) print("NEGATIVE DIFFERENCE!!");

		for (int i = 0; i < number.Length - 1; i++) {
			
			result[i + difference] = number[i];
		}

		return result;
		//return number;
	}

	public string turnHexNumber10To36(long[] number){
		string text = "";

		/*if(number[number.Length - 1] == 1){
			//there was a rest
			string restAsString = DecimalToArbitrarySystem(number[0],36,HEXCHARACTERSET);
			print("rest as string: " + restAsString);
			text += restAsString;
		}else{*/
		text += DecimalToArbitrarySystem(number[0],36,HEXCHARACTERSET);//,9,'0'); //addMissingCharacters(
		//}

		// find first index where entry is not 0
		int start = 1;
		while(number[start] == 0 && start < number.Length - 1) {
			start++;
		}


		for(int i = start; i < number.Length - 1; i++){
			//if (i != number.Length - 1 && number[i] == 0) continue;
			text += addMissingCharacters(DecimalToArbitrarySystem(number[i],36,HEXCHARACTERSET),9,'0');
		}

		// remove trailing 0s
		text = removeTrailingZerosFromString(text);

		// set text to "0" if text is empty;
		if (text == "") text = "0";

		return text;
	}

	private string removeTrailingZerosFromString(string number) {

		int start = 0;

		while(start < number.Length && number[start] == '0') {

			start++;
		}

		if (start == number.Length) return "";

		return number.Substring(start);
	}

	private void testTrailingZeroRemove() {

		string testString = "0000000asdlkgh0000dfghj";
		print("Trailing Zero Removal: input: " + testString + " result:  |" + removeTrailingZerosFromString(testString) + " |");

		testString = "000";
		print("Trailing Zero Removal: input: " + testString + " result:  |" + removeTrailingZerosFromString(testString) + " |");
	}


	public string getHexNumberBase36(){
		return hexNumberInBase36;
	}

	private void test36ConversionMethods(){
		string oldNumber = hexNumberInBase36 = generateRandomHexagonNameInBase36();
		print ("random hex: " + hexNumberInBase36);
		string result = turnHexNumber10To36(turnHexNumber36To10());
		print ("after conv: " + result);
		print( "Conversion works: " + oldNumber == result);
	}

	/*
	private void createPage(){
		long randomPagePosition = (long)Random.Range(0,3431410);
		pageData = hexagonNumber;
		pageData[pageData.Length - 1] += randomPagePosition;
	}


	public void generateRandomPage(){
		print ("random page generated");
		this.generateRandomHexagonNumber();
		this.createPage();
	}
	*/

	//turns the argument data into a string (base 10 -> base 29)
	public string getPageFromData(long[] data){
		string text = "";
		for(int i = 0; i < data.Length; i++){
			text += addMissingCharacters(DecimalToArbitrarySystem(data[i],29,BOOKCHARACTERSET),10,'a');
		}
		return text;
	}

	//turns the LOCAL pageData into a string
	public string getPageFromData(){
		string text = "";
		for(int i = 0; i < pageData.Length; i++){
			text += addMissingCharacters(DecimalToArbitrarySystem(pageData[i],29,BOOKCHARACTERSET),10,'a');
		}
		return text;
	}
	
	public long[] turnPageIntoData(string pagetext){
		if(pagetext.Length != 3200){
			print("pagetext length is not 3200");
			return null;
		}
		long[] number = new long[LongNumber];
		int startindex = 0;
		for(int i = 0; i < LongNumber; i++){
			startindex = i * 10;
			number[i] = ArbitraryToDecimalSystem(pagetext.Substring(startindex,10),29,BOOKCHARACTERSET);
		}
		return number;
	}

	public void setHexNumber(long[] number){
		for(int i = 0; i< number.Length; i++){
			hexagonNumber[i] = number[i];
		}
	}

	public string getHexagonNumberAsString(){
		string hexstring = "";
		for(int i=0; i < hexagonNumber.Length; i++){
			hexstring += addMissingCharacters(hexagonNumber[i].ToString(),15,'0');
		}
		return hexstring;
	}

	public string getHexagonNumberAsString(long[] number){
		string hexstring = "";
		for(int i=0; i < number.Length; i++){
			hexstring += addMissingCharacters(number[i].ToString(),15,'0');
		}
		return hexstring;
	}

	//only accepts strings with exactly 320 * 15 = 4800 characters
	public long[] turnStringIntoHexagonNumber(string s){
		if(s.Length != 4800){
			return null;
		}
		long[] number = new long[LongNumber];
		for(int i = 0; i < LongNumber; i++){
			int startIndex = i * 15;	//because each long takes 15 characters		=> 	substring begin at: 0,15,30,45...
			if(!long.TryParse(s.Substring(startIndex,15), out number[i]) == true){		//turns the substring into a long value, if not possible returns null (not allowed character in string)
				return null;
			}
			number[i] = number[i] % MAXLONG;
		}
		return number;
	}

	public void testStringToHexNumberMethods(){
		print(getHexagonNumberAsString());
		hexagonNumber = turnStringIntoHexagonNumber(getHexagonNumberAsString());
		print(getHexagonNumberAsString());
	}

	//THE ALGORITHM that turns the hexagonNumber into the pageData
	public long[] algorithm(int roomPos){
		for(int i = 0; i < hexagonNumber.Length; i++){
			pageData[i] = lcg(hexagonNumber[i],roomPos);
		}
		return pageData;
	}

	//The Algorithm that turns a pageData into the hexagon number AND saves the roomposition
	public long[] algorithmInverted(long[] pageText){
		long[] searchedHexagonNumber = new long[LongNumber];
	
		roomposition = generateRandomRoomPosition();
		int roomPosNumber = calcRoomPosNumber(roomposition);

		/*
		//test
		roomposition[0] = 1;
		roomposition[1] = 10;
		roomposition[2] = 2;
		roomposition[3] = 3;
		roomPosNumber = calcRoomPosNumber(roomposition);
		*/

		print("This page can be found at this position in the hexagon: " + roomPosNumber);

		for(int i = 0; i < pageText.Length; i++){
			searchedHexagonNumber[i] = lcgInverted(pageText[i],roomPosNumber);
		}

		return searchedHexagonNumber;
	}

	/*
	private int[] generateRandomRoomPosition(){
		int[] randPos = new int[4];
		//at index 0 BETWEEN 0 - 409
		randPos[0] = (int)Random.Range(0,409);
		//at index 1 BETWEEN 0 - 31
		randPos[1] = (int)Random.Range(0,31);
		//at index 2 BETWEEN 0 - 4
		randPos[2] = (int)Random.Range(0,4);
		//at index 3 BETWEEN 0 - 3
		randPos[3] = (int)Random.Range(0,3);
		return randPos;
	}
	*/

	private int[] generateRandomRoomPosition(){
		int[] randPos = new int[4];
		//at index 0 BETWEEN 0 - 409
		randPos[1] = (int)Random.Range(0,PlayerPrefs.GetInt("MAXPAGE"));
		//at index 1 BETWEEN 0 - 31
		randPos[2] = (int)Random.Range(0,PlayerPrefs.GetInt("MAXBOOK"));		
		//at index 2 BETWEEN 0 - 4
		randPos[3] = (int)Random.Range(0,PlayerPrefs.GetInt("MAXSHELF"));
		//at index 3 BETWEEN 0 - 3
		randPos[0] = (int)Random.Range(0,PlayerPrefs.GetInt("MAXWALL"));
		/*
		//REstrictions
		randPos[0] = 0; 		//always search only in first wall
		randPos[1] = randPos[1] % 99;	//only search in the first 99 pages
		*/
		return randPos;
	}

	private long lcg(long number,int factor){
		long newNumber = number;
		for(int i = 0; i < factor; i++){
			newNumber = ((A * newNumber) + B) % MOD;	//linear congruential generator: y = (a * y + b) mod m
		}
		return newNumber;
	}

	private long lcgInverted(long number, int factor){
		long newNumber = number;
		for(int i = 0; i < factor; i++){
			newNumber = bigMod(AINVERSE,newNumber - B,MOD);
			//newNumber = AINVERSE * (newNumber - B) % MOD;
			if(newNumber < 0){
				print("newNumber was less than 0");
				newNumber += MAXLONG;
			}
		}
		return newNumber;
	}

	/*
	//returns the specific position of the page/book/shelf/wall
	public int calcRoomPosNumber(int wallNumber, int shelfNumber, int bookNumber, int pageNumber){
		int pos = 0;

		pos = pageNumber * 10000;
		pos += bookNumber * 100;
		pos += shelfNumber * 10;
		pos += wallNumber;

		return pos;
	}
*/

	//returns the specific position of the page/book/shelf/wall
	public int calcRoomPosNumber(int wallNumber, int shelfNumber, int bookNumber, int pageNumber){
		int pos = 0;
		
		pos += wallNumber * 1000000;
		pos += pageNumber * 1000;
		pos += bookNumber * 10;
		pos += shelfNumber;
		
		return pos;
	}
	
	private int calcRoomPosNumber(int[] array){
		int pos = 0;
		
		pos += array[0] * 1000000;
		pos += array[1] * 1000;
		pos += array[2] * 10;
		pos += array[3];
		
		return pos;
	}

	public int[] getRoomPosition(){
		return roomposition;
	}

	public void addToHexNumber(int n){
		int currentIndex = hexagonNumber.Length - 1;	//lastIndex
		hexagonNumber[currentIndex] += n;
		if(n > 0){
			while(hexagonNumber[currentIndex] > MAXLONG){
				hexagonNumber[currentIndex] = hexagonNumber[currentIndex] % MAXLONG;
				currentIndex --;
				hexagonNumber[currentIndex] += n;
			}
		}
		if(n < 0){
			while(hexagonNumber[currentIndex] < 0){
				hexagonNumber[currentIndex] = hexagonNumber[currentIndex] + MAXLONG;
				currentIndex --;
				hexagonNumber[currentIndex] += n;
			}
		}
	}

	public void addToAllHexNumbers(int n){
		for(int i = 0; i < hexagonNumber.Length; i++){
			hexagonNumber[i] = (hexagonNumber[i] + n) % MAXLONG;
			if(hexagonNumber[i] < 0){
				hexagonNumber[i] = hexagonNumber[i] + MAXLONG;
			}
		}
	}

	/** Currently only works for n = 1. */
	public void addToHexNumber36(int n){
		long[] hexagonNumber = turnHexNumber36To10();

		//printNumberArray(hexagonNumber);

		int currentIndex = hexagonNumber.Length - 2 < 0 ? 0 : hexagonNumber.Length - 2;	//lastIndex
		//print("Index: " + currentIndex + " with number: " + hexagonNumber[currentIndex]);
		hexagonNumber[currentIndex] += n;

		if(n > 0){
			while(currentIndex >= 0 && hexagonNumber[currentIndex] > MAXLONGFORBASE36){
				hexagonNumber[currentIndex] = (hexagonNumber[currentIndex] % MAXLONGFORBASE36) - 1;
				currentIndex --;
				if (currentIndex < 0) break;
				hexagonNumber[currentIndex] += 1;
			}
		}
		if(n < 0){
			while(currentIndex >= 0 && hexagonNumber[currentIndex] < 0){
				hexagonNumber[currentIndex] = MAXLONGFORBASE36;  //hexagonNumber[currentIndex] + MAXLONGFORBASE36;
				currentIndex --;
				if (currentIndex < 0) break;
				hexagonNumber[currentIndex] -= 1;
			}
		}

		//print("n:" + n);
		//print("Last Number:" + hexagonNumber[361]);
		hexNumberInBase36 = turnHexNumber10To36(hexagonNumber);
	}

	public void addToAllHexNumbers36(int n){
		long[] hexagonNumber = turnHexNumber36To10();

		for(int i = 0; i < hexagonNumber.Length - 1; i++){
			hexagonNumber[i] = (hexagonNumber[i] + n) % MAXLONGFORBASE36;
			if(hexagonNumber[i] < 0){
				hexagonNumber[i] = hexagonNumber[i] + MAXLONGFORBASE36;
			}
		}

		hexNumberInBase36 = turnHexNumber10To36(hexagonNumber);
	}

	//return (a * b) mod c
	public long bigMod(long  a, long  b, long c) {
		if (a == 0 || b == 0) {
			return 0;
		}
		if (a == 1) {
			return b;
		}
		if (b == 1) {
			return a;
		} 
		
		// Returns: (a * b/2) mod c
		long a2 = bigMod(a, b / 2, c);
		
		// Even factor
		if ((b & 1) == 0) {
			// [((a * b/2) mod c) + ((a * b/2) mod c)] mod c
			return (a2 + a2) % c;
		} else {
			// Odd exponent
			// [(a mod c) + ((a * b/2) mod c) + ((a * b/2) mod c)] mod c
			return ((a % c) + (a2 + a2)) % c;
		}
	}
	
	private void testLCGs(){
		long testNumber = 123456789;
		print("The test number is: " + testNumber);
		long numberAfterLCG = lcg(testNumber,1);
		print("The number after passing the lcg is: " + numberAfterLCG);
		long numberAfterLCGInverted = lcgInverted(numberAfterLCG,1);
		print("After passing the LCGInverted the number is: " + numberAfterLCGInverted);
	}

	public string getAlphabet(){
		return BOOKCHARACTERSET;
	}

	private void printNumberArray(long[] number) {
		string result = "";
		foreach (long num in number) {
			result += " " + num;
		}
		print("long number array: " + result);

	}

	/// <summary>
	/// Converts the given decimal number to the numeral system with the
	/// specified radix (in the range [2, 36]).
	/// </summary>
	/// <param name="decimalNumber">The number to convert.</param>
	/// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
	/// <returns></returns>
	public static string DecimalToArbitrarySystem(long decimalNumber, int radix,string characterset)
	{
		const int BitsInLong = 64;
		string Digits = characterset;
		
		if (radix < 2 || radix > Digits.Length)
			throw new System.ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());
		
		if (decimalNumber == 0)
			return "0";
		
		int index = BitsInLong - 1;
		long currentNumber = System.Math.Abs(decimalNumber);
		char[] charArray = new char[BitsInLong];
		
		while (currentNumber != 0)
		{
			int remainder = (int)(currentNumber % radix);
			charArray[index--] = Digits[remainder];
			currentNumber = currentNumber / radix;
		}
		
		string result = new string(charArray, index + 1, BitsInLong - index - 1);
		if (decimalNumber < 0)
		{
			result = "-" + result;
		}
		
		return result;
	}

	/// <summary>
	/// Converts the given number from the numeral system with the specified
	/// radix (in the range [2, 36]) to decimal numeral system.
	/// </summary>
	/// <param name="number">The arbitrary numeral system number to convert.</param>
	/// <param name="radix">The radix of the numeral system the given number
	/// is in (in the range [2, 36]).</param>
	/// <returns></returns>
	public static long ArbitraryToDecimalSystem(string number, int radix,string characterset)
	{
		string Digits = characterset;
		
		if (radix < 2 || radix > Digits.Length)
			throw new System.ArgumentException("The radix must be >= 2 and <= " +
			                            Digits.Length.ToString());
		
		if (string.IsNullOrEmpty(number))
			return 0;
		
		// Make sure the arbitrary numeral system number is in lower case
		number = number.ToLowerInvariant();
		
		long result = 0;
		long multiplier = 1;
		for (int i = number.Length - 1; i >= 0; i--)
		{
			char c = number[i];
			if (i == 0 && c == '-')
			{
				// This is the negative sign symbol
				result = -result;
				break;
			}
			
			int digit = Digits.IndexOf(c);
			if (digit == -1)
				throw new System.ArgumentException(
					"Invalid character in the arbitrary numeral system number",
					"number");
			
			result += digit * multiplier;
			multiplier *= radix;
		}
		
		return result;
	}

	/*
	public void setPageNumber(int p){
		pageNumber = p;
	}

	public void setBookNumber(int p){
		bookNumber = p;
	}

	public void setShelfNumber(int p){
		shelfNumber = p;
	}

	public void setWallNumber(int p){
		wallNumber = p;
	}
*/

	//Euclid Functions

	private EuclidExtendedSolution extEuclid(long a,long b)
	{
		long x0 = 1, xn = 1;
		long y0 = 0, yn = 0;
		long x1 = 0;
		long y1 = 1;
		long q;
		long r = a % b;
		
		while (r > 0)
		{
			q = a / b;
			xn = x0 - q * x1;
			yn = y0 - q * y1;
			
			x0 = x1;
			y0 = y1;
			x1 = xn;
			y1 = yn;
			a = b;
			b = r;
			r = a % b;
		}
		
		return new EuclidExtendedSolution(xn, yn, b);
	}
	
	private class EuclidExtendedSolution
	{
		private long x, y, d;
		
		public long X
		{
			get
			{
				return this.x;
			}
		}
		
		public long Y
		{
			get
			{
				return this.y;
			}
		}
		
		public long D
		{
			get
			{
				return this.d;
			}
		}
		
		public EuclidExtendedSolution(long x, long y, long d)
		{
			this.x = x;
			this.y = y;
			this.d = d;
		}
	}
}
