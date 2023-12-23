using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FieldManager.Instance.Init();
        NetworkManager.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
