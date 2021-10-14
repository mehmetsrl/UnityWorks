using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public interface IComponent
{

    bool IsMeetPrerequests { get; }
    int GetItemIndex(IComponent component);
    void PlaceItem(IComponent component, Vector3 relativePosition);
    void PlaceItem(IComponent component, Vector2 relativePosition);

    ComponentBase.State CurrentState { get; }
    ComponentBase.ComponentType Type { get; }
    ComponentBase.InteractionState CurrentInteraction { get; }
    ICompImp Implayer { get; }
    void SetImplayer(ref ICompImp implayer);
    void ActivateItem(ComponentBase.State compState);
    void SetRenderQueueLevel(ComponentBase.RenderQueueLevel order);

    void SetVisibility(bool visible);

    void AttachChildren(ExperimentComponent child);
    void RemoveChild(ExperimentComponent child);
    GameObject GetRootUI();
}

[System.Serializable]
public class StateAudioPair
{
    public ComponentBase.InteractionState state;
    public AudioClip audioClip;
}

public abstract class ComponentBase : MonoBehaviour, IComponent
{

    public enum RenderQueueLevel
    {
        Base=3000,
        Level1=3005,
        Level2=3010,
        Level3=3015,
        Level4=3020,
        Level5=3025
    }

    public enum State
    {
        Active,
        Passive
    }

    public enum ComponentType
    {
        UI,
        Static3D
    }
    public enum InteractionState
    {
        None,
        Activated,
        InContact,
        Holded
    }

    //Collision checker
    [HideInInspector]
    public GameObject handleObject;
    //Root Gameobject for component2
    [SerializeField]
    public GameObject headObject;

    [HideInInspector]
    public GameObject movingObject;
    
    protected Dictionary<InteractionState, AudioSource> stateAudioPlayerPairs;
    [SerializeField] protected List<StateAudioPair> stateAudioPairs = new List<StateAudioPair>();

    [SerializeField]
    protected List<ExperimentComponent> childrens = new List<ExperimentComponent>();
    protected ICompImp componentImplayer;
    public ICompImp Implayer
    {
        get { return componentImplayer; }
    }

    protected Renderer[] renderers;

    protected Transform linkedTransform;
    protected Vector3 startLocalPosition;
    protected Vector3 movementDirection;
    protected float movementDelay = 0;
    protected float movementTime = 0;

    protected Vector3 targetScale = Vector3.one;
    protected Vector3 startScale;
    protected float scaleTime = 0;
    protected float scaleDelay = 0;

    protected List<iTweenAnims> UIAnimList = new List<iTweenAnims>();
    protected List<iTweenAnims> closeUIAnimList = new List<iTweenAnims>();

    public List<Prerequest> Prerequests = new List<Prerequest>();
    public abstract bool IsMeetPrerequests { get; }

    private bool _enableOnUIOpened = true;
    public bool EnableOnUIOpened
    {
        get { return _enableOnUIOpened; }
        set
        {
            _enableOnUIOpened = value;
        }
    }

    public abstract void SetImplayer(ref ICompImp implayer);
    public abstract int GetItemIndex(IComponent component);
    public abstract void PlaceItem(IComponent component, Vector3 relativePosition);
    public abstract void PlaceItem(IComponent component, Vector2 relativePosition);

    [SerializeField]
    ComponentType type = ComponentType.UI;

    [SerializeField]
    protected State currentState = State.Active;

    protected State dictatedState = State.Active;
    public State CurrentState
    {
        get
        {
            if (currentState == State.Active && dictatedState == State.Active)
                return State.Active;
            else
                return State.Passive;
        }
        protected set { currentState = value; }
    }

    [SerializeField]
    RenderQueueLevel currentRendOrder = RenderQueueLevel.Base;
    public RenderQueueLevel CurrentRendOrder
    {
        get { return currentRendOrder; }
        protected set { currentRendOrder = value; }
    }

    public ComponentType Type
    {
        get { return type; }
    }

    [SerializeField]
    private ComponentBase.InteractionState interaction = InteractionState.None;
    public ComponentBase.InteractionState CurrentInteraction
    {
        get { return interaction; }
        protected set
        {
            interaction = value;
            if (stateAudioPlayerPairs!=null && stateAudioPlayerPairs.Count>0 && stateAudioPlayerPairs.ContainsKey(interaction))
            {
                if (!stateAudioPlayerPairs[interaction].isPlaying)
                    stateAudioPlayerPairs[interaction].Play();
            }
        }
    }

