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
    public bool DirectionChanged;
}

public class InputManager : MonoSingleton<InputManager>
{
    public UsedInput GameInput => _input;
    UsedInput _input;

    public delegate void InputEvent(ref UsedInput input);
    public event InputEvent OnInput;

    float _prevXInput = 0;
    float _prevZInput = 0;

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
            _input.Function1 = Input.GetButton("Fire1");
            _input.Function2 = Input.GetButton("Fire2");

            if(_prevXInput != _input.XInput || _prevZInput != _input.ZInput)
            {
                _input.DirectionChanged = true;
                _prevXInput = _input.XInput;
                _prevZInput = _input.ZInput;
            }
            else
            {
                _input.DirectionChanged = false;
            }

            OnInput.Invoke(ref _input);
        }
    }
}
