using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePannel : MonoBehaviour
{
    protected float _canvasWidth = 0;
    protected float _canvasHeight = 0;

    public void SetResolution(float width, float height)
    {
        _canvasWidth = width;
        _canvasHeight = height;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
