using UnityEngine;
using System.Collections;

public enum SearchError {
	None,
	Offline,
	IllegalInput
}

public struct SearchResult {
	
	public string Title { get; set; }
	public string HexName { get; set; }
	public int WallNum { get; set; }
	public int ShelfNum { get; set; }
	public int BookNum { get; set; }
	public int PageNum { get; set; }
}

public delegate void OnSearchCompleted(SearchResult result);

public interface ILibrarySearch {

	void Search(string text, bool exactMatch, OnSearchCompleted onCompletion); 
}

