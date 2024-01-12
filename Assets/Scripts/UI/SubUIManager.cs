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
        // Canvas�� �г��� ���� ���� ���� �׻� �ٲ� ���ִ�.
        // ��� ����UI �Ŵ������� Ȱ��ȭ�� Canvas�� ã�� �ű⿡ UIPannels�� �����.
        // ���� ������ Canvas�� UI �Ŵ����� ����� �д�

        if (_canvas == null)
        {
            _canvas = Object.FindFirstObjectByType<Canvas>().transform;
        }
        else
        {
            Debug.LogWarning("�� ���� UI�� Active ȣ�� �ߺ�");
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
