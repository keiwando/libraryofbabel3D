using System;

public class OfflineLibrary: ILibrary {
	
	public OfflineLibrary() {

	}

	public ILibrarySearch GetSearch() {
		return new OfflineSearch();
	}

	public void RequestPages(PageLocation[] pages, OnPageRequestCompleted onCompletion) {
		// TODO: Implement offline library
	}

	public void RequestBookTitles(ShelfLocation shelfLocation, OnTitleRequestCompleted onCompletion) {
		// TODO: Implement offline library
	}

	public void RequestBookTitle(PageLocation page, OnTitleRequestCompleted onCompletion) {
		// TODO: Implement offline library
	}

	private class OfflineSearch: ILibrarySearch {

		public void Search(string text, bool exactMatch, OnSearchCompleted onCompletion) {
			// TODO: Implement Offline library
		}
	}
}


