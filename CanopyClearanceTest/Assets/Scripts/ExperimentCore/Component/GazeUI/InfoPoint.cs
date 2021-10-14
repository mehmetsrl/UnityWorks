using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Info
{
    public string label;
    public int labelFontSize;

    public string info;
    public int infoFontSize;
}

public class InfoPoint : BaseGazeObject
{
    [SerializeField]
    int id;
    public int ID
    {
        get { return id; }
    }

    public Vector3 centerOffset=Vector3.zero;

    public enum ContactType
    {
        NoAction,
        SendOtherUnit,
        Engage
    }

    public ContactType contactType = ContactType.NoAction;

    public enum EntityType
    {
        AirVehicleFigter,
        GroundVehicle,
        SeaVehicle
    }

    public EntityType entityType = EntityType.AirVehicleFigter;

    public enum ObjectType
    {
        Air,
        Surface
    }

    [SerializeField]
    private ObjectType _type;
    public string TargetType { get {
        switch (_type)
        {
                case ObjectType.Air: return "AA";
                case ObjectType.Surface: return "AS";
        }
        return "";
    } }

    public enum Faction
    {
        Ally,
        Enemy,
        Unknown
    }
    public Faction faction;

    [SerializeField] private List<Info> _infoList=new List<Info>(4);

    public  List<Info> InfoList
    {
        get { return _infoList; }
    }

    [SerializeField]
    string _infoName;
    public string InfoName
    {
        get { return _infoName; }
    }

    [SerializeField]
    [TextArea]
    string _infoText;
    public string InfoText
    {
        get { return _infoText; }
    }

    [SerializeField]
    Texture2D _infoImage;
    public Texture2D InfoImage
    {
        get { return _infoImage; }
    }
    
    private ExperimentComponent bindedComponent=null;
    public ExperimentComponent BindedComponent
    {
        get { return bindedComponent; }
    }

    public void BindInfoPanel(ExperimentComponent panelComponent)
    {
        bindedComponent = panelComponent;
    }

    new void Update()
    {
        base.Update();

        if (bindedComponent != null)
        {
            bindedComponent.transform.position = transform.position;
        }
    }
}
