using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfInterestExperimentComponent : ExperimentComponent {

    public enum Action { rotate, activate }
    public List<AOIAction> actionList;
    public Material feedbackMaterial;
    public Color activatedColor = Color.red;
    public Color initialColor;

    public bool deactivateAOI = false;

    // Use this for initialization
    void Start()
    {
        initialColor = feedbackMaterial.color;
        //tag = "AreaOfInterest";

        OnContactBegin += () =>
        {
            DoAction(true);
        };
        OnContactEnd += () =>
        {
            DoAction(false);
        };
    }

    private void HoverAction()
    {
        //feedbackMaterial.color = Color.Lerp(initialColor, activatedColor, FocusProgress);
    }

    void OnDestroy()
    {
        feedbackMaterial.color = initialColor;
        OnContactBegin = null;
        OnContactEnd = null;
    }

    bool inRotation = false;
    bool value;
    public void DoAction(bool value)
    {
        foreach (var act in actionList)
        {
            bool actionLogic = value;

            if (act.inverseAction) actionLogic = !actionLogic;
            
        }
    }
}
