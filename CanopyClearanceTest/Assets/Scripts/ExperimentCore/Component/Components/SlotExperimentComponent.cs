using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotExperimentComponent : ExperimentComponent
{
    private static List<SlotExperimentComponent> allSlotComponents;

    public enum SlotType
    {
        Inner,
        Outer
    }

    public SlotType Type = SlotType.Inner;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    private PopUpExperimentComponent placedItem = null;

    public bool IsAvailable
    {
        get { return placedItem == null; }
    }
    
    public void PlaceItem(PopUpExperimentComponent popUpExperimentComponent)
    {
        placedItem = popUpExperimentComponent;
        Transform initialPopupParent= placedItem.transform.parent;
        placedItem.transform.parent = transform;
        placedItem.transform.localPosition=Vector3.zero;
        placedItem.transform.localRotation=Quaternion.identity;

        placedItem.transform.parent = initialPopupParent;
    }

    public void RemoveItem()
    {
        if (!IsAvailable)
        {
            placedItem = null;
        }
    }

}
