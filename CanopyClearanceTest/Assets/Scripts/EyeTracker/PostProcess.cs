using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class PostProcess : MonoBehaviour
{

    private LogFileOperation logFileOperation = null; //new LogFileOperation();
    private string result;
    private bool _jsonWriteResult = false;


    private KeyValuePair<string, T> getKeyValuePair<T>(KeyValuePair<string, dynamic> pair)
    {
        return new KeyValuePair<string, T>(pair.Key, (T)pair.Value);
    }

    // Use this for initialization
    void Start()
    {
        logFileOperation =  new GameObject("logFileOperationGO").AddComponent<LogFileOperation>();
        logFileOperation.Init();

        prepearJson();


    }

    // Update is called once per frame
    void Update()
    {
    }



    private void prepearJson()
    {

        string json;

        JsonOperation.init();

        json = JsonOperation.LoadJson(TransferData.Instance.logFileOperationPath);
        json = JsonOperation.fixArrayJson(json, "logs");

        result = JsonOperation.jsonPrintNotations[JsonOperation.JsonNotations.Start];

        foreach (var item in TransferData.Instance.usageForAdditionalJsonObjectsWhileFixing)
        {
            if (item.Value is string)
            {
                KeyValuePair<string, string> temp = getKeyValuePair<string>(item);
                result += JsonOperation.jsonObjectWithoutCurly<string>(temp.Key, temp.Value);

            }
            else if (item.Value is int)
            {
                KeyValuePair<string, int> temp = getKeyValuePair<int>(item);
                result += JsonOperation.jsonObjectWithoutCurly<int>(temp.Key, temp.Value);

            }
            else if (item.Value is double)
            {
                KeyValuePair<string, double> temp = getKeyValuePair<double>(item);
                result += JsonOperation.jsonObjectWithoutCurly<double>(temp.Key, temp.Value);

            }
            else if (item.Value is bool)
            {
                KeyValuePair<string, bool> temp = getKeyValuePair<bool>(item);
                result += JsonOperation.jsonObjectWithoutCurly<bool>(temp.Key, temp.Value);

            }

        }

        result += json;
        result += JsonOperation.jsonPrintNotations[JsonOperation.JsonNotations.End];


        StartCoroutine(WaitForCallback());
        //logFileOperation.writeJson(result, true, "gaze.json");
        JsonOperation.clear(TransferData.Instance.logFileOperationPath);

    }

    IEnumerator WaitForCallback()
    {
        _jsonWriteResult = false;
        logFileOperation.writeJson(result, ref _jsonWriteResult, true, "gaze.json");
        yield return new WaitUntil(()=>_jsonWriteResult);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

}
