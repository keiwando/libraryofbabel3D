using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Keiwando.Lob {

  [RequireComponent(typeof(RectTransform))]
  public class RelativeLocationFlowLayout : MonoBehaviour {

    [SerializeField]
    private bool flex = true;

    private RectTransform rectTransform;

    void Start() {
      rectTransform = GetComponent<RectTransform>();
    }

    public void Refresh() {

     Canvas.ForceUpdateCanvases();

      var totalChildWidth = 0f;
      var childWidths = new List<float>();
      for (int i = 0; i < transform.childCount; i++) {
        var child = transform.GetChild(i);
        var nameLabel = child.transform.GetChild(0);
        var valueLabel = child.transform.GetChild(1);
        var nameTransform = nameLabel.GetComponent<RectTransform>();
        var valueTransform =valueLabel.GetComponent<RectTransform>();
        var childWidth = nameTransform.rect.width + valueTransform.rect.width;
        childWidths.Add(childWidth);
        totalChildWidth += childWidth;
      }

      var gap = 0f;
      if (flex) {
        gap = (rectTransform.rect.width - totalChildWidth) / (transform.childCount - 1);
      }
      var x = 0f;
      for (int i = 0; i < transform.childCount; i++) {
        var child = transform.GetChild(i);
        var childTransform = child.GetComponent<RectTransform>();
        var position = childTransform.anchoredPosition;
        position.x = x;
        childTransform.anchoredPosition = position;
        var childWidth = childWidths[i];
        x += (childWidth + gap);
      }
    }
  }
}