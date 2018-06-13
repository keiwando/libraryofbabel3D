using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PageTextView : MonoBehaviour {

	public string Text {
		set { 
			smallText.text = SplitTextIntoLines(value, Universe.CHARACTERS_PER_LINE);
			scrollText.text = SplitTextIntoLines(value, 25);

			if (scaleButton != null) {
				if (value == "")
					scaleButton.gameObject.SetActive(false);
				else 
					scaleButton.gameObject.SetActive(true);
			}
			text = value;
		}
		get { 
			return text;
		}
	}
	private string text;

	private readonly Regex newlineRegex = new Regex("\n");

	[SerializeField]
	private Text smallText;
	[SerializeField]
	private ScrollRect scrollContainer;
	[SerializeField]
	private Text scrollText;
	[SerializeField]
	private Sprite zoomInTexture;
	[SerializeField]
	private Sprite zoomOutTexture;

	private Button scaleButton;

	private float scrollOffset;
	private float scrollThreshold = 0.1f;

	void Start() {
		smallText.gameObject.SetActive(true);
		scrollContainer.gameObject.SetActive(false);

		scaleButton = GetComponentInChildren<Button>();
		scaleButton.onClick.AddListener(delegate {
			SwitchDisplayType();
		});
	}

	private void SwitchDisplayType() {
		smallText.gameObject.SetActive(!smallText.gameObject.activeSelf);
		scrollContainer.gameObject.SetActive(!scrollContainer.gameObject.activeSelf);

		scaleButton.image.sprite = smallText.gameObject.activeSelf ? zoomInTexture : zoomOutTexture;
	}

	private string SplitTextIntoLines(string text, int lineLength) {

		text = newlineRegex.Replace(text, "");
		var stringBuilder = new StringBuilder();
		bool inTag = false;

		int counter = 0;

		for (int i = 0; i < text.Length; i++) {

			if (text[i] == '<') {
				inTag = true;
			} else if (text[i] == '>') {
				inTag = false;
			}

			stringBuilder.Append(text[i]);

			counter += inTag ? 0 : 1;

			if (counter >= lineLength) {
				stringBuilder.Append("\n");
				counter = 0;
			}
		}

		return stringBuilder.ToString();
	}
}
