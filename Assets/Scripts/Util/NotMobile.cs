using UnityEngine;

public class NotMobile : MonoBehaviour {

  void Awake() {
    #if UNITY_IOS || UNITY_ANDROID
    gameObject.SetActive(false);
    #endif
  }
}