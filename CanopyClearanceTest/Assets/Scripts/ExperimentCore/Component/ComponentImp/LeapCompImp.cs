using Assets.Scripts.InputManagement;
using Leap;
using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using LeapInternal;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity.Interaction;

public class LeapCompImp : BaseLeapObject, ICompImp
{
    ExperimentComponent _linkedExperimentComponent;

    public float pressInvokationTreshold = 0.2f;

    public Vector3 expectedButtonMoveDirection;
    public float maxAngleDif = 5f;
    [HideInInspector] public Vector3 interactionMoveDirection;
    public float movementRefreshAmount = 0.01f;
    float refreshTime = 0f;
    

    protected Action onCkeckState;

    public IComponent GetLinkedComponent()
    {
        return _linkedExperimentComponent;
    }

    public void Initiate(IComponent linkedComponent, string tag)
    {
        Initiate(linkedComponent);
        gameObject.tag = tag;
        ignoreGrasping = true;
    }

    public void Initiate(IComponent linkedComponent)
    {
        this._linkedExperimentComponent = linkedComponent as ExperimentComponent;

        if (this._linkedExperimentComponent == null) return;

        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        
    }

    public void NotifyButtonPress()
    {
        if (_linkedExperimentComponent == null) return;
        LeapInputs.Instance.OnPress(_linkedExperimentComponent);
    }

    public void NotifyButtonUnPress()
    {
        if (_linkedExperimentComponent == null) return;
        LeapInputs.Instance.OnUnpress(_linkedExperimentComponent);
    }

    public void SetState(ComponentBase.State compState)
    {

    }


    protected override void OnEnable()
    {
        base.OnEnable();

        //For Getting Leap's original events
        OnContactBegin += ContactBegin;
        OnContactEnd += ContactEnd;
        OnContactStay += ContactStay;
        OnGraspBegin += GraspBegin;
        OnGraspEnd += GraspEnd;
        OnGraspedMovement += GraspMovement;

        //For invoking after expected invokation
        //onCkeckState += CheckButtonInteraction;

        if(_linkedExperimentComponent!=null)
            if (typeof(SliderExperimentComponent) == _linkedExperimentComponent.GetType())
                onCkeckState += CheckSliderInteraction;
    }

    private KinematicGraspedMovement _lazyKinematicGraspedMovement;
    private KinematicGraspedMovement _kinematicGraspedMovement
    {
        get
        {
            if (_lazyKinematicGraspedMovement == null)
            {
                _lazyKinematicGraspedMovement = new KinematicGraspedMovement();
            }
            return _lazyKinematicGraspedMovement;
        }
    }

    private NonKinematicGraspedMovement _lazyNonKinematicGraspedMovement;
    private NonKinematicGraspedMovement _nonKinematicGraspedMovement
    {
        get
        {
            if (_lazyNonKinematicGraspedMovement == null)
            {
                _lazyNonKinematicGraspedMovement = new NonKinematicGraspedMovement();
            }
            return _lazyNonKinematicGraspedMovement;
        }
    }
    private bool _grasped = false;

    protected override void fixedUpdateGraspedMovement(Leap.Unity.Pose origPose, Leap.Unity.Pose newPose,
        List<InteractionController> controllers)
    {
        if (!_grasped) return;
        IGraspedMovementHandler graspedMovementHandler
            = rigidbody.isKinematic ?
                (IGraspedMovementHandler)_kinematicGraspedMovement
                : (IGraspedMovementHandler)_nonKinematicGraspedMovement;
        graspedMovementHandler.MoveTo(newPose.position, newPose.rotation,
            this, _grasped);

        OnGraspedMovement(origPose.position, origPose.rotation,
            newPose.position, newPose.rotation,
            controllers);
    }

    private void GraspMovement(Vector3 oldPosition, Quaternion oldRotation, Vector3 newPosition, Quaternion newRotation, List<InteractionController> graspingControllers)
    {
        if (this._linkedExperimentComponent.CurrentState == ComponentBase.State.Active)
        {
            _linkedExperimentComponent?.OnItemMove(newPosition);
        }
    }

    private void GraspEnd()
    {
        if (!_grasped) return;
        _grasped = false;
        if (this._linkedExperimentComponent.CurrentState == ComponentBase.State.Active)
        {
            _linkedExperimentComponent?.OnItemUnhold();
        }
    }

