using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChoiceIndicator : MonoBehaviour {
	
	private Text text;

	void Start() {
		this.text = GetComponent<Text>();
	}

	public void Refresh(int wall, int shelf, int book){
		text.text = getWall() + " " + getShelf() + " " + getBook();
	}

	private string getWall(int wall){

		return wall == 0 ? "" : "W:" + wall;
	}

	private string getShelf(int shelf){

		return shelf == 0 ? "" : "S:" + shelf;
	}

	private string getBook(int book){

		return book == 0 ? "" : "B:" + book;
	}

	public string ToString(){
		return text.text;
	}

	public void Reset(){
		Refresh(0, 0, 0);
	}

	public void setVisible(bool b){
		if(b){
			text.enabled = true;
		}else{
			text.enabled = false;
		}
	}

}
