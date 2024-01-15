using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubUIManagerLogin : SubUIManager
{
    LoginPannel _loginPannel;
    LoginPopup _loginPopup;

    public SubUIManagerLogin()
    {
        _subUIManagerID = (int)ESubUIManagerID.Login;
    }

    public override void Active()
    {
        base.Active();

        string pannelName = "Prefabs/UI/loginPannel";
        var pannelPrefab = Resources.Load(pannelName) as GameObject;
        var pannelObject = Object.Instantiate(pannelPrefab, _UIPannels.transform);

        _loginPannel = pannelObject.GetComponent<LoginPannel>();
        _loginPopup = pannelObject.GetComponentInChildren<LoginPopup>();

        if (_loginPannel == null || _loginPopup == null)
        {
            Debug.LogError("�α��� UI �ε� ����");
        }

        _loginPopup.LoginButton.onClick.AddListener(Login);
    }

    public override void Deactive()
    {
        base.Deactive();
    }

    // Login UI ���� ���
    public void Login()
    {
        DataManager.Instance.SetAccountInfo(_loginPopup.GetAccount(), _loginPopup.GetPassword());

        _loginPopup.gameObject.SetActive(false);

        GameInstance.Instance.OperateGameNetwork();
    }
}