    private void GraspBegin()
    {
        //if (ClosestHand?.GrabStrength < .5f) return;
        //if (ClosestHand?.PinchDistance > 70f) return;
        _grasped = true;
        //Debug.Log("pinch str: "+ClosestHand.PinchStrength+" pinch dist: "+ClosestHand.PinchDistance+" is pinc: "+ClosestHand.IsPinching());

        if (this._linkedExperimentComponent.CurrentState == ComponentBase.State.Active)
        {
            _linkedExperimentComponent?.OnItemHold();
        }
    }

    protected override void OnDisable()
    {

        //For Getting Leap's original events
        OnContactBegin = null;
        OnContactEnd = null;
        OnContactStay = null;
        OnGraspBegin = null;
        OnGraspEnd = null;

        //For invoking after expected invokation
        onCkeckState = null;

        base.OnDisable();
    }



    private void ContactBegin()
    {
        if (this._linkedExperimentComponent.CurrentState == ComponentBase.State.Active)
        {
            inContactStay = true;
            if (ClosestHand != null)
            {
                initialHandPos = GetClosestFinger(ClosestHand).TipPosition;
                lastHandPos = initialHandPos;
            }

            _linkedExperimentComponent.OnContactBegin();


            LeapInputs.Instance.OnContactBegin(this._linkedExperimentComponent);
        }
    }

    bool inContactStay = false;

    private void ContactStay()
    {
        if (this._linkedExperimentComponent.CurrentState == ComponentBase.State.Active)
        {
            inContactStay = true;
            if (ClosestHand != null)
                lastHandPos = GetClosestFinger(ClosestHand).TipPosition;

            LeapInputs.Instance.OnContactStay(this._linkedExperimentComponent);
        }
    }

    private void ContactEnd()
    {
        if (this._linkedExperimentComponent.CurrentState == ComponentBase.State.Active)
        {
            if (_linkedExperimentComponent == null) return;

            if (_linkedExperimentComponent.CurrentInteraction != ComponentBase.InteractionState.None)
                _linkedExperimentComponent.OnContactEnd.Invoke();

            inContactStay = false;

            closestFinger = null;


            LeapInputs.Instance.OnContactEnd(this._linkedExperimentComponent);

            //readyToInvoke = true;
        }
    }

    bool readyToInvoke = true;

    IEnumerator InvokePressEvent(float time, Action action)
    {
        if (readyToInvoke && _linkedExperimentComponent.CurrentState==ComponentBase.State.Active)
        {
            readyToInvoke = false;
            action.Invoke();
            NotifyButtonPress();
        }

        yield return new WaitForSeconds(time);
        //yield return new WaitUntil(() => !inContactStay);
        readyToInvoke = true;
    }

    Finger GetClosestFinger(Hand h)
    {
        float dist = float.MaxValue;
        Finger closestFinger = null;
        if (h == null) return null;
        foreach (var f in h.Fingers)
        {
            Vector3 tipPos = new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
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
                if (onCkeckState != null)
                    onCkeckState.Invoke();
            }
            else
            {
                if (refreshTime >= 0)
                    refreshTime += Time.deltaTime;
            }

            inContactStay = false;

        }

