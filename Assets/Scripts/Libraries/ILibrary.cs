using System;

public interface ILibrary {

	ILibrarySearch GetSearch();
	void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion);
}


