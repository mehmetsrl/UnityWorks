using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Leap.Unity.Attributes;

public class SliderExperimentComponent : ButtonExperimentComponent
{
    [Header("Slider Variables")]
    public GameObject slideArea;

    protected float _horizontalSliderPercent;
    protected float _verticalSliderPercent;
    protected RectTransform parent;

    private bool _started = false;
    Vector3 localTransitionPosition;

    #region Accesors

    public enum SliderType
    {
        Vertical,
        Horizontal,
        TwoDimensional,
        Rotational
    }

    [Header("Slider Settings")]
    public SliderType sliderType = SliderType.Horizontal;

    public bool dispatchSlideValueOnStart = true;

    [Tooltip("Manually specify sliderExperiment limits even if the sliderExperiment's parent has a RectTransform.")]
    [DisableIf("_parentHasRectTransform", isEqualTo: false)]
    public bool overrideRectLimits = false;
    [SerializeField, HideInInspector]
#pragma warning disable 0414
    private bool _parentHasRectTransform = false;
#pragma warning restore 0414

    [Header("Horizontal Axis")]
    public float defaultHorizontalValue;

    [Tooltip("The minimum and maximum values that the sliderExperiment reports on the horizontal axis.")]
    [FormerlySerializedAs("horizontalValueRange")]
    [SerializeField]
    private Vector2 _horizontalValueRange = new Vector2(0f, 1f);
    public float minHorizontalValue
    {
        get
        {
            return _horizontalValueRange.x;
        }
        set
        {
            if (value != _horizontalValueRange.x)
            {
                _horizontalValueRange.x = value;
                HorizontalSlideEvent(HorizontalSliderValue);
            }
        }
    }

    public float maxHorizontalValue
    {
        get
        {
            return _horizontalValueRange.y;
        }
        set
        {
            if (value != _horizontalValueRange.y)
            {
                _horizontalValueRange.y = value;
                HorizontalSlideEvent(HorizontalSliderValue);
            }
        }
    }

    [Tooltip("The minimum and maximum horizontal extents that the sliderExperiment can slide to in world space.")]
    [MinMax(-0.5f, 0.5f)]
    public Vector2 horizontalSlideLimits = new Vector2(-0.05f, 0.05f);

    [Tooltip("The number of discrete quantized notches **beyond the first** that this "
           + "sliderExperiment can occupy on the horizontal axis. A value of zero indicates a "
           + "continuous (non-quantized) sliderExperiment for this axis.")]
    [MinValue(0)]
    public int horizontalSteps = 0;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    ///<summary> Triggered while this sliderExperiment is depressed. </summary>
    [SerializeField]
    [FormerlySerializedAs("horizontalSlideEvent")]
    public FloatEvent _horizontalSlideEvent = new FloatEvent();

    [Header("Vertical Axis")]
    public float defaultVerticalValue;

    [Tooltip("The minimum and maximum values that the sliderExperiment reports on the horizontal axis.")]
    [FormerlySerializedAs("verticalValueRange")]
    [SerializeField]
    private Vector2 _verticalValueRange = new Vector2(0f, 1f);
    public float minVerticalValue
    {
        get
        {
            return _verticalValueRange.x;
        }
        set
        {
            if (value != _verticalValueRange.x)
            {
                _verticalValueRange.x = value;
                VerticalSlideEvent(VerticalSliderValue);
            }
        }
    }

    public float maxVerticalValue
    {
        get
        {
            return _verticalValueRange.y;
        }
        set
        {
            if (value != _verticalValueRange.y)
            {
                _verticalValueRange.y = value;
                VerticalSlideEvent(VerticalSliderValue);
            }
        }
    }

    [MinMax(-0.5f, 0.5f)]
    [Tooltip("The minimum and maximum vertical extents that the sliderExperiment can slide to in world space.")]
    public Vector2 verticalSlideLimits = new Vector2(0f, 0f);

    [Tooltip("The number of discrete quantized notches **beyond the first** that this "
           + "sliderExperiment can occupy on the vertical axis. A value of zero indicates a "
           + "continuous (non-quantized) sliderExperiment for this axis.")]
    [MinValue(0)]
    public int verticalSteps = 0;

    ///<summary> Triggered while this sliderExperiment is depressed. </summary>
    [SerializeField]
    [FormerlySerializedAs("verticalSlideEvent")]
    public FloatEvent _verticalSlideEvent = new FloatEvent();

