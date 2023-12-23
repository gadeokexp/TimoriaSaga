using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SubUIManager
{
    protected int _subUIManagerID = 0;
    public int SubUIManagerID => _subUIManagerID;

    protected GameObject _canvas = null;
    protected GameObject _subUIManagers = null;

    public virtual void Active()
    {
        if(_canvas == null)
        {
            _canvas = Object.FindFirstObjectByType<Canvas>().gameObject;
            _subUIManagers = new GameObject("UISubManaagers");

            var rectTransform = _subUIManagers.AddComponent<RectTransform>();
            rectTransform.SetParent(_canvas.transform);
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.sizeDelta = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }
    }
    public virtual void DeActive() { }
}
