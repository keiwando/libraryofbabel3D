using UnityEngine;

public class OnlyMobileAndWebGL : MonoBehaviour {

  void Awake() {
    #if !UNITY_IOS && !UNITY_ANDROID && !UNITY_WEBGL
    gameObject.SetActive(false);
    #endif
  }
}