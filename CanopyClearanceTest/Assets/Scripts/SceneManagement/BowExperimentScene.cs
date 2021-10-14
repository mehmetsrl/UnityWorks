using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using UnityEngine;
using Valve.VR;

public class BowExperimentScesne : TempScene
{
    [System.Serializable]
    public class BowPosition
    {
        public List<GameObject> bowWidths;
    }

    private ActiveDeactiveObject activeDeactiveObject;
    private SetInfo setInfo;

    public List<BowPosition> bowPositions;
    int bowPosIndex = 0, bowWidthIndex = 0;

    protected override void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded)
    {
        handlersNeeded = new List<InputManager.Handler>
        {
            InputManager.Handler.ViveDiscreateHandler
        };
    }

    protected override void Init()
    {
        activeDeactiveObject= new ActiveDeactiveObject();

        foreach (var positionTypes in bowPositions)
        {
            foreach (var bowWidth in positionTypes.bowWidths)
                activeDeactiveObject.Execute(null,new EventRegistery.TransformData(bowWidth,false));
        }


        if (bowPositions != null && bowPositions[bowPosIndex] != null && bowPositions[bowPosIndex].bowWidths != null && bowPositions[bowPosIndex].bowWidths[bowWidthIndex] != null)
            activeDeactiveObject.Execute(null, new EventRegistery.TransformData(bowPositions[bowPosIndex].bowWidths[bowWidthIndex], true));
        
    }

    protected override void Run()
    {
    }

    protected override void BindEvents()
    {
        Events.OnViveButtonUp.AddListener((data) =>
        {
            EventRegistery.ViveInputData viveData = data as EventRegistery.ViveInputData;

            if(viveData==null) return;

            if(viveData.button!= EVRButtonId.k_EButton_SteamVR_Trigger) return;

            //Deactivate current object
            if (bowPositions[bowPosIndex].bowWidths[bowWidthIndex] != null)
                activeDeactiveObject.Execute(data,
                    new EventRegistery.TransformData(bowPositions[bowPosIndex].bowWidths[bowWidthIndex].gameObject,
                        false));

            //Chance active object index
            if (viveData.device == ViveInputs.Instance.rightController)
            {
                ShowNextBowWidth();
            }
            else
            {
                ShowNextBowPosition();
            }

            //Activate current object
            if (bowPositions[bowPosIndex].bowWidths[bowWidthIndex] != null)
                activeDeactiveObject.Execute(data,
                    new EventRegistery.TransformData(bowPositions[bowPosIndex].bowWidths[bowWidthIndex].gameObject, true));
        });
        
    }

    protected override void UnBindEvents()
    {
        Events.OnViveButtonUp.RemoveAllListeners();
    }

    public void ShowNextBowWidth()
    {
        if (bowPositions[bowPosIndex] != null)
        {
            //if (bowWidthIndex < bowPositions[bowPosIndex].bowWidths.Count - 1)
            bowWidthIndex++;

            if (bowWidthIndex >= bowPositions[bowPosIndex].bowWidths.Count) bowWidthIndex = 0;

        }
    }
    public void ShowPrevBowWidth()
    {
        if (bowPositions[bowPosIndex] != null)
        {
            //if (bowWidthIndex > 0)
            bowWidthIndex--;

            if (bowWidthIndex < 0) bowWidthIndex = bowPositions[bowPosIndex].bowWidths.Count - 1;
            
        }
    }

    public void ShowNextBowPosition()
    {
        if (bowPositions[bowPosIndex] != null)
        {
            //if (bowPosIndex < bowPositions.Count - 1)
            bowPosIndex++;

            if (bowPosIndex >= bowPositions.Count) bowPosIndex = 0;
            
        }
    }
    public void ShowPrevBowPosition()
    {
        if (bowPositions[bowPosIndex] != null)
        {
            //if (bowPosIndex > 0)
            bowPosIndex--;

            if (bowPosIndex < 0) bowPosIndex = bowPositions.Count - 1;
            
        }
    }

    protected override void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded)
    {
        contiunousHandlersNeeded=new List<InputManager.ContiunousHandler>();
    }
}
