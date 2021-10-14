using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holder class for transportation configurations.
[System.Serializable]
public class Configs
{
    public string pathPrefix = "VRCalibration_";
    string configPath;

    public List<VirtualCP_Container> virtualCPs;
    
    public List<PhysicalCP_Container> physicalCPs;

    public string sceneSettingsConfigName = "SceneSettings.xml";
    //public CameraRig_Container cameraRigCon;
    public SceneSettings_Container sceneSettingsCon;

    public GameObject cameraRig;
    
    ConfigDataListIOHandler<VirtualCP_Container> virDataConfigHandler;
    ConfigDataListIOHandler<PhysicalCP_Container> phyDataConfigHandler;
    ConfigDataIOHandler<SceneSettings_Container> sceneSettingsConfigHandler;

    public void Init(){

        configPath = Application.streamingAssetsPath + "/" + pathPrefix;

        sceneSettingsConfigHandler = new ConfigDataXMLSerializer<SceneSettings_Container>(configPath + sceneSettingsConfigName);
        sceneSettingsConfigHandler.GetConfigs(out sceneSettingsCon);

        if (sceneSettingsCon == null)
        {
            SaveSceneSettingConfigs();
        }

        if (sceneSettingsCon != null)
        {
            virDataConfigHandler = new ConfigDataListXMLSerializer<VirtualCP_Container>(configPath + sceneSettingsCon.virConfigName);
            phyDataConfigHandler = new ConfigDataListXMLSerializer<PhysicalCP_Container>(configPath + sceneSettingsCon.phyConfigName);
        }
    }

    public void FillConfigs()
    {
        if (virDataConfigHandler != null)
            virDataConfigHandler.GetConfigs(out virtualCPs);

        if (phyDataConfigHandler != null)
            phyDataConfigHandler.GetConfigs(out physicalCPs);
       

        if (sceneSettingsCon!=null && sceneSettingsCon.cameraRig != null)
        {
            cameraRig.transform.position = sceneSettingsCon.cameraRig.position;
            cameraRig.transform.rotation = Quaternion.Euler(sceneSettingsCon.cameraRig.rotation);
        }
    }

    public void SaveVirtualConfigs(List<VirtualCP_Container> virtualCPs){
        if (virDataConfigHandler != null)
        {
            Debug.Log("Saving virtual configs from " + virDataConfigHandler.ConfigPath + "...");
            virDataConfigHandler.SaveConfigData(ref virtualCPs);
        }
    }
    public void SavePhyicalConfigs(List<PhysicalCP_Container> physicalCPs)
    {
        if (phyDataConfigHandler != null)
        {
            Debug.Log("Saving physical configs from " + phyDataConfigHandler.ConfigPath + "...");
            phyDataConfigHandler.SaveConfigData(ref physicalCPs);
        }
    }
    public void SaveSceneSettingConfigs(SceneSettings_Container settings = null)
    {
        if (settings == null)
        {
            //There is no settings file. Create new settings for current scene
            List<SceneHolder> sceneHolderList = new List<SceneHolder>();
            List<string> sceneNames;
            Utils.ReadSceneNames(out sceneNames);
            foreach (var scene in sceneNames)
                sceneHolderList.Add(new SceneHolder(scene));

            settings = new SceneSettings_Container(cameraRig.transform, sceneHolderList);
            sceneSettingsCon = settings;
            
            //Create other handlers with this new settings 
            virDataConfigHandler = new ConfigDataListXMLSerializer<VirtualCP_Container>(configPath + settings.virConfigName);
            phyDataConfigHandler = new ConfigDataListXMLSerializer<PhysicalCP_Container>(configPath + settings.phyConfigName);
        }

        Debug.Log("Saving camera scene configs from " + sceneSettingsConfigHandler.ConfigPath + "...");
        sceneSettingsConfigHandler.SaveConfigData(ref settings);
    }
    public void SaveAllConfigs()
    {
        SaveSceneSettingConfigs(sceneSettingsCon);
        SaveVirtualConfigs(virtualCPs);
        SavePhyicalConfigs(physicalCPs);
    }
}
