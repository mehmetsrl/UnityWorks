using System;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.EyeTracker
{


    public class EyeTrackerPviManagement:MonoBehaviour
    {

        public static EyeTrackerPviManagement Instance
        {
            get { return _instance; }
            private set
            {
                if (_instance == null) _instance = value;
                
            }
        }

        private static EyeTrackerPviManagement _instance;

        // Variables
        [HideInInspector]
        public string EyeTrackerSoftwarePath;
        [HideInInspector]
        public string VR_StartingConfigBatchFile;
        [HideInInspector]
        public string VR_StoppingConfigBatchFile;

        
        private string _VR_StartingConfigPath;
        private string _VR_StoppingConfigPath;

        private void Awake()
        {
            Instance = this;

            if (Environment.GetEnvironmentVariable("EYE_TRACKER_SOFTWARE_PATH") == null)
                EyeTrackerSoftwarePath = @"D:\PupilLab\Software\capture";

             VR_StartingConfigBatchFile = @"startingConfig.bat";
            VR_StoppingConfigBatchFile = @"stoppingConfig.bat";

            Init();
        }

        private void Init()
        {

            try
            {
                // setting Env.
                if (VR_StartingConfigBatchFile != null)
                    Environment.SetEnvironmentVariable("VR_STARTING_CONFIG_PATH", Application.streamingAssetsPath + @"/Resources/" + VR_StartingConfigBatchFile);
                if (VR_StoppingConfigBatchFile != null)
                    Environment.SetEnvironmentVariable("VR_STOPPING_CONFIG_PATH", Application.streamingAssetsPath + @"/Resources/" + VR_StoppingConfigBatchFile);
                if (EyeTrackerSoftwarePath != null)
                    Environment.SetEnvironmentVariable("EYE_TRACKER_SOFTWARE_PATH", EyeTrackerSoftwarePath);


                //getting Env.
                _VR_StartingConfigPath = _VR_StartingConfigPath ?? Environment.GetEnvironmentVariable("VR_STARTING_CONFIG_PATH");
                _VR_StoppingConfigPath = _VR_StoppingConfigPath ?? Environment.GetEnvironmentVariable("VR_STOPPING_CONFIG_PATH");

            }
            catch (Exception e)
            {
                throw e;
            }

        }





        /// <summary>
        /// It is to prepare to be ready for PVI and EyeTracking API Applications.
        /// </summary>
        public void ConfigStartProcess()
        {
            try
            {


                UnityEngine.Debug.Log("path:" + _VR_StartingConfigPath);

                //process
                Process ConfigStartProcess = new Process();
                ConfigStartProcess.StartInfo.FileName = _VR_StartingConfigPath;
                ConfigStartProcess.Start();

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
                throw;
            }
        }


        public void ConfigStopProcess()
        {
            try
            {
                Process ConfigStopProcess = new Process();
                ConfigStopProcess.StartInfo.FileName = _VR_StoppingConfigPath;
                ConfigStopProcess.Start();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
                throw;
            }
        }


        private void OnDestroy()
        {
            ConfigStopProcess();
        }
    }


}
