using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class VirtualizedCalibrationPoint : MonoBehaviour
{
    public enum CPType{
        [Description("virCP")]
        virtualCP,
        [Description("phyCP")]
        physicalCp
    }

    #region Properties
    //SomeProperties
    public CPType Type {
        get { return type; }
        set { type = value; if(type== CPType.physicalCp) RigRelativePosition = transform.position - CalibrationManger.Instance.config.cameraRig.transform.position; }
    }

    [SerializeField]
    CPType type;
    [HideInInspector]
    public Vector3 RigRelativePosition = Vector3.zero;

    public TextMesh text;
    public Collider OutCollider;
    public GameObject origin;
    public int Index;
    #endregion
    

    private void Update()
    {
        if (ConfigHandler.Instance.configMode==ConfigHandler.Mode.calibrationMode && RigRelativePosition!=Vector3.zero && Type == CPType.physicalCp)
        {
            //Debug.Log(transform);
            //Debug.Log(CalibrationManger.Instance);
            //Debug.Log(CalibrationManger.Instance.config);
            //Debug.Log(CalibrationManger.Instance.config.cameraRig);
            //Debug.Log(CalibrationManger.Instance.config.cameraRig.transform);
            //Debug.Log(RigRelativePosition);

            transform.position = CalibrationManger.Instance.config.cameraRig.transform.position + RigRelativePosition;
        }
    }

    internal void TryAttachToPhysicalPoint()
    {
        if(Type == CPType.virtualCP)
        {
            Collider[] results = Physics.OverlapSphere(origin.transform.position, origin.transform.lossyScale.x);
            foreach (Collider col in results)
            {
                VirtualizedCalibrationPoint otherCP = col.GetComponentInParent<VirtualizedCalibrationPoint>();
                if (otherCP != null && otherCP.Type==CPType.physicalCp)
                {
                    transform.position = otherCP.transform.position;
                    transform.rotation = otherCP.transform.rotation;
                }
            }
        }
    }


}
