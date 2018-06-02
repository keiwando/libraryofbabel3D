using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour {

	public Canvas canvas;

	public Toggle soundToggle;
	public Toggle musicToggle;
	public Toggle invertCameraToggle;

	private const string FIRSTTIME_KEY = "FIRSTTIME";
	private const string SOUND_KEY = "SOUND";
	private const string MUSIC_KEY = "MUSIC";
	public static readonly string INVERTCAM_KEY = "INVERTCAMERA";

	// Use this for initialization
	void Start () {

		//print("\n FIRSTTIME: " + PlayerPrefs.GetInt(FIRSTTIME_KEY));

		canvas.enabled = false;
		if (PlayerPrefs.GetInt(FIRSTTIME_KEY, 0) == 0) {
			PlayerPrefs.SetInt(FIRSTTIME_KEY, 1);
			PlayerPrefs.Save();
			setupFirstTime();
		} else {
			setup();
		}

		musicToggle.onValueChanged.AddListener(playStopMusic);
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

		int soundSetting = soundToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt(SOUND_KEY,soundSetting);

		int musicSetting = musicToggle.isOn ? 1 : 0;
		if (!musicToggle.isOn) {
			GameObject.Find("SoundController").GetComponent<SoundController>().stopMusic();
		}
		PlayerPrefs.SetInt(MUSIC_KEY,musicSetting);

		int invCamera = invertCameraToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt(INVERTCAM_KEY, invCamera);

		PlayerPrefs.Save();
	}

	private void setup(){
		
		musicToggle.isOn = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;
		soundToggle.isOn = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
		invertCameraToggle.isOn = PlayerPrefs.GetInt(INVERTCAM_KEY, 0) == 1;
	}

	private void setupFirstTime(){
		
		PlayerPrefs.SetInt(SOUND_KEY, 1);
		PlayerPrefs.SetInt(MUSIC_KEY, 1);
		PlayerPrefs.SetInt(INVERTCAM_KEY, 0);
		GameObject.Find("SoundController").GetComponent<SoundController>().startMusic();

		PlayerPrefs.Save();

		setup();
	}
}
