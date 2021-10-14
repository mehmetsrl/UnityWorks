using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationTest : MonoBehaviour {

    public List<PhysicalCP_Container> PhysicalCP_List;
    public List<VirtualCP_Container> VirtualCP_List;
    public string phyConfigPath = "phyCalibrationPoint_Configs.xml";
    public string virConfigPath = "virCalibrationPoint_Configs.xml";
    public int cpNumber = 5;
    public Vector2 randomRanges;
    public RigidbodyConstraints DOF6_Constraints;
    public bool generateAndWriteRandomConfigs = false;

	// Use this for initialization
	void Start () {

        phyConfigPath = Application.streamingAssetsPath + "/" + phyConfigPath;
        virConfigPath = Application.streamingAssetsPath + "/" + virConfigPath;

        ConfigDataListIOHandler<PhysicalCP_Container> phyDataConfigHandler = new ConfigDataListXMLSerializer<PhysicalCP_Container>(phyConfigPath);
        ConfigDataListIOHandler<VirtualCP_Container> virDataConfigHandler = new ConfigDataListXMLSerializer<VirtualCP_Container>(virConfigPath);

        if (generateAndWriteRandomConfigs)
        {

            //Generate random dummy phyconfigs
            List<PhysicalCP_Container> myPhysicalCPList = new List<PhysicalCP_Container>();
            for (int i = 0; i < cpNumber; i++)
            {
                myPhysicalCPList.Add(
                    new PhysicalCP_Container(
                        "CP" + (i + 1), (i + 1), 
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                        GetRandomVector3ByConstraints(ref randomRanges,DOF6_Constraints,Vector3Type.position),
                        GetRandomVector3ByConstraints(ref randomRanges, DOF6_Constraints, Vector3Type.rotation))
                );
            }

            //Save random dummy configs
            phyDataConfigHandler.SaveConfigData(ref myPhysicalCPList);
            PhysicalCP_List = myPhysicalCPList;


            //Generate random dummy virconfigs
            List<VirtualCP_Container> myVirtualCPList = new List<VirtualCP_Container>();
            for (int i = 0; i < cpNumber; i++)
            {
                myVirtualCPList.Add(
                    new VirtualCP_Container(
                        "CP" + (i + 1), (i + 1),
                        GetRandomVector3ByConstraints(ref randomRanges, DOF6_Constraints, Vector3Type.position),
                        GetRandomVector3ByConstraints(ref randomRanges, DOF6_Constraints, Vector3Type.rotation))
                );
            }

            //Save random dummy configs
            virDataConfigHandler.SaveConfigData(ref myVirtualCPList);
            VirtualCP_List = myVirtualCPList;

        }
        else
        {
            //Read saved data
            phyDataConfigHandler.GetConfigs(out PhysicalCP_List);
            virDataConfigHandler.GetConfigs(out VirtualCP_List);
        }

	}

    enum Vector3Elements{
        x,y,z
    }
    enum Vector3Type
    {
        position,
        rotation
    }
    
    Vector3 GetRandomVector3ByConstraints(ref Vector2 range, RigidbodyConstraints cons, Vector3Type type)
    {
        float x, y, z;
        x = IsLimited(Vector3Elements.x, type, cons) ? 0 : Random.Range(range.x, range.y);
        y = IsLimited(Vector3Elements.y, type, cons) ? 0 : Random.Range(range.x, range.y);
        z = IsLimited(Vector3Elements.z, type, cons) ? 0 : Random.Range(range.x, range.y);

        return new Vector3(x, y, z);
    }

    bool IsLimited(Vector3Elements element, Vector3Type type, RigidbodyConstraints constraints){
        if (constraints == RigidbodyConstraints.FreezeAll) return true;

        switch (type)
        {
            case Vector3Type.position:
                if (constraints == RigidbodyConstraints.FreezePosition) return true;
                switch (element)
                {
                    case Vector3Elements.x:
                        if (constraints == RigidbodyConstraints.FreezePositionX) return true;
                        break;
                    case Vector3Elements.y:
                        if (constraints == RigidbodyConstraints.FreezePositionY) return true;
                        break;
                    case Vector3Elements.z:
                        if (constraints == RigidbodyConstraints.FreezePositionZ) return true;
                        break;
                }
                break;
            case Vector3Type.rotation:
                if (constraints == RigidbodyConstraints.FreezeRotation) return true;
                switch (element)
                {
                    case Vector3Elements.x:
                        if (constraints == RigidbodyConstraints.FreezeRotationX) return true;
                        break;
                    case Vector3Elements.y:
                        if (constraints == RigidbodyConstraints.FreezeRotationY) return true;
                        break;
                    case Vector3Elements.z:
                        if (constraints == RigidbodyConstraints.FreezeRotationZ) return true;
                        break;
                }
                break;
        }
        return false;
    }

}
