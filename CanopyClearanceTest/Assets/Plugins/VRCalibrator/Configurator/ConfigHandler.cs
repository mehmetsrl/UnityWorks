using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Valve.VR.InteractionSystem;
using System;

public class ConfigHandler : MonoBehaviour {

    static ConfigHandler instance = null;

    #region Accesors
    public static ConfigHandler Instance
    {
        get { return instance; }
        private set
        {
            if (instance == null)
                instance = value;
        }
    }

    public List<PhysicalCP_Container> GetCurrentPhysicalContainers
    {
        get { return GeneratePhysicalContainerList(phyPointVirtualizations); }
    }
    public List<VirtualCP_Container> GetCurrentVirtualContainers
    {
        get { return GenerateVirtualContainerList(virPointVirtualizations); }
    }
    public SceneSettings_Container GetCurrentSceneSettingsContainer
    {
        get
        {
            Vector3 lastPos = cameraRigTransform.position;
            Quaternion lastRot = cameraRigTransform.rotation;

            cameraRigTransform.position = initialRigPos;
            cameraRigTransform.rotation = initialRigRot;

            //TODO: It writes configs of camera rig object in active scene. This must befor all scenes
            SceneSettings_Container value = GenerateSceneSettingsContainer(cameraRigTransform);

            cameraRigTransform.position = lastPos;
            cameraRigTransform.rotation = lastRot;
            return value;
        }
    }

    public bool IsThereAnyCPInScene
    {
        get { return cpIndexisInScene.Count > 0; }
    }

    #endregion

    public enum  Mode
    {
        calibrationMode,
        playMode,
        tuning
    }

    #region Properties

    Configs configs;
    Transform cameraRigTransform;

    public List<VirtualizedCalibrationPoint> virPointVirtualizations, phyPointVirtualizations;
    public Mode configMode = Mode.playMode;
    public VirtualizedCalibrationPoint cpPrefab, cpPrefabCalibrationMode;
    public GameObject virVirtualizationRoot, phyVirtualizationRoot;
    public List<int> cpIndexisInScene=new List<int>();
    public List<int> allocatedIndexes = new List<int>();


    bool overridable = false;
    bool initiated = false;
    Vector3 initialRigPos;
    Quaternion initialRigRot;
    #endregion

    void Awake()
    {
        Instance = this;
    }

    public void Initiate(ref Configs configs, bool overridable = false)
    {
        if (initiated)
        {
            if (this.overridable)
            {
                this.configs = configs;
                this.overridable = overridable;
                this.initiated = true;
                InitCPTransforms(true);
                cameraRigTransform = this.configs.cameraRig.transform;
            }
            initialRigPos = cameraRigTransform.position;
            initialRigRot = cameraRigTransform.rotation;
        }
        else
        {
            this.configs = configs;
            this.overridable = overridable;
            this.initiated = true;
            InitCPTransforms();
            cameraRigTransform = this.configs.cameraRig.transform;
        }


    }
    
    public void SaveAllConfigs()
    {
        configs.SaveAllConfigs();
    }


    public void UpdatePhysicalPoints()
    {
        if (phyVirtualizationRoot.activeInHierarchy)
        {
            phyPointVirtualizations = phyVirtualizationRoot.GetComponentsInChildren<VirtualizedCalibrationPoint>().ToList();
            phyPointVirtualizations = phyPointVirtualizations.OrderBy(x => x.Index).ToList();
        }
    }

    public void UpdateVirtualPoints()
    {
        if (virVirtualizationRoot.activeInHierarchy)
        {
            virPointVirtualizations = virVirtualizationRoot.GetComponentsInChildren<VirtualizedCalibrationPoint>().ToList();
            virPointVirtualizations = virPointVirtualizations.OrderBy(x => x.Index).ToList();
        }
    }


