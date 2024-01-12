using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESubUIManagerID
{
    Login = 1,
    Field,
}

public class UIManager : Singleton<UIManager>
{
    public SubUIManager CurrentUISubManager;
    Dictionary<int, SubUIManager> _subManagerlist;

    public UIManager()
    {
        _subManagerlist = new Dictionary<int, SubUIManager>();
        _subManagerlist.Clear();

        // Sub UI�� ���
        _subManagerlist.Add((int)ESubUIManagerID.Login, new SubUIManagerLogin());
        _subManagerlist.Add((int)ESubUIManagerID.Field, new SubUIManageField());

        // ���۽� �α��� UI
        if(_subManagerlist.TryGetValue((int)ESubUIManagerID.Login, out CurrentUISubManager))
        {
            CurrentUISubManager.Active();
        }
        else
        {
            Debug.LogError("UI �ʱ�ȭ ����, �α��� UI�� ����");
        }
    }

    public void ChangeUISubManager(ESubUIManagerID subUIId)
    {
        if(CurrentUISubManager != null)
        {
            CurrentUISubManager.Deactive();
            CurrentUISubManager = null;
        }    

        if (!_subManagerlist.TryGetValue((int)subUIId, out CurrentUISubManager))
        {
            Debug.LogError("Sub UI �ʱ�ȭ ����, �ش� Sub UI�� ����");
        }

        CurrentUISubManager.Active();
    }
}
