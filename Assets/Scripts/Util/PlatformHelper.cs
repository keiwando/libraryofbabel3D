using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlatformHelper {

	public static bool IsPlatformMobile() {
		return IsPlatformAndroid() || IsPlatformIOS();
	}

	public static bool IsPlatformEditor() {
		return 
			Application.platform == RuntimePlatform.OSXEditor || 
			Application.platform == RuntimePlatform.WindowsEditor || 
			Application.platform == RuntimePlatform.LinuxEditor;
	}

	public static bool IsPlatformAndroid() {
		return Application.platform == RuntimePlatform.Android;
	}

	public static bool IsPlatformIOS() {
		return Application.platform == RuntimePlatform.IPhonePlayer;
	}

	public static bool IsPlaformWebGL() {
		return Application.platform == RuntimePlatform.WebGLPlayer;
	}

	public static bool IsPlatformMac() {
		return Application.platform == RuntimePlatform.OSXPlayer;
	}

	public static bool IsPlatformPC() {
		return Application.platform == RuntimePlatform.WindowsPlayer;
	}

	public static bool IsPlatformLinux() {
		return Application.platform == RuntimePlatform.LinuxPlayer;
	}

	/// <summary>
	/// True if the runtime platform is Windows or Mac or Linux.
	/// </summary>
	public static bool IsPlatformDesktop() {
		return IsPlatformPC() || IsPlatformMac() || IsPlatformLinux();  
	}
}