    #region ContainerDataToTransform
    public void UpdateCPTransforms()
    {
        InitCPTransforms(true);
    }
    private void InitCPTransforms(bool clearPreviousInstances = false)
    {
        if (clearPreviousInstances)
        {
            foreach (VirtualizedCalibrationPoint points in virPointVirtualizations)
            {
                DestroyImmediate(points.gameObject);
            }
            foreach (VirtualizedCalibrationPoint points in virVirtualizationRoot.GetComponentsInChildren<VirtualizedCalibrationPoint>()){
                DestroyImmediate(points.gameObject);
            }
            virPointVirtualizations.Clear();


            foreach (VirtualizedCalibrationPoint points in phyPointVirtualizations)
            {
                DestroyImmediate(points.gameObject);
            }
            foreach (VirtualizedCalibrationPoint points in phyVirtualizationRoot.GetComponentsInChildren<VirtualizedCalibrationPoint>())
            {
                DestroyImmediate(points.gameObject);
            }
            phyPointVirtualizations.Clear();
        }
        cpIndexisInScene.Clear();

        for (int i = 0; i < configs.physicalCPs.Count; i++)
        {
            allocatedIndexes.Add(configs.physicalCPs[i].index);
            if (configs.physicalCPs[i].linkedScene == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name && !cpIndexisInScene.Contains(i))
                cpIndexisInScene.Add(configs.physicalCPs[i].index);
        }
        
        foreach (int cpIndex in cpIndexisInScene)
        {
            if (cpIndex > -1)
            {

                int linkIndex = configs.virtualCPs.IndexOf(configs.virtualCPs.Find(x => x.index == cpIndex));

                VirtualizedCalibrationPoint targetPrefab = (configMode == Mode.playMode) ? cpPrefab : cpPrefabCalibrationMode;

                targetPrefab.gameObject.SetActive((configMode == Mode.calibrationMode));
                
                if (targetPrefab != null)
                {
                    //Create virtual points
                    if (configs.virtualCPs[linkIndex] != null)
                    {
                        VirtualizedCalibrationPoint newVirtualPoint =
                            Instantiate(targetPrefab, virVirtualizationRoot.transform);

                        newVirtualPoint.name = configs.virtualCPs[linkIndex].name;
                        newVirtualPoint.Index = configs.virtualCPs[linkIndex].index;

                        newVirtualPoint.transform.position =
                            (configs.virtualCPs[linkIndex].position); //Relative position to cameraRig
                        newVirtualPoint.transform.rotation = Quaternion.Euler(configs.virtualCPs[linkIndex].rotation);

                        newVirtualPoint.Type = VirtualizedCalibrationPoint.CPType.virtualCP;
                        newVirtualPoint.text.anchor = TextAnchor.LowerLeft;
                        newVirtualPoint.text.text = configs.virtualCPs[linkIndex].name;

                        virPointVirtualizations.Add(newVirtualPoint);
                    }

                    //Create physical points
                    if (configs.physicalCPs[linkIndex] != null)
                    {
                        VirtualizedCalibrationPoint newPoint =
                            Instantiate(targetPrefab, phyVirtualizationRoot.transform);
                        newPoint.name = configs.physicalCPs[linkIndex].name;
                        newPoint.Index = configs.physicalCPs[linkIndex].index;

                        newPoint.transform.position =
                            (configs.physicalCPs[linkIndex].position); //Relative position to cameraRig
                        newPoint.transform.rotation = Quaternion.Euler(configs.physicalCPs[linkIndex].rotation);

                        newPoint.Type = VirtualizedCalibrationPoint.CPType.physicalCp;
                        newPoint.text.anchor = TextAnchor.UpperLeft;
                        newPoint.text.text = configs.physicalCPs[linkIndex].name;

                        phyPointVirtualizations.Add(newPoint);
                    }
                }
            }
        }
    }
    #endregion

    #region TransformToContainerData

