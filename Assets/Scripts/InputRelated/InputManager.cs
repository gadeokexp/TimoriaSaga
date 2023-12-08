using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsedInput
{
    public float XInput;
    public float ZInput;
    public bool Jump;
    public bool Function1;
    public bool Function2;
}

public class InputManager : MonoSingleton<InputManager>
{
    UsedInput _input;
    public delegate void InputEvent(ref UsedInput input);
    public event InputEvent OnInput;

    private void Start()
    {
        _input = new UsedInput();
    }

    // Update is called once per frame
    void Update()
    {
        if (OnInput != null)
        {
            _input.XInput = Input.GetAxisRaw("Horizontal");
            _input.ZInput = Input.GetAxisRaw("Vertical");
            _input.Jump = Input.GetButtonDown("Jump");
            _input.Function1 = Input.GetButtonDown("Fire1");
            _input.Function2 = Input.GetButtonDown("Fire2");

            OnInput.Invoke(ref _input);
        }
    }
}