    bool initiated = false;

    public bool Initiated
    {
        get { return initiated; }
        protected set { initiated = value; }
    }


    public Action OnContactBegin;
    public Action OnContactEnd;
    public Action OnItemActivated;

    public Action OnItemHold;
    public Action<Vector3> OnItemMove;
    public Action OnItemUnhold;

    protected abstract void ContactBeginActions();
    protected abstract void ContactEndActions();
    protected abstract void ItemActivated();
    protected abstract void ItemHolded();
    protected abstract void ItemUnholded();

    public abstract void ActivateItem(State compState);
    protected abstract void DictateActivation(State parentState);
    protected abstract void HandleActivation(State compState);

    public abstract void SetRenderQueueLevel(RenderQueueLevel order);
    protected abstract void HandleRenderQueueLevelAssignment(RenderQueueLevel order);

    public abstract void SetVisibility(bool visible);
    protected abstract void DictateVisibility(bool parentVisibility);
    protected abstract void HandleVisibility(bool visible);

    protected bool isVisible = true;
    protected bool dictatedVisiblity = true;

    public bool IsVisible
    {
        get { return isVisible && dictatedVisiblity; }
    }
        
    public void AttachChildren(ExperimentComponent child)
    {
        childrens.Add(child);
        child.DictateVisibility(IsVisible);
        child.DictateActivation(CurrentState);
        child.SetRenderQueueLevel(CurrentRendOrder);
        //child.EnableOnUIOpened = EnableOnUIOpened;
    }

    public void RemoveChild(ExperimentComponent child)
    {
        if (childrens.Contains(child))
            childrens.Remove(child);
    }
    public abstract GameObject GetRootUI();
}

public class iTweenAnims
{
    private Hashtable forwardAnimHash, backAnimHash;
    private Action animStart;
    public enum AnimType
    {
        moveTo,
        scaleTo,
        alphaTo
    }

    private AnimType type;

    public ExperimentComponent tweenObject;
    public Transform linkedTransform,targetTransform,sourceTransform;
    public Vector3 startLocalPosition;
    public Vector3 movementDirection;
    public Vector3 startScale;
    public Vector3 targetScale;
    public float delay = 0;
    public float tweenTime = 0;


    public iTweenAnims(ExperimentComponent tweenObject, Transform linkedTransform)
    {
        if (linkedTransform != null)
            this.linkedTransform = linkedTransform;

        if (tweenObject != null)
            this.tweenObject = tweenObject;

    }



    public void SetMovementAnimation(Vector3 startLocalPosition =default(Vector3), Vector3 movementDirection = default(Vector3), float tweenTime = .3f, float delay = 0)
    {
        this.type = AnimType.moveTo;

        if (startLocalPosition == default(Vector3))
            startLocalPosition = Vector3.zero;

        if (movementDirection == default(Vector3))
            movementDirection = Vector3.zero;

        this.startLocalPosition = startLocalPosition;
        this.movementDirection = movementDirection;
        this.tweenTime = tweenTime;
        this.delay = delay;


        targetTransform = new GameObject("HelperTargetTransform").transform;
        targetTransform.transform.parent = linkedTransform;
        targetTransform.transform.position =
            linkedTransform.position + linkedTransform.TransformVector(movementDirection);
        

        sourceTransform = new GameObject("HelperSourceTransform").transform;
        sourceTransform.transform.parent = linkedTransform;
        sourceTransform.transform.position =
            linkedTransform.position + linkedTransform.TransformVector(startLocalPosition);


        UpdateHashArgs();
    }



    public void SetScaleAnimation(Vector3 targetScale = default(Vector3), Vector3 startScale = default(Vector3), float tweenTime = .3f, float delay = 0)
    {
        this.type = AnimType.scaleTo;

        if (startScale == default(Vector3))
            startScale = Vector3.zero;
        this.startScale = startScale;

        if (targetScale == default(Vector3))
            targetScale = Vector3.one;
        this.targetScale = targetScale;

        this.tweenTime = tweenTime;
        this.delay = delay;

        UpdateHashArgs();

    }


    public void OnMove(object param)
    {
        Debug.Log("params: "+param);
    }

