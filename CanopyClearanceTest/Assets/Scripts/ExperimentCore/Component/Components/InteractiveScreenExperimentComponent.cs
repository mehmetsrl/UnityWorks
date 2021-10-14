using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractiveScreenExperimentComponent : ExperimentComponent
{
    private List<SlotExperimentComponent> slotList;

    [HideInInspector]
    //public List<Collider> ScreenCollider;
    public Collider ScreenCollider;

    new void Awake()
    {
        base.Awake();

        slotList = GetComponentsInChildren<SlotExperimentComponent>().ToList();
        //ScreenCollider = GetComponents<Collider>().ToList();
        ScreenCollider = GetComponent<Collider>();
    }

    SlotExperimentComponent GetNearestAvailableSlot(Vector3 position, bool inCollider=true)
    {
        IEnumerable<SlotExperimentComponent> nearestSlots = slotList.OrderBy((x) =>
        {
            float distance = Vector3.Distance(x.Position, position);
            return distance;
        });

        foreach (var slot in nearestSlots)
        {
            if (inCollider?(slot.Type==SlotExperimentComponent.SlotType.Inner):(slot.Type == SlotExperimentComponent.SlotType.Outer))
                if (slot.IsAvailable)
                    return slot;
        }

        return null;
    }

    

	public SlotExperimentComponent InserItem(PopUpExperimentComponent popUpExperimentComponent)
    {
        SlotExperimentComponent availableSlot = GetNearestAvailableSlot(popUpExperimentComponent.transform.position);

        if (availableSlot != null)
        {
            availableSlot.PlaceItem(popUpExperimentComponent);
            return availableSlot;
        }
        return null;
    }
	public SlotExperimentComponent RemoveItem(PopUpExperimentComponent popUpExperimentComponent)
    {
        SlotExperimentComponent availableSlot =
            GetNearestAvailableSlot(popUpExperimentComponent.transform.position, false);

        if (availableSlot != null)
        {
            availableSlot.PlaceItem(popUpExperimentComponent);
            return availableSlot;
        }
        return null;
    }


    public bool IsIn(Vector3 point)
    {
        //foreach (var col in ScreenCollider)
        //{
        //    if (col.bounds.Contains(point))
        //        return true;
        //}
        //return false;
        return ScreenCollider.bounds.Contains(point);
    }

}
