using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using UnityEngine;
using Valve.VR;
using ExperimentCore;
using System;
using System.ComponentModel;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using ExperimentCore.IO;

public class CanopyClearance : TempScene
{
    public enum DistanceLevel
    {
        Hit=20,
        Level4=21,
        Level3=22,
        Level2=23,
        Level1=24,
        Safe=25
    }

    public DistanceLevel distance = DistanceLevel.Safe;


    private float loopTime = 1;
    private bool playSound = false;
    IEnumerator SoundLoop()
    {
        audioSource.Play();
        yield return new WaitUntil(() => playSound && giveSoundFeedback);
        yield return new WaitForSeconds(loopTime * audioSource.clip.length);
        StartCoroutine(SoundLoop());
    }

    public AudioSource audioSource,hitAudioSource;

    public Transform hmdRoot;
    public GameObject avciHMD, strikerHMD;

    public bool moveOnPath = true;
    public bool giveSoundFeedback = true;

    public GameObject eyePoint;

    [SerializeField]
    public Slider distanceFeedbackSlider;

    public CircularPositionCreator circularPosition;

    public enum GazeInput
    {
        EyeTracking,
        CenterScreen
    }

    public GazeInput gazeInputType = GazeInput.CenterScreen;

    public bool SendFighters { get; set; }
    public bool CheckHeadClearance { get; set; }
    
    private static Record<CanopyClearanceRecord> subjectRecord;
    private static Record<CanopyClearanceRecordItem> eyePointRecords;
    
    private static Queue<CanopyClearanceRecordItem> itemsToRecord;
    public static int SubjectID
    {
        get { return Settings.ExperimentInfo.NumberOfSubjects; }
    }

    private static string recordFolderName
    {
        get { return "Subject_" + SubjectID; }
    }

    private static string recordFileName = "SubjectInfo";

    private static string recordItemFolderName
    {
        get { return "SubjectRecords"; }
    }

    private static string recordItemFileName
    {
        get { return "Record_" + recordID; }
    }
    private static long recordID = 0;

    private bool workerIsWorking = true;

    [SerializeField]
    private PositioningData posData;
    [SerializeField]
    private CanopyClearanceRecordItem logItem;

    [SerializeField] private float maxAngle = 0, minAngle = 0;//max angle will give max left view, min angle will give max right angle
    [SerializeField] private Material radialProgressBarMat;
    [SerializeField] private float radialProgressBarOffset = 0f;


    protected override void Init()
    {
        StartCoroutine(SoundLoop());
        subjectRecord = new Record<CanopyClearanceRecord>(Settings.RecordFilePath(recordFolderName, recordFileName));
        eyePointRecords = new Record<CanopyClearanceRecordItem>(Settings.RecordItemFilePath(recordFolderName, recordItemFolderName, recordItemFileName));

        switch (gazeInputType)
        {
            case GazeInput.CenterScreen:
                GazeInputs.Instance.TrackingType = GazeInputs.TrackingMethodType.LookDirection;
                break;
            case GazeInput.EyeTracking:
                GazeInputs.Instance.TrackingType = GazeInputs.TrackingMethodType.EyePoint;
                break;
        }

        status = ExperimentState.CollectingSubjectInfo;
        itemsToRecord = new Queue<CanopyClearanceRecordItem>();
        
        workerIsWorking = true;
        radialProgressBarMat.SetFloat("_Arcrange", 0);
        radialProgressBarMat.SetFloat("_Rotation", 0);

        StartCoroutine(RecordItems());
        
    }

    IEnumerator RecordItems()
    {
        yield return new WaitWhile(() => !workerIsWorking);

        if (Status == ExperimentState.OnRecord)
        {
            if (CanopyClearance.itemsToRecord.Count > 0)
            {
                eyePointRecords.UpdateFilePath(Settings.RecordItemFilePath(recordFolderName,
                    recordItemFolderName, recordItemFileName));
                CanopyClearanceRecordItem item = itemsToRecord.Dequeue();
                eyePointRecords.Save(ref item);
                recordID++;
            }
        }
        else
        {
            if (CanopyClearance.itemsToRecord.Count > 0)
            {
                itemsToRecord.Dequeue();
            }
        }


        StartCoroutine(RecordItems());
    }


    void OnDestroy()
    {
        workerIsWorking = false;

        if(Status==ExperimentState.OnRecord)
            DiscardRecord();
            //EndExperiment();
    }

    [Range(0f, 3f)] public float minTimeForAnimBreak;
    [Range(1f, 5f)] public float maxTimeForAnimBreak;
    
