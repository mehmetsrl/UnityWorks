using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vector3UIOperator : MonoBehaviour {
    public enum textType { label, inputField}
    public textType type;
    public RectTransform xVal, yVal, zVal;
    public float x
    {
        get
        {
            float val;
            switch (type)
            {
                case textType.label:
                    float.TryParse(xVal.GetComponentInChildren<Text>().text, out val);
                    break;
                case textType.inputField:
                    float.TryParse(xVal.GetComponentInChildren<InputField>().text, out val);
                    break;
                default:
                    val = 0;
                    break;
            }
            return val;
        }
    }

    public float y
    {
        get
        {
            float val;
            switch (type)
            {
                case textType.label:
                    float.TryParse(yVal.GetComponentInChildren<Text>().text, out val);
                    break;
                case textType.inputField:
                    float.TryParse(yVal.GetComponentInChildren<InputField>().text, out val);
                    break;
                default:
                    val = 0;
                    break;
            }
            return val;
        }
    }

    public float z
    {
        get
        {
            float val;
            switch (type)
            {
                case textType.label:
                    float.TryParse(zVal.GetComponentInChildren<Text>().text, out val);
                    break;
                case textType.inputField:
                    float.TryParse(zVal.GetComponentInChildren<InputField>().text, out val);
                    break;
                default:
                    val = 0;
                    break;
            }
            return val;
        }
    }


    public bool isActive = true;
    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value;  }
    }

    void UpdateView()
    {
        switch (type)
        {
            case textType.label:
                if (!isActive)
                {
                    xVal.GetComponentInChildren<Text>().text = "";
                    yVal.GetComponentInChildren<Text>().text = "";
                    zVal.GetComponentInChildren<Text>().text = "";
                }
                break;
            case textType.inputField:
                xVal.GetComponentInChildren<InputField>().interactable = isActive;
                yVal.GetComponentInChildren<InputField>().interactable = isActive;
                zVal.GetComponentInChildren<InputField>().interactable = isActive;

                if (!isActive)
                {
                    xVal.GetComponentInChildren<InputField>().text = "";
                    yVal.GetComponentInChildren<InputField>().text = "";
                    zVal.GetComponentInChildren<InputField>().text = "";
                }
                break;
        }
    }

    public bool SetValues(Vector3 values)
    {
        if (!isActive)
            return false;

        switch (type)
        {
            case textType.label:
                xVal.GetComponentInChildren<Text>().text = values.x.ToString();
                yVal.GetComponentInChildren<Text>().text = values.y.ToString();
                zVal.GetComponentInChildren<Text>().text = values.z.ToString();
                break;
            case textType.inputField:
                xVal.GetComponentInChildren<InputField>().text = values.x.ToString();
                yVal.GetComponentInChildren<InputField>().text = values.y.ToString();
                zVal.GetComponentInChildren<InputField>().text = values.z.ToString();
                break;
        }
        return true;
    }


    public bool SetValues(Vector3UIOperator other)
    {
        if (!isActive)
            return false;

        Vector3 values;
        other.GetValues(out values);
        return SetValues(values);
    }

    public Vector3 GetValues()
    {
        Vector3 result;
        GetValues(out result);
        return result;
    }

    public bool GetValues(out Vector3 values)
    {
        if(!isActive)
        {
            values = Vector3.zero;
            return false;
        }

        float newX, newY, newZ;

        switch (type)
        {
            case textType.label:
                float.TryParse(xVal.GetComponentInChildren<Text>().text,out newX);
                float.TryParse(yVal.GetComponentInChildren<Text>().text, out newY);
                float.TryParse(zVal.GetComponentInChildren<Text>().text, out newZ);
                break;
            case textType.inputField:
                float.TryParse(xVal.GetComponentInChildren<InputField>().text, out newX);
                float.TryParse(yVal.GetComponentInChildren<InputField>().text, out newY);
                float.TryParse(zVal.GetComponentInChildren<InputField>().text, out newZ);
                break;
            default:
                newX = 0; newY = 0; newZ = 0;
                break;
        }
        values = new Vector3(newX, newY, newZ);
        return true;
    }
}
