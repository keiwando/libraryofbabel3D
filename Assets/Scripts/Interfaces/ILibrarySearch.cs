using UnityEngine;
using System.Collections;

public enum SearchError {
	None,
	Offline,
	IllegalInput
}

public struct SearchResult {

	public PageLocation PageLocation { get; set; }

	public SearchError Error { get; set; }
}

public delegate void OnSearchCompleted(SearchResult result);

public interface ILibrarySearch {

	void Search(string text, bool exactMatch, OnSearchCompleted onCompletion); 
}