    bool overrideMoveSpeed = false;
    public void OverrideMoveSpeed(bool isOverrided)
    {
        overrideMoveSpeed = isOverrided;
    }

    private bool repotingProgress = false;
    public Action<float> OnPlaneSpeedChanged;
    private float planeMoveSpeed = 0;
    public float PlaneMoveSpeed
    {
        private get { return planeMoveSpeed;}
        set
        {
            if (value > 1) planeMoveSpeed = 1;
            else if (value < 0) planeMoveSpeed = 0;
            else planeMoveSpeed = value;

            if(OnPlaneSpeedChanged!=null)
                OnPlaneSpeedChanged.Invoke(planeMoveSpeed);
        }
    }

    protected override void Run()
    {
        if (SendFighters)
        {
            if (!circularPosition.isAnimating)
            {
                circularPosition.PlayRandomAnimationFromPool(Random.Range(minTimeForAnimBreak, maxTimeForAnimBreak),
                    true);
            }
        }


        if (PlaneMoveSpeed > 0 && moveOnPath)
        {
            PlaneController.Instance.AdvanceOnPath(PlaneMoveSpeed);
        }

    }

    protected override void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded)
    {
        contiunousHandlersNeeded = new List<InputManager.ContiunousHandler>();
        contiunousHandlersNeeded.Add(InputManager.ContiunousHandler.LookDirectionContiunousHandler);
        contiunousHandlersNeeded.Add(InputManager.ContiunousHandler.ViveContiunousHandler);
    }

    protected override void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded)
    {
        handlersNeeded=new List<InputManager.Handler>();
        handlersNeeded.Add(InputManager.Handler.CollisionHandler);
        handlersNeeded.Add(InputManager.Handler.ViveDiscreateHandler);
        handlersNeeded.Add(InputManager.Handler.ShortcutHandler);
        handlersNeeded.Add(InputManager.Handler.LookDirectionDiscreateHandler);
    }

    public enum Helmet
    {
        None,
        Avci,
        Stiker
    }

    public Helmet CurrentHelmet
    {
        get { return currentHelmet;}
        set
        {
            currentHelmet = value;
            SetVisibility(ref hmdRoot, ref strikerHMD, false);
            SetVisibility(ref hmdRoot, ref avciHMD, false);

            switch (currentHelmet)
            {
                case Helmet.None:
                    break;
                case Helmet.Avci:
                    SetVisibility(ref hmdRoot, ref avciHMD,true);
                    break;
                case Helmet.Stiker:
                    SetVisibility(ref hmdRoot, ref strikerHMD, true);
                    break;
            }
        }
    }
    
    void SetVisibility(ref Transform rootTransform,ref GameObject obj, bool isVisible)
    {
        if (isVisible)
        {
            obj.transform.parent = rootTransform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
        else
        {
            obj.transform.parent = null;
            obj.transform.position = new Vector3(10000,10000,10000);
        }
        
    }


    [SerializeField]
    Helmet currentHelmet = Helmet.Avci;


    public enum TestModal
    {
        TFX_V10,
        TFX_V11,
        TFX_V40,
        F16
    }

    public Transform fighterModalRoot;
    public GameObject TFX_V10_GO, TFX_V11_GO, TFX_V40_GO, F16;

    public TestModal CurrentTestModal
    {
        get { return currentTestModal; }
        set
        {
            currentTestModal = value;

            SetVisibility(ref fighterModalRoot, ref TFX_V10_GO, false);
            SetVisibility(ref fighterModalRoot, ref TFX_V11_GO, false);
            SetVisibility(ref fighterModalRoot, ref TFX_V40_GO, false);
            SetVisibility(ref fighterModalRoot, ref F16, false);

            switch (currentTestModal)
            {
                case TestModal.TFX_V10:
                    SetVisibility(ref fighterModalRoot, ref TFX_V10_GO, true);
                    break;
                case TestModal.TFX_V11:
                    SetVisibility(ref fighterModalRoot, ref TFX_V11_GO, true);
                    break;
                case TestModal.TFX_V40:
                    SetVisibility(ref fighterModalRoot, ref TFX_V40_GO, true);
                    break;
                case TestModal.F16:
                    SetVisibility(ref fighterModalRoot, ref F16, true);
                    break;
            }
        }
    }

    [SerializeField]
    TestModal currentTestModal = TestModal.TFX_V10;
    
    protected override void BindEvents()
    {
        Events.OnCollisionOccour.AddListener((collisionData) =>
        {
            LayerMask safeLayer = (int) DistanceLevel.Safe;
            LayerMask hitLayer = (int) DistanceLevel.Hit;
            distance = DistanceLevel.Safe;

            foreach (var cols in collisionData.collisionPairs)
            {
                if (cols.reporterObject.layer >= hitLayer && cols.reporterObject.layer < safeLayer)
                {
                    if (cols.reporterObject.layer < (int) distance)
                        distance = (DistanceLevel) cols.reporterObject.layer;
                }
            }

            GiveHelmetDistFeedback();

        });
        Events.OnEyeFocused.AddListener(eyeData =>
        {

        });
        Events.OnEyeHover.AddListener(eyeData =>
        {
            //Collecting Eye Data 
            if (eyeData.hit.transform.tag != BaseGazeObject.gazeObjectTag) return;
            if (distance == DistanceLevel.Hit) return;

            Vector3 lookDirection = (eyeData.hit.point - eyePoint.transform.position).normalized;

            Vector3 xyPlaneLookDirection = fighterModalRoot.transform.TransformVector(lookDirection);

            Debug.DrawLine(eyePoint.transform.position, eyeData.hit.point,Color.cyan);

            if (CheckHeadClearance)
            {
                logItem = new CanopyClearanceRecordItem(SubjectID, eyePoint.transform.position,
                    eyeData.hit.point,
                    Vector3.SignedAngle(xyPlaneLookDirection, fighterModalRoot.transform.up,
                        fighterModalRoot.transform.right),
                    Vector3.SignedAngle(lookDirection, fighterModalRoot.transform.up,
                        fighterModalRoot.transform.right));

                if (maxAngle < logItem.DepthProjectedSideViewAngle)
                    maxAngle = logItem.DepthProjectedSideViewAngle;

                if (minAngle > logItem.DepthProjectedSideViewAngle)
                    minAngle = logItem.DepthProjectedSideViewAngle;

                UpdateProgressBar();

                if (Status == ExperimentState.OnRecord)
                    CanopyClearance.itemsToRecord.Enqueue(logItem);
            }

        });

        Events.OnViveButtonUp.AddListener((viveInputData) =>
        {
            if(viveInputData.button!= EVRButtonId.k_EButton_SteamVR_Trigger) return;

            if (viveInputData.device == ViveInputs.Instance.leftController)
                NextHelmet();
            else
                circularPosition.PlayAnimation();
        });

        float rotateTrashold = .2f;
        Events.OnViveButton.AddListener((viveInputData) =>
        {

            if(viveInputData.button!=EVRButtonId.k_EButton_SteamVR_Touchpad) return;

            if(viveInputData.device!=ViveInputs.Instance.rightController) return;

            if (Mathf.Abs(viveInputData.axis.x) > rotateTrashold)
            {
                if (viveInputData.axis.x > 0)
                    circularPosition.RotateFighterForward();
                else
                    circularPosition.RotateFighterBackward();
            }

            if (Mathf.Abs(viveInputData.axis.y) > rotateTrashold)
            {
                if (viveInputData.axis.y > 0)
                    circularPosition.MoveFighterForward(5f);
                else
                    circularPosition.MoveFighterBackward(5f);
            }

        });

        Events.ALT_CombinationEvent.AddListener((keyboardData) =>
        {
            if (keyboardData.keys.Contains(KeyCode.N))
            {
                NextHelmet();
            }

        });

        Events.OnJoystickAxisChange.AddListener((joystickAxisData) =>
        {
            if (joystickAxisData.type == EventRegistery.JoystickAxisData.AxisType.Stick)
            {
                EventRegistery.JoystickAxisStickData stickData =
                    joystickAxisData as EventRegistery.JoystickAxisStickData;

                Vector2 significantMove = stickData.vals.OrderBy(vector => vector.magnitude).First();
                PlaneController.Instance.Pitch(-significantMove.x);
                PlaneController.Instance.Roll(significantMove.y);
            }
            else if (joystickAxisData.type == EventRegistery.JoystickAxisData.AxisType.Throttle)
            {
                EventRegistery.JoystickAxisThrottleData throttleData =
                    joystickAxisData as EventRegistery.JoystickAxisThrottleData;

                float throttleVal = throttleData.vals.Last();

                throttleVal = (throttleVal + 1) / 2;

                if (!overrideMoveSpeed)
                    PlaneMoveSpeed = throttleVal;
                //PlaneController.Instance.AdvanceOnPath(throttleVal);
            }


        });

    }

    protected override void UnBindEvents()
    {
        EndExperiment();

        Events.OnCollisionOccour.RemoveAllListeners();

        Events.OnEyeFocused.RemoveAllListeners();

        Events.OnViveButtonUp.RemoveAllListeners();
        Events.OnViveButton.RemoveAllListeners();

        Events.ALT_CombinationEvent.RemoveAllListeners();
    }

    private DistanceLevel prevDist = DistanceLevel.Hit;
    void GiveHelmetDistFeedback()
    {

        switch (distance)
        {
            case DistanceLevel.Safe:
                if (audioSource.isPlaying)
                    audioSource.Stop();
                if (hitAudioSource.isPlaying)
                    hitAudioSource.Stop();
                playSound = false;

                distanceFeedbackSlider.value = 0;
                break;
            case DistanceLevel.Level1:
                if (audioSource.isPlaying)
                    audioSource.Stop();
                if (hitAudioSource.isPlaying)
                    hitAudioSource.Stop();
                playSound = true;
                loopTime = 1.5f;
                distanceFeedbackSlider.value = .2f;
                break;
            case DistanceLevel.Level2:
                if (audioSource.isPlaying)
                    audioSource.Stop();
                if (hitAudioSource.isPlaying)
                    hitAudioSource.Stop();
                playSound = true;
                loopTime = 1.25f;
                distanceFeedbackSlider.value = .4f;
                break;
            case DistanceLevel.Level3:
                if (audioSource.isPlaying)
                    audioSource.Stop();
                if (hitAudioSource.isPlaying)
                    hitAudioSource.Stop();
                playSound = true;
                loopTime = 1f;
                distanceFeedbackSlider.value = .6f;
                break;
            case DistanceLevel.Level4:
                if (audioSource.isPlaying)
                    audioSource.Stop();
                if (hitAudioSource.isPlaying)
                    hitAudioSource.Stop();
                playSound = true;
                loopTime = .75f;
                distanceFeedbackSlider.value = .8f;
                break;
            case DistanceLevel.Hit:
                playSound = false;
                if (audioSource.isPlaying)
                    audioSource.Stop();

                //prevent loops
                if (prevDist != distance)
                {
                    if (!hitAudioSource.isPlaying)
                        hitAudioSource.Play();
                }

                loopTime = .1f;
                distanceFeedbackSlider.value = 1f;
                break;
        }

        prevDist = distance;
    }

    void NextHelmet()
    {
        if (CurrentHelmet == Helmet.Stiker) CurrentHelmet = Helmet.Avci;
        else if (CurrentHelmet == Helmet.Avci) CurrentHelmet = Helmet.Stiker;
    }

    void UpdateProgressBar()
    {
        radialProgressBarOffset = 270 + Mathf.Abs(minAngle);
        radialProgressBarOffset %= 360;

        radialProgressBarMat.SetFloat("_Arcrange", maxAngle - minAngle);
        radialProgressBarMat.SetFloat("_Rotation", radialProgressBarOffset);
    }

    void ResetProgressBar()
    {
        minAngle = 0;
        maxAngle = 0;
        UpdateProgressBar();
    }


    //Flow Management
    public enum ExperimentState
    {
        None,
        CollectingSubjectInfo,
        OnRecord,
        Paused,
        ExperimentEnded
    }

    [SerializeField]
    ExperimentState status = ExperimentState.None;

    public ExperimentState Status
    {
        get { return status; }
    }

    private CanopyClearanceRecord subject;
    public CanopyClearanceRecord DefineSubject(string subjectName, string subjectGroup, CanopyClearanceRecord.Gender subjectGender)
    {

        posData.FillTransformPositionVals();

        subject = new CanopyClearanceRecord(SubjectID, subjectName, subjectGroup, subjectGender,
            System.DateTime.Now, Mathf.Max(Mathf.Abs(minAngle), Mathf.Abs(maxAngle)), -1, posData);
        return subject;
    }

    public System.DateTime ExperimentDate
    {
        get
        {
            if (subject != null) return subject.TestDate;
            return default(System.DateTime);
        }
    }
    
    public void StartExperiment()
    {
        ResetProgressBar();
        status = ExperimentState.OnRecord;
    }
    public void PauseExperiment()
    {
        status = ExperimentState.Paused;

    }

    public void DiscardRecord()
    {
        status = ExperimentState.ExperimentEnded;
        if (System.IO.Directory.Exists(Settings.SubjectFolderPath(recordFolderName)))
        {
             ExperimentUtils.DeleteDirectory(Settings.SubjectFolderPath(recordFolderName));
        }

        ResetProgressBar();
    }

    public void EndExperiment()
    {
        status = ExperimentState.ExperimentEnded;
        
        subjectRecord.UpdateFilePath(Settings.RecordFilePath(recordFolderName, recordFileName));
        subjectRecord.Save(ref subject);

        Settings.SaveExperiment();
        ResetProgressBar();
    }
}
