using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class UIViveController : MonoBehaviour {

    public Hand[] calibrationHands;
    public Vector3UIOperator controllerPosition;
    public Vector3UIOperator controllerRotation;
    
    List<SteamVR_Controller.Device> calibrationDevices;
    public SteamVR_Controller.Device calibrationDevice;


    #region HandAccesors
    //-------------------------------------------------
    // Get the number of active Hands.
    //-------------------------------------------------
    public int handCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < calibrationHands.Length; i++)
                if (calibrationHands[i].gameObject.activeInHierarchy)
                    count++;
            return count;
        }
    }
    //-------------------------------------------------
    // Get the i-th active Hand.
    //
    // i - Zero-based index of the active Hand to get
    //-------------------------------------------------
    public Hand GetHand(int i)
    {
        for (int j = 0; j < calibrationHands.Length; j++)
        {
            if (!calibrationHands[j].gameObject.activeInHierarchy)
                continue;
            if (i > 0)
            {
                i--;
                continue;
            }
            return calibrationHands[j];
        }

        return null;
    }
    //-------------------------------------------------
    public Hand leftHand
    {
        get
        {
            for (int j = 0; j < calibrationHands.Length; j++)
            {
                if (!calibrationHands[j].gameObject.activeInHierarchy)
                    continue;
                if (calibrationHands[j].GuessCurrentHandType() != Hand.HandType.Left)
                    continue;
                return calibrationHands[j];
            }
            return null;
        }
    }
    //-------------------------------------------------
    public Hand rightHand
    {
        get
        {
            for (int j = 0; j < calibrationHands.Length; j++)
            {
                if (!calibrationHands[j].gameObject.activeInHierarchy)
                    continue;
                if (calibrationHands[j].GuessCurrentHandType() != Hand.HandType.Right)
                    continue;
                return calibrationHands[j];
            }
            return null;
        }
    }
    //-------------------------------------------------
    public SteamVR_Controller.Device leftController
    {
        get
        {
            Hand h = leftHand;
            if (h) return h.controller;
            return null;
        }
    }
    //-------------------------------------------------
    public SteamVR_Controller.Device rightController
    {
        get
        {
            Hand h = rightHand;
            if (h)
                return h.controller;
            return null;
        }
    }

    public Vector3 Position
    {
        get { return controllerPosition.GetValues(); }
        set { controllerPosition.SetValues(value); }
    }
    public Quaternion Rotation
    {
        get { return Quaternion.Euler(controllerRotation.GetValues()); }
        set { controllerRotation.SetValues(value.eulerAngles); }
    }
    public Vector3 RotationV3
    {
        get { return controllerRotation.GetValues(); }
        set { controllerRotation.SetValues(value); }
    }

    #endregion

    // Use this for initialization
    void Start () {
        calibrationHands = FindObjectsOfType<Valve.VR.InteractionSystem.Hand>();
        calibrationDevices = new List<SteamVR_Controller.Device>();
        calibrationDevice = null;

        for (int i=0;i<calibrationHands.Length;i++)
        {
            StartCoroutine(CollectControllers(i));
        }
    }

    IEnumerator CollectControllers(int index)
    {
        yield return new WaitWhile(() => { return calibrationHands[index].controller == null; });
        calibrationDevices.Add(calibrationHands[index].controller);
    }

    
    void SetAsCalibrationDevice(Hand h)
    {
        if (calibrationDevice!=null)
        {
            if (h.controller != calibrationDevice)
                calibrationDevice = h.controller;
        }
        else
        {
            calibrationDevice = h.controller;
        }
    }

    void OnHandsUpdate()
    {
        if (calibrationDevice == null || (calibrationDevice != null && !calibrationDevice.connected))
        {
            for (int i = 0; i < calibrationDevices.Count; i++)
            {
                if (calibrationDevices[i].connected)
                {
                    calibrationDevice = calibrationDevices[i];
                    break;
                }
            }
        }        
    }

    // Update is called once per frame
    void Update () {

        OnHandsUpdate();

        if (calibrationDevice != null)
        {
            if (controllerPosition != null)
            {
                controllerPosition.SetValues(calibrationDevice.transform.pos);
            }
            if (controllerRotation != null)
            {
                controllerRotation.SetValues(calibrationDevice.transform.rot.eulerAngles);
            }
        }

        foreach (Hand h in calibrationHands)
        {
            if (h != null && h.controller!=null)
            {
                if (h.controller.GetPressUp(EVRButtonId.k_EButton_Grip))
                {
                    SetAsCalibrationDevice(h);
                }
                

                if (h.controller.GetHairTriggerUp())
                {
                    Collider[] cols = Physics.OverlapSphere(h.hoverSphereTransform.position, h.hoverSphereRadius);
                    if (cols.Length > 0 && cols[0] != null) {
                        
                        VirtualizedCalibrationPoint point = cols[0].gameObject.GetComponentInParent<VirtualizedCalibrationPoint>();
                        if (point != null)
                        {
                            int linkIndex = CalibrationConfigurationUI.Instance.cpItemList.IndexOf(CalibrationConfigurationUI.Instance.cpItemList.Find(x => x.containerIndex == point.Index));
                            
                            if (point.Index > -1 && CalibrationConfigurationUI.Instance.cpItemList[linkIndex] != null)
                            {
                                if (h.AttachedObjects.Count > 1)
                                {
                                    h.DetachObject(point.gameObject, true);

                                    point.TryAttachToPhysicalPoint();

                                    CalibrationConfigurationUI.Instance.FillUIConfigFromTransforms(point.Index);
                                }
                                else
                                {
                                    if (point.Type == VirtualizedCalibrationPoint.CPType.virtualCP)
                                        h.AttachObject(point.gameObject, Hand.AttachmentFlags.ParentToHand);
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
