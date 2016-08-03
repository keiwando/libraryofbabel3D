using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour {

	public Canvas canvas;
	
	public InputField maxPage;
	public InputField maxBook;
	public InputField maxShelf;
	public InputField maxWall;

	public Toggle soundToggle;
	public Toggle musicToggle;

	public const int RECPAGE = 45;
	public const int RECBOOK = 31;
	public const int RECSHELF = 4;
	public const int RECWALL = 0;

	// Use this for initialization
	void Start () {
		canvas.enabled = false;
		if(PlayerPrefs.GetInt("FIRSTTIME",0) == 0){
			setupFirstTime();
			PlayerPrefs.SetInt("FIRSTTIME",1);
		}else{
			setup();
		}

		musicToggle.onValueChanged.AddListener(playStopMusic);
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void playStopMusic(bool b){
		if(musicToggle.isOn){
			GameObject.Find("SoundController").GetComponent<SoundController>().startMusic();
		}else{
			GameObject.Find("SoundController").GetComponent<SoundController>().stopMusic();
		}
	}

	public void backClick(){
		setVisible(false);
		saveSettings();
	}

	public void setVisible(bool v){
		if(v){
			canvas.enabled = true;
			//Update Canvas
			Canvas.ForceUpdateCanvases();
		}else{
			canvas.enabled = false;
			saveSettings();
		}
	}

	public void saveSettings(){
		int mPage = (int.Parse(maxPage.text) - 1) % 409;
		int mBook = (int.Parse(maxBook.text) - 1) % 31;
		int mShelf = (int.Parse(maxShelf.text) - 1) % 4;
		int mWall = (int.Parse(maxWall.text) - 1) % 3;

		if(mPage < 0) mPage = 0;
		if(mBook < 0) mBook = 0;
		if(mShelf < 0) mShelf = 0;
		if(mWall < 0) mWall = 0;

		PlayerPrefs.SetInt("MAXPAGE",mPage);
		PlayerPrefs.SetInt("MAXBOOK",mBook);
		PlayerPrefs.SetInt("MAXSHELF",mShelf);
		PlayerPrefs.SetInt("MAXWALL",mWall);

		int soundSetting;
		if(soundToggle.isOn){
			soundSetting = 1;
		}else{
			soundSetting = 0;
		}
		PlayerPrefs.SetInt("SOUND",soundSetting);

		int musicSetting;
		if(musicToggle.isOn){
			musicSetting = 1;
		}else{
			musicSetting = 0;
			GameObject.Find("SoundController").GetComponent<SoundController>().stopMusic();
		}
		PlayerPrefs.SetInt("MUSIC",musicSetting);
	}

	private void setup(){
		maxPage.text = (PlayerPrefs.GetInt("MAXPAGE") + 1).ToString();
		maxBook.text = (PlayerPrefs.GetInt("MAXBOOK") + 1).ToString();
		maxShelf.text = (PlayerPrefs.GetInt("MAXSHELF") + 1).ToString();
		maxWall.text = (PlayerPrefs.GetInt("MAXWALL") + 1).ToString();

		if(PlayerPrefs.GetInt("MUSIC") == 1){
			musicToggle.isOn = true;
		}else{
			musicToggle.isOn = false;
		}

		if(PlayerPrefs.GetInt("SOUND") == 1){
			soundToggle.isOn = true;
		}else{
			soundToggle.isOn = false;
		}

		PlayerPrefs.SetInt("CONNECTED",1);
	}

	private void setupFirstTime(){
		maxPage.text = (RECPAGE + 1).ToString();
		maxBook.text = (RECBOOK + 1).ToString();
		maxShelf.text = (RECSHELF + 1).ToString();
		maxWall.text = (RECWALL + 1).ToString();

		PlayerPrefs.SetInt("MAXPAGE",RECPAGE);
		PlayerPrefs.SetInt("MAXBOOK",RECBOOK);
		PlayerPrefs.SetInt("MAXSHELF",RECSHELF);
		PlayerPrefs.SetInt("MAXWALL",RECWALL);

		PlayerPrefs.SetInt("SOUND",1);
		PlayerPrefs.SetInt("MUSIC",1);
		GameObject.Find("SoundController").GetComponent<SoundController>().startMusic();

		PlayerPrefs.SetInt("CONNECTED",1);

	}
}
