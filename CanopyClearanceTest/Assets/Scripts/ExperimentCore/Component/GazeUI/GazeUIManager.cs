using Leap.Unity.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeUIManager : UIManager
{

    private static GazeUIManager _instance;
    public static GazeUIManager Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance == null) _instance = value;
        }
    }
    public TransformTweenBehaviour tweenBehaviour;

    private bool UIOpen = false;
    public bool IsUIOpen
    {
        get { return UIOpen; }
    }

    //Accesors
    public static ButtonExperimentComponent ButtonExperimentComp { get; private set; }
    public static SliderExperimentComponent SliderExperimentComp { get; private set; }
    public static FrameExperimentComponent FrameExperimentComp { get; private set; }
    public static FrameExperimentComponent BackFrameExperimentComp { get; private set; }
    public static VideoPlayerExperimentComponent VideoPlayerExperimentComp { get; private set; }
    public static TextExperimentComponent TextExperimentComp { get; private set; }
    public static ImageExperimentComponent ImageExperimentComp { get; private set; }
    public static HolographicViewExperimentComponent HologramComp { get; private set; }
    public static FrameExperimentComponent PageComp { get; private set; }

    void Awake()
    {
        Instance = this;

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
    protected override void HandleCreatedUIItem(ExperimentComponent experimentComponent)
    {
        if (experimentComponent.Implayer == null)
        {
            ICompImp implayer = experimentComponent.gameObject.AddComponent<GazeCompImp>();
            experimentComponent.SetImplayer(ref implayer);
        }
        //Adding UI behavior with gaze gestures (Open-Close gestures)
        if (experimentComponent.Type == ComponentBase.ComponentType.UI)
        {
            tweenBehaviour.tween.OnLeaveStart(() => { OnActivationStart(experimentComponent); });
            tweenBehaviour.tween.OnReachEnd(() => { OnActivationEnd(experimentComponent); });
            tweenBehaviour.tween.OnLeaveEnd(() => { OnDeactivationStart(experimentComponent); });
            tweenBehaviour.tween.OnReachStart(() => { OnDeactivationEnd(experimentComponent); });
        }
    }

    void OnActivationStart(ExperimentComponent experimentComponent)
    {
        UIrootGO.gameObject.SetActive(true);
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
        UIrootGO.gameObject.SetActive(false);
        experimentComponent.OnUIDeactivationEnd();
    }

    public void OpenUI(Vector3 source, Vector3 position)
    {
        UpdatePosition(source, position);
        UIOpen = true;
        tweenBehaviour.PlayForward();
    }

    public void UpdatePosition(Vector3 source, Vector3 position)
    {
        UIrootGO.transform.position = source + ((position - source).normalized * DistanceFromCamera);
        UIrootGO.transform.LookAt(source);
        UIrootGO.transform.localRotation = Quaternion.Euler(UIrootGO.transform.localRotation.eulerAngles.x + 180,
            UIrootGO.transform.localRotation.eulerAngles.y, UIrootGO.transform.localRotation.eulerAngles.z);
    }

    public void CloseUI()
    {
        UIOpen = false;
        tweenBehaviour.PlayBackward();
    }
    public void CloseUIImmediately()
    {
        UIOpen = false;
        tweenBehaviour.SetTargetToStart();
    }
}
