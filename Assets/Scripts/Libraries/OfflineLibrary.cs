using System;

public class OfflineLibrary: ILibrary {
	
	public OfflineLibrary() {

	}

	public ILibrarySearch GetSearch() {
		return new OfflineSearch();
	}

	private class OfflineSearch {
		
	}
}


