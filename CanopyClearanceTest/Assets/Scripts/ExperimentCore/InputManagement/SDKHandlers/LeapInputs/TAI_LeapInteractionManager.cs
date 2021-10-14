using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class TAI_LeapInteractionManager : InteractionManager
{
    private static TAI_LeapInteractionManager instance;

    public static TAI_LeapInteractionManager Instance
    {
        get { return instance; }
        private set
        {
            if (instance != null) Destroy(value.gameObject);
            else
                instance = value;
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
