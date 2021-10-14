using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.InputManagement;
using Leap;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LeapButton : BaseLeapObject
{
    //public enum interactionDirectionType { none, forward, backward, up, down, right, left }

    #region Properties
    public float pressInvokationTreshold = 0.2f;

    public GameObject movingObject;
    public Vector3 moveDirection;
    public float moveAmount;
    protected Vector3 initialLocation;
    Vector3 targetLocation;

    public GameObject handleObject;

    public Vector3 expectedButtonMoveDirection;
    public float maxAngleDif = 5f;
    [HideInInspector]
    public Vector3 interactionMoveDirection;
    public float movementRefreshAmount = 0.1f;
    float refreshTime = 0f;

    protected Action onCkeckState;
    #endregion
    
    #region Accesors
    protected bool _isPressed = false;
    /// <summary> Gets whether the button is currently held down. </summary>
    public bool isPressed { get { return _isPressed; } }

    private float _pressedAmount = 0F;
    /// <summary>
    /// Gets a normalized value between 0 and 1 based on how depressed the button
    /// currently is relative to its maximum depression. 0 represents a button fully at
    /// rest or pulled out beyond its resting position; 1 represents a fully-pressed
    /// button.
    /// </summary>
    public float pressedAmount { get { return _pressedAmount; } }

    [Header("UI Control")]

    [Tooltip("When set to false, this UI control will not be functional. Use this instead "
             + "of disabling the @object itself when you want to disable the user's "
             + "ability to affect this UI control while keeping the GameObject active and, "
             + "for example, rendering, and able to receive primaryHover state.")]
    [SerializeField, FormerlySerializedAs("controlEnabled")]
    private bool _controlEnabled = true;

    public bool controlEnabled
    {
        get { return _controlEnabled; }
        set { _controlEnabled = value; }
    }


    #endregion

    #region RootEvents
    [SerializeField]
    [FormerlySerializedAs("OnContactBegin")]
    public UnityEvent _onPress = new UnityEvent();

    [SerializeField]
    [FormerlySerializedAs("OnContactEnd")]
    public UnityEvent _onUnpress = new UnityEvent();

    Action OnPress = () => { };
    Action OnUnpress = () => { };
    #endregion

    private LineRenderer line1, line2;

    protected override void Start()
    {
        base.Start();
        if (handleObject == null)
        {
            if (GetComponentInChildren<Collider>() != null)
                handleObject = GetComponentInChildren<Collider>().gameObject;
        }

        //moveDirection = transform.TransformDirection(moveDirection);
        //expectedButtonMoveDirection = transform.TransformDirection(expectedButtonMoveDirection);

        if (movingObject == null)
            movingObject = GetComponentInChildren<MeshRenderer>().gameObject;


        if (handleObject != null)
        {
            initialLocation = movingObject.transform.localPosition;
            targetLocation = (moveDirection * moveAmount) + initialLocation;
        }

        //line1 = new GameObject().AddComponent<LineRenderer>();
        //line2 = new GameObject().AddComponent<LineRenderer>();

        //line1.widthCurve = AnimationCurve.Constant(0, 1, .01f);
        //line2.widthCurve = AnimationCurve.Constant(0, 1, .01f);


        //line1.material = new Material(Shader.Find("Unlit/Color"));
        //line2.material = new Material(Shader.Find("Unlit/Color"));

        //line1.material.color = Color.red;
        //line2.material.color = Color.green;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //For Getting Leap's original events
        OnContactBegin += ContactBegin;
        OnContactEnd += ContactEnd;
        OnContactStay += ContactStay;

        //For Generating discreate movements
        OnPress += PressActions;
        OnUnpress += UnPressActions;

        //For invoking after expected invokation
        onCkeckState += CheckButtonInteraction;
    }

    protected override void OnDisable()
    {

        //For Getting Leap's original events
        OnContactBegin = null;
        OnContactEnd = null;

        //For Generating discreate movements
        OnPress = null;
        OnUnpress = null;

        //For invoking after expected invokation
        onCkeckState = null;

        base.OnDisable();
    }



    private void UnPressActions()
    {
        //_isPressed = false;
        //if (InInteraction)
        //{
        //    if (moveDirection != Vector3.zero)
        //        movingObject.transform.localPosition = initialLocation;
        //    _onUnpress.Invoke();
        //    LeapInputs.Instance.OnContactEnd(this);
        //}
    }

    private void PressActions()
    {
        //    _isPressed = true;
        //    if (InInteraction)
        //    {
        //        if (moveDirection != Vector3.zero)
        //            movingObject.transform.localPosition = targetLocation;
        //        _onPress.Invoke();
        //        LeapInputs.Instance.OnContactBegin(this);
        //    }
    }

    private void ContactBegin()
    {
        inContactStay = true;
        if (ClosestHand != null)
        {
            initialHandPos = GetClosestFinger(ClosestHand).TipPosition;
            lastHandPos = initialHandPos;
        }


    }

    public bool inContactStay = false;
    private void ContactStay()
    {
        inContactStay = true;
        if (ClosestHand != null)
            lastHandPos = GetClosestFinger(ClosestHand).TipPosition;
    }

    private void ContactEnd()
    {
        if (_isPressed)
        {
            OnUnpress.Invoke();
        }
        inContactStay = false;
    }


    bool readyToInvoke = true;
    IEnumerator InvokePressEvent(float time,Action action)
    {
        if (readyToInvoke)
        {
            readyToInvoke = false;
            action.Invoke();
        }
        yield return new WaitForSeconds(time);
        readyToInvoke = true;
    }

    Finger GetClosestFinger(Hand h)
    {
        float dist = float.MaxValue;
        Finger closestFinger = null;
        foreach (var f in h.Fingers)
        {
            Vector3 tipPos=new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
            float newDistance = Vector3.Distance(transform.position, tipPos);
            if (newDistance < dist)
            {
                closestFinger = f;
                dist = newDistance;
            }
        }
        return closestFinger;
    }
    
    Leap.Vector initialHandPos, lastHandPos;
    Leap.Vector leapHandMoveDirection;
    protected Leap.Vector moveDelta;
    Vector3 GetInteractionDirectionType()
    {

        moveDelta = (lastHandPos - initialHandPos);
        leapHandMoveDirection = moveDelta.Normalized;

        return new Vector3(leapHandMoveDirection.x, leapHandMoveDirection.y, leapHandMoveDirection.z);
    }

     

    void FixedUpdate()
    {

        if (inContactStay)
        {

            if (refreshTime > movementRefreshAmount)
            {
                interactionMoveDirection = GetInteractionDirectionType();
                refreshTime = 0;
                onCkeckState.Invoke();
            }
            else
            {
                if (refreshTime >= 0)
                    refreshTime += Time.deltaTime;
            }

        }

        //DrwaLines();

    }

    void CheckButtonInteraction()
    {
        //User may limit button press interaction with direction or choose none for all directions
        if (!_isPressed)
        {
            //Debug.Log(Vector3.Angle(interactionMoveDirection, transform.TransformDirection(expectedButtonMoveDirection))+"  "+ interactionMoveDirection.normalized+" - "+ transform.TransformDirection(expectedButtonMoveDirection).normalized);
            if (Mathf.Abs(Vector3.Angle(interactionMoveDirection, transform.TransformDirection(expectedButtonMoveDirection))) < maxAngleDif ||
                expectedButtonMoveDirection == Vector3.zero)
            {
                StartCoroutine(InvokePressEvent(pressInvokationTreshold, OnPress));
            }
        }
    }

    //ToDebug Directions
    void DrwaLines()
    {
        Vector3 source = new Vector3(lastHandPos.x, lastHandPos.y, lastHandPos.z);
        line1.SetPositions(new Vector3[2]{ source, source + interactionMoveDirection });
        Vector3 source2 = transform.position;
        line2.SetPositions(new Vector3[2] { source2, source2 + transform.TransformDirection(expectedButtonMoveDirection) });
    }

    void OnDrawGizmoSelected()
    {
        Gizmos.color = Color.red;
        Vector3 source = new Vector3(lastHandPos.x, lastHandPos.y, lastHandPos.z);
        Gizmos.DrawLine(source, source + interactionMoveDirection);

        Gizmos.color = Color.green;
        Vector3 source2 = transform.position;
        Gizmos.DrawLine(source2, source2 + transform.TransformDirection(expectedButtonMoveDirection));
    }


}
