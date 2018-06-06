using System;

public struct ShelfLocation {

	public HexagonLocation Hex { get; set; }
	public int Wall { get; set; }
	public int Shelf { get; set; }

	public static ShelfLocation FromPageLocation(PageLocation page) {
		return new ShelfLocation() {
			Hex = page.Hex,
			Wall = page.Wall,
			Shelf = page.Shelf
		};
	}
}