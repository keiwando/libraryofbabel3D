using System;

public struct PageLocation {	

	public static readonly int MinPage = 1;
	public static readonly int MaxPage = Universe.PAGES_PER_BOOK;

	public HexagonLocation Hex { get; set; }
	/// <summary>
	/// The wall that this page is on. 1-4
	/// </summary>
	public int Wall { get; set; }
	/// <summary>
	/// The shelf that this page is on. 1-5
	/// </summary>
	public int Shelf { get; set; }
	/// <summary>
	/// The book that this page is in. 1-32
	/// </summary>
	public int Book { get; set; }
	/// <summary>
	/// The page number. 1-410
	/// </summary>
	/// <value>The page.</value>
	public int Page { get; set; }

	public bool HasPreviousPage() {
		return Page > 1;
	}

	public bool HasNextPage() {
		return Page < 410;
	}

	public PageLocation PreviousPage() {

		if (!HasPreviousPage())
			throw new InvalidOperationException("This is the first page!");

		var copy = Clone();
		copy.Page -= 1;
		return copy;
	}

	public PageLocation NextPage() {

		if (!HasNextPage())
			throw new InvalidOperationException("This is the last page!");

		var copy = Clone();
		copy.Page += 1;
		return copy;
	}

	public PageLocation Clone() {
		
		return new PageLocation() {
			Hex = Hex,
			Wall = Wall,
			Shelf = Shelf,
			Book = Book,
			Page = Page
		};
	}

	public ShelfLocation GetShelfLocation() {
		return new ShelfLocation () {
			Hex = Hex,
			Wall = Wall,
			Shelf = Shelf
		};
	}
}


