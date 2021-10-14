using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// CalibrationTransporter class is responsible for transport a rigged player to a spesific virtual point.
/// It links virtual and physical points and manages synchronization.
/// </summary>
public class CalibrationTransporter : MonoBehaviour
{

    static CalibrationTransporter instance = null;
    static Transform initialParent = null;

    #region Accesors
    public static CalibrationTransporter Instance
    {
        get{return instance;}
        private set
        {
            if (instance == null)
                instance = value;
        }
    }

    public static bool Initiated{get{return Instance;}}
    #endregion

    #region Properties
    Configs configs;
    Transform cameraRigTransform;
    
    #endregion

    private void Awake()
    {
        Instance = this;
        initialParent = transform.parent;
        ResetTransporter();
    }

    public void Initiate(ref Configs configs)
    {
        this.configs = configs;
        cameraRigTransform = this.configs.cameraRig.transform;
    }
    
    #region CalibrationMagic
    void ResetTransporter()
    {
        transform.parent = initialParent;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void Calibrate(int taretCalibrationIndex)
    {
        VirtualizedCalibrationPoint virPoint = ConfigHandler.Instance.virPointVirtualizations.Find(x => x.Index == taretCalibrationIndex);
        VirtualizedCalibrationPoint phyPoint = ConfigHandler.Instance.phyPointVirtualizations.Find(x => x.Index == taretCalibrationIndex);

        if (phyPoint != null && virPoint != null )
        {

            ConfigHandler.Instance.AdobtAllPhysicalPoints();
            #region ActualTransportationProcedure
            Transform rigParentBackup = cameraRigTransform.parent;

            transform.position = phyPoint.transform.position;
            transform.rotation = phyPoint.transform.rotation;

            cameraRigTransform.parent = transform;

            transform.parent = virPoint.transform;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            cameraRigTransform.parent = rigParentBackup;

            ResetTransporter();
            #endregion
            ConfigHandler.Instance.AbandonAllPhysicalPoints();
        }
    }
    #endregion
}

