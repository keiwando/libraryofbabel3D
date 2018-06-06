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
		text.text = GetWall(wall) + " " + GetShelf(shelf) + " " + GetBook(book);
	}

	private string GetWall(int wall){

		return wall == 0 ? "" : "W:" + wall;
	}

	private string GetShelf(int shelf){

		return shelf == 0 ? "" : "S:" + shelf;
	}

	private string GetBook(int book){

		return book == 0 ? "" : "B:" + book;
	}

	public override string ToString(){
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
