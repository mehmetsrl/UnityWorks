using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity.Animation;
using UnityEngine;
using UnityEngine.UI;

public class LeapUIManager : UIManager
{
    private static LeapUIManager _instance;
    public static LeapUIManager Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance == null) _instance = value;
        }
    }

    public TransformTweenBehaviour tweenBehaviour;

    //Accesors
    //public static UiFrame UIFrame { get; private set; }
    //public static UiButton UIButton { get; private set; }
    //public static UiSlider UISlider { get; private set; }
    //public static UiVideoPlayer UIVideoPlayer { get; private set; }
    
    public static ButtonExperimentComponent ButtonExperimentComp { get; private set; }
    public static SliderExperimentComponent SliderExperimentComp { get; private set; }
    public static FrameExperimentComponent FrameExperimentComp { get; private set; }
    public static FrameExperimentComponent BackFrameExperimentComp { get; private set; }
    public static VideoPlayerExperimentComponent VideoPlayerExperimentComp { get; private set; }
    public static TextExperimentComponent TextExperimentComp { get; private set; }
    public static ImageExperimentComponent ImageExperimentComp { get; private set; }
    public static HolographicViewExperimentComponent HologramComp { get; private set; }
    public static FrameExperimentComponent PageComp { get; private set; }

    
    private bool UIOpen = false;
    public bool IsUIOpen
    {
        get { return UIOpen; }
    }

    private bool initUIGO = false;

    void Awake()
    {
        Instance = this;
        //UIFrame = frameExperimentPrefab;
        //UIButton = buttonExperimentPrefab;
        //UISlider = sliderExperimentPrefab;
        //UIVideoPlayer = videoPlayer;

       ButtonExperimentComp = buttonExperimentPrefab;
        SliderExperimentComp = sliderExperimentPrefab;
        FrameExperimentComp = frameExperimentPrefab;
        BackFrameExperimentComp = backFrameExperimentPrefab;
        VideoPlayerExperimentComp = videoPlayerExperimentPrefab;
        TextExperimentComp = textExperimentPrefab;
        ImageExperimentComp = imageExperimentPrefab;
        HologramComp = hologramPrefab;
        PageComp = pagePrefab;

        tweenBehaviour.PlayBackward();

    }

    private bool firstInit = false;
    private List<Transform> childList;


    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.5f);
        firstInit = true;
        childList = UIrootGO.transform.GetComponentsInChildren<Transform>().ToList();
        foreach (var child in childList)
        {
            child.gameObject.SetActive(false);
        }
        CloseUI();
    }

    protected override void HandleCreatedUIItem(ExperimentComponent experimentComponent)
    {

        if (experimentComponent.Implayer == null)
        {
            ICompImp implayer = experimentComponent.gameObject.AddComponent<LeapCompImp>();
            experimentComponent.SetImplayer(ref implayer);
        }

        //Adding UI behavior with leap gestures (Open-Close gestures)

        if (experimentComponent.Type == ComponentBase.ComponentType.UI)
        {
            tweenBehaviour.tween.OnLeaveStart(() => { OnActivationStart(experimentComponent); });
            tweenBehaviour.tween.OnReachEnd(() => { OnActivationEnd(experimentComponent); });
            tweenBehaviour.tween.OnLeaveEnd(() => { OnDeactivationStart(experimentComponent); });
            tweenBehaviour.tween.OnReachStart(() => { OnDeactivationEnd(experimentComponent); });

        }
    }

    public void CloseUI()
    {
        UIOpen = false;
    }

    void OnActivationStart(ExperimentComponent experimentComponent)
    {
        if (firstInit)
        {
            firstInit = false;
            foreach (var child in childList)
            {
                child.gameObject.SetActive(true);
            }
        }

        UIOpen = true;

        //UIrootGO.gameObject.SetActive(true);
        experimentComponent.OnUIActivationStart();
        
    }
    void OnActivationEnd(ExperimentComponent experimentComponent)
    {
        experimentComponent.OnUIActivationEnd();
    }
    void OnDeactivationStart(ExperimentComponent experimentComponent)
    {
        experimentComponent.OnUIDeactivationStart();
    }
    void OnDeactivationEnd(ExperimentComponent experimentComponent)
    {
        experimentComponent.OnUIDeactivationEnd();
        //Leap SDK gives null exception when it is disabled 
        //UIrootGO.gameObject.SetActive(false);
        UIOpen = false;
    }
}
