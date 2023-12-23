using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPopup : GamePopup
{
    [SerializeField] InputField Account;
    [SerializeField] InputField Password;
    [SerializeField] public Button LoginButton;

    public string GetAccount()
    {
        return Account.text;
    }

    public string GetPassword()
    {
        return Password.text;
    }
}
