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
    public event MoveEvent OnMoveInput;

    public delegate void Fuction1Event();
    public event Fuction1Event OnFunction1Input;

    public delegate void Fuction2Event();
    public event Fuction1Event OnFunction2Input;

    // Update is called once per frame
    void Update()
    {
        if (OnMoveInput != null)
        {
            _xInput = Input.GetAxisRaw("Horizontal");
            _zInput = Input.GetAxisRaw("Vertical");

            OnMoveInput.Invoke(_xInput, _zInput);
        }

        if (OnFunction1Input != null && Input.GetKeyDown("Fire1"))
        {
            OnFunction1Input.Invoke();
        }

        if (OnFunction2Input != null && Input.GetKeyDown("Fire2"))
        {
            OnFunction2Input.Invoke();
        }
    }
}
