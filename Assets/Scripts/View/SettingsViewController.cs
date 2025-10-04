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
	[SerializeField]
	private Toggle postProcessingEnabledToggle;
	[SerializeField]
	private Button quitGameButton;

	private SoundController soundController;
	private ViewController viewController;

	void Start () {

		viewController = ViewController.Find();
		soundController = SoundController.Find();

		if (Settings.FirstTime) {
			Settings.SetupFirstTime();
		}

		SetupToggleStates();

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

		postProcessingEnabledToggle.onValueChanged.AddListener(delegate(bool arg0) {
			Settings.PostProcessingEnabled = arg0;
			viewController.PostProcessingSettingUpdated();
		});

		quitGameButton.onClick.AddListener(delegate() {
			Application.Quit();
		}); 
		#if !UNITY_STANDALONE
		quitGameButton.gameObject.SetActive(false);
		#endif
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
		postProcessingEnabledToggle.isOn = Settings.PostProcessingEnabled;
	}
}
