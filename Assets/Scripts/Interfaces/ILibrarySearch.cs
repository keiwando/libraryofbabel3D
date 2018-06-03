using UnityEngine;
using System.Collections;

public enum SearchError {
	None,
	Offline,
	IllegalInput
}

public struct SearchResult {

	public string Hex { get; set; }
	public int Wall { get; set; }
	public int Shelf { get; set; }
	public int Book { get; set; }
	public int Page { get; set; }

	public SearchError Error { get; set; }
}

public delegate void OnSearchCompleted(SearchResult result);

public interface ILibrarySearch {

	void Search(string text, bool exactMatch, OnSearchCompleted onCompletion); 
}

