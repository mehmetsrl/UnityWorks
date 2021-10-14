using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Constants : Singleton<Constants>
{

    //Singleton


    public enum EyeMovementType
    {
        Saccade,
        Fixation
    }

    private const float _msPerHz = (float)1000 / 120;

    public  float msPerHz
    {
        get
        {
            return _msPerHz;
        }

    }


    public enum LogPrintType
    {
        gazePosition = 1,
        cameraPosition = 2,
        cameraRotation = 3,
        all = 4
    }








}
