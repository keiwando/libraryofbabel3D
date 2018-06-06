using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsViewController : MonoBehaviour {

	[SerializeField]
	private Toggle soundToggle;
	[SerializeField]
	private Toggle musicToggle;
	[SerializeField]
	private Toggle invertCameraToggle;

	private SoundController soundController;

	void Start () {

		soundController = SoundController.Find();

		if (Settings.FirstTime) {
			Settings.SetupFirstTime();
		}

		soundToggle.onValueChanged.AddListener(delegate(bool arg0) {
			Settings.SoundEnabled = arg0;	
		});
		musicToggle.onValueChanged.AddListener(delegate(bool arg0) {

			Settings.MusicEnabled = arg0;
			PlayStopMusic(arg0);
		});
		invertCameraToggle.onValueChanged.AddListener(delegate(bool arg0) {
			Settings.ShouldInvertCamera = arg0;	
		});

		SetupToggleStates();
	}

	public void Show() {
		this.gameObject.SetActive(true);
	}

	public void Hide() {
		this.gameObject.SetActive(false);
	}

	private void PlayStopMusic(bool b){
		if (b) {
			soundController.PlayMusic();
		}else{
			soundController.StopMusic();
		}
	}

	private void SetupToggleStates(){
		
		musicToggle.isOn = Settings.MusicEnabled;
		soundToggle.isOn = Settings.SoundEnabled;
		invertCameraToggle.isOn = Settings.ShouldInvertCamera;
	}
}
