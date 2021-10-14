using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExperimentCore.IO
{
    public class Record<T>
    {
        private string recordFile;
        private bool isStreamRecord;
        private ExperimentCore.IO.ConfigDataIOHandler<T> recordHandler;
        private ExperimentCore.IO.ConfigDataListIOHandler<T> recordListHandler;
        private T refData;
        private List<T> refDataList;

        public IOHandler<T> RecordHandler
        {
            get { return isStreamRecord ? (IOHandler<T>) recordListHandler : recordHandler; }
        }

        public Record(string recordFile, bool isStreamRecord = false)
        {
            this.recordFile = recordFile;
            this.isStreamRecord = isStreamRecord;
            if (isStreamRecord)
                recordListHandler = IOManager.Instance.CreateListHandler<T>(recordFile);
            else
                recordHandler = IOManager.Instance.CreateHandler<T>(recordFile);
        }

        /// <summary>
        /// Set link with container and save item
        /// </summary>
        /// <param name="itemToSave">Data item to be linked</param>
        public void Save(ref T itemToSave)
        {
            BindObject(ref itemToSave);
            Save();
        }

        /// <summary>
        /// Set link with container and save item
        /// </summary>
        /// <param name="itemToSave">DataList item to be linked</param>
        public void Save(ref List<T> itemToSave)
        {
            BindObject(ref itemToSave);
            Save();
        }

        /// <summary>
        /// Save initially linked object
        /// </summary>
        public void Save()
        {
            if (isStreamRecord)
            {
                recordListHandler.SaveConfigData(ref refDataList);
            }
            else
            {
                recordHandler.SaveConfigData(ref refData);
            }
        }

        public bool GetData(ref T refData)
        {
            if (isStreamRecord) return false;
            this.refData = refData;
            T data;
            if (recordHandler.GetConfigs(out data))
            {
                refData = data;
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool GetData(ref List<T> refDataList)
        {
            if (!isStreamRecord) return false;
            this.refDataList = refDataList;
            List<T> dataList;
            if (recordListHandler.GetConfigs(out dataList))
            {
                refDataList = dataList;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BindObject(ref T refData)
        {
            this.refData = refData;
        }

        public void BindObject(ref List<T> refDataList)
        {
            this.refDataList = refDataList;
        }

        public void UpdateFilePath(string path)
        {
            RecordHandler.SetFilePath(path);
        }
    }
}