using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GazeInputs : MonoBehaviour {

    public enum TrackingMethodType
    {
        EyePoint,
        LookDirection,
        CustomInput
    }
    

    public TrackingMethodType TrackingType = TrackingMethodType.LookDirection;
    [SerializeField]
    float baseDistanceToCam = .5f;
    [SerializeField]
    float coursorScaleFactor = .1f;
    [SerializeField]
    private AnimationCurve coursorDistanceCurve;
    [SerializeField]
    int maxDistanceToGrowCursore=500;
    [SerializeField]
    GameObject coursorModel;

    [Header("2D Coursor Settings")] [SerializeField]
    private Canvas vizorCanvas;
    [SerializeField]
    Image coursorPointImage;
    [SerializeField]
    Image coursorPointProgressImage;
    [SerializeField]
    Sprite coursorImage, cousorHoverImage;
    [SerializeField]
    Vector2 normalCoursorSize = new Vector2(20, 20), hoverCoursorSize = new Vector2(40, 40);
    
    public GameObject CoursorModel
    {
        get { return coursorModel; }
    }

    private CustomGazeInput customGazeInput;
    private Camera lastCam;

    private Camera LastCam
    {
        get
        {
            if (lastCam == null)
            {
                if (Camera.main.gameObject != null)
                {
                    lastCam = GameObject.Instantiate(Camera.main.gameObject, transform).GetComponent<Camera>();

                    foreach (var scripts in lastCam.GetComponentsInChildren<MonoBehaviour>())
                    {
                        Destroy(scripts);
                    }

                    if (lastCam.GetComponentInChildren<AudioListener>() != null)
                        lastCam.GetComponentInChildren<AudioListener>().enabled = false;
                    
                    lastCam.stereoTargetEye = StereoTargetEyeMask.None;
                }
            }

            return lastCam;
        }
    }

    private Ray ray;
    private RaycastHit hit;
    private RaycastHit[] hitArray;

    private float thresholdTime = 30;
    private float thresholdTolerans = 0;
    public float focusTimer = 0;
    private float toleransTimer = 0;
    private BaseGazeObject _lastBaseGazeObject = null;

    public bool showGazeHitPoint;
    public GameObject gazeGO;
    private Renderer gazeGORenderer;

    private bool initiated = false;
    public bool Initiated
    {
        get { return initiated; }
    }

    private bool collectGazeInput = true;
    public bool CollectGazeInput
    {
        get { return collectGazeInput; }
        set
        {
            collectGazeInput=value;
            SetCoursorImage(false);
            EnableHUDCoursor(collectGazeInput);
        }
    }

    public Queue<RaycastHit> HitQueue
    {
        get { return hitQueue; }
    }

    Queue<RaycastHit> hitQueue = new Queue<RaycastHit>();

    public RaycastHit EyeHit
    {
        get { return hit; }
    }

    public RaycastHit LastEyeHit
    {
        get { return lastHit; }
    }

    private RaycastHit lastHit;

    static GazeInputs _instance;

    public static GazeInputs Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }

    public Queue<Tuple<GazeInteractable, bool>> FocusActions
    {
        get { return focusActions; }
    }

    private Queue <Tuple<GazeInteractable, bool>> focusActions = new Queue<Tuple<GazeInteractable, bool>>();

    public Tuple<GazeInteractable, bool> FocusAction
    {
        get { return focusAction; }
    }
    private Tuple<GazeInteractable, bool> focusAction=null;

    private int maxFocusEventNumber = 5;

    void Awake()
    {
        Instance = this;
        TempScene.OnExperimentLoaded += SetUpVizor;
    }

    void OnDestroy()
    {
        TempScene.OnExperimentLoaded -= SetUpVizor;
    }

    void SetUpVizor()
    {
        vizorCanvas.worldCamera = Camera.main;
    }

    // Use this for initialization
    void Start ()
    {

        if (showGazeHitPoint)
        {
            gazeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gazeGO.GetComponent<Collider>().enabled = false;
            gazeGO.name = "GazePoint";
            gazeGO.transform.parent = transform;
            gazeGO.transform.localScale = Vector3.one / 150f;
            gazeGO.transform.position = Vector3.zero;
            gazeGO.transform.rotation = Quaternion.identity;
            gazeGO.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"));
            gazeGO.GetComponent<MeshRenderer>().material.color = Color.magenta;
            gazeGORenderer = gazeGO.GetComponent<MeshRenderer>();
        }

        if (customGazeInput == null)
            customGazeInput = GetComponent<CustomGazeInput>();
        

        SetCoursorImage(false);
        coursorPointProgressImage.rectTransform.sizeDelta = hoverCoursorSize;

        initiated = true;
    }

    void SetCoursorImage(bool isHover)
    {
        if (isHover)
        {
            coursorPointImage.sprite = cousorHoverImage;
            coursorPointImage.rectTransform.sizeDelta = hoverCoursorSize;
        }
        else
        {
            coursorPointImage.sprite = coursorImage;
            coursorPointImage.rectTransform.sizeDelta = normalCoursorSize;
        }
    }

    public void EnableHUDCoursor(bool enabled)
    {
        coursorPointImage.enabled = enabled;
    }

    private bool coursorStoped = false;

    private bool isHover=false;

    void FixedUpdate()
    {
        if(!Initiated || !CollectGazeInput) return;

        if(Camera.main==null) return;

        bool activateClickByTime = true;
        switch (TrackingType)
        {
            case TrackingMethodType.LookDirection:
                ray = Camera.main.ViewportPointToRay(Vector2.one / 2);
                break;
            case TrackingMethodType.EyePoint:
                if (PupilTools.IsConnected && !PupilTools.IsGazing)
                {
                    PupilGazeTracker.Instance.StartVisualizingGaze();
                    Debug.Log("We are gazing");
                }

                if(!PupilTools.IsGazing) return;

                Vector2 gazePointLeft = PupilData._2D.GetEyePosition(PupilSettings.Instance.currentCamera, PupilData.leftEyeID);
                Vector2 gazePointRight = PupilData._2D.GetEyePosition(PupilSettings.Instance.currentCamera, PupilData.rightEyeID);
                Vector2 gazePointCenter = PupilData._2D.GazePosition;

                //Debug.Log("gazePointCenter    x:" + gazePointCenter.x+" y:"+gazePointCenter.y);

                //Vector3 viewportPoint = new Vector3(gazePointCenter.x, gazePointCenter.y, 1f);
                ray = PupilSettings.Instance.currentCamera.ViewportPointToRay(gazePointCenter);
                break;
            case TrackingMethodType.CustomInput:
                if (LastCam == null)
                {
                    ray = Camera.main.ViewportPointToRay(Vector2.one / 2);

                }
                else
                {
                    if (customGazeInput.IsCoursorMoving)
                    {
                        ray = LastCam.ViewportPointToRay(customGazeInput.ViewportCoords);
                        coursorStoped = true;
                    }
                    else
                    {
                        if (coursorStoped)
                        {
                            customGazeInput.ViewportCoords =
                                Camera.main.WorldToViewportPoint(ray.origin + ray.direction);

                            coursorStoped = false;
                        }

                        LastCam.transform.position = Camera.main.transform.position;
                        LastCam.transform.rotation = Camera.main.transform.rotation;
                        ray = Camera.main.ViewportPointToRay(customGazeInput.ViewportCoords);
                    }
                }

                activateClickByTime = false;
                break;
            default:
                return;
        }

        Vector2 coursorPos2D = Camera.main.WorldToViewportPoint(ray.origin);
        coursorPos2D=new Vector2(Mathf.Round(coursorPos2D.x*10)/10, Mathf.Round(coursorPos2D.y*10)/10);
        coursorPointImage.rectTransform.anchorMin = coursorPos2D;
        coursorPointImage.rectTransform.anchorMax = coursorPos2D;


        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        hitArray = Physics.RaycastAll(ray,1000,~(1<< LayerMask.NameToLayer("IgnoreRaycasting"))).OrderBy(h => h.distance).Take(2).ToArray();
        
        bool hitGazeObj = false;

        if (hitArray.Length > 0)
        {
            hit = hitArray[0];
            
            if (hit.transform.tag == "BaseGazeObject")
            {
                _lastBaseGazeObject = null;
                //if(hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
                _lastBaseGazeObject = hit.transform.GetComponent<BaseGazeObject>();

                if (hitArray.Length > 1)
                    if (hit.collider.bounds.Contains(hitArray[1].transform.position))
                        hit = hitArray[1];

                _lastBaseGazeObject.NotifyGazeHover(hit, activateClickByTime);
                hitGazeObj = true;
            }
            else if(hit.transform.tag == "UI")
            {

                if (_lastBaseGazeObject != null)
                {
                    _lastBaseGazeObject.NotifyGazeHover(hit, activateClickByTime);
                    hitGazeObj = true;
                }

            }
            else
            {
                _lastBaseGazeObject = null;
                hit = new RaycastHit();
            }

            if (_lastBaseGazeObject != null)
            {
                //If it hits a component hide coursor
                if (_lastBaseGazeObject.GetComponent<ExperimentComponent>() != null)
                {
                    EnableHUDCoursor(false);
                    coursorPointProgressImage.enabled = false;
                    coursorPointProgressImage.fillAmount = 1;
                }
                else
                {
                    EnableHUDCoursor(true);
                    coursorPointProgressImage.enabled = true;
                    coursorPointProgressImage.fillAmount = _lastBaseGazeObject.FocusProgress;
                }
            }
            else
            {
                EnableHUDCoursor(true);
                coursorPointProgressImage.fillAmount = 0;
                coursorPointProgressImage.enabled = false;
            }

            if (gazeGO != null)
            {
                gazeGO.transform.position = hit.point;
                if (!gazeGORenderer.enabled)
                    gazeGORenderer.enabled = true;
            }

            lastHit = hit;
        }
        else
        {
            hit = new RaycastHit();
            if (gazeGO != null)
            {
                if (gazeGORenderer.enabled)
                    gazeGORenderer.enabled = false;
            }

            if (!coursorPointImage.enabled)
                EnableHUDCoursor(true);

            if(coursorPointProgressImage.enabled)
                coursorPointProgressImage.enabled = false;

        }

        Vector3 coursorPos= hit.point;
        //CoursorModel.SetActive(coursorPos != Vector3.zero);

        if (isHover != hitGazeObj)
        {
            isHover = hitGazeObj;
            SetCoursorImage(isHover);
        }


        if (coursorPos == Vector3.zero)
            coursorPos = ray.origin + ray.direction * baseDistanceToCam;


        float distance = Vector3.Distance(Camera.main.transform.position, CoursorModel.transform.position);

        if (distance > maxDistanceToGrowCursore)
            CoursorModel.transform.position = ray.origin + ray.direction * distance;


        CoursorModel.transform.position = Vector3.Lerp(CoursorModel.transform.position, coursorPos, .3f);
        
        CoursorModel.transform.localScale = Vector3.one * distance * coursorScaleFactor;

        
    }

    internal void ClearFocusActions()
    {
        focusAction = null;
    }

    public void NotifyFocusBegin(BaseGazeObject baseGazeObject)
    {
        focusAction = new Tuple<GazeInteractable, bool>(baseGazeObject, true);
    }

    public void NotifyFocusEnd(BaseGazeObject baseGazeObject)
    {
        focusAction = new Tuple<GazeInteractable, bool>(baseGazeObject, false);
    }

    public void TriggerPress()
    {
        if (_lastBaseGazeObject != null)
            _lastBaseGazeObject.OnGazeFocusBegin();
    }
    public void TriggerUnPress()
    {
        if (_lastBaseGazeObject != null)
            _lastBaseGazeObject.OnGazeFocusEnd();
    }


    void LateUpdate()
    {
        if(Camera.main==null) return;
        CoursorModel.transform.LookAt(Camera.main.transform);
    }
}
