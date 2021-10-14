using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SharpTunning : MonoBehaviour
{
    public UIViveController controller;
    public float multiplyer=.2f;

    private GameObject tuningGO=null;
    private Transform initialParent;
    public bool Tunning
    {
        get { return tunning; }
        set
        {
            Debug.Log("tuninng");
            tunning = value;


            if (CalibrationManger.Instance.ActiveCpIndex > -1)
            {
                if (tunning)
                {
                    ConfigHandler.Instance.configMode = ConfigHandler.Mode.tuning;
                    tuningGO = new GameObject("TuningGO");
                    tuningGO.transform.parent = ConfigHandler.Instance
                        .virPointVirtualizations[CalibrationManger.Instance.ActiveCpIndex].transform;

                    tuningGO.transform.localPosition = Vector3.zero;
                    tuningGO.transform.localRotation = Quaternion.identity;

                    initialParent = CalibrationManger.Instance.config.cameraRig.transform.parent;
                    CalibrationManger.Instance.config.cameraRig.transform.parent = tuningGO.transform;
                    
                }
                else
                {
                    if (tuningGO != null)
                    {

                        Vector3 posDif = tuningGO.transform.localPosition;
                        Quaternion rotDif = tuningGO.transform.localRotation;
                        //Make them siblings
                        tuningGO.transform.parent = ConfigHandler.Instance
                            .virPointVirtualizations[CalibrationManger.Instance.ActiveCpIndex].transform.parent;

                        ConfigHandler.Instance.virPointVirtualizations[CalibrationManger.Instance.ActiveCpIndex]
                            .transform.localPosition += posDif;
                        ConfigHandler.Instance.virPointVirtualizations[CalibrationManger.Instance.ActiveCpIndex]
                            .transform.localRotation *= rotDif;

                        CalibrationManger.Instance.config.cameraRig.transform.parent = initialParent;

                        Destroy(tuningGO);
                    }
                }
            }
        }
    }

    private Vector2 lastAxis = Vector2.zero;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private bool initialTunningSet = false;
    private bool tunning = false;

    // Use this for initialization
    void Start ()
    {
        if (controller == null)
            controller = GetComponent<UIViveController>();
        if (controller == null)
            this.enabled = false;
    }

    public enum TuneProperty
    {
        YAxisPosRot,
        XZAxisPos,
        Count
    }

    enum tuningType
    {
        posXZ,
        posX,
        posY,
        posZ,
        rotY
    }

    private tuningType tType = tuningType.posX;
    public TuneProperty property=TuneProperty.YAxisPosRot;
    private float activationTime = 3f;
    private float gripHoldTime;
	// Update is called once per frame
	void Update () {
        if (controller.calibrationDevice != null)
        {
            //Change tuning property
            if (controller.calibrationDevice.GetPressUp(EVRButtonId.k_EButton_Grip))
            {
                if (Tunning && (gripHoldTime < activationTime))
                {
                    Tunning = false;
                    controller.calibrationDevice.TriggerHapticPulse(800);
                }
                gripHoldTime = 0;
            }
            else if (controller.calibrationDevice.GetPress(EVRButtonId.k_EButton_Grip))
            {
                if (!Tunning)
                {
                    gripHoldTime += Time.deltaTime;
                    if (gripHoldTime > activationTime)
                    {
                        Tunning = true;
                        controller.calibrationDevice.TriggerHapticPulse(1500);
                    }
                }
            }

            if (Tunning)
            {
                if (controller.calibrationDevice.GetHairTriggerUp())
                {
                    if ((int) property < (int) TuneProperty.Count - 1)
                    {
                        property++;
                        property = (TuneProperty) (int) property;
                    }
                    else
                        property = (TuneProperty) 0;
                }

                //Handle tunings
                if (tuningGO != null)
                {
                    Vector2 axisInput = controller.calibrationDevice.GetAxis();
                    //Debug.Log(axisInput+" "+ lastAxis+"  "+ (axisInput - lastAxis));

                    if (Mathf.Max(Mathf.Abs(axisInput.x), Mathf.Abs(axisInput.y)) > 0.5f)
                    {
                        //Detect tuning type
                        switch (property)
                        {
                            case TuneProperty.YAxisPosRot:
                                tType = (Mathf.Abs(axisInput.x) > Mathf.Abs(axisInput.y))
                                    ? tuningType.rotY
                                    : tuningType.posY;
                                break;
                            case TuneProperty.XZAxisPos:
                                if (Mathf.Abs(axisInput.x) > 0.7f && Mathf.Abs(axisInput.y) > 0.7f)
                                {
                                    tType = tuningType.posXZ;
                                }
                                else
                                {
                                    tType = (Mathf.Abs(axisInput.x) > Mathf.Abs(axisInput.y))
                                        ? tuningType.posX
                                        : tuningType.posZ;
                                }
                                break;
                        }

                        axisInput *= multiplyer;

                        switch (tType)
                        {
                            case tuningType.posXZ:
                                tuningGO.transform.position += new Vector3(axisInput.x / 100, 0, axisInput.y / 100);
                                break;
                            case tuningType.posX:
                                tuningGO.transform.position += new Vector3(axisInput.x / 100, 0, 0);
                                break;
                            case tuningType.posZ:
                                tuningGO.transform.position += new Vector3(0, 0, axisInput.y / 10);
                                break;
                            //--------------------------- 
                            case tuningType.rotY:
                                tuningGO.transform.rotation *= Quaternion.Euler(new Vector3(0, axisInput.x, 0));
                                break;
                            case tuningType.posY:
                                tuningGO.transform.position += new Vector3(0, axisInput.y / 100, 0);
                                break;
                        }
                    }
                }
            }
        }
    }
}
