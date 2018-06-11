using System;
using UnityEngine;

public static class Settings {

	private const string FIRSTTIME_KEY = "FIRSTTIME";
	private const string SOUND_KEY = "SOUND";
	private const string MUSIC_KEY = "MUSIC";
	private const string ONLINE_KEY = "ONLINE";
	private const string INVERTCAM_KEY = "INVERTCAMERA";
	private const string POST_PROCESSING_ENABLED_KEY = "POST_PROCESSING_ENABLED";

	public static bool Offline { 
		get { return GetBool(ONLINE_KEY); }
		set { SetBool(ONLINE_KEY, value); }
	}

	public static bool FirstTime {
		get { return GetBool(FIRSTTIME_KEY); }
		set { SetBool(FIRSTTIME_KEY, value); }
	}

	public static bool SoundEnabled {
		get { return GetBool(SOUND_KEY); }
		set { SetBool(SOUND_KEY, value); }
	}

	public static bool MusicEnabled {
		get { return GetBool(MUSIC_KEY); }
		set { SetBool(MUSIC_KEY, value); }
	}

	public static bool ShouldInvertCamera {
		get { return GetBool(INVERTCAM_KEY); }
		set { SetBool(INVERTCAM_KEY, value); }
	}

	public static bool PostProcessingEnabled {
		get { return GetBool(POST_PROCESSING_ENABLED_KEY); }
		set { SetBool(POST_PROCESSING_ENABLED_KEY, value); }
	}

	public static void SetupFirstTime() {

		if (!FirstTime)
			return;

		SoundEnabled = true;
		MusicEnabled = true;
		ShouldInvertCamera = false;
		PostProcessingEnabled = false;

		FirstTime = false;
	}

	private static bool GetBool(string key) {

		return PlayerPrefs.GetInt(key, 0) == 1;
	}

	private static void SetBool(string key, bool b) {
		PlayerPrefs.SetInt(key, b ? 1 : 0);
		PlayerPrefs.Save();
	}
}


