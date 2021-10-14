using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Composit pattern inplementation
/// </summary>
/// 
public abstract class UIComponent : UIComponentBase
{
    public BaseLeapObject thisLeapObject;
    
    public sealed override int GetItemIndex(UIComponent component)
    {
        return childrens.IndexOf(component);
    }

    public sealed override void PlaceItem(UIComponent component, Vector3 relativePosition)
    {
    }

    public sealed override void PlaceItem(UIComponent component, Vector2 relativePosition)
    {
    }

    public sealed override void ActivateItem(bool isActive)
    {
        foreach (var clild in childrens)
            clild.ActivateItem(isActive);

        HandleActivation(isActive);
    }

    protected override void HandleActivation(bool isActive)
    {
        //if (thisLeapObject != null)
        //    thisLeapObject.SetState(isActive);
    }


    public sealed override void SetVisibility(bool visible)
    {
        HandleVisibility(visible);
    }

    protected override void HandleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }

    // Use this for initialization
    void Awake()
    {
        thisLeapObject = GetComponent<BaseLeapObject>();
        if (headObject == null)
            headObject = gameObject;
    }
}