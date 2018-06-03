using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SearchMenu : MonoBehaviour {

	private ILibrarySearch search;


	void Start() {
		
	}
	
	public void Show(ILibrarySearch search = null) {

		this.search = search;

		gameObject.SetActive(true);
	}

	public void Hide() {

		gameObject.SetActive(false);
	}
}

