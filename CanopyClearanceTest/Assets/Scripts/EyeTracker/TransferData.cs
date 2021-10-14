using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EyeTracker;
using UnityEngine;

public class TransferData : Singleton<TransferData>
{
    private string _logFileOperationPath;


    public List<KeyValuePair<string, dynamic>> usageForAdditionalJsonObjectsWhileFixing;
    public string logFileOperationPath
    {
        get { return _logFileOperationPath; }
        set { _logFileOperationPath = value; }
    }

    private TransferData()
    {
        usageForAdditionalJsonObjectsWhileFixing = new List<KeyValuePair<string, dynamic>>();
        _logFileOperationPath = null;
        
    }
}
