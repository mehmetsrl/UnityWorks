using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using ExperimentCore;
using UnityEngine;
using UnityEngine.Serialization;

public class MainScene : TempScene
{
    private static MainScene _instance;
    public static MainScene Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance == null)
            {
                _instance = value;
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }

    [SerializeField]
    [FormerlySerializedAs("ExperimentSettings")]
    Settings settings;

    protected override void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded)
    {
        handlersNeeded=new List<InputManager.Handler>();
        handlersNeeded.Add(InputManager.Handler.ShortcutHandler);
        handlersNeeded.Add(InputManager.Handler.JoystickHandler);
    }
    protected override void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded)
    {
        contiunousHandlersNeeded = new List<InputManager.ContiunousHandler>();
        contiunousHandlersNeeded.Add(InputManager.ContiunousHandler.JoystickAxisHandler);
    }

    private OpenCalibrationUI openCloseUI;
    protected override void Init()
    {
        Instance = this;
        Settings = settings;
        openCloseUI = new OpenCalibrationUI();
    }

    protected override void Run()
    {
    }

    protected override void BindEvents()
    {
        Events.CTRL_CombinationEvent.AddListener((data) =>
        {
            EventRegistery.KeyboardInputData keyboardInputData = data as EventRegistery.KeyboardInputData;
            if (keyboardInputData.keys.Count == 1 && keyboardInputData.keys.Contains(KeyCode.C))
                openCloseUI.Execute(data, null);
        });


        //Events.OnJoystickAxisChange.AddListener((data) =>
        //{
        //    foreach (var val in data.vals)
        //    {
        //        Debug.Log("val: " + val);

        //    }
        //});
        //Events.OnJoystickDown.AddListener((data) =>
        //{
        //    foreach (var key in data.keys)
        //    {
        //        Debug.Log("key: " + key);

        //    }
        //});
    }

    protected override void UnBindEvents()
    {
        Events.CTRL_CombinationEvent.RemoveAllListeners();
    }
    
}
