using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIListItem : MonoBehaviour {
    public int containerIndex;
    public Button button;
    public Text itemNameTextObj;
    public string itemName;
    public Vector3 phyPosition;
    public Vector3 phyRotation;
    public Vector3 virPosition;
    public Vector3 virRotation;

    public void Select()
    {
        if (button != null)
            button.onClick.Invoke();
    }
}
