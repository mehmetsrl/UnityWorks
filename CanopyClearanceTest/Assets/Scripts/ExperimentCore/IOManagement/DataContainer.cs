using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace ExperimentCore.IO
{
    #region Containers

    #region SerializableXMLContainers

    #region CanopyExperiment

    [System.Serializable]
    [XmlType(TypeName = "ExperimentInfo")]
    public class ExperimentInfo
    {
        [XmlElement("Title")] public string Title;
        [XmlElement("Manifest")] public string Manifest;
        [XmlElement("NumberOfSubjects")] public int NumberOfSubjects;

        public ExperimentInfo(string Title, string Manifest, int NumberOfSubjects)
        {
            this.Title = Title;
            this.Manifest = Manifest;
            this.NumberOfSubjects = NumberOfSubjects;
        }
        public ExperimentInfo(){}
    }

    [System.Serializable]
    [XmlType(TypeName = "CanopyClearanceRecord")]
    public class CanopyClearanceRecord
    {
        public enum Gender
        {
            Male,
            Female
        }

        [XmlAttribute(AttributeName = "SubjectID")]
        public int SubjectID;

        [XmlElement("SubjectName")] public string SubjectName;
        [XmlElement("Group")] public string Group;
        [XmlElement("SubjectGender")] public Gender SubjectGender;
        [XmlElement("TestDate")] public DateTime TestDate;
        [XmlElement("MaxSideViewAngle")] public float MaxSideViewAngle;
        [XmlElement("MaxDepthProjectedSideViewAngle")] public float MaxDepthProjectedSideViewAngle;

        [XmlElement("PositionData")] public PositioningData positionData;

        public CanopyClearanceRecord(int SubjectID,string SubjectName, string Group, Gender SubjectGender, DateTime TestDate,float MaxSideViewAngle,float MaxDepthProjectedSideViewAngle, PositioningData positionData)
        {
            this.SubjectID = SubjectID;
            this.SubjectName = SubjectName;
            this.Group = Group;
            this.SubjectGender = SubjectGender;
            this.TestDate = TestDate;
            this.MaxSideViewAngle = MaxSideViewAngle;
            this.MaxDepthProjectedSideViewAngle = MaxDepthProjectedSideViewAngle;
            this.positionData = positionData;
        }
        public CanopyClearanceRecord() { }
    }

    [System.Serializable]
    [XmlType(TypeName = "CanopyClearanceRecordItem")]
    public class CanopyClearanceRecordItem
    {
        [XmlAttribute(AttributeName = "SubjectID")]
        public int SubjectID;

        [XmlAttribute(AttributeName = "recordItemID")]
        public long recordItemID;

        [XmlElement("HeadPosition")] public Vector3 HeadPosition;
        [XmlElement("EyeVisionHitPosition")] public Vector3 EyeVisionHitPosition;
        [XmlElement("DepthProjectedSideViewAngle")] public float DepthProjectedSideViewAngle;
        [XmlElement("SideViewAngle")] public float SideViewAngle;



        public CanopyClearanceRecordItem(int SubjectID, Vector3 HeadPosition, Vector3 EyeVisionHitPosition,float DepthProjectedSideViewAngle, float SideViewAngle)
        {
            this.SubjectID = SubjectID;
            this.HeadPosition = HeadPosition;
            this.EyeVisionHitPosition = EyeVisionHitPosition;
            this.DepthProjectedSideViewAngle = DepthProjectedSideViewAngle;
            this.SideViewAngle = SideViewAngle;
        }
        CanopyClearanceRecordItem() { }
    }


    [System.Serializable]
    [XmlType(TypeName = "PositioningData")]
    public class PositioningData
    {
        [XmlIgnore]
        public Transform DesignEyePoint;

        [HideInInspector] [XmlElement("DesignEyePoint")]
        public Vector3 DesignEyePointPos;

        [XmlIgnore]
        public Transform Seat_1;

        [HideInInspector] [XmlElement("SeatPos1")]
        public Vector3 SeatPos1;

        [XmlIgnore]
        public Transform Seat_2;

        [HideInInspector] [XmlElement("SeatPos2")]
        public Vector3 SeatPos2;

        [XmlIgnore]
        public Transform Seat_3;

        [HideInInspector] [XmlElement("SeatPos3")]
        public Vector3 SeatPos3;

        [XmlIgnore]
        public Transform Seat_4;

        [HideInInspector] [XmlElement("SeatPos4")]
        public Vector3 SeatPos4;


        public PositioningData(Vector3 DesignEyePointPos, Vector3 SeatPos1, Vector3 SeatPos2, Vector3 SeatPos3,
            Vector3 SeatPos4)
        {
            this.DesignEyePointPos = DesignEyePointPos;
            this.SeatPos1 = SeatPos1;
            this.SeatPos2 = SeatPos2;
            this.SeatPos3 = SeatPos3;
            this.SeatPos4 = SeatPos4;
        }

        public void FillTransformPositionVals()
        {
            if (DesignEyePoint != null)
                DesignEyePointPos = DesignEyePoint.position;
            if (Seat_1 != null)
                SeatPos1 = Seat_1.position;
            if (Seat_2 != null)
                SeatPos2 = Seat_2.position;
            if (Seat_3 != null)
                SeatPos3 = Seat_3.position;
            if (Seat_4 != null)
                SeatPos4 = Seat_4.position;
        }

        public PositioningData()
        {
        }
    }


    #endregion


    #region ExampleContainers

    [System.Serializable]
    [XmlType(TypeName = "PhysicalCalibrationPoint")]
    public class PhysicalCP_Container
    {
        [XmlAttribute(AttributeName = "Name")] public string name;

        [XmlAttribute(AttributeName = "Index")]
        public int index;

        [XmlAttribute(AttributeName = "LinkedScene")]
        public string linkedScene;

        [XmlElement("Position")] public Vector3 position;
        [XmlElement("Rotation")] public Vector3 rotation;

        public PhysicalCP_Container(string name, int index, string linkedScene, Vector3 position, Vector3 rotation)
        {
            this.name = name;
            this.index = index;
            this.linkedScene = linkedScene;
            this.position = position;
            this.rotation = rotation;
        }

        public PhysicalCP_Container()
        {
        }
    }

    [System.Serializable]
    [XmlType(TypeName = "VirtualCalibrationPoint")]
    public class VirtualCP_Container
    {
        [XmlAttribute(AttributeName = "Name")] public string name;

        [XmlAttribute(AttributeName = "Index")]
        public int index;

        [XmlElement("Position")] public Vector3 position;
        [XmlElement("Rotation")] public Vector3 rotation;

        public VirtualCP_Container(string name, int index, Vector3 position, Vector3 rotation)
        {
            this.name = name;
            this.index = index;
            this.position = position;
            this.rotation = rotation;
        }

        public VirtualCP_Container()
        {
        }
    }

    [System.Serializable]
    [XmlRoot("SceneSettings")]
    public class SceneSettings_Container
    {
        public CameraRigHolder cameraRig;

        [XmlIgnore] public const string virConfigNameDefault = "Virtual.xml";

        [XmlElement("virCP_ConfigFileName")] public string virConfigName;

        [XmlIgnore] public const string phyConfigNameDefault = "Physical.xml";

        [XmlElement("phyCP_ConfigFileName")] public string phyConfigName;

        [XmlArray("Scenes")] [XmlArrayItem("Scene")]
        public List<SceneHolder> scenes;

        public SceneSettings_Container(Transform cameraRigTransform, List<SceneHolder> scenes)
        {
            cameraRig = new CameraRigHolder(cameraRigTransform);
            this.scenes = scenes;

            this.virConfigName = virConfigNameDefault;
            this.phyConfigName = phyConfigNameDefault;
        }

        public SceneSettings_Container(Transform cameraRigTransform, List<SceneHolder> scenes, string virConfigName,
            string phyConfigName)
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

        public SceneSettings_Container()
        {
        }

    }

    [System.Serializable]
    [XmlType(TypeName = "CameraRigHolder")]
    public class CameraRigHolder
    {
        [XmlAttribute(AttributeName = "Name")] public string name;
        [XmlElement("Position")] public Vector3 position;
        [XmlElement("Rotation")] public Vector3 rotation;

        public CameraRigHolder(Transform camRigTransform)
        {
            this.name = camRigTransform.name;
            this.position = camRigTransform.position;
            this.rotation = camRigTransform.rotation.eulerAngles;
        }

        public CameraRigHolder()
        {
        }
    }

    [System.Serializable]
    [XmlType(TypeName = "SceneHolder")]
    public class SceneHolder
    {
        [XmlAttribute(AttributeName = "Name")] public string name;

        public SceneHolder(string name)
        {
            this.name = name;
        }

        public SceneHolder()
        {
        }
    }
    
    #endregion
   

    #endregion

//Some other containers ...

    #endregion
}
