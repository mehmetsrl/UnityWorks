using System;
using System.Collections;
using System.Collections.Generic;
using ExperimentCore;
using UnityEngine;
using UnityEngine.UI;

public class AdminUI : MonoBehaviour
{
    public CanopyClearance canopyClearanceExperiment;
    

    private bool SubjectDefined
    {
        get { return subjectDefinedToggle.isOn; }
        set
        {
            subjectDefinedToggle.isOn = value;

            if (subjectDefinedToggle.isOn)
            {
                subjectDefinedToggleText.color = Color.green;
                subjectDefinedToggleText.text = "Subject Defined";
                defineSubjectButton.interactable = false;
            }
            else
            {
                subjectDefinedToggleText.color = subjectDefinedToggleTextColor;
                subjectDefinedToggleText.text = "Define Subject";
                defineSubjectButton.interactable = true;
            }
        }
    }


    #region SubjectInfoPanel
    [Header("SubjectInfoPanel")]
    public Text subjectID;
    public InputField subjectName,subjectGroup;
    public Dropdown subjectGender;
    public Text dateText;
    public Toggle subjectDefinedToggle;
    public Text subjectDefinedToggleText;
    private Color subjectDefinedToggleTextColor;
    public Button defineSubjectButton;
    #endregion

    #region RecordPanel

    [Header("RecordPanel")]
    public Button startRecordButton;
    public Button pauseResumerecordButton, stopRecordButton,discardRecordBtn;
    #endregion

    #region RecordInfoPanel
    [Header("RecordInfoPanel")]
    public Text headPos;
    public Text eyeVisionHitPos, sideViewAngle,sideVidewAngleDP;
    public Toggle isRecording;
    #endregion

    #region ControlPanel

    [Header("ControlPanel")]
    public Dropdown helmetDropdown;
    public Dropdown modalDropdown;
    public Toggle sendEnemies, checkClearance;
    public Slider planeSpeedSlider,flightPathSlider;

    #endregion

    #region LogPanel

    [Header("LogPanel")]
    public Toggle sendRandomFighterToggle;
    public InputField sendFighterAngleInputField;
    public Button sendFighterAngleButton;
    public SubjectLog logPrefab;
    public Text passedFighterAngleText;
    public ScrollRect passedFighterLogs;

    #endregion

    void Start()
    {
        subjectDefinedToggleTextColor = subjectDefinedToggleText.color;
        SubjectDefined = false;

        subjectID.text = CanopyClearance.SubjectID.ToString();
        headPos.text = "";
        eyeVisionHitPos.text = "";
        sideViewAngle.text = "";
        sideVidewAngleDP.text = "";
        isRecording.isOn = false;
        dateText.text = System.DateTime.Now.ToString();

        startRecordButton.interactable = false;
        pauseResumerecordButton.interactable = false;
        stopRecordButton.interactable = false;
        discardRecordBtn.interactable = false;

        sendEnemiesRandomLock = sendRandomFighterToggle.isOn;
        sendEnemiesLock = sendEnemies.isOn;
        
        defineSubjectButton.onClick.AddListener(DefineSubject);
        subjectName.onValueChanged.AddListener((string val) => SubjectDefinitionChanged());
        subjectGroup.onValueChanged.AddListener((string val) => SubjectDefinitionChanged());
        subjectGender.onValueChanged.AddListener((int val) => SubjectDefinitionChanged());


        startRecordButton.onClick.AddListener(StartRecord);
        pauseResumerecordButton.onClick.AddListener(PauseResumeRecord);
        stopRecordButton.onClick.AddListener(StopRecord);
        discardRecordBtn.onClick.AddListener(DiscardRecord);

        helmetDropdown.onValueChanged.AddListener((int val) => OnHelmetChanged((CanopyClearance.Helmet) val));
        helmetDropdown.value = (int)CanopyClearance.Helmet.Avci;
        OnHelmetChanged(CanopyClearance.Helmet.Avci);

        modalDropdown.onValueChanged.AddListener((int val) => OnModalChanged((CanopyClearance.TestModal) val));
        modalDropdown.value = (int) CanopyClearance.TestModal.TFX_V10;
        OnModalChanged(CanopyClearance.TestModal.TFX_V10);

        sendEnemies.onValueChanged.AddListener(SendEnemies);
        checkClearance.onValueChanged.AddListener(CheckClearance);

        planeSpeedSlider.onValueChanged.AddListener(OnPlaneSpeedSliderChanged);

        flightPathSlider.onValueChanged.AddListener(OnFlightPathSliderChanged);

        canopyClearanceExperiment.OnPlaneSpeedChanged += OnPlaneSpeedChanged;

        PlaneController.Instance.OnAnimTimeUpdated += OnAnimTimeUpdated;

        canopyClearanceExperiment.circularPosition.OnAnimationPlayed += TakeLog;

        sendRandomFighterToggle.onValueChanged.AddListener(SendRandomFighter);

        sendFighterAngleButton.onClick.AddListener(SendRandomFighterAngle);
    }

