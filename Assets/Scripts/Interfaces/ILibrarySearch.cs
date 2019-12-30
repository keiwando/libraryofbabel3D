using UnityEngine;
using System.Collections;

public enum SearchError {
	None,
	Offline,
	IllegalInput
}

public struct SearchResult {
	
	public string Title { get; set; }
	public HexagonLocation Hex { get; set; }
	/// <summary> The wall number between 1 and 4.</summary>
	public int Wall { get; set; }
	/// <summary> The shelf number between 1 and 5.</summary>
	public int Shelf { get; set; }
	/// <summary> The book number between 1 and 32.</summary>
	public int Book { get; set; }
	/// <summary> The page number between 1 and 410.</summary>
	public int Page { get; set; }
}

public delegate void OnSearchCompleted(SearchResult result);

public interface ILibrarySearch {

	void Search(string text, bool exactMatch, OnSearchCompleted onCompletion); 
}

