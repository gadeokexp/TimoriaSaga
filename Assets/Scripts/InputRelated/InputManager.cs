using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    float _xInput;
    float _zInput;

    public float XInput => _xInput;
    public float ZInput => _zInput;

    public delegate void MoveEvent(float x, float z);
    public event MoveEvent OnMove;

    // Update is called once per frame
    void Update()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _zInput = Input.GetAxisRaw("Vertical");

        if (OnMove != null && (_xInput != 0f || _zInput != 0f))
        {
            OnMove.Invoke(_xInput, _zInput);
        }   
    }
}
