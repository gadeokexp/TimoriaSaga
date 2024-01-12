using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubUIManageField : SubUIManager
{
    public FieldPannel _fieldPannel;
    public StatusPopup _statusPopup;
    public InventoryPopup _inventoryPopup;

    public override void Active()
    {
        base.Active();

        string pannelName = "Prefabs/UI/fieldPannel";
        var pannelPrefab = Resources.Load(pannelName) as GameObject;
        var pannelObject = Object.Instantiate(pannelPrefab, _UIPannels.transform);

        _fieldPannel = pannelObject.GetComponent<FieldPannel>();
        _statusPopup = pannelObject.GetComponentInChildren<StatusPopup>();
        _inventoryPopup = pannelObject.GetComponentInChildren<InventoryPopup>();

        if (_fieldPannel == null)
        {
            Debug.LogError("필드 UI 로딩 에러");
        }

        _fieldPannel.SetResolution(_canvasWidth, _canvasHeight);

        _statusPopup.SetSize(_canvasWidth * 0.2f, _canvasHeight);
        float statusX = _canvasWidth * 0.4f;
        _statusPopup.SetPosition(-statusX, 0);
        _statusPopup.InventoryButton.onClick.AddListener(ShowInventoryPopup);

        _inventoryPopup.SetSize(_canvasWidth * 0.2f, _canvasHeight * 0.6f);
        float inventoryX = _canvasWidth * 0.4f;
        float inventoryY = _canvasHeight * 0.2f;
        _inventoryPopup.SetPosition(inventoryX, inventoryY);
        _inventoryPopup.gameObject.SetActive(false);
    }

    public override void Deactive()
    {
        base.Deactive();
    }

    public void ShowInventoryPopup()
    {
        if(_inventoryPopup.gameObject.activeSelf)
        {
            _inventoryPopup.gameObject.SetActive(false);
        }
        else
        {
            _inventoryPopup.gameObject.SetActive(true);
        }
    }
}
