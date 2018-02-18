using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RampUIVisibility : MonoBehaviour {

	[SerializeField] private float maxAlpha = 1.0f;
	private float animDuration = 2f;

	private Image image;
	private float startOpacity;

	void Start () {

		image = GetComponent<Image>();
		if (image == null) return;

		startOpacity = image.color.a;

		StartCoroutine(FullFade());

	}

	private IEnumerator FullFade() {
		yield return ChangeImageOpacity(startOpacity, maxAlpha, animDuration);
		yield return Wait(animDuration);
		yield return ChangeImageOpacity(maxAlpha, startOpacity, animDuration);
	}
		
	private IEnumerator Wait(float seconds) {
		yield return new WaitForSeconds(seconds);
	}

	private IEnumerator ChangeImageOpacity(float start, float end, float time) {

		var color = image.color;

		while (Mathf.Abs(end - color.a) > 0.01) {

			color.a = color.a + (end - start) * Time.deltaTime / time;
			image.color = color;

			yield return new WaitForEndOfFrame();
		}

		color.a = end;
		image.color = color;
	}
}