    private void SendRandomFighterAngle()
    {
        float val;
        
        if (float.TryParse(sendFighterAngleInputField.text, out val))
        {
            canopyClearanceExperiment.circularPosition.PlayClosestAngleAnimation(val);
        }
    }

    private bool sendEnemiesRandomLock = false;
    private void SendRandomFighter(bool val)
    {
        sendFighterAngleInputField.interactable = !val;
        sendFighterAngleButton.interactable = !val;

        sendEnemiesRandomLock = val;

        SubmitSendFighters();
    }

    void OnDestroy()
    {

        defineSubjectButton.onClick.RemoveAllListeners();
        subjectName.onValueChanged.RemoveAllListeners();
        subjectGroup.onValueChanged.RemoveAllListeners();
        subjectGender.onValueChanged.RemoveAllListeners();


        startRecordButton.onClick.RemoveAllListeners();
        pauseResumerecordButton.onClick.RemoveAllListeners();
        stopRecordButton.onClick.RemoveAllListeners();
        discardRecordBtn.onClick.RemoveAllListeners();


        helmetDropdown.onValueChanged.RemoveAllListeners();
        modalDropdown.onValueChanged.RemoveAllListeners();
        sendEnemies.onValueChanged.RemoveAllListeners();
        checkClearance.onValueChanged.RemoveAllListeners();

        canopyClearanceExperiment.OnPlaneSpeedChanged -= OnPlaneSpeedChanged;

        PlaneController.Instance.OnAnimTimeUpdated -= OnAnimTimeUpdated;

        canopyClearanceExperiment.circularPosition.OnAnimationPlayed -= TakeLog;


        sendRandomFighterToggle.onValueChanged.RemoveAllListeners();


        ClearLogs();
    }

    private void DiscardRecord()
    {
        canopyClearanceExperiment.DiscardRecord();

        subjectID.text = CanopyClearance.SubjectID.ToString();
        subjectName.interactable = true;
        subjectGroup.interactable = true;
        subjectGender.interactable = true;

        headPos.text = "";
        eyeVisionHitPos.text = "";
        sideViewAngle.text = "";
        sideVidewAngleDP.text = "";
        isRecording.isOn = false;

        startRecordButton.interactable = false;
        pauseResumerecordButton.interactable = false;
        stopRecordButton.interactable = false;
        discardRecordBtn.interactable = false;
    }

    private void StopRecord()
    {
        canopyClearanceExperiment.EndExperiment();

        subjectID.text = CanopyClearance.SubjectID.ToString();
        subjectName.interactable = true;
        subjectGroup.interactable = true;
        subjectGender.interactable = true;

        headPos.text = "";
        eyeVisionHitPos.text = "";
        sideViewAngle.text = "";
        sideVidewAngleDP.text = "";
        isRecording.isOn = false;

        startRecordButton.interactable = false;
        pauseResumerecordButton.interactable = false;
        stopRecordButton.interactable = false;
        discardRecordBtn.interactable = false;
    }

