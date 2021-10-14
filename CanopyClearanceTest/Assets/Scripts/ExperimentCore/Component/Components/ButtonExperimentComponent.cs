using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonExperimentComponent : ExperimentComponent
{
    public Vector3 moveDirection;
    public float moveAmount;
    [HideInInspector]
    public Vector3 initialLocation;
    [HideInInspector]
    public Vector3 targetLocation;


    private bool inInteraction = true;
    public bool InInteraction
    {
        get { return inInteraction; }
    }
    
    public GameObject background;
    private Renderer imgRenderer;
    [SerializeField]
    private Material material;
    [SerializeField]
    private Texture2D texture;

    public Texture2D Texture
    {
        get { return texture; }
        set
        {
            texture = value;
            if (material != null)
                material.SetTexture("_MainTex", texture);

            if (material != null && imgRenderer != null)
                imgRenderer.material = material;
        }
    }

    public Color ReflectiveColor
    {
        get { return material.GetColor("_ReflectColor");}
        set
        {
            material.SetColor("_ReflectColor",value);
        }
    }
    public Color MainColor
    {
        get { return material.GetColor("_Color"); }
        set
        {
            material.SetColor("_Color", value);
        }
    }

    [SerializeField]
    [FormerlySerializedAs("OnContactBegin")]
    public UnityEvent onPress = new UnityEvent();

    [SerializeField]
    [FormerlySerializedAs("OnContactEnd")]
    public UnityEvent onUnpress = new UnityEvent();


    protected new void Awake()
    {
        base.Awake();
        Initiated = false;
        if (handleObject == null)
        {
            if (GetComponentInChildren<Collider>() != null)
                handleObject = GetComponentInChildren<Collider>().gameObject;
        }

        if (movingObject != null)
        {
            initialLocation = movingObject.transform.localPosition;
            targetLocation = (moveDirection * moveAmount) + initialLocation;
        }

        if (material == null)
            material = new Material(Shader.Find("Reflective/Diffuse Transperant"));

        if (material!=null)
            ReflectiveColor = new Color(0, 0, 0, 255);

        if (background == null && transform.childCount > 0)
            background = transform.GetChild(0).gameObject;

        imgRenderer = background.GetComponent<Renderer>();

        if (imgRenderer == null)
            imgRenderer = background.AddComponent<MeshRenderer>();

        Texture2D prevTexture = null;
        Color prevMainColor=default(Color), prevReflectiveColor= default(Color);
        if (imgRenderer.material != null)
        {
            prevTexture = imgRenderer.material.GetTexture("_MainTex") as Texture2D;
            prevMainColor = imgRenderer.material.GetColor("_Color");
            prevReflectiveColor = imgRenderer.material.GetColor("_ReflectColor");
        }

        if (prevTexture != null)
            Texture = prevTexture;
        if (prevMainColor != default(Color))
            MainColor = prevMainColor;
        if (prevReflectiveColor != default(Color))
            ReflectiveColor = prevReflectiveColor;

        if (material != null && imgRenderer != null)
            imgRenderer.material = material;
        

        Initiated = true;
    }

    protected Vector3 initialLocalPos;
    void Start()
    {
        initialLocalPos = transform.localPosition;
    }

    

    private bool inContact = false;
    private bool clicked = false;

    private float unPressTime = 0f;

    protected  override void ContactEndActions()
    {
        base.ContactEndActions();
        //Debug.Log("Contact End");


        inContact = false;
    }
    
    private GameObject contactObj = null;
    private Vector3 contactPoint = default(Vector3);
    protected override void ContactBeginActions()
    {
        //Debug.Log("Contact Begin");
        base.ContactBeginActions();
        if (InInteraction && CurrentState == ComponentBase.State.Active)
        {
            if(!componentImplayer.IsValidAction()|| clicked) return;

            if (moveDirection != Vector3.zero)
                movingObject.transform.localPosition = targetLocation;

            contactObj = Implayer.GetContactItem();
            contactPoint = transform.InverseTransformDirection(contactObj.transform.position);

            onPress.Invoke();
            OnItemActivated.Invoke();
            componentImplayer.NotifyButtonPress();
        }
        inContact = true;
        clicked = true;
    }
    
    public void SetColor(Color color)
    {
        MainColor = color;
        ReflectiveColor = color;
    }


    protected override void HandleRenderQueueLevelAssignment(RenderQueueLevel order)
    {
        if(material!=null)
            material.renderQueue = (int)order;
    }
    
    void Update()
    {
        //Debug.Log(unPressTime);
        if (inContact) unPressTime = 0f;
        else
        {
            if (unPressTime >.5f)
            {
                if (clicked)
                {
                    if (InInteraction && CurrentState == ComponentBase.State.Active)
                    {
                        if (moveDirection != Vector3.zero)
                            movingObject.transform.localPosition = initialLocation;
                        onUnpress.Invoke();
                        componentImplayer.NotifyButtonUnPress();
                    }
                    clicked = false;
                }
            }
            else
            {
                unPressTime += Time.deltaTime;
            }
        }

        //if (Initiated && componentImplayer != null && !inContact && contactObj!=null&&contactPoint!=default(Vector3))
        //{

        //    float dist = Vector3.Distance(transform.InverseTransformDirection(contactObj.transform.position), contactPoint);
        //    Debug.Log(transform.InverseTransformDirection(contactObj.transform.position)+"   " + contactPoint+"  dist: "+ dist);
        //    if (dist > 10f)
        //    {
        //        if (InInteraction && CurrentState == ComponentBase.State.Active)
        //        {
        //            if (moveDirection != Vector3.zero)
        //                movingObject.transform.localPosition = initialLocation;
        //            onUnpress.Invoke();
        //            componentImplayer.NotifyButtonUnPress();
        //        clicked = false;
        //        contactObj = null;
        //        contactPoint = default(Vector3);
        //        }
        //    }
        //}


        //if (Initiated&& componentImplayer!=null && !inContact)
        //{
        //    unPressTime += Time.deltaTime;
        //    if (unPressTime > 1f)
        //    {
        //        if (InInteraction && CurrentState == ComponentBase.State.Active)
        //        {
        //            if (moveDirection != Vector3.zero)
        //                movingObject.transform.localPosition = initialLocation;
        //            onUnpress.Invoke();
        //            componentImplayer.NotifyButtonUnPress();
        //        }
        //        clicked = false;
        //    }
        //}
        //else
        //{
        //    if (unPressTime > 0) unPressTime = 0;
        //}
    }

    //void OnColliderExit(Collision col)
    //{
    //    if (Initiated && componentImplayer != null && !inContact)
    //    {
    //        //unPressTime += Time.deltaTime;
    //        //if (unPressTime > 1f)
    //        {
    //            if (InInteraction && CurrentState == ComponentBase.State.Active)
    //            {
    //                if (moveDirection != Vector3.zero)
    //                    movingObject.transform.localPosition = initialLocation;
    //                onUnpress.Invoke();
    //                componentImplayer.NotifyButtonUnPress();
    //            }
    //            clicked = false;
    //        }
    //    }
    //}

}