    //Update source and target positions for tween
    public void UpdateHashArgs()
    {
        switch (type)
        {
            case AnimType.moveTo:
                Vector3 targetPos = linkedTransform.position + linkedTransform.TransformVector(movementDirection);
                Vector3 startPos = linkedTransform.position + linkedTransform.TransformVector(startLocalPosition);
                this.forwardAnimHash = iTween.Hash(
                    "onStart", "ForwardPositionTweenStart", "onStartTarget", tweenObject.gameObject, "onStartParams",this,
                    "onComplete", "ForwardPositionTweenEnd", "onCompleteTarget", tweenObject.gameObject, "onUpdate",
                    (Action<object>) (valParam =>
                    {
                        float amount = (float) valParam;
                        tweenObject.transform.position =
                            Vector3.Lerp(tweenObject.transform.position, targetTransform.transform.position, amount);
                    }),
                    "from", 0, "to", 1,
                    "time", this.tweenTime, "delay", this.delay,
                    "eastType", iTween.EaseType.easeOutQuad);

                this.backAnimHash = iTween.Hash(
                    "onStart", "BackwardPositionTweenStart", "onStartTarget", tweenObject.gameObject, "onStartParams", this,
                    "onComplete", "BackwardPositionTweenEnd", "onCompleteTarget", tweenObject.gameObject, "onUpdate",
                    (Action<object>)(valParam =>
                    {
                        float amount = (float)valParam;
                        tweenObject.transform.position =
                            Vector3.Lerp(tweenObject.transform.position, sourceTransform.transform.position, amount);
                    }),
                    "from", 0, "to", 1,
                    "time", this.tweenTime,
                    "delay", 0, "eastType", iTween.EaseType.easeInQuad);
                break;
            case AnimType.scaleTo:
                this.forwardAnimHash = iTween.Hash(
                    "onStart", "ForwardScaleTweenStart", "onStartTarget", tweenObject.gameObject, "onStartParams", this,
                    "onComplete", "ForwardScaleTweenEnd", "onCompleteTarget", tweenObject.gameObject,
                    "scale", targetScale, "time", this.tweenTime, "delay", this.delay, "easeType",
                        iTween.EaseType.easeOutBack);

                this.backAnimHash = iTween.Hash(
                    "onStart", "BackwardScaleTweenStart", "onStartTarget", tweenObject.gameObject, "onStartParams", this,
                    "onComplete", "BackwardScaleTweenEnd", "onCompleteTarget", tweenObject.gameObject,
                    "scale", startScale, "time", this.tweenTime, "delay", 0, "easeType",
                    iTween.EaseType.easeInBack);
                break;
        }
    }

    internal void PlayForward()
    {
        switch (type)
        {
            case AnimType.moveTo:
                //iTween.MoveTo(tweenObject.gameObject, forwardAnimHash);
                iTween.ValueTo(tweenObject.gameObject, forwardAnimHash);
                break;
            case AnimType.scaleTo:
                iTween.ScaleTo(tweenObject.gameObject, forwardAnimHash);
                break;
            case AnimType.alphaTo:
                iTween.MoveTo(tweenObject.gameObject, forwardAnimHash);
                break;
        }
    }
    internal void PlayBackward()
    {
        switch (type)
        {
            case AnimType.moveTo:
                iTween.ValueTo(tweenObject.gameObject, backAnimHash);
                break;
            case AnimType.scaleTo:
                iTween.ScaleTo(tweenObject.gameObject, backAnimHash);
                break;
            case AnimType.alphaTo:
                iTween.ValueTo(tweenObject.gameObject, backAnimHash);
                break;
        }
    }
}


/// <summary>
/// Includes base experimentComponent behaviours
/// </summary>
public class ExperimentComponent : ComponentBase
{
    protected void Awake()
    {
        Initiated = true;

        if (headObject == null)
            headObject = gameObject;

        if (movingObject == null)
            movingObject = GetComponentInChildren<MeshRenderer>()?.gameObject;

        OnContactBegin += ContactBeginActions;
        OnContactEnd += ContactEndActions;
        OnItemActivated += ItemActivated;


        OnItemHold += ItemHolded;
        OnItemUnhold += ItemUnholded;


        renderers = gameObject.GetComponentsInChildren<Renderer>();

        movementDirection = Vector3.zero;
        targetScale = Vector3.one;


        stateAudioPlayerPairs = new Dictionary<InteractionState, AudioSource>();
        foreach (var stateAudioPair in stateAudioPairs)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = stateAudioPair.audioClip;
            //audioSource.playOnAwake = false;
            //audioSource.loop = false;
            stateAudioPlayerPairs.Add(stateAudioPair.state, audioSource);
        }


