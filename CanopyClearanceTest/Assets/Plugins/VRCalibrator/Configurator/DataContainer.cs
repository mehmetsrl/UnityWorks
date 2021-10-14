using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

#region Containers
#region SerializableXMLContainers
[System.Serializable]
[XmlType(TypeName = "PhysicalCalibrationPoint")]
public class PhysicalCP_Container
{
    [XmlAttribute(AttributeName = "Name")]
    public string name;
    [XmlAttribute(AttributeName = "Index")]
    public int index;

    [XmlAttribute(AttributeName = "LinkedScene")]
    public string linkedScene;

    [XmlElement("Position")]
    public Vector3 position;
    [XmlElement("Rotation")]
    public Vector3 rotation;

    public PhysicalCP_Container(string name, int index, string linkedScene, Vector3 position, Vector3 rotation)
    {
        this.name = name;
        this.index = index;
        this.linkedScene = linkedScene;
        this.position = position;
        this.rotation = rotation;
    }
    public PhysicalCP_Container() { }
}

[System.Serializable]
[XmlType(TypeName = "VirtualCalibrationPoint")]
public class VirtualCP_Container
{
    [XmlAttribute(AttributeName = "Name")]
    public string name;
    [XmlAttribute(AttributeName = "Index")]
    public int index;

    [XmlElement("Position")]
    public Vector3 position;
    [XmlElement("Rotation")]
    public Vector3 rotation;

    public VirtualCP_Container(string name, int index, Vector3 position, Vector3 rotation)
    {
        this.name = name;
        this.index = index;
        this.position = position;
        this.rotation = rotation;
    }
    public VirtualCP_Container() { }
}

[System.Serializable]
[XmlRoot("SceneSettings")]
public class SceneSettings_Container
{
    public CameraRigHolder cameraRig;
    
    [XmlIgnore]
    public const string virConfigNameDefault = "Virtual.xml";

    [XmlElement("virCP_ConfigFileName")]
    public string virConfigName;

    [XmlIgnore]
    public const string phyConfigNameDefault = "Physical.xml";

    [XmlElement("phyCP_ConfigFileName")]
    public string phyConfigName;
    
    [XmlArray("Scenes")]
    [XmlArrayItem("Scene")]
    public List<SceneHolder> scenes;

    public SceneSettings_Container(Transform cameraRigTransform, List<SceneHolder> scenes)
    {
        cameraRig = new CameraRigHolder(cameraRigTransform);
        this.scenes = scenes;
        
        this.virConfigName = virConfigNameDefault;        
        this.phyConfigName = phyConfigNameDefault;
    }
    public SceneSettings_Container(Transform cameraRigTransform, List<SceneHolder> scenes, string virConfigName, string phyConfigName)
    {
        cameraRig = new CameraRigHolder(cameraRigTransform);
        this.scenes = scenes;

        if (virConfigName == "")
            virConfigName = virConfigNameDefault;
        this.virConfigName = virConfigName;

        if (phyConfigName == "")
            phyConfigName = phyConfigNameDefault;
        this.phyConfigName = phyConfigName;
    }
    public SceneSettings_Container() { }
    
}

[System.Serializable]
[XmlType(TypeName = "CameraRigHolder")]
public class CameraRigHolder
{
    [XmlAttribute(AttributeName = "Name")]
    public string name;
    [XmlElement("Position")]
    public Vector3 position;
    [XmlElement("Rotation")]
    public Vector3 rotation;
    public CameraRigHolder(Transform camRigTransform)
    {
        this.name = camRigTransform.name;
        this.position = camRigTransform.position;
        this.rotation = camRigTransform.rotation.eulerAngles;
    }
    public CameraRigHolder() { }
}

[System.Serializable]
[XmlType(TypeName = "SceneHolder")]
public class SceneHolder
{
    [XmlAttribute(AttributeName = "Name")]
    public string name;
    public SceneHolder(string name)
    {
        this.name = name;
    }
    public SceneHolder() { }
}

#endregion

//Some other containders ...

#endregion
