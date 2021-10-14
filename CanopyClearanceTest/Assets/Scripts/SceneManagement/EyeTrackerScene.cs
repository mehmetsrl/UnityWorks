using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EyeTracker;
using UnityEngine;
using UnityEngine.UI;

public class EyeTrackerScene : TempScene
{

    public Calibration.Mode calibrationMode = Calibration.Mode._2D;
    public bool displayEyeImages = true;
    GameObject cameraObject;
    Text calibrationText;
    private FramePublishing framePublishing;

    protected override void Init()
    {
        PupilSettings.Instance.currentCamera = GetComponentInChildren<Camera>();
        cameraObject = PupilSettings.Instance.currentCamera.gameObject;

        EyeTrackerPviManagement.Instance.ConfigStartProcess();

        ResetCalibrationText();
    }

    protected override void BindEvents()
    {
        PupilTools.OnConnected += OnConnected;
        PupilTools.OnDisconnecting += OnDisconnecting;
        PupilTools.OnCalibrationStarted += OnCalibtaionStarted;
        PupilTools.OnCalibrationEnded += OnCalibrationEnded;
        PupilTools.OnCalibrationFailed += OnCalibrationFailed;
    }



    protected override void UnBindEvents()
    {
        PupilTools.OnConnected -= OnConnected;
        PupilTools.OnDisconnecting -= OnDisconnecting;
        PupilTools.OnCalibrationStarted -= OnCalibtaionStarted;
        PupilTools.OnCalibrationEnded -= OnCalibrationEnded;
        PupilTools.OnCalibrationFailed -= OnCalibrationFailed;
    }

    protected override void Run(){}

    protected override void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded)
    {
        contiunousHandlersNeeded=new List<InputManager.ContiunousHandler>();
    }

    protected override void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded)
    {
        handlersNeeded=new List<InputManager.Handler>();
    }



    #region SceneSpecificMethods

    private void OnConnected()
    {
        calibrationText.text = "Success";

        PupilTools.CalibrationMode = calibrationMode;

        InitializeCalibrationPointPreview();

        if (displayEyeImages && framePublishing == null)
            framePublishing = gameObject.AddComponent<FramePublishing>();

        Invoke("ShowCalibrate", 1f);
    }

    private void OnDisconnecting()
    {
        ResetCalibrationText();

        if (displayEyeImages && framePublishing != null)
            framePublishing.enabled = false;
    }

    private void OnCalibtaionStarted()
    {
        cameraObject.SetActive(true);
        PupilSettings.Instance.currentCamera = cameraObject.GetComponent<Camera>();
        calibrationText.text = "";

        if (displayEyeImages && framePublishing != null)
            framePublishing.enabled = false;
        
    }

    private void OnCalibrationEnded()
    {
        calibrationText.text = "Calibration ended.";
        MainScene.Instance.MoveNextSceneGroup();
    }

    private void OnCalibrationFailed() { }

    void ShowCalibrate()
    {
        calibrationText.text = "Press 'c' to start calibration. \r\n Press 'v' to visualize Gaze";
    }

    void ResetCalibrationText()
    {
        if (calibrationText == null)
            calibrationText = cameraObject.GetComponentInChildren<Text>();

        calibrationText.text = "Trying to connect to Pupil.\nPlease start Pupil Service/Capture\n(if you have not done so, already)";
    }

    void InitializeCalibrationPointPreview()
    {
        var type = PupilTools.CalibrationType;
        var camera = PupilSettings.Instance.currentCamera;
        Vector3 centerPoint = PupilTools.CalibrationType.centerPoint;
        foreach (var vector in type.vectorDepthRadius)
        {
            Transform previewCircle = GameObject.Instantiate<Transform>(Resources.Load<Transform>("CalibrationPointExtendPreview"));
            previewCircle.parent = camera.transform;
            float scaleFactor = (centerPoint.x + vector.y) * 0.2f;
            if (PupilTools.CalibrationMode == Calibration.Mode._2D)
            {
                centerPoint.z = type.vectorDepthRadius[0].x;
                scaleFactor = camera.worldToCameraMatrix.MultiplyPoint3x4(camera.ViewportToWorldPoint(centerPoint + Vector3.right * vector.y)).x * 0.2f;
                centerPoint = camera.worldToCameraMatrix.MultiplyPoint3x4(camera.ViewportToWorldPoint(centerPoint));
            }
            previewCircle.localScale = new Vector3(scaleFactor, scaleFactor / PupilSettings.Instance.currentCamera.aspect, 1);
            previewCircle.localPosition = new Vector3(centerPoint.x, centerPoint.y, vector.x);
            previewCircle.localEulerAngles = Vector3.zero;
        }
    }

    #endregion


}
