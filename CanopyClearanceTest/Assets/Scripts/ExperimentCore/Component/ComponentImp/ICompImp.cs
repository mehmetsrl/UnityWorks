using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICompImp
{
    void Initiate(IComponent linkedComponent);
    void Initiate(IComponent linkedComponent,string tag);
    IComponent GetLinkedComponent();
    void SetState(ComponentBase.State compState);

    void NotifyButtonPress();
    void NotifyButtonUnPress();

    bool IsValidAction();

    UnityEngine.Component GetComponent();

    Vector3 GetClosestContactPoint();
    Vector3 GetContactItemPosition();

    GameObject GetContactItem();

    void IgnoreGrasping(bool ignore);
    bool IgnoreGrasping();

    UIManager GetManager();
}