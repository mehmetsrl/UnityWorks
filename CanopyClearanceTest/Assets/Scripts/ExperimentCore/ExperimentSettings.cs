using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentCore.IO;

namespace ExperimentCore
{
    [CreateAssetMenu (fileName = "ExperimentSettings")]
    public class Settings : ScriptableObject
    {

        private static Settings _instance;
        [SerializeField] string experimentFolderName = "ExperimentResluts";
        [SerializeField] string ExperimentInfoFileName = "ExperimentInfo";
        [SerializeField] IOManager.FileExtention extention = IOManager.FileExtention.XML;


        #region Accesors

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<Settings>("ExperimentSettings");
                return _instance;
            }
        }

        public string ExperimentFolder
        {
            get { return IOManager.StreamingAssetsFolder + "/" + experimentFolderName; }
        }

        public string ExperimentFile
        {
            get { return ExperimentFolder + "/" + ExperimentInfoFileName + ExperimentUtils.GetEnumDescription(extention); }
        }

        private Record<ExperimentInfo> experimentRecord;
        [HideInInspector]
        public ExperimentInfo ExperimentInfo;
        public ExperimentInfo DefaultExperimentInfo;

        #endregion


        void Awake()
        {
            
            experimentRecord = new Record<ExperimentInfo>(ExperimentFile);

            experimentRecord.GetData(ref ExperimentInfo);
            if (ExperimentInfo == null)
            {
                ExperimentInfo = new ExperimentInfo(DefaultExperimentInfo.Title, DefaultExperimentInfo.Manifest,
                    DefaultExperimentInfo.NumberOfSubjects);
                experimentRecord.Save();
            }

        }

        public string SubjectFolderPath(string recordFolderName)
        {
            return ExperimentFolder + "/" + recordFolderName;
        }
        public string RecordFilePath(string recordFolderName, string recordFileName)
        {
            return ExperimentFolder + "/" + recordFolderName + "/" + recordFileName + ExperimentUtils.GetEnumDescription(extention);
        }
        public string RecordItemFolderPath(string recordFolderName, string recordItemFolderName)
        {
            return ExperimentFolder + "/" + recordFolderName + "/" + recordItemFolderName;
        }
        public string RecordItemFilePath(string recordFolderName, string recordItemFolderName, string recordItemName)
        {
            return ExperimentFolder + "/" + recordFolderName + "/" + recordItemFolderName + "/" + recordItemName +
                   ExperimentUtils.GetEnumDescription(extention);
        }


        public void SaveExperiment()
        {
            ExperimentInfo.NumberOfSubjects++;
            experimentRecord.Save();
        }


    }
}