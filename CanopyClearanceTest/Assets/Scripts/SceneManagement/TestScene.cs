using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using UnityEngine;

public class TestScene : TempScene {
    protected override void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded)
    {
        handlersNeeded = new List<InputManager.Handler>();
        handlersNeeded.Add(InputManager.Handler.ViveDiscreateHandler);
        handlersNeeded.Add(InputManager.Handler.LeapComponentHandler);
        handlersNeeded.Add(InputManager.Handler.LeapGesturetHandler);
    }

    private TestAction testAction;
    protected override void Init()
    {
        testAction=new TestAction();
    }

    protected override void Run()
    {
    }

    protected override void BindEvents()
    {
        Events.OnViveButtonUp.AddListener((data) =>
        {
            testAction.Execute(data, null);
        });
        Events.OnLeapComponentTriggered.AddListener((data) =>
        {
            EventRegistery.LeapInputData leapData = data as EventRegistery.LeapInputData;
            if (leapData != null)
            {
                Debug.Log(leapData.actionType+" "+leapData.@object.name);
            }
        });

        Events.OnLeapGesture.AddListener((data) =>
        {
            EventRegistery.LeapInputData leapData = data as EventRegistery.LeapInputData;
            //if (leapData != null)
            //{
            //    Debug.Log(leapData.actionType);
            //}
        });
    }

    protected override void UnBindEvents()
    {
        Events.OnViveButtonUp.RemoveAllListeners();
        Events.OnLeapComponentTriggered.RemoveAllListeners();
    }
    protected override void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded)
    {
        contiunousHandlersNeeded = new List<InputManager.ContiunousHandler>();
    }
}
