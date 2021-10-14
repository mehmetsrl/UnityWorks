using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ExperimentCore.IO
{
    public class IOManager
    {

        public enum FileExtention
        {
            [Description(".xml")] XML,
            [Description(".json")] JSON,
            [Description(".txt")] TXT
        }

        private static IOManager _instance;
        FileExtention extention = FileExtention.XML;
        

        #region Accesors

        public static IOManager Instance
        {
            get { return IOManager._instance == null ? new IOManager() : IOManager._instance; }
        }

        public static string StreamingAssetsFolder { get; private set; }

        #endregion
        
        protected IOManager()
        {
            StreamingAssetsFolder = Application.streamingAssetsPath;
        }


        public ConfigDataIOHandler<T> CreateHandler<T>(string fullFileName)
        {
            return new ConfigDataXMLSerializer<T>(fullFileName);
        }

        public ConfigDataListIOHandler<T> CreateListHandler<T>(string fullFileName)
        {
            return new ConfigDataListXMLSerializer<T>(fullFileName);
        }

    }
}