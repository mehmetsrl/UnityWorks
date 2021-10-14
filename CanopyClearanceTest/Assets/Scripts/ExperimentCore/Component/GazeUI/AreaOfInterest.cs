using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AOIAction
{
    public GameObject affectedObject;
    public AreaOfInterest.Action action;
    public bool inverseAction = false;
    public Quaternion targetRotation;
    public Quaternion initialRotation;

    public AOIAction()
    {
        if (affectedObject != null)
            initialRotation = affectedObject.transform.rotation;
    }
}

public class AreaOfInterest : BaseGazeObject {

    public enum Action { rotate,activate}
    public List<AOIAction> actionList;
    public Material feedbackMaterial;
    public Color activatedColor=Color.red;
    public Color initialColor;

    public bool deactivateAOI=false;
    
    // Use this for initialization
    void Start ()
    {

        initialColor = feedbackMaterial.color;
        //tag = "AreaOfInterest";

        OnGazeFocusBegin += () =>
        {
            DoAction(true);
        };
        OnGazeFocusEnd += () =>
        {
            DoAction(false);
        };
        OnGazeHover += HoverAction;
    }

    private void HoverAction(RaycastHit hit)
    {
        feedbackMaterial.color = Color.Lerp(initialColor, activatedColor, FocusProgress);
    }

    new void OnDestroy()
    {
        base.OnDestroy();
        feedbackMaterial.color = initialColor;
        OnGazeFocusBegin = null;
        OnGazeFocusEnd = null;
        OnGazeHover = null;
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();
        //if (deactivateAOI)
        //{
        //    deactivateAOI = false;
        //    DoAction(false);
        //}
	}
    bool inRotation = false;
    bool value;
    public void DoAction(bool value)
    {
        foreach (var act in actionList)
        {
            bool actionLogic = value;

            if (act.inverseAction) actionLogic = !actionLogic;

            switch (act.action)
            {
                case Action.activate:
                    act.affectedObject.SetActive(actionLogic);
                    if (actionLogic)
                        feedbackMaterial.color = activatedColor;
                    else
                        feedbackMaterial.color = initialColor;


                    break;
                case Action.rotate:
                    inRotation = true;
                    if (actionLogic)
                    {
                        act.affectedObject.transform.rotation = Quaternion.Lerp(act.affectedObject.transform.rotation, act.targetRotation, 0.1f);
                    }
                    else
                    {
                        act.affectedObject.transform.rotation = Quaternion.Lerp(act.affectedObject.transform.rotation, act.initialRotation, 0.1f);
                    }
                    break;
            }
        }
    }
}
