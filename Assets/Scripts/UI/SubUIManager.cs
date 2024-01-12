using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SubUIManager
{
    protected int _subUIManagerID = 0;
    public int SubUIManagerID => _subUIManagerID;

    protected Transform _canvas = null;
    protected GameObject _UIPannels = null;

    protected float _canvasWidth = 0;
    protected float _canvasHeight = 0;

    public virtual void Active()
    {
        // Canvas는 패널이 들어가는 씬에 따라서 항상 바뀔 수있다.
        // 모든 서브UI 매니저들은 활성화시 Canvas를 찾고 거기에 UIPannels를 만든다.
        // 이후 인지된 Canvas는 UI 매니저에 등록해 둔다

        if (_canvas == null)
        {
            _canvas = Object.FindFirstObjectByType<Canvas>().transform;
        }
        else
        {
            Debug.LogWarning("한 서브 UI의 Active 호출 중복");
        }

        RectTransform rectTransform = _canvas.gameObject.GetComponent<RectTransform>();
        _canvasWidth = rectTransform.rect.width;
        _canvasHeight = rectTransform.rect.height;

        Transform _subUIManagerTransform = _canvas.Find("UIPannels");

        if (_subUIManagerTransform == null)
        {
            _UIPannels = new GameObject("UIPannels");

            var UITransform = _UIPannels.AddComponent<RectTransform>();
            UITransform.SetParent(_canvas);
            UITransform.anchoredPosition = Vector3.zero;
            UITransform.sizeDelta = Vector3.zero;
            UITransform.localScale = Vector3.one;
        }
        else
        {
            _UIPannels = _subUIManagerTransform.gameObject;
        }
    }
    public virtual void Deactive()
    {
        _canvas = null;
        _canvasWidth = 0;
        _canvasHeight = 0;
    }
}
