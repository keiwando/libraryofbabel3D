using System;

public delegate void OnPageRequestCompleted(Page[] pages);
public delegate void OnTitleRequestCompleted(string[] titles);

public interface ILibrary {

	ILibrarySearch GetSearch();
	void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion);
	void RequestBookTitles(ShelfLocation shelfLocation, OnTitleRequestCompleted onCompletion);
	void RequestBookTitle(PageLocation page, OnTitleRequestCompleted onCompletion);
}


