using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GazeInteractable
{
    bool InInteraction { get; }
    Action OnGazeFocusBegin { get; set; }
    Action OnGazeFocusEnd { get; set; }
    Action<RaycastHit> OnGazeHover { get; set; }
    float FocusTresholdTime { get; }
    float FocusProgress { get; }
    GameObject gameObject { get; }
}

public class BaseGazeObject : MonoBehaviour, GazeInteractable
{
    Action onGazeFocusBegin;
    Action onGazeFocusEnd;
    Action<RaycastHit> onGazeHover;
    public static string gazeObjectTag = "BaseGazeObject";

    [SerializeField] private float focusTresholdTime = 2f;
    private float focusTimer = 0;
    [SerializeField] private float toleransTimer = 0;

    [SerializeField]
    private bool inInteraction = false;
    private bool isActive = false;
    private bool initiated = false;
    
    protected void Awake()
    {
        Init();
    }

    protected void Init()
    {
        if (initiated) return;
        initiated = true;

        tag = gazeObjectTag;
        onGazeFocusBegin += GazeFocusBegin;
        onGazeFocusEnd += GazeFocusEnd;
        onGazeHover += GazeHover;
    }

    private bool hover = false;
    public void NotifyGazeHover(RaycastHit hit, bool activateClickByTime = true)
    {
        GazeHover(hit);
        if (activateClickByTime)
        {
            if (focusTimer >= 0 && focusTimer <= focusTresholdTime)
            {
                focusTimer += Time.deltaTime;

                if (toleransTimer > 0)
                    toleransTimer = 0;
            }

        }
    }
    private void GazeHover(RaycastHit hit)
    {
        hover = true;
    }
    private void GazeFocusEnd()
    {
        inInteraction = false;
        GazeInputs.Instance.NotifyFocusEnd(this);
    }

    protected void OnDestroy()
    {
        onGazeFocusBegin -= GazeFocusBegin;
        onGazeFocusEnd -= GazeFocusEnd;
        onGazeHover -= GazeHover;
    }

    private void GazeFocusBegin()
    {
        inInteraction = true;
        GazeInputs.Instance.NotifyFocusBegin(this);
    }
    
    public bool InInteraction
    {
        get { return inInteraction; }
    }
    public bool IsActive
    {
        get { return isActive; }
    }

    public bool IsInitiated
    {
        get { return initiated; }
    }

    public GameObject gameObject
    {
        get { return transform.gameObject; }
    }

    public Action OnGazeFocusBegin
    {
        get { return onGazeFocusBegin; }
        set { onGazeFocusBegin = value; }
    }

    public Action OnGazeFocusEnd
    {
        get { return onGazeFocusEnd; }
        set { onGazeFocusEnd = value; }
    }

    public Action<RaycastHit> OnGazeHover
    {
        get { return onGazeHover; }
        set { onGazeHover = value; }
    }

    public float FocusTresholdTime
    {
        get { return focusTresholdTime; }
    }

    public float FocusTresholdTolerance
    {
        get { return focusTresholdTime * 0.25f; }
    }

    public float FocusProgress
    {
        private set { focusTimer = value * focusTresholdTime; }
        //  -1 -> Clicked
        //   0 -> Not Clicked
        //  >0 -> In Click Progress
        get{if (focusTimer < 0) return 1; return focusTimer / focusTresholdTime; }
    }

    public enum InteractionStatus { Open, Close }
    public InteractionStatus status = InteractionStatus.Open;

    public void SetState(bool isActive)
    {
        status = (isActive) ? InteractionStatus.Open : InteractionStatus.Close;
        this.isActive = isActive;
    }

    protected void Update()
    {

        if (focusTimer > 0)
        {
            if (focusTimer > focusTresholdTime)
            {
                focusTimer = -1;
                OnGazeFocusBegin();
            }
        }

        if (!hover)
        {
            if (toleransTimer > FocusTresholdTolerance)
            {
                if (InInteraction)
                {
                    OnGazeFocusEnd();
                }

                focusTimer = 0;
            }
            else
            {
                if (FocusProgress > .8f)
                    FocusProgress = .8f;
                focusTimer -= Time.deltaTime;
                toleransTimer += Time.deltaTime;
            }
        }

    }

    void LateUpdate()
    {
        hover = false;
    }
}
