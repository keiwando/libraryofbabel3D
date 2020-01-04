using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keiwando.Unity.Experiments {

  public class MouseHoverExperiment : MonoBehaviour {

    void OnHover() {
      Debug.Log("OnHover " + name);
    }

    void OnHoverExit() {
      Debug.Log("OnHoverExit " + name);
    }

    void OnMouseOver() {
      Debug.Log("OnMouseOver " + name);
    }

    void OnMouseExit() {
      Debug.Log("OnMouseExit " + name);
    }
  }
}
