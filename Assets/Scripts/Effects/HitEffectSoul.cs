using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectSoul : MonoBehaviour
{
    float _existTime = 0;

    void OnEnable()
    {
        _existTime = 0f;    
    }

    // Update is called once per frame
    void Update()
    {
        _existTime += Time.deltaTime;

        if ( _existTime > 0.5f) gameObject.SetActive(false);
    }
}