    private void PauseResumeRecord()
    {
        if (subjectDefinedToggle.isOn)
        {
            if (canopyClearanceExperiment.Status == CanopyClearance.ExperimentState.OnRecord)
            {
                canopyClearanceExperiment.PauseExperiment();
                isRecording.isOn = false;
            }
            else if (canopyClearanceExperiment.Status == CanopyClearance.ExperimentState.Paused)
            {
                canopyClearanceExperiment.StartExperiment();
                isRecording.isOn = true;
            }
        }
    }

    private void StartRecord()
    {
        if (subjectDefinedToggle.isOn)
        {
            canopyClearanceExperiment.StartExperiment();
            subjectName.interactable = false;
            subjectGroup.interactable = false;
            subjectGender.interactable = false;
            isRecording.isOn = true;

            startRecordButton.interactable = false;
            pauseResumerecordButton.interactable = true;
            stopRecordButton.interactable = true;
            discardRecordBtn.interactable = true;

            checkClearance.isOn = true;
        }
    }

    private void SubjectDefinitionChanged()
    {
        SubjectDefined = false;

        startRecordButton.interactable = subjectDefinedToggle.isOn;
        pauseResumerecordButton.interactable = subjectDefinedToggle.isOn;
        stopRecordButton.interactable = subjectDefinedToggle.isOn;
        discardRecordBtn.interactable = subjectDefinedToggle.isOn;
    }

    private void DefineSubject()
    {
        SubjectDefined = true;

        startRecordButton.interactable = true;
        pauseResumerecordButton.interactable = false;
        stopRecordButton.interactable = false;
        discardRecordBtn.interactable = false;

        canopyClearanceExperiment.DefineSubject(subjectName.text,subjectGroup.text,(ExperimentCore.IO.CanopyClearanceRecord.Gender)subjectGender.value);

        dateText.text = canopyClearanceExperiment.ExperimentDate.ToString();

    }

    private void OnHelmetChanged(CanopyClearance.Helmet helmetType)
    {
        canopyClearanceExperiment.CurrentHelmet = helmetType;
    }
    private void OnModalChanged(CanopyClearance.TestModal modalType)
    {
        canopyClearanceExperiment.CurrentTestModal = modalType;
    }

    private bool sendEnemiesLock = false;
    private void SendEnemies(bool isOn)
    {
        sendEnemiesLock = isOn;
        SubmitSendFighters();
    }

    void SubmitSendFighters()
    {

        canopyClearanceExperiment.SendFighters = sendEnemiesLock&& sendEnemiesRandomLock;
    }

    private void CheckClearance(bool isOn)
    {
        canopyClearanceExperiment.CheckHeadClearance = isOn;
    }

    private object sliderLock = new object();

    private void OnPlaneSpeedSliderChanged(float value)
    {
        lock (sliderLock)
        {
            canopyClearanceExperiment.PlaneMoveSpeed = value;
        }
    }

    private void OnPlaneSpeedChanged(float value)
    {
            planeSpeedSlider.value = value;
    }
    
    
    private void OnFlightPathSliderChanged(float value)
    {
        lock (sliderLock)
        {
            PlaneController.Instance.AnimTime = value;
        }
    }


    private void OnAnimTimeUpdated(float value)
    {
        flightPathSlider.value = value;
    }


    public void TakeLog(int passedFighterIndex)
    {
        float angle = canopyClearanceExperiment.circularPosition.AngleFromIndex(passedFighterIndex);

        passedFighterAngleText.text = angle.ToString();

        SubjectLog log = Instantiate(logPrefab, passedFighterLogs.content.transform);
        log.transform.SetAsFirstSibling();

        log.logText.text = angle.ToString();

        log.logButton.onClick.AddListener(() =>
            canopyClearanceExperiment.circularPosition.PlaySpesificAnimation(passedFighterIndex));
    }

    void ClearLogs()
    {
        foreach (SubjectLog log in passedFighterLogs.content.transform.GetComponentsInChildren<SubjectLog>())
        {
            log.logButton.onClick.RemoveAllListeners();
            Destroy(log.gameObject);
        }
    }

    
}
