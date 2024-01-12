using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubUIManageField : SubUIManager
{
    public FieldPannel _fieldPannel;
    public StatusPopup _statusPopup;

    public override void Active()
    {
        base.Active();

        string pannelName = "Prefabs/UI/fieldPannel";
        var pannelPrefab = Resources.Load(pannelName) as GameObject;
        var pannelObject = Object.Instantiate(pannelPrefab, _UIPannels.transform);

        _fieldPannel = pannelObject.GetComponent<FieldPannel>();
        _statusPopup = pannelObject.GetComponentInChildren<StatusPopup>();

        if (_fieldPannel == null)
        {
            Debug.LogError("필드 UI 로딩 에러");
        }

        _fieldPannel.SetResolution(_canvasWidth, _canvasHeight);
        _statusPopup.SetSize(_canvasWidth * 0.2f, _canvasHeight);

        float statusX = _canvasWidth * 0.4f;
        _statusPopup.SetPosition(-statusX, 0);
    }

    public override void Deactive()
    {
        base.Deactive();
    }
}
