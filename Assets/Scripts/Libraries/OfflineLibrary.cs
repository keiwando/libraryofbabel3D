using ScottGarland;
using Keiwando.Lob;

public class OfflineLibrary: ILibrary {

	// LCG variables
	private Math.LCGParameters pageLCGParams;
	private Math.LCGParameters titleLCGParams;

	// Bitshift variables
	private static readonly int w = 15546; // 2^15545 < 29^3200 < 2^15546
	private static readonly int safeHighBits = 5;
	private static readonly int flipLowCount = 40;
	private static readonly int flipHighCount = w - flipLowCount - safeHighBits;
	private static readonly int bitsToFlip = flipLowCount + flipHighCount;
	private static readonly BigInteger lowMask = (new BigInteger(1) << flipLowCount) - 1;
	private static readonly BigInteger invLowMask = (new BigInteger(1) << flipHighCount) - 1;
	private static readonly BigInteger flipMask = (new BigInteger(1) << bitsToFlip) - 1;

	private BigInteger relativeLocationOffset = BigInteger.Pow(10, 24);

	public OfflineLibrary() {

		// Setup page LCG
		{
			var min_m = BigInteger.Pow(29, 3200);
			var a_minus_1 = (min_m / 2);
			pageLCGParams.a = a_minus_1 + 1;
			pageLCGParams.m = 4 * a_minus_1;
			pageLCGParams.c = (BigInteger.Pow(2, 11213) - 1) * (BigInteger.Pow(2, 4253) - 1) * 2305843009213693951 * 8191 * 13;
			pageLCGParams.aInverse = Math.extendedGcd(pageLCGParams.a, pageLCGParams.m).x;
		}

		// Setup title LCG
		{
			var min_m = BigInteger.Pow(29, 26);
			var a_minus_1 = (min_m / 2);
			titleLCGParams.a = a_minus_1 + 1;
			titleLCGParams.m = 4 * a_minus_1;
			titleLCGParams.c = BigInteger.Pow(2, 127) - 1;
			titleLCGParams.aInverse = Math.extendedGcd(titleLCGParams.a, titleLCGParams.m).x;
		}
	}

	private Page PageAtLocation(PageLocation location) {

		var hexName = new BigInteger(location.Hex.Name, 36);
		BigInteger absoluteLocation = hexName + relativeLocationOffset * ((location.Page - 1) * 10000 + (location.Book - 1) * 100 + (location.Shelf - 1) * 10 + (location.Wall - 1));
		// BigInteger absoluteLocation = hexName + 10000000 * ((location.Wall - 1)  * 10000 + (location.Book - 1) * 100 + (location.Shelf - 1) * 10 + (location.Page - 1));
		absoluteLocation = BigInteger.ActualModulus(absoluteLocation, pageLCGParams.m);

		return new Page() {
			Location = location,
			Text = AbsoluteLocationToPageText(absoluteLocation)
		};
	}

	private string AbsoluteLocationToPageText(BigInteger absoluteLocation) {

		// Flip some of the first and last of the location bits
		// so that consecutive rooms lead to bigger differences in book contents
		// We can't touch the first 5 bits in order to guarantee that the input
		// of the LCG is always < m
		var flipSegment = absoluteLocation & flipMask;
		var safeBits = absoluteLocation & (~flipMask);
		var highBits = flipSegment >> flipLowCount;
		var lowBits = lowMask & flipSegment;
		var flippedLocation = (lowBits << flipHighCount) + highBits + safeBits;

		var y = Math.LCG(flippedLocation, pageLCGParams);

		var contents = y.ToString(29, Universe.Alphabet).PadLeft(3201, ' ').Substring(1, 3200);
		return contents.Substring(415, 3200 - 415) + contents.Substring(0, 415);
	}

	private BigInteger PageTextToAbsoluteLocation(string text) {

		var contents = text.Substring(3200 - 415, 415) + text.Substring(0, 3200 - 415);
		var y = new BigInteger(contents, 29, Universe.Alphabet);
		
		var flippedLocation = Math.InverseLCG(y, pageLCGParams);

		var flipSegment = flippedLocation & flipMask;
		var safeBits = flippedLocation & (~flipMask);
		var highBits = flipSegment >> flipHighCount;
		var lowBits = invLowMask & flipSegment;
		var absoluteLocation = (lowBits << flipLowCount) + highBits + safeBits;

		return absoluteLocation;
	}

	private string AbsoluteLocationToTitle(BigInteger absoluteLocation) {

		var y = Math.LCG(absoluteLocation, titleLCGParams);
		return y.ToString(29, Universe.Alphabet);
	}

	private BigInteger PageLocationToAbsoluteLocationForTitle(PageLocation location) {
		var absoluteLocation = location.Hex.Value + relativeLocationOffset * (location.Book * 100 + location.Shelf * 10 + location.Wall);
		var highBits = absoluteLocation >> flipLowCount;
		var lowBits = lowMask & absoluteLocation;
		var flippedLocation = (lowBits << flipHighCount) + highBits;
		return flippedLocation;
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
		
		var result = new string[32];
		for (int i = 0; i < 32; i++) {
			var location = new PageLocation() {
				Hex = shelfLocation.Hex,
				Wall = shelfLocation.Wall,
				Shelf = shelfLocation.Shelf,
				Book = i + 1,
				Page = 1
			};
			var absoluteLocation = PageLocationToAbsoluteLocationForTitle(location);
			result[i] = AbsoluteLocationToTitle(absoluteLocation);
		}
		onCompletion(result);
	}

	public void RequestBookTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {
		
		var absoluteLocation = PageLocationToAbsoluteLocationForTitle(page);
		var title = AbsoluteLocationToTitle(absoluteLocation);
		onCompletion(new string[] { title });
	}

	private class OfflineSearch: ILibrarySearch {

		private OfflineLibrary library;
		internal OfflineSearch(OfflineLibrary library) {
			this.library = library;
		}

		public void Search(string text, bool exactMatch, OnSearchCompleted onCompletion) {
			
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
			var maxPage = System.Math.Min(409, BigInteger.ToInt32(pageSegment));
			var maxBook = System.Math.Min(31, BigInteger.ToInt32(bookSegment));
			var maxShelf = System.Math.Min(4, BigInteger.ToInt32(shelfSegment));
			var maxWall  = System.Math.Min(3, BigInteger.ToInt32(wallSegment));

			// Pick random a random relative location within the given limits
			var page = UnityEngine.Random.Range(0, maxPage);
			var book = UnityEngine.Random.Range(0, maxBook);
			var shelf = UnityEngine.Random.Range(0, maxShelf);
			var wall = UnityEngine.Random.Range(0, maxWall);

			// Reconstruct the hexagon location
			var relativeLocation = page * 10000 + book * 100 + shelf * 10 + wall;
			var hexLocation = absoluteLocation - library.relativeLocationOffset * relativeLocation;

			if (absoluteLocation > library.pageLCGParams.m) {
				UnityEngine.Debug.Break();
			}

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