    public Action<float> HorizontalSlideEvent = (f) => { };
    public Action<float> VerticalSlideEvent = (f) => { };

    public static unsafe bool IsNaN(float f)
    {
        int binary = *(int*) (&f);
        return ((binary & 0x7F800000)==0x7F800000) && ((binary & 0x007FFFFF) !=0 );
    }

    public float HorizontalSliderPercent
    {
        get
        {
            return _horizontalSliderPercent;
        }
        set
        {
            if (!_started)
            {
                Debug.LogWarning("An object is attempting to access this sliderExperiment's value before it has been initialized!  Initializing now; this could yield unpected behaviour...", this);
                Start();
            }
            _horizontalSliderPercent = value;
            localTransitionPosition = new Vector3(Mathf.Lerp(initialLocation.x + horizontalSlideLimits.x, initialLocation.x + horizontalSlideLimits.y, _horizontalSliderPercent), handleObject.transform.localPosition.y, handleObject.transform.localPosition.z);

            if (IsNaN(localTransitionPosition.x))
                return;
                //localTransitionPosition = new Vector3(0, localTransitionPosition.y, localTransitionPosition.z);
            if (IsNaN(localTransitionPosition.y))
                return;
            //localTransitionPosition = new Vector3(localTransitionPosition.x, 0, localTransitionPosition.z);
            if (IsNaN(localTransitionPosition.z))
                return;
            //localTransitionPosition = new Vector3(localTransitionPosition.x, localTransitionPosition.y, 0);

            handleObject.transform.localPosition = localTransitionPosition;
        }
    }

    public float VerticalSliderPercent
    {
        get
        {
            return _verticalSliderPercent;
        }
        set
        {
            if (!_started)
            {
                Debug.LogWarning("An object is attempting to access this sliderExperiment's value before it has been initialized!  Initializing now; this could yield unpected behaviour...", this);
                Start();
            }
            _verticalSliderPercent = value;
            localTransitionPosition = new Vector3(handleObject.transform.localPosition.x, Mathf.Lerp(initialLocation.y + verticalSlideLimits.x, initialLocation.y + verticalSlideLimits.y, _verticalSliderPercent), handleObject.transform.localPosition.z);

            handleObject.transform.localPosition = localTransitionPosition;
        }
    }

    ///<summary> This sliderExperiment's horizontal sliderExperiment value, mapped between the values in the HorizontalValueRange. </summary>
    public float HorizontalSliderValue
    {
        get
        {
            return Mathf.Lerp(_horizontalValueRange.x, _horizontalValueRange.y, _horizontalSliderPercent);
        }
        set
        {
            HorizontalSliderPercent = Mathf.InverseLerp(_horizontalValueRange.x, _horizontalValueRange.y, value);
        }
    }

    ///<summary> This sliderExperiment's current vertical sliderExperiment value, mapped between the values in the VerticalValueRange. </summary>
    public float VerticalSliderValue
    {
        get
        {
            return Mathf.Lerp(_verticalValueRange.x, _verticalValueRange.y, _verticalSliderPercent);
        }
        set
        {
            VerticalSliderPercent = Mathf.InverseLerp(_verticalValueRange.x, _verticalValueRange.y, value);
        }
    }

    public void CalculateSliderValues()
    {
        // Calculate renormalized sliderExperiment values.
        if (horizontalSlideLimits.x != horizontalSlideLimits.y)
        {
            _horizontalSliderPercent = Mathf.InverseLerp(initialLocation.x + horizontalSlideLimits.x, initialLocation.x + horizontalSlideLimits.y, handleObject.transform.localPosition.x);
            HorizontalSlideEvent(HorizontalSliderValue);
        }

        if (verticalSlideLimits.x != verticalSlideLimits.y)
        {
            _verticalSliderPercent = Mathf.InverseLerp(initialLocation.y + verticalSlideLimits.x, initialLocation.y + verticalSlideLimits.y, handleObject.transform.localPosition.y);
            VerticalSlideEvent(VerticalSliderValue);
        }
    }

    public float normalizedHorizontalValue
    {
        get
        {
            return _horizontalSliderPercent;
        }
        set
        {
            var newValue = Mathf.Clamp01(value);
            if (newValue != _horizontalSliderPercent)
            {
                _horizontalSliderPercent = newValue;
                HorizontalSlideEvent(HorizontalSliderValue);
            }
        }
    }

