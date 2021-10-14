using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputType
{
    mouse,
    touch,
    eyeTracker,
    leapMotion,
}

public class InputEventArgs : EventArgs
{
    public InputType type;
    public ushort indicatorId;
    public Vector2 coords;
    public float amount;
}

public abstract class CustomInput : IDisposable
{
    protected abstract ushort inputCount();
    protected abstract bool checkInputDown(ushort indicatorId = 0);
    protected abstract Vector2 getInputPosition(ushort indicatorId = 0);

    public ushort InputCount { get { return inputCount(); } }

    InputType type;
    InputEventArgs eventArgs;
    public Action<InputEventArgs> OnClick, OnDragStart, OnDrag, OnDragEnd, OnRotate, OnZoom, OnPan;
    bool disposed = false;
    MonoBehaviour inputHandler;

    public CustomInput(MonoBehaviour inputHandler, InputType type)
    {
        this.inputHandler = inputHandler;
        this.type = type;
        disposed = false;
        eventArgs = new InputEventArgs();

        inputHandler.StartCoroutine(Run());
    }

    ushort _inputCount = 0;
    bool _isDown = false;
    bool _atLeastTwoIndicatorDown = false;
    bool _dragStarted = false;
    float dragTrashold = 0.5f;
    Vector2 prevPos = Vector2.zero, currentPos = Vector2.zero;
    Vector2 prevPos2 = Vector2.zero, currentPos2 = Vector2.zero;
    Vector2 centerPos = Vector2.zero, prevCenterPos = Vector2.zero;
    IEnumerator Run()
    {
        yield return new WaitWhile(() => disposed);
        yield return new WaitForEndOfFrame();

        if (checkInputDown())
        {
            currentPos = getInputPosition();
            eventArgs.coords = currentPos;
            _inputCount = inputCount();

            if (_isDown)
            {
                //Events continue
                //Check Drag
                _atLeastTwoIndicatorDown = _inputCount > 1;
                if (Vector2.Distance(currentPos, prevPos) > dragTrashold)
                {
                    if (!_dragStarted)
                    {
                        if (OnDragStart != null)
                            OnDragStart.Invoke(eventArgs);
                        _dragStarted = true;
                    }
                    else
                    {
                        if (OnDrag != null)
                            OnDrag.Invoke(eventArgs);

                        if (_atLeastTwoIndicatorDown)
                        {
                            Vector2 cumulativePos = Vector2.zero;
                            for (ushort i = 0; i < _inputCount; i++)
                            {
                                cumulativePos += getInputPosition(i);
                            }

                            currentPos2 = getInputPosition((ushort)(_inputCount - 1));
                            centerPos = cumulativePos / _inputCount;
                            eventArgs.coords = centerPos;

                            eventArgs.amount = Vector2.SignedAngle((prevPos - eventArgs.coords), (currentPos - eventArgs.coords));
                            if (OnRotate != null)
                                OnRotate.Invoke(eventArgs);

                            if (prevCenterPos != Vector2.zero)
                            {
                                eventArgs.coords = centerPos - prevCenterPos;
                                eventArgs.amount = Vector2.Distance(prevCenterPos, centerPos);
                                if (OnPan != null)
                                    OnPan.Invoke(eventArgs);
                            }

                            if (prevPos2 != Vector2.zero)
                            {
                                eventArgs.amount = Vector2.Distance(currentPos, currentPos2) - Vector2.Distance(prevPos, prevPos2);
                                if (OnZoom != null)
                                    OnZoom.Invoke(eventArgs);
                            }
                            prevPos2 = currentPos2;
                            prevCenterPos = centerPos;
                        }
                    }
                }

            }
            else
            {
                //Events begin
                _isDown = true;

            }

            prevPos = currentPos;
        }
        else
        {
            //Events end
            if (_dragStarted)
            {
                if (OnDragEnd != null)
                    OnDragEnd.Invoke(eventArgs);
                _dragStarted = false;
                _isDown = false;
            }
            else if (_isDown)
            {
                if (OnClick != null)
                    OnClick.Invoke(eventArgs);

                _isDown = false;
            }

            prevPos = Vector2.zero;
            prevPos2 = Vector2.zero;
            prevCenterPos = Vector2.zero;
        }


        inputHandler.StartCoroutine(Run());
    }

    public void Dispose()
    {
        disposed = true;

        OnClick = null;
        OnDragStart = null;
        OnDrag = null;
        OnDragEnd = null;
    }
}


public class MouseInput : CustomInput
{
    public enum MouseKey
    {
        left = 0,
        right = 1,
    }
    public MouseKey key;
    public MouseInput(MonoBehaviour inputHandler, MouseKey key) : base(inputHandler, InputType.mouse)
    {
        this.key = key;
    }
    protected override ushort inputCount()
    {
        //TODO:mocking need to be implemented
        return 1;
    }
    protected override bool checkInputDown(ushort indicatorId = 0)
    {
        //TODO:mocking need to be implemented
        return Input.GetMouseButton((ushort)key);
    }

    protected override Vector2 getInputPosition(ushort indicatorId = 0)
    {
        //TODO:mocking need to be implemented
        return Input.mousePosition;
    }

}

public class TouchInput : CustomInput
{
    public TouchInput(MonoBehaviour inputHandler) : base(inputHandler, InputType.touch)
    {
    }

    protected override ushort inputCount()
    {
        return (ushort)Input.touchCount;
    }
    protected override bool checkInputDown(ushort indicatorId = 0)
    {
        return (InputCount > indicatorId);
    }

    protected override Vector2 getInputPosition(ushort indicatorId = 0)
    {
        if (checkInputDown(indicatorId))
        {
            return Input.GetTouch(indicatorId).position;
        }
        return Vector2.zero;
    }
}
