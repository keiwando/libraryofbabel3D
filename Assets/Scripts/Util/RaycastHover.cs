using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class RaycastHover : MonoBehaviour {

  [SerializeField]
  private bool onlyClosestHit = true;

  // Arrays for reuse
  private RaycastHit[] singleHitArray = new RaycastHit[1];
  private static readonly RaycastHit[] emptyHits = new RaycastHit[0];

	private HashSet<Collider> hoverColliders = new HashSet<Collider>();

	void Update () {

    if (Cursor.lockState != CursorLockMode.Locked) {
      ExitAll();
      return;
    }

    // var screenPoint = Input.mousePosition;
    // var screenPoint = new Vector3(Screen.width / 2, Screen.height / 2);
		// Ray ray = Camera.main.ScreenPointToRay(screenPoint);
    var viewportPoint = new Vector3(0.5f, 0.5f, 0f);
    Ray ray = Camera.main.ViewportPointToRay(viewportPoint);

    var previousHoverColliders = new HashSet<Collider>(hoverColliders);

    RaycastHit[] hits;
    if (onlyClosestHit) {
      RaycastHit hit;
      var didHit = Physics.Raycast(ray, out hit);
      if (didHit) {
        hits = singleHitArray;
        hits[0] = hit;
      } else {
        hits = emptyHits;
      }
    } else {
      hits = Physics.RaycastAll(ray);
    }

		if (hits.Length > 0) {
      
      var hitSet = new HashSet<Collider>();
      foreach (var hit in hits) {
        SendOnHover(hit.collider);

				hoverColliders.Add(hit.collider);

        hitSet.Add(hit.collider);
      }

			foreach (var collider in hoverColliders.Except(hitSet)) {
				SendOnHoverExit(collider);
			}

			hoverColliders = hitSet;
		
		} else if (hoverColliders.Count > 0) {
			// Nothing hovered over -> exit on all
			ExitAll();
		}

		#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		if (hoverColliders.Count > 0 && Input.touchCount == 0) {
			ExitAll();	
		}
		#endif


    if (Input.GetMouseButtonDown(0)) {
      foreach (var hit in hits) {
        if (previousHoverColliders.Contains(hit.collider)) {
          SendOnHoverMouseDown(hit.collider); 
        }
      } 
    } else if (Input.GetMouseButtonUp(0)) {
      foreach (var hit in hits) {
        if (previousHoverColliders.Contains(hit.collider)) {
          SendOnHoverMouseUp(hit.collider);
        }
      }
    }
  }

	private void ExitAll() {
		foreach (var collider in hoverColliders) {
			SendOnHoverExit(collider);
		}

		hoverColliders.Clear();
	} 

	private void SendOnHover(Collider collider) {

		if (collider == null || collider.gameObject == null) return;

		collider.SendMessage("OnHover", SendMessageOptions.DontRequireReceiver);
	}

	private void SendOnHoverExit(Collider collider) {

		if (collider == null || collider.gameObject == null) return;

		collider.SendMessage("OnHoverExit", SendMessageOptions.DontRequireReceiver);
	}

  private void SendOnHoverMouseDown(Collider collider) {

    if (collider == null || collider.gameObject == null) return;

    collider.SendMessage("OnHoverMouseDown", SendMessageOptions.DontRequireReceiver);
  }

  private void SendOnHoverMouseUp(Collider collider) {

    if (collider == null || collider.gameObject == null) return;

    collider.SendMessage("OnHoverMouseUp", SendMessageOptions.DontRequireReceiver);
  }
}
