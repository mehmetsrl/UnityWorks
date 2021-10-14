using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIComponentBase : MonoBehaviour {
    
    public GameObject headObject;
    public List<UIComponent> childrens;

    public abstract int GetItemIndex(UIComponent component);
    public abstract void PlaceItem(UIComponent component, Vector3 relativePosition);
    public abstract void PlaceItem(UIComponent component, Vector2 relativePosition);

    public abstract void ActivateItem(bool isActive);
    protected abstract void HandleActivation(bool isActive);

    public abstract void SetVisibility(bool visible);
    protected abstract void HandleVisibility(bool visible);
}
