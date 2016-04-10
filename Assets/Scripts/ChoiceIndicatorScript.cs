using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChoiceIndicatorScript : MonoBehaviour {

	public int wall {get;set;}
	public int shelf {get;set;}
	public int book {get;set;}

	public Text text;

	void Start () {
	
	}
	

	void Update () {
	
	}

	public void updateText(){
		text.text = getWall() + " " + getShelf() + " " + getBook();
	}

	private string getWall(){
		if(wall != 0){
			return "W:" + wall;
		}else{
			return "";
		}
	}

	private string getShelf(){
		if(shelf != 0){
			return "S:" + shelf;
		}else{
			return "";
		}
	}

	private string getBook(){
		if(book != 0){
			return "B:" + book;
		}else{
			return "";
		}
	}

	public string toString(){
		return text.text;
	}

	public void reset(){
		wall = 0;
		shelf = 0;
		book = 0;
		updateText();
	}

	public void setVisible(bool b){
		if(b){
			text.enabled = true;
		}else{
			text.enabled = false;
		}
	}

}
