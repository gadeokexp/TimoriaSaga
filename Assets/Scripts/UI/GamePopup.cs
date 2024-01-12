using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePopup : MonoBehaviour
{
    public void SetSize(float width, float height)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetPosition(float x, float y)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x, y);
    }
}
