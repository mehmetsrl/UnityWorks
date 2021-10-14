using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Valve.VR.InteractionSystem;

public class CalibrationManger : MonoBehaviour {

    static CalibrationManger instance = null;

    #region Accesors
    public static CalibrationManger Instance
    {
        get { return instance; }
        private set
        {
            if (instance == null)
                instance = value;
        }
    }

    public int ActiveCpIndex
    {
        get { return activeCPIndex; }
    }

    public bool CalibrationConfiguratorWindowOpen
    {
        get { return calibrationConfiguratorWindowOpen; }
    }

    #endregion

    #region Properties
    public Configs config;
    bool calibrationConfiguratorWindowOpen = false;
    private int activeCPIndex = -1;
    #endregion


    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };

    void Awake () {
        Instance = this;
        config.Init();
    }

	private void Start()
	{
        
        config.FillConfigs();
        if (ConfigHandler.Instance != null)
            ConfigHandler.Instance.Initiate(ref config);

        if (CalibrationTransporter.Instance != null)
            CalibrationTransporter.Instance.Initiate(ref config);

        if (CalibrationConfigurationUI.Instance != null)
            CalibrationConfigurationUI.Instance.Initiate(ref config);
        
        if (ConfigHandler.Instance.IsThereAnyCPInScene)
        {
            activeCPIndex = 0;
            Teleport(ConfigHandler.Instance.cpIndexisInScene[activeCPIndex]);
            ShowConfigurator(false);
        }
        else
        {
            ShowConfigurator(true);
        }
    }

    public void Update()
    {
        if (CalibrationTransporter.Instance != null)
        {
            if ((Input.GetKey(KeyCode.T)))
            {
                for (int numberPressed = 0; numberPressed < keyCodes.Length; numberPressed++)
                {
                    if (Input.GetKeyDown(keyCodes[numberPressed]))
                    {
                        Debug.Log(numberPressed);
                        CalibrationTransporter.Instance.Calibrate(numberPressed);
                    }
                }
            }
        }

        ////Open calibration configuration window
        //if ((Input.GetKey(KeyCode.LeftControl)|| Input.GetKey(KeyCode.RightControl))&& Input.GetKeyUp(KeyCode.C))
        //{
        //    ShowConfigurator(!calibrationConfiguratorWindowOpen);
        //}
    }


	public void Teleport(int pointIndex){
        if (CalibrationTransporter.Instance != null)
            CalibrationTransporter.Instance.Calibrate(pointIndex);
    }

    public void ShowConfigurator(bool show=true)
    {
        calibrationConfiguratorWindowOpen = show;
        if (CalibrationConfigurationUI.Instance != null)
            CalibrationConfigurationUI.Instance.Show(show);
        if (ConfigHandler.Instance != null)
        {
            ConfigHandler.Instance.configMode = show ? ConfigHandler.Mode.calibrationMode : ConfigHandler.Mode.playMode;
        }

        ConfigHandler.Instance.UpdateCPTransforms();
    }
}
