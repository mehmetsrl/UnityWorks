using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationConfigurationUI : MonoBehaviour {
        
    static CalibrationConfigurationUI instance = null;
    public static CalibrationConfigurationUI Instance
    {
        get { return instance; }
        private set
        {
            if (instance == null)
            {
                instance = value;
                DontDestroyOnLoad(instance.gameObject);
            }

        }
    }

    public Vector2 workingResolution;
    public Canvas canvas;
    float scaleFactor;
    public enum Stage { physicalCPConfig, virtualCPConfig }
    public Stage currentStage = Stage.physicalCPConfig;
    
    public Configs configs;

    #region UIElements
    public InputField FileName;
    public Text prefixText;
    public Button loadButton, saveButton;
    //public 
    public ToggleGroup containerToggleGroup;
    public RectTransform SceneList;
    public ScrollRect SceneListScroll;
    public Button fillScFromFileBtn, fillScFromAppBtn;
    public List<UIListItem> sceneItemList;
    public int activeSceneIndex = -1;

    public RectTransform CPList;
    public ScrollRect CPScroll;
    public Button addCPBtn, removeCPBtn;
    public List<UIListItem> cpItemList;
    public int activeCPIndex = -1;

    public Button saveCPValuesButton;

    public RectTransform gridViewListItemPrefab;

    //ContainerViews
    public InputField phyCPName;
    public Vector3UIOperator phyCPPosition;
    public Vector3UIOperator phyCPRotation;

    public Text virCPName;
    public Vector3UIOperator virCPPosition;
    public Vector3UIOperator virCPRotation;

    public Button FillConfigFromTransformBtn;
    public UIViveController viveController;

    #endregion

    private void Awake()
    {
        Instance = this;
        canvas.enabled = true;
        scaleFactor = Mathf.Max((canvas.pixelRect.width / workingResolution.x), (canvas.pixelRect.height / workingResolution.y));
    }
    void Start()
    {
        ReScaleTexts();
        InitUIFeatures();
        BindEvents();
    }
    public void Initiate(ref Configs config)
    {
        configs = config;
        FileName.text = configs.sceneSettingsConfigName;
        prefixText.text = config.pathPrefix;
    }
    void InitUIFeatures()
    {

#if !UNITY_EDITOR
        fillScFromAppBtn.interactable = false;
#endif
    }
    void ReScaleTexts()
    {
        List<Text> allTextComponents;
        allTextComponents = GetComponentsInChildren<Text>().ToList();
        foreach (Text t in allTextComponents)
        {
            t.fontSize = Mathf.RoundToInt(t.fontSize * scaleFactor);
        }
    }
#region EventBindings
    void BindEvents()
    {
        loadButton.onClick.AddListener(LoadConfigFromFile);
        saveButton.onClick.AddListener(SaveConfigToFile);

        fillScFromFileBtn.onClick.AddListener(FillSceneListFromFile);
        fillScFromAppBtn.onClick.AddListener(FillSceneListFromApp);

        addCPBtn.onClick.AddListener(AddCP);
        removeCPBtn.onClick.AddListener(RemoveCP);

        FillConfigFromTransformBtn.onClick.AddListener(FillUIConfigFromController);
        phyCPName.onValueChanged.AddListener(SetCPName);
        saveCPValuesButton.onClick.AddListener(SaveCPValues);

        foreach (Toggle t in containerToggleGroup.GetToggles())
        {
            t.onValueChanged.AddListener((
                bool isActive)=> {
                    if (isActive)
                        SetStage((Stage)(t.transform.GetSiblingIndex()));
                });
        }

    }
    void RemoveEvents()
    {
        loadButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();

        fillScFromFileBtn.onClick.RemoveAllListeners();
        fillScFromAppBtn.onClick.RemoveAllListeners();

        addCPBtn.onClick.RemoveAllListeners();
        removeCPBtn.onClick.RemoveAllListeners();

        FillConfigFromTransformBtn.onClick.RemoveAllListeners();
        phyCPName.onValueChanged.RemoveAllListeners();
        saveCPValuesButton.onClick.RemoveAllListeners();

        foreach (Toggle t in containerToggleGroup.GetToggles())
        {
            t.onValueChanged.RemoveAllListeners();
        }
    }
    #endregion

#region UIFunctions

    void LoadConfigFromFile()
    {
        configs.sceneSettingsConfigName = FileName.text;
        configs.Init();
    }

    bool isIncludeCP(int index)
    {
        foreach (PhysicalCP_Container cp in configs.physicalCPs)
        {
            if (cp.index == index)
                return true;
        }
        return false;
    }

    void SaveConfigToFile()
    {
        ConfigHandler.Instance.UpdatePhysicalPoints();
        ConfigHandler.Instance.UpdateVirtualPoints();
        ConfigHandler.Instance.UpdateContainersFromVirtualizations();
        ConfigHandler.Instance.SaveAllConfigs();
    }

    void AddCP()
    {
        List<UIListItem> cpDrafts = new List<UIListItem>(cpItemList);

        int fisableIndex = ConfigHandler.Instance.GetUniqueCPIndex();

        UIListItem newItem = new UIListItem();
        newItem.containerIndex = fisableIndex;
        newItem.itemName = gridViewListItemPrefab.GetComponent<UIListItem>().itemNameTextObj.text;
        cpDrafts.Add(newItem);

        GenerateListView(CPScroll, gridViewListItemPrefab, ref cpDrafts, ref cpItemList, SetCPIndex);
        if (cpItemList != null && cpItemList.Count > 0 && cpItemList[cpItemList.Count - 1] != null)
        {
            cpItemList[cpItemList.Count - 1].Select();
        }
        ClearPhyCPAttributes();
        ClearVirCPAttributes();
    }
    void RemoveCP()
    {
        if (activeCPIndex > -1 && activeCPIndex < cpItemList.Count)
        {

            List<UIListItem> cpDrafts = new List<UIListItem>(cpItemList);

            UIListItem itemToRemove = cpDrafts.Find(x => x.transform.GetSiblingIndex() == activeCPIndex);

            cpDrafts.Remove(itemToRemove);

            configs.physicalCPs.Remove(configs.physicalCPs.Find(x => x.index == itemToRemove.containerIndex));
            configs.virtualCPs.Remove(configs.virtualCPs.Find(x => x.index == itemToRemove.containerIndex));

            GenerateListView(CPScroll, gridViewListItemPrefab, ref cpDrafts, ref cpItemList, SetCPIndex);

            if (activeCPIndex != 0)
            {
                activeCPIndex--;
            }

            if (cpItemList != null && cpItemList.Count > 0 && cpItemList[activeCPIndex] != null)
            {
                cpItemList[activeCPIndex].Select();
            }
        }
    }
    void FillCpList()
    {

        List<UIListItem> cpDrafts = new List<UIListItem>();
        if (activeSceneIndex > -1 && sceneItemList[activeSceneIndex] != null)
        {
            foreach (PhysicalCP_Container phyPoint in configs.physicalCPs){
                if (phyPoint.linkedScene == sceneItemList[activeSceneIndex].itemNameTextObj.text)
                {
                    UIListItem newItem = new UIListItem();
                    newItem.containerIndex = phyPoint.index;
                    newItem.itemName = phyPoint.name;

                    newItem.phyPosition = phyPoint.position;
                    newItem.phyRotation = phyPoint.rotation;
                    newItem.virPosition = configs.virtualCPs.Find(x=>x.index==newItem.containerIndex).position;
                    newItem.virRotation = configs.virtualCPs.Find(x => x.index == newItem.containerIndex).rotation;
                    cpDrafts.Add(newItem);
                }
            }
        }
        GenerateListView(CPScroll, gridViewListItemPrefab, ref cpDrafts, ref cpItemList, SetCPIndex);

        if (cpItemList.Count > 0 && cpItemList[0] != null)
        {
            cpItemList[0].Select();
        }
    }

    void FillSceneListFromFile()
    {
        List<UIListItem> sceneDrafts = new List<UIListItem>();
        foreach (SceneHolder scene in configs.sceneSettingsCon.scenes)
        {
            UIListItem newItem = new UIListItem();
            newItem.containerIndex = configs.sceneSettingsCon.scenes.IndexOf(scene);
            newItem.itemName = scene.name;
            sceneDrafts.Add(newItem);
        }

        GenerateListView(SceneListScroll, gridViewListItemPrefab, ref sceneDrafts, ref sceneItemList, SetSceneIndex);
        if (sceneItemList != null && sceneItemList.Count > 0 && sceneItemList[0] != null)
        {
            sceneItemList[0].Select();
        }
    }

    void FillSceneListFromApp()
    {
        List<UIListItem> sceneDrafts = new List<UIListItem>();
        List<string> sceneNames;
        Utils.ReadSceneNames(out sceneNames);
        for(int i=0;i< sceneNames.Count;i++)
        {
            UIListItem newItem = new UIListItem();
            newItem.containerIndex = i;
            newItem.itemName = sceneNames[i];
            sceneDrafts.Add(newItem);
        }

        GenerateListView(SceneListScroll, gridViewListItemPrefab, ref sceneDrafts, ref sceneItemList, SetSceneIndex);
        if (sceneItemList!=null && sceneItemList.Count > 0 && sceneItemList[0] != null)
        {
            sceneItemList[0].Select();
        }
    }

    public void Show(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }

    void GenerateListView(ScrollRect holderScroll, RectTransform listItemPrefab,ref List<UIListItem> itemDrafts,ref List<UIListItem> listItem, Action<int> selectionCallback)
    {
        GridLayoutGroup SceneListLayoutGroup = holderScroll.content.GetComponent<GridLayoutGroup>();
        Vector2 scaledSize = new Vector2(SceneListScroll.GetComponent<RectTransform>().rect.width - (SceneListLayoutGroup.padding.left + SceneListLayoutGroup.padding.right), SceneListLayoutGroup.cellSize.y);
        SceneListLayoutGroup.cellSize = scaledSize;

        foreach (UIListItem oldItems in listItem)
        {
            DestroyImmediate(oldItems.gameObject);
        }
        listItem.Clear();

        for(int i =0; i< itemDrafts.Count;i++)
        {
            UIListItem newListItem = Instantiate(listItemPrefab, holderScroll.content.transform).GetComponentInChildren<UIListItem>();
            if (newListItem != null)
            {
                newListItem.itemNameTextObj.fontSize = Mathf.RoundToInt(newListItem.itemNameTextObj.fontSize * scaleFactor);
                newListItem.itemNameTextObj.text = itemDrafts[i].itemName;
                newListItem.itemName = itemDrafts[i].itemName;
                newListItem.containerIndex = itemDrafts[i].containerIndex;
                newListItem.phyPosition = itemDrafts[i].phyPosition;
                newListItem.phyRotation = itemDrafts[i].phyRotation;
                newListItem.virPosition = itemDrafts[i].virPosition;
                newListItem.virRotation = itemDrafts[i].virRotation;
                newListItem.button.onClick.AddListener(() => { selectionCallback(newListItem.transform.GetSiblingIndex()); });
                listItem.Add(newListItem);
            }
        }

    }

    void SetSceneIndex(int index)
    {
        if (activeSceneIndex > -1 && activeSceneIndex< sceneItemList.Count)
        {
            sceneItemList[activeSceneIndex].button.interactable = true;
        }

        activeSceneIndex = index;
        sceneItemList[activeSceneIndex].button.interactable = false;
        FillCpList();
    }
    void SetCPIndex(int index)
    {
        if (activeCPIndex > -1 && activeCPIndex < cpItemList.Count)
        {
            cpItemList[activeCPIndex].button.interactable = true;
        }

        activeCPIndex = index;
        cpItemList[activeCPIndex].button.interactable = false;

        ClearPhyCPAttributes();
        if (activeCPIndex>-1 && activeCPIndex<configs.physicalCPs.Count && configs.physicalCPs[activeCPIndex] != null)
        {
            FillPhyCPAttributes(configs.physicalCPs[activeCPIndex]);
        }

        ClearVirCPAttributes();
        if (activeCPIndex > -1 && activeCPIndex < configs.virtualCPs.Count && configs.virtualCPs[activeCPIndex] != null)
        {
            FillVirCPAttributes(configs.virtualCPs[activeCPIndex]);
        }
    }

    void FillPhyCPAttributes(PhysicalCP_Container phyContainer)
    {
        ClearPhyCPAttributes();
        if (phyContainer !=null)
        {
            phyCPName.text = phyContainer.name;
            phyCPPosition.SetValues(phyContainer.position);
            phyCPRotation.SetValues(phyContainer.rotation);
        }
    }

    void ClearPhyCPAttributes()
    {
        phyCPName.text = "";
        phyCPPosition.SetValues(Vector3.zero);
        phyCPRotation.SetValues(Vector3.zero);
    }

    void FillVirCPAttributes(VirtualCP_Container virContainer)
    {
        ClearVirCPAttributes();
        if (virContainer != null)
        {
            virCPName.text = virContainer.name;
            virCPPosition.SetValues(virContainer.position);
            virCPRotation.SetValues(virContainer.rotation);
        }
    }
    void ClearVirCPAttributes()
    {
        virCPName.text = "";
        virCPPosition.SetValues(Vector3.zero);
        virCPRotation.SetValues(Vector3.zero);
    }

    void FillUIConfigFromController()
    {
        FillUIConfigFromController(-1);
    }
    public void FillUIConfigFromController(int index=-1)
    {
        if (index > -1) activeCPIndex = index;
        if (activeCPIndex < 0 || activeCPIndex >= cpItemList.Count) return;

        Vector3 position, rotation;

        //TODO: Stage logic may not be needed..
        switch (currentStage)
        {
            case Stage.physicalCPConfig:
                GameObject referanceObj= new GameObject("ReferanceGO");
                referanceObj.transform.parent = configs.cameraRig.transform;
                referanceObj.transform.localPosition = viveController.Position;
                referanceObj.transform.localRotation = viveController.Rotation;
                referanceObj.transform.parent = null;

                //phyCPPosition.SetValues(configs.cameraRig.transform.InverseTransformPoint(viveController.controllerPosition.GetValues()));
                //phyCPRotation.SetValues(Quaternion.Inverse(configs.cameraRig.transform.rotation) * Quaternion.Euler(viveController.controllerRotation.GetValues()).eulerAngles);
                phyCPPosition.SetValues(referanceObj.transform.position);
                phyCPRotation.SetValues(referanceObj.transform.rotation.eulerAngles);

                phyCPPosition.GetValues(out position);
                phyCPRotation.GetValues(out rotation);
                cpItemList[activeCPIndex].phyPosition = position;
                cpItemList[activeCPIndex].phyRotation = rotation;

                //This settigns placed for set virtual position object to proper place in order to hold by users
                virCPPosition.SetValues(new Vector3(0, referanceObj.transform.position.y, 0));
                virCPRotation.SetValues(referanceObj.transform.rotation.eulerAngles);

                virCPPosition.GetValues(out position);
                virCPRotation.GetValues(out rotation);
                cpItemList[activeCPIndex].virPosition = position;
                cpItemList[activeCPIndex].virRotation = rotation;

                Destroy(referanceObj);
                break;
            case Stage.virtualCPConfig:
                //TODO
                virCPPosition.SetValues(viveController.Position);
                virCPRotation.SetValues(viveController.RotationV3);
                
                virCPPosition.GetValues(out position);
                virCPRotation.GetValues(out rotation);
                cpItemList[activeCPIndex].virPosition = position;
                cpItemList[activeCPIndex].virRotation = rotation;
                break;
        }
        FillConfigFileFromUIElements();
    }

    public void FillUIConfigFromTransforms(int containerIndex = -1)
    {
        if (containerIndex > -1)
        {
            activeCPIndex= cpItemList.IndexOf(cpItemList.Find(x=>x.containerIndex==containerIndex));
        }

        Vector3 phyPosition, phyRotation, virPosition, virRotation;
        VirtualizedCalibrationPoint phyCp, virCp;
        UIListItem listItem = cpItemList[activeCPIndex];
        phyCp = ConfigHandler.Instance.phyPointVirtualizations.Find(x => x.Index == listItem.containerIndex);
        virCp = ConfigHandler.Instance.virPointVirtualizations.Find(x => x.Index == listItem.containerIndex);

        phyCPPosition.SetValues(phyCp.transform.position);
        phyCPRotation.SetValues(phyCp.transform.rotation.eulerAngles);

        phyCPPosition.GetValues(out phyPosition);
        phyCPRotation.GetValues(out phyRotation);
        listItem.phyPosition = phyPosition;
        listItem.phyRotation = phyRotation;


        virCPPosition.SetValues(virCp.transform.position);
        virCPRotation.SetValues(virCp.transform.rotation.eulerAngles);

        virCPPosition.GetValues(out virPosition);
        virCPRotation.GetValues(out virRotation);
        listItem.virPosition = virPosition;
        listItem.virRotation = virRotation;

        FillConfigFileFromUIElements();
    }

    void SaveCPValues(){

        Vector3 phyPosition, phyRotation, virPosition, virRotation;
        UIListItem listItem = cpItemList[activeCPIndex];

        phyCPPosition.GetValues(out phyPosition);
        phyCPRotation.GetValues(out phyRotation);
        listItem.phyPosition = phyPosition;
        listItem.phyRotation = phyRotation;


        virCPPosition.GetValues(out virPosition);
        virCPRotation.GetValues(out virRotation);
        listItem.virPosition = virPosition;
        listItem.virRotation = virRotation;


        FillConfigFileFromUIElements();
        
        ConfigHandler.Instance.UpdateCPTransforms();

    }

    void FillConfigFileFromUIElements()
    {
        if (configs.sceneSettingsCon != null)
        {
            if (configs.sceneSettingsCon.scenes == null)
                configs.sceneSettingsCon.scenes = new List<SceneHolder>();

            foreach (var scene in sceneItemList)
            {
                bool sceneFound = false;
                foreach (SceneHolder sc in configs.sceneSettingsCon.scenes)
                {
                    if (sc.name == scene.itemName)
                    {
                        sceneFound = true;
                        break;
                    }
                }
                if (!sceneFound)
                    configs.sceneSettingsCon.scenes.Add(new SceneHolder(scene.itemName));
            }

        }

        foreach (var cpItem in cpItemList)
        {
            configs.physicalCPs.Remove(configs.physicalCPs.Find(x => x.index == cpItem.containerIndex));
            configs.virtualCPs.Remove(configs.virtualCPs.Find(x => x.index == cpItem.containerIndex));

            configs.physicalCPs.Add(new PhysicalCP_Container(cpItem.itemName, cpItem.containerIndex, sceneItemList[activeSceneIndex].itemName, cpItem.phyPosition, cpItem.phyRotation));
            configs.virtualCPs.Add(new VirtualCP_Container(cpItem.itemName + "_Virtual", cpItem.containerIndex, cpItem.virPosition, cpItem.virRotation));
        }
    }

    void SetCPName(string name)
    {
        if (activeCPIndex > -1 && cpItemList[activeCPIndex] != null)
        {
            cpItemList[activeCPIndex].itemNameTextObj.text = name;
            cpItemList[activeCPIndex].itemName = name;
            virCPName.text = name + "_Virtual";
        }
    }

    void OnToggleChanged(Toggle activeToggle)
    {
        SetStage((Stage)activeToggle.transform.GetSiblingIndex());
    }

    public void SetStage(Stage stage)
    {
        currentStage = stage;
    }

    #endregion


    
    private void OnDestroy()
    {
        RemoveEvents();
    }
}

public static class UIExtentions
{
    private static System.Reflection.FieldInfo _toggleListMember;
    public static IList<Toggle> GetToggles(this ToggleGroup toggleGrp)
    {
        if (_toggleListMember == null)
        {
            _toggleListMember = typeof(ToggleGroup).GetField("m_Toggles", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (_toggleListMember == null)
            {
                throw new System.Exception("UnityEngine.UI.ToggleGroup source code mush have changed");
            }
        }
        return _toggleListMember.GetValue(toggleGrp) as IList<Toggle>;
    }
}
