using System;
using ScottGarland;

public class OfflineLibrary: ILibrary {
	
	// LCG variables
	private BigInteger m;
	private BigInteger a;
	private BigInteger c;
	private BigInteger aInverse;

	// Bitshift variables
	private static readonly int w = 15546; // 2^15545 < 29^3200 < 2^15546
	private static readonly int flipLowCount = 340;
	private static readonly int flipHighCount = w - flipLowCount;
	private static readonly BigInteger lowMask = (new BigInteger(1) << flipLowCount) - 1;
	private static readonly BigInteger invLowMask = (new BigInteger(1) << flipHighCount) - 1;

	internal BigInteger relativeLocationOffset = BigInteger.Pow(10, 100);

	public OfflineLibrary() {

		var min_m = BigInteger.Pow(29, 3200);
		var a_minus_1 = (min_m / 2);
		a = a_minus_1 + 1;
		m = 4 * a_minus_1;
		c = (BigInteger.Pow(2, 11213) - 1) * (BigInteger.Pow(2, 4253) - 1) * 2305843009213693951 * 8191 * 13;
		aInverse = Keiwando.Lob.Math.extendedGcd(a, m).x;

		// TODO: Remove after debugging
		// var testLocation = new BigInteger("2265987465878734658734653346748672368749748569837464128689712675349875618365378256435");
		// // var testLocation = 0;
		// var testPage = AbsoluteLocationToPageText(testLocation);
		// var locationFromPage = PageTextToAbsoluteLocation(testPage);
		// var pageFromLocationFromPage = AbsoluteLocationToPageText(locationFromPage);
		// var loc2 = PageTextToAbsoluteLocation(pageFromLocationFromPage);
		// var pg2 = AbsoluteLocationToPageText(loc2);
		// UnityEngine.Debug.Log(pageFromLocationFromPage == testPage);
		// UnityEngine.Debug.Log(InvLCG(LCG(testLocation)) == testLocation);
	}

	private Page PageAtLocation(PageLocation location) {

		var hexName = new BigInteger(location.Hex.Name, 36);
		BigInteger absoluteLocation = hexName + relativeLocationOffset * ((location.Page - 1)  * 10000 + (location.Book - 1) * 100 + (location.Shelf - 1) * 10 + (location.Wall - 1));

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
		var flippedLocation = (lowBits << flipHighCount) + highBits;

		var y = LCG(flippedLocation);

		var contents = y.ToString(29, Universe.Alphabet).PadLeft(3201, ' ').Substring(1, 3200);
		return contents;
		// return contents.Substring(415, 3200 - 415) + contents.Substring(0, 415);
	}

	private BigInteger PageTextToAbsoluteLocation(string text) {

		// var contents = text.Substring(3200 - 415, 415) + text.Substring(0, 3200 - 415);
		var contents = text;
		var y = new BigInteger(contents, 29, Universe.Alphabet);
		
		var flippedLocation = InvLCG(y);

		var highBits = flippedLocation >> flipHighCount;
		var lowBits = invLowMask & flippedLocation;
		var absoluteLocation = (lowBits << flipLowCount) + highBits;

		return absoluteLocation;
	}

	private BigInteger LCG(BigInteger x) {
		return BigInteger.ActualModulus(a * x + c, m);
	}

	private BigInteger InvLCG(BigInteger x) {
		return BigInteger.ActualModulus(aInverse * (x - c), m);
	}

	// MARK: - ILibrary

	public ILibrarySearch GetSearch() {
		return new OfflineSearch(this);
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

		private OfflineLibrary library;
		internal OfflineSearch(OfflineLibrary library) {
			this.library = library;
		}

		public void Search(string text, bool exactMatch, OnSearchCompleted onCompletion) {
			// TODO: Implement Offline library
			text = text.ToLower();
			text = exactMatch ? Universe.FillPageBlank(text) : Universe.FillPageRandomly(text);

			var absoluteLocation = library.PageTextToAbsoluteLocation(text);
			
			// Extract the segment(s) in the absolute location that contain the relative location information
			var relativeSegment = (absoluteLocation % (library.relativeLocationOffset * 10000000)) / library.relativeLocationOffset;
			var pageSegment = relativeSegment / 10000;
			var bookSegment = (relativeSegment % 100000) / 100;
			var shelfSegment = (relativeSegment % 100) / 10;
			var wallSegment = relativeSegment % 10;

			// Extract the possible range of wall, shelf, book and page numbers
			var maxPage = Math.Min(409, BigInteger.ToInt32(pageSegment));
			var maxBook = Math.Min(31, BigInteger.ToInt32(bookSegment));
			var maxShelf = Math.Min(4, BigInteger.ToInt32(shelfSegment));
			var maxWall  = Math.Min(3, BigInteger.ToInt32(wallSegment));

			// Pick random a random relative location within the given limits
			var page = UnityEngine.Random.Range(0, maxPage);
			var book = UnityEngine.Random.Range(0, maxBook);
			var shelf = UnityEngine.Random.Range(0, maxShelf);
			var wall = UnityEngine.Random.Range(0, maxWall);

			// TODO: Remove after debugging
			// page = 0; book = 0; shelf = 0; wall = 0;

			// Reconstruct the hexagon location
			var relativeLocation = page * 10000 + book * 100 + shelf * 10 + wall;
			var hexLocation = absoluteLocation - library.relativeLocationOffset * relativeLocation;

			onCompletion(new SearchResult() {
				Title = "", // TODO: Add book titles
				Hex = new HexagonLocation(hexLocation), // TODO: Turn this into a HexagonLocation
				Wall = wall + 1,
				Shelf = shelf + 1,
				Book = book + 1,
				Page = page + 1
			});
		}
	}
}


