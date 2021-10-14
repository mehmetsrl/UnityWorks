using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonOperation
{
    public static string logFileLocation;
    public static Dictionary<JsonNotations, string> jsonPrintNotations = new Dictionary<JsonNotations, string>();

    public static void init()
    {
        arrangeJsonPrintNotations();
        
    }

    public static string LoadJson(string filelocation)
    {
        using (StreamReader r = new StreamReader(filelocation))
        {
            
            return r.ReadToEnd();

        }
    }

 

    public static void writeJson(string jsonFileName, string json, bool isJsonFixedMode = false)
    {
        using (FileStream f = new FileStream(logFileLocation + "/" + jsonFileName, isJsonFixedMode == false ? FileMode.Append : FileMode.Create))
        {
            using (StreamWriter w = new StreamWriter(f))
            {
                if (!isJsonFixedMode)
                    w.Write(json + "," + Environment.NewLine);
                else
                    w.Write(json);
            }
        }
    }

    public static string fixArrayJson(string value, string arrayName)
    {

        return "\"" + arrayName + "\":[" + value + "]";
    }


    public enum JsonNotations
    {
        Start, End
    }


    private static void arrangeJsonPrintNotations()
    {
        jsonPrintNotations.Add(JsonNotations.Start, "{" + Environment.NewLine);
        jsonPrintNotations.Add(JsonNotations.End, Environment.NewLine + "}");
    }

    public static string jsonObjectWithoutCurly<T>(string objectName, T value)
    {
        string text = "\"" + objectName + "\":";
        if (typeof(T) == typeof(string))
            text += "\"" + value + "\"";
        else
            text += value;

        text += "," + Environment.NewLine;

        return text;
    }

    public static bool clear(string fileLocation)
    {
        if (File.Exists(fileLocation))
        {
            File.Delete(fileLocation);
            return true;
        }

        return false;
    }








}