    public float normalizedVerticalValue
    {
        get
        {
            return _verticalSliderPercent;
        }
        set
        {
            var newValue = Mathf.Clamp01(value);
            if (newValue != _verticalSliderPercent)
            {
                _verticalSliderPercent = newValue;
                VerticalSlideEvent(VerticalSliderValue);
            }
        }
    }

    /// <summary>
    /// Returns the number of horizontal steps past the minimum value of the sliderExperiment, for
    /// sliders with a non-zero number of horizontal steps. This value is independent of
    /// the horizontal value range of the sliderExperiment. For example a sliderExperiment with a
    /// horizontalSteps value of 9 could have horizontalStepValues of 0-9.
    /// </summary>
    public int horizontalStepValue
    {
        get
        {
            float range = _horizontalValueRange.y - _horizontalValueRange.x;
            if (range == 0F) return 0;
            else
            {
                return (int)(_horizontalSliderPercent * horizontalSteps * 1.001F);
            }
        }
    }
    #endregion



    protected void Start()
    {
        if (_started) return;

        _started = true;

        CalculateSliderLimits();

        switch (sliderType)
        {
            case SliderType.Horizontal:
                verticalSlideLimits = new Vector2(0, 0);
                break;
            case SliderType.Vertical:
                horizontalSlideLimits = new Vector2(0, 0);
                break;
        }

        HorizontalSliderValue = defaultHorizontalValue;
        VerticalSliderValue = defaultVerticalValue;

    }

    protected void OnEnable()
    {
        if (dispatchSlideValueOnStart)
        {
            HorizontalSlideEvent(HorizontalSliderValue);
            VerticalSlideEvent(VerticalSliderValue);
        }
        
        BindEvents();
    }
    protected void OnDisable()
    {
        UnbindEvents();
    }

    void BindEvents()
    {

        HorizontalSlideEvent += OnHorizontalMove;
        VerticalSlideEvent += OnVerticalMove;

    }

    void UnbindEvents()
    {
        HorizontalSlideEvent -= OnHorizontalMove;
        VerticalSlideEvent -= OnVerticalMove;
    }


    void OnHorizontalMove(float amount)
    {
        _horizontalSlideEvent.Invoke(amount);
    }

    void OnVerticalMove(float amount)
    {
        _verticalSlideEvent.Invoke(amount);
    }

    private void CalculateSliderLimits()
    {
        if (transform.parent != null)
        {
            parent = transform.parent.GetComponent<RectTransform>();

            if (overrideRectLimits) return;

            if (parent != null)
            {
                if (parent.rect.width < 0f || parent.rect.height < 0f)
                {
                    Debug.LogError("Parent Rectangle dimensions negative; can't set sliderExperiment boundaries!", parent.gameObject);
                    enabled = false;
                }
                else
                {
                    var self = transform.GetComponent<RectTransform>();
                    if (self != null)
                    {
                        horizontalSlideLimits = new Vector2(parent.rect.xMin - transform.localPosition.x + self.rect.width / 2F, parent.rect.xMax - transform.localPosition.x - self.rect.width / 2F);
                        if (horizontalSlideLimits.x > horizontalSlideLimits.y)
                        {
                            horizontalSlideLimits = new Vector2(0F, 0F);
                        }
                        if (Mathf.Abs(horizontalSlideLimits.x) < 0.0001F)
                        {
                            horizontalSlideLimits.x = 0F;
                        }
                        if (Mathf.Abs(horizontalSlideLimits.y) < 0.0001F)
                        {
                            horizontalSlideLimits.y = 0F;
                        }

                        verticalSlideLimits = new Vector2(parent.rect.yMin - transform.localPosition.y + self.rect.height / 2F, parent.rect.yMax - transform.localPosition.y - self.rect.height / 2F);
                        if (verticalSlideLimits.x > verticalSlideLimits.y)
                        {
                            verticalSlideLimits = new Vector2(0F, 0F);
                        }
                        if (Mathf.Abs(verticalSlideLimits.x) < 0.0001F)
                        {
                            verticalSlideLimits.x = 0F;
                        }
                        if (Mathf.Abs(verticalSlideLimits.y) < 0.0001F)
                        {
                            verticalSlideLimits.y = 0F;
                        }
                    }
                    else
                    {
                        horizontalSlideLimits = new Vector2(parent.rect.xMin - transform.localPosition.x, parent.rect.xMax - transform.localPosition.x);
                        verticalSlideLimits = new Vector2(parent.rect.yMin - transform.localPosition.y, parent.rect.yMax - transform.localPosition.y);
                    }
                }
            }
        }
    }
}
