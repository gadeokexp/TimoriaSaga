using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPopup : GamePopup
{
    [SerializeField] public Button InventoryButton;
    [SerializeField] Text _unitName;

    private void Start()
    {
        _unitName.text = DataManager.Instance.UnitName;
    }
}