        if (componentImplayer == null)
            componentImplayer = GetComponent<ICompImp>();

        if (componentImplayer == null) return;
        componentImplayer.Initiate(this);

    }

    public void SetMovementTween(Transform linkedTransform, Vector3 startLocalPosition = default(Vector3), Vector3 movementDirection = default(Vector3), float movementTime = .3f, float movementDelay = 0)
    {
        if (startLocalPosition == default(Vector3))
            startLocalPosition = Vector3.zero;

        if (movementDirection == default(Vector3))
            movementDirection = Vector3.zero;

        this.startLocalPosition = startLocalPosition;
        this.movementDirection = movementDirection;
        this.movementTime = movementTime;
        this.movementDelay = movementDelay;

        if (linkedTransform != null)
            this.linkedTransform = linkedTransform;


        if (movementDirection != Vector3.zero)
        {
            UIAnimList.Add(new iTweenAnims(this, linkedTransform));
            UIAnimList.Last().SetMovementAnimation(startLocalPosition, movementDirection, movementTime, movementDelay);
        }

    }

    IEnumerator DelayedAction(float delayTime, Action calbackAction)
    {
        yield return new WaitForSeconds(delayTime);
        calbackAction.Invoke();
    }

    private Action animEndAction = null;

    void ForwardPositionTweenStart(object anim)
    {
        //Make parentVisibility when anim starts
        DictateVisibility(true);
        transform.position = linkedTransform.position + linkedTransform.TransformVector(startLocalPosition);
    }
    void BackwardPositionTweenStart(object anim)
    {
        transform.position = linkedTransform.position + linkedTransform.TransformVector(movementDirection);
    }
    void ForwardPositionTweenEnd()
    {
        EndAnimation();
    }
    void BackwardPositionTweenEnd()
    {
        EndAnimation();
    }

    void EndAnimation()
    {
        if (animEndAction != null)
        {
            animEndAction.Invoke();
            animEndAction = null;
        }
    }


    public void SetScaleTween(Transform linkedTransform, Vector3 targetScale = default(Vector3), Vector3 startScale = default(Vector3), float scaleTime = .3f, float scaleDelay = 0)
    {
        if (targetScale == default(Vector3))
            targetScale = Vector3.one;

        if (startScale == default(Vector3))
            startScale = Vector3.zero;
        //transform.localScale = startScale;
        this.targetScale = targetScale;
        this.startScale = startScale;
        this.scaleDelay = scaleDelay;
        this.scaleTime = scaleTime;

        if (linkedTransform != null)
            this.linkedTransform = linkedTransform;

        if (this.targetScale != Vector3.zero)
        {
            UIAnimList.Add(new iTweenAnims(this, linkedTransform));
            UIAnimList.Last().SetScaleAnimation(this.targetScale, this.startScale, this.scaleTime, this.scaleDelay);
        }
    }

    void ForwardScaleTweenStart(object anim)
    {
        //Make parentVisibility when anim starts
        DictateVisibility(true);
        transform.localScale = ((iTweenAnims) anim).startScale;
    }
    void BackwardScaleTweenStart(object anim)
    {
        transform.localScale = ((iTweenAnims)anim).targetScale;
    }
    void ForwardScaleTweenEnd()
    {
        EndAnimation();
    }
    void BackwardScaleTweenEnd()
    {
        EndAnimation();
    }



    public void SetAplhaTween(float initialAlpha, float targetAlpha)
    {

    }

    public sealed override bool IsMeetPrerequests
    {
        get
        {
            foreach (Prerequest prereq in Prerequests)
            {
                if (!prereq.IsSatisfy)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public sealed override void SetImplayer(ref ICompImp implayer)
    {

        if (implayer == null) return;

        if (componentImplayer != null)
        {
            Destroy(componentImplayer as MonoBehaviour);
        }

        componentImplayer = implayer;
        componentImplayer.Initiate(this);
        componentImplayer.IgnoreGrasping(true);

        foreach (var clild in childrens)
        {
            ICompImp childCompImp = clild.gameObject.AddComponent(implayer.GetComponent().GetType()) as ICompImp;
            if (childCompImp == null) Debug.LogError("Cannot cast experimentComponent implementation");
            clild.SetImplayer(ref childCompImp);
        }
    }

    public sealed override int GetItemIndex(IComponent component)
    {
        return childrens.IndexOf(component as ExperimentComponent);
    }

    public sealed override void PlaceItem(IComponent component, Vector3 relativePosition)
    {
    }
    public sealed override void PlaceItem(IComponent component, Vector2 relativePosition)
    {
    }

    public sealed override void ActivateItem(State compState)
    {
        currentState = compState;
        foreach (var clild in childrens)
            clild.DictateActivation(CurrentState);

        HandleActivation(CurrentState);
    }
    
    protected sealed override void DictateActivation(State parentCompState)
    {
        dictatedState = parentCompState;

        foreach (var clild in childrens)
            clild.DictateActivation(CurrentState);

        HandleActivation(CurrentState);
    }

    protected override void HandleActivation(State compState)
    {
        currentState = compState;
        if (componentImplayer != null)
            componentImplayer.SetState(currentState);
    }
    

    public sealed override void SetRenderQueueLevel(RenderQueueLevel order)
    {

        foreach (var clild in childrens)
            clild.SetRenderQueueLevel(order);

        CurrentRendOrder = order;
        HandleRenderQueueLevelAssignment(order);
    }

    protected override void HandleRenderQueueLevelAssignment(RenderQueueLevel order)
    {

        if (renderers != null && renderers.Length>0)
        {
            foreach (var rend in renderers)
            {
                rend.material.renderQueue = (int)order;
            }
        }
    }


    public void AnimateTweens(bool isForward,Action endCallBackAction=null)
    {
        if (linkedTransform == null) return;
        animEndAction = endCallBackAction;

        if (isForward)
        {
            foreach (var anim in UIAnimList)
            {
                anim.UpdateHashArgs();
                anim.PlayForward();
            }
        }
        else
        {
            foreach (var anim in UIAnimList)
            {
                anim.PlayBackward();
            }
        }
    }


    //These 4 methods is comes from main open/close animation. (Comes from Leap Motion SDK)
    //This main animation will be replaced with iTween or removed later
    public virtual void OnUIActivationStart()
    {
        //if it have individual animation, it needs to wait base animation end
        if (UIAnimList.Count > 0)
            DictateVisibility(false);
    }
    public virtual void OnUIActivationEnd()
    {
        //DictateActivation(ComponentBase.State.Active);
        if (EnableOnUIOpened)
        {
            AnimateTweens(true);
        }
    }
    public virtual void OnUIDeactivationStart()
    {
        AnimateTweens(false);
        //DictateActivation(ComponentBase.State.Passive);
    }
    public virtual void OnUIDeactivationEnd()
    {
    }


    public sealed override void SetVisibility(bool visible)
    {
        isVisible = visible;
        foreach (var clild in childrens)
            clild.DictateVisibility(IsVisible);

        HandleVisibility(IsVisible);
    }

    protected sealed override void DictateVisibility(bool parentVisibility)
    {
        dictatedVisiblity = parentVisibility;

        foreach (var clild in childrens)
            clild.DictateVisibility(IsVisible);

        HandleVisibility(IsVisible);
    }

    protected override void HandleVisibility(bool visible)
    {

        if (renderers == null)
            renderers = GetComponentsInChildren<Renderer>();
        if (renderers == null) return;

        foreach (var rend in renderers)
        {
            if (rend != null && rend.transform.parent == transform)
                rend.enabled = visible;
        }
        //gameObject.SetActive(visible);
    }
    public sealed override GameObject GetRootUI()
    {
        if (Implayer == null) return null;
        return Implayer.GetManager().UIrootGO;
    }

    protected override void ContactEndActions()
    {
        if (CurrentInteraction != InteractionState.Holded)
            CurrentInteraction = InteractionState.None;
    }
    protected override void ContactBeginActions()
    {
        if (CurrentInteraction == InteractionState.None)
            CurrentInteraction = InteractionState.InContact;
    }
    protected override void ItemActivated()
    {
        CurrentInteraction = InteractionState.Activated;
    }

    protected override void ItemHolded()
    {
        CurrentInteraction = InteractionState.Holded;
    }
    protected override void ItemUnholded()
    {
        if (CurrentInteraction == InteractionState.Holded)
            CurrentInteraction = InteractionState.None;
    }
}



[System.Serializable]
public class Prerequest
{
    public IComponent interactableObj;
    public ComponentBase.State objStatus;

    public bool IsSatisfy
    {
        get
        {
            return (interactableObj.CurrentState == objStatus);
        }
    }
}

static class Utils
{
    public static T GetCopyOf<T>(this UnityEngine.Component comp, T other) where T : UnityEngine.Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }
}