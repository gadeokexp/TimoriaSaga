using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FieldTileManager.Instance.Init();
        UIManager.Instance.ChangeUISubManager(ESubUIManagerID.Field);
    }
}
