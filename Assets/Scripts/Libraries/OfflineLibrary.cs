using ScottGarland;

public class OfflineLibrary: ILibrary {
	
	// LCG variables
	private BigInteger m;
	private BigInteger a;
	private BigInteger c;
	private BigInteger aInverse;

	// Bitshift variables
	private static readonly int w = 15546; // 2^15545 < 29^3200 < 2^15546
	private static readonly int flipHighCount = 40;
	private static readonly int flipLowCount = w - flipLowCount;
	private static readonly BigInteger lowMask = (new BigInteger(1) << flipLowCount) - 1;
	private static readonly BigInteger highMask = ~lowMask;

	private BigInteger relativeLocationOffset = BigInteger.Pow(10, 100);

	public OfflineLibrary() {

		var min_m = BigInteger.Pow(29, 3200);
		var a_minus_1 = (min_m / 2);
		a = a_minus_1 + 1;
		m = 4 * a_minus_1;
		c = (BigInteger.Pow(2, 11213) - 1) * (BigInteger.Pow(2, 4253) - 1) * 2305843009213693951 * 8191 * 13;
		aInverse = Keiwando.Lob.Math.extendedGcd(a, m).x;
	}

	private Page PageAtLocation(PageLocation location) {

		var hexName = new BigInteger(location.Hex.Name, 36);
		BigInteger absoluteLocation = hexName + relativeLocationOffset * (location.Page * 10000 + location.Book * 100 + location.Shelf * 10 + location.Wall);
		// BigInteger absoluteLocation = hexName + relativeLocationOffset * (location.Wall * 10000 + location.Book * 100 + location.Shelf * 10 + location.Page);

		return new Page() {
			Location = location,
			Text = AbsoluteLocationToPageText(absoluteLocation)
		};
	}

	private string AbsoluteLocationToPageText(BigInteger absoluteLocation) {

		// Flip some of the first and last of the location bits
		// so that consecutive rooms lead to bigger differences in book contents
		var highBits = absoluteLocation >> flipLowCount;
		var lowBits = lowMask & absoluteLocation;
		var flippedLocation = (lowBits << flipLowCount) + highBits;

		var y = LCG(flippedLocation);

		var contents = y.ToString(29, Universe.Alphabet).PadRight(3200);
		return contents.Substring(415, 3200 - 415) + contents.Substring(0, 415);
	}

	private BigInteger LCG(BigInteger x) {
		return BigInteger.Modulus(a * x + c, m);
	}

	private BigInteger InvLCG(BigInteger x) {
		return BigInteger.Modulus(aInverse * (x - c), m);
	}

	// MARK: - ILibrary

	public ILibrarySearch GetSearch() {
		return new OfflineSearch();
	}

	public void RequestPages(PageLocation[] locations, OnPageRequestCompleted onCompletion) {
		
		var pages = new Page[locations.Length];
		for (int i = 0; i < locations.Length; i++) {
			pages[i] = PageAtLocation(locations[i]);
		}
		onCompletion(pages);
	}

	public void RequestBookTitles(ShelfLocation shelfLocation, OnTitleRequestCompleted onCompletion) {
		// TODO: Implement offline library
		onCompletion(new string[32]);
	}

	public void RequestBookTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {
		// TODO: Implement offline library
		onCompletion(new string[1]);
	}

	private class OfflineSearch: ILibrarySearch {

		public void Search(string text, bool exactMatch, OnSearchCompleted onCompletion) {
			// TODO: Implement Offline library
			onCompletion(new SearchResult());
		}
	}
}


