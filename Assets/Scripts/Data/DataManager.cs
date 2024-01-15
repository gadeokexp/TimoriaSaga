using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    UserData _userData;

    public string UnitName
    {
        get => _userData.UnitName;
        set
        {
            _userData.UnitName = value;
        }
    }

    public string AccountName { get => _userData.AccountName; set { _userData.AccountName = value; } }
    public string Password { get => _userData.Password; set { _userData.Password = value; } }

    public DataManager()
    {
        _userData = new UserData();
    }

    public void SetAccountInfo(string accountName, string password)
    {
        _userData.AccountName = accountName;
        _userData.Password = HashString(password);
    }

    public static string HashString(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
