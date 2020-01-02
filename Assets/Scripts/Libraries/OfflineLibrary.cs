using ScottGarland;
using Keiwando.Lob;

public class OfflineLibrary: ILibrary {

	private Math.LCGParameters pageLCGParams;
	private Math.FlipParameters pageFlipParams;

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

		// We can't touch the first 5 bits in order to guarantee that the input
		// of the LCG is always < m
		pageFlipParams = new Math.FlipParameters(15546, 5, 40); // 2^15545 < 29^3200 < 2^15546
	}

	private Page PageAtLocation(PageLocation location) {

		var hexName = new BigInteger(location.Hex.Name, 36);
		BigInteger absoluteLocation = hexName + relativeLocationOffset * ((location.Page - 1) * 10000 + (location.Book - 1) * 100 + (location.Shelf - 1) * 10 + (location.Wall - 1));
		absoluteLocation = BigInteger.ActualModulus(absoluteLocation, pageLCGParams.m);

		return new Page() {
			Location = location,
			Text = AbsoluteLocationToPageText(absoluteLocation)
		};
	}

	private string AbsoluteLocationToPageText(BigInteger absoluteLocation) {

		// Flip some of the first and last of the location bits
		// so that consecutive rooms lead to bigger differences in book contents
		var flippedLocation = Math.FlipBits(absoluteLocation, pageFlipParams);

		var y = Math.LCG(flippedLocation, pageLCGParams);

		var contents = y.ToString(29, Universe.Alphabet).PadLeft(3201, ' ').Substring(1, 3200);
		return contents.Substring(414, 3200 - 414) + contents.Substring(0, 414);
	}

	private BigInteger PageTextToAbsoluteLocation(string text) {

		var contents = text.Substring(3200 - 414, 414) + text.Substring(0, 3200 - 414);
		var y = new BigInteger(contents, 29, Universe.Alphabet);
		var testStr = y.ToString(29, Universe.Alphabet);
		
		var flippedLocation = Math.InverseLCG(y, pageLCGParams);

		return Math.InverseFlipBits(flippedLocation, pageFlipParams);
	}

	private string[] AbsoluteLocationToTitles(BigInteger absoluteLocation) {

		var page = AbsoluteLocationToPageText(absoluteLocation);
		var titlesAsOne = page.Substring(1899, 26 * 32);
		var titles = new string[32];
		for (int i = 0; i < 32; i++) {
			titles[i] = titlesAsOne.Substring(i * 26, 26);
		}
		return titles;
	}

	private string[] ShelfLocationToTitles(ShelfLocation location) {
		var absoluteLocation = location.Hex.Value + relativeLocationOffset * (97 * location.Shelf + 42 * location.Wall);
		return AbsoluteLocationToTitles(absoluteLocation);
	}

	private string PageLocationToTitle(PageLocation location) {
		var titles = ShelfLocationToTitles(location.GetShelfLocation());
		return titles[location.Book - 1];
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
		
		onCompletion(ShelfLocationToTitles(shelfLocation));
	}

	public void RequestBookTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {
		
		onCompletion(new string[] { PageLocationToTitle(page) });
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
			UnityEngine.Debug.Log(library.AbsoluteLocationToPageText(absoluteLocation) == text);
			
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

			var pageLocation = new PageLocation() {
				Hex = new HexagonLocation(hexLocation),
				Wall = wall + 1,
				Shelf = shelf + 1,
				Book = book + 1,
				Page = page + 1
			};

			onCompletion(new SearchResult() {
				Title = "", //library.PageLocationToTitle(pageLocation), 
				Hex = pageLocation.Hex,
				Wall = pageLocation.Wall,
				Shelf = pageLocation.Shelf,
				Book = pageLocation.Book,
				Page = pageLocation.Page
			});
		}
	}
}