    //TODO
    public void UpdateContainersFromVirtualizations()
    {
        List<int> indexListInCurrentScene = configs.physicalCPs.Where(X => X.linkedScene == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            .Select(x => x.index).ToList();

        List<PhysicalCP_Container> phyContainerList = GetCurrentPhysicalContainers;
        List<VirtualCP_Container> virContainerList = GetCurrentVirtualContainers;

        foreach (var index in indexListInCurrentScene)
        {
            PhysicalCP_Container overridedPhyContainer = configs.physicalCPs.Find(x => x.index == index);

            if (overridedPhyContainer != null)
                configs.physicalCPs.Remove(overridedPhyContainer);
            
            VirtualCP_Container overridedVirContainer = configs.virtualCPs.Find(x => x.index == index);

            if (overridedPhyContainer != null)
                configs.virtualCPs.Remove(overridedVirContainer);
        }

        Debug.Log(phyContainerList.Count);

        configs.physicalCPs.AddRange(phyContainerList);
        configs.virtualCPs.AddRange(virContainerList);
        
    }

    List<PhysicalCP_Container> GeneratePhysicalContainerList(List<VirtualizedCalibrationPoint> virtualizedPoints)
    {
        List<PhysicalCP_Container> containerList = new List<PhysicalCP_Container>();
        foreach (var point in virtualizedPoints)
        {
            containerList.Add(
                new PhysicalCP_Container(
                    point.name,
                    point.Index,
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                    point.transform.position, (point.transform.rotation).eulerAngles));
                    /*
                    configs.cameraRig.transform.InverseTransformPoint(point.transform.position), //Relative position to cameraRig
                    (Quaternion.Inverse(configs.cameraRig.transform.rotation)* point.transform.rotation).eulerAngles));//Relative rotation to cameraRig*/
        }
        return containerList;
    }

    List<VirtualCP_Container> GenerateVirtualContainerList(List<VirtualizedCalibrationPoint> virtualizedPoints)
    {
        List<VirtualCP_Container> containerList = new List<VirtualCP_Container>();
        foreach (var point in virtualizedPoints)
        {
            containerList.Add(new VirtualCP_Container(point.name, point.Index, point.transform.position, point.transform.rotation.eulerAngles));
        }
        return containerList;
    }

    SceneSettings_Container GenerateSceneSettingsContainer(Transform cameraRigTransform)
    {
        List<SceneHolder> sceneHolderList = new List<SceneHolder>();
        List<string> sceneNames;
        Utils.ReadSceneNames(out sceneNames);
        foreach (var scene in sceneNames)
        {
            sceneHolderList.Add(new SceneHolder(scene));
        }
        SceneSettings_Container sceneSettingsContainer = new SceneSettings_Container(cameraRigTransform, sceneHolderList);//, configs.sceneSettingsCon.virConfigName, configs.sceneSettingsCon.phyConfigName);
        return sceneSettingsContainer;
    }
    #endregion


    List<Transform> initialphyPointParents = new List<Transform>();
    public void AdobtAllPhysicalPoints()
    {
        initialphyPointParents.Clear();
        foreach (VirtualizedCalibrationPoint point in phyPointVirtualizations)
        {
            initialphyPointParents.Add(point.transform.parent);
            point.transform.parent = cameraRigTransform;
        }
    }

    public void AbandonAllPhysicalPoints()
    {
        int index = 0;
        foreach (VirtualizedCalibrationPoint point in phyPointVirtualizations)
        {
            if (initialphyPointParents[index] != null)
                point.transform.parent = initialphyPointParents[index];
            index++;
        }
    }



    public int GetUniqueCPIndex()
    {
        for(int i = 0; i < 100; i++)
        {
            bool fisable = true;
            foreach (int cpIndex in allocatedIndexes)
            {
                if (cpIndex == i)
                {
                    fisable = false;
                    break;
                }
            }
            if (fisable)
            {
                allocatedIndexes.Add(i);
                return i;
            }
        }
        return -1;
    }
    




}