        //DrwaLines();
    }

    public bool IsValidAction()
    {
        InteractionBehaviour intObj = _linkedExperimentComponent.GetComponent<InteractionBehaviour>();
        float _fingerTipRadius = .01f;

        if (ClosestHand == null || intObj == null) return false;
        if (intObj.GetHoverDistance(ClosestHand.Fingers[1].TipPosition.ToVector3())< _fingerTipRadius)
        {
            return true;
        }
        else
        {
            return false;
        }

        //return true;
        //return Mathf.Abs(Vector3.Angle(interactionMoveDirection,
        //           transform.TransformDirection(expectedButtonMoveDirection))) < maxAngleDif ||
        //       expectedButtonMoveDirection == Vector3.zero;
    }


    //Not working well TODOO
    void CheckButtonInteraction()
    {
        //if (_linkedExperimentComponent.CurrentInteraction != ComponentBase.InteractionState.InContact)
        {
            //Debug.Log(Vector3.Angle(interactionMoveDirection, transform.TransformDirection(expectedButtonMoveDirection))+"  "+ interactionMoveDirection.normalized+" - "+ transform.TransformDirection(expectedButtonMoveDirection).normalized);
            if (Mathf.Abs(Vector3.Angle(interactionMoveDirection,
                    transform.TransformDirection(expectedButtonMoveDirection))) < maxAngleDif ||
                expectedButtonMoveDirection == Vector3.zero)
            {
                StartCoroutine(InvokePressEvent(/*pressInvokationTreshold*/1, _linkedExperimentComponent.OnItemActivated));
            }
        }
    }
    

    private void CheckSliderInteraction()
    {
        if (closestHoveringHand == null) return;

        if (closestFinger == null)
            closestFinger = GetClosestFinger(closestHoveringHand.Fingers, _linkedExperimentComponent.handleObject.transform);

        Vector3 fingerPos = new Vector3(closestFinger.TipPosition.x, closestFinger.TipPosition.y,
            closestFinger.TipPosition.z);

        Vector3 localFingerPos = _linkedExperimentComponent.handleObject.transform.parent.InverseTransformPoint(fingerPos);

        Vector3 offsetBetweenHandleObj = _linkedExperimentComponent.movingObject.transform.position - _linkedExperimentComponent.handleObject.transform.position;

        switch ((_linkedExperimentComponent as SliderExperimentComponent).sliderType)
        {
            case SliderExperimentComponent.SliderType.Horizontal:
                if (localFingerPos.y > (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.y)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.y,
                        localFingerPos.z);
                else if (localFingerPos.y < (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.x)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.x,
                        localFingerPos.z);

                _linkedExperimentComponent.handleObject.transform.localPosition = new Vector3(localFingerPos.x,
                    _linkedExperimentComponent.handleObject.transform.localPosition.y, _linkedExperimentComponent.handleObject.transform.localPosition.z);
                break;

            case SliderExperimentComponent.SliderType.Vertical:
                if (localFingerPos.y > (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.y)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.y,
                        localFingerPos.z);
                else if (localFingerPos.y < (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.x)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.x,
                        localFingerPos.z);

                _linkedExperimentComponent.handleObject.transform.localPosition = new Vector3(_linkedExperimentComponent.handleObject.transform.localPosition.x,
                    localFingerPos.y, _linkedExperimentComponent.handleObject.transform.localPosition.z);
                break;

            case SliderExperimentComponent.SliderType.TwoDimensional:

                if (localFingerPos.y > (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.y)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.y,
                        localFingerPos.z);
                else if (localFingerPos.y < (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.x)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).horizontalSlideLimits.x,
                        localFingerPos.z);

                if (localFingerPos.y > (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.y)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.y,
                        localFingerPos.z);
                else if (localFingerPos.y < (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.x)
                    localFingerPos = new Vector3(localFingerPos.x, (_linkedExperimentComponent as ButtonExperimentComponent).initialLocation.y + (_linkedExperimentComponent as SliderExperimentComponent).verticalSlideLimits.x,
                        localFingerPos.z);

                _linkedExperimentComponent.handleObject.transform.localPosition = new Vector3(localFingerPos.x, localFingerPos.y,
                    _linkedExperimentComponent.handleObject.transform.localPosition.z);
                break;
        }

        Vector3 sliderPos = _linkedExperimentComponent.handleObject.transform.position;

        sliderPos = new Vector3(sliderPos.x * 1 - Mathf.Abs((_linkedExperimentComponent as ButtonExperimentComponent).moveDirection.x),
            sliderPos.y * 1 - Mathf.Abs((_linkedExperimentComponent as ButtonExperimentComponent).moveDirection.y), sliderPos.z * 1 - Mathf.Abs((_linkedExperimentComponent as ButtonExperimentComponent).moveDirection.z));

        _linkedExperimentComponent.movingObject.transform.position = sliderPos + offsetBetweenHandleObj;

        (_linkedExperimentComponent as SliderExperimentComponent).CalculateSliderValues();

    }

    Leap.Finger GetClosestFinger(List<Leap.Finger> fingers, Transform targetObj)
    {
        float minDistance = float.MaxValue;
        float distance;
        Leap.Finger closestFinger = null;
        foreach (Leap.Finger fing in fingers)
        {
            Vector3 fingPos = new Vector3(fing.TipPosition.x, fing.TipPosition.y, fing.TipPosition.z);
            distance = Vector3.Distance(fingPos, targetObj.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestFinger = fing;
            }
        }
        return closestFinger;
    }
    public UnityEngine.Component GetComponent()
    {
        return this;
    }

    Leap.Finger closestFinger = null;

    [SerializeField]
    private bool GetAverageOfContactPoints = false;

    public Vector3 GetClosestContactPoint()
    {
        Vector3 result = Vector3.zero;

        if (GetAverageOfContactPoints)
        {
            int contactedFingerCout = 0;

            foreach (var finger in Hands.Right.Fingers)
            {
                foreach (var col in _interactionColliders)
                {
                    Vector3 fingerTipPos = new Vector3(finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
                    if (col.bounds.Contains(fingerTipPos))
                    {
                        contactedFingerCout++;
                        result += fingerTipPos;
                        break;
                    }
                }
            }
            foreach (var finger in Hands.Left.Fingers)
            {
                foreach (var col in _interactionColliders)
                {
                    Vector3 fingerTipPos = new Vector3(finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
                    if (col.bounds.Contains(fingerTipPos))
                    {
                        contactedFingerCout++;
                        result += fingerTipPos;
                        break;
                    }
                }
            }

            if (result==Vector3.zero)
                result= Vector3.positiveInfinity;
            else
                result /= contactedFingerCout;
        }
        else
        {
            Leap.Vector contactPosition;

            if (ClosestHand == null) return Vector3.positiveInfinity;

            Finger ClosestFinger = GetClosestFinger(ClosestHand);
            if (ClosestFinger == null) return Vector3.positiveInfinity;

            contactPosition = ClosestFinger.TipPosition;

            result = new Vector3(contactPosition.x, contactPosition.y, contactPosition.z);
        }

        return result;
    }

    public Vector3 GetContactItemPosition()
    {
        Vector3 result = Vector3.zero;

        if (GetAverageOfContactPoints)
        {
            Vector3 RightPalmPos = Hands.Right == null
                ? Vector3.zero
                : new Vector3(Hands.Right.PalmPosition.x, Hands.Right.PalmPosition.y, Hands.Right.PalmPosition.z);
            Vector3 LeftPalmPos = Hands.Left == null
                ? Vector3.zero
                : new Vector3(Hands.Left.PalmPosition.x, Hands.Left.PalmPosition.y, Hands.Left.PalmPosition.z);


            int contactHandCount = 0;
            if (RightPalmPos != Vector3.zero)
                foreach (var col in _interactionColliders)
                {
                    if (col.bounds.Contains(RightPalmPos))
                    {
                        contactHandCount++;
                        result += RightPalmPos;
                        break;
                    }
                }

            if (LeftPalmPos != Vector3.zero)
                foreach (var col in _interactionColliders)
                {
                    if (col.bounds.Contains(LeftPalmPos))
                    {
                        contactHandCount++;
                        result += LeftPalmPos;
                        break;
                    }
                }

            if (result == Vector3.zero)
                result = Vector3.positiveInfinity;
            else
                result /= contactHandCount;
        }
        else
        {
            if (ClosestHand == null) return Vector3.positiveInfinity;

            Leap.Vector contactPosition = ClosestHand.PalmPosition;
            result = new Vector3(contactPosition.x, contactPosition.y, contactPosition.z);
        }

        return result;
    }

    public GameObject GetContactItem()
    {
        //int handID = ClosestHand.Id;
        //HandModel m = LeapInputs.Instance.ModelManager.GetHandModel<HandModel>(ClosestHand.Id);
        //GameObject h = m.gameObject;
        
        foreach (var controller in TAI_LeapInteractionManager.Instance.interactionControllers)
        {
            foreach (IInteractionBehaviour contactingObj in controller.contactingObjects)
            {
                if (contactingObj == this as IInteractionBehaviour)
                {
                    return controller.intHand.contactBones[0].gameObject;
                }
            }


            //if (ClosestHand == controller.intHand.leapHand && controller.isGraspingObject)
            //    return controller.intHand.contactBones[0].gameObject;
        }
        return null;
    }
    
    public void IgnoreGrasping(bool ignore)
    {
        ignoreGrasping = ignore;
    }

    public bool IgnoreGrasping()
    {
        return ignoreGrasping;
    }

    public UIManager GetManager()
    {
        return LeapUIManager.Instance;
    }
}
