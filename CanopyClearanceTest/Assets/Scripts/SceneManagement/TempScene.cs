using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.InputManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using ExperimentCore;

[Serializable]
public class SceneGroup
{
    public string[] scenesToLoad;
}

/// <summary>
/// Base structure of algorithm for templete method
/// </summary>
public abstract class TempScene : MonoBehaviour
{

    [SerializeField]
    [FormerlySerializedAs("RootEvents")]
    EventRegistery _events = new EventRegistery();
    
    protected static Settings Settings;

    public EventRegistery Events { get { return _events; } }

    public List<SceneGroup> sceneGroupsToLoad=new List<SceneGroup>();
    public int startIndex = 0;
    static int scenarioStartIndex = -1;
    static int prevScenarioStartIndex = -1;

    private static int Index
    {
        get { return scenarioStartIndex; }
        set
        {
            prevScenarioStartIndex = scenarioStartIndex;
            scenarioStartIndex = value;
        }
    }

    private static int PrevIndex
    {
        get { return prevScenarioStartIndex; }
    }

    [SerializeField]
    string[] scenesToLoad;

    [SerializeField] private bool setAsActiveScene = false;

    static Dictionary<string, int> sceneDictionary = new Dictionary<string, int>();

    public static Action OnExperimentLoaded,OnExperimentEnded;
    public static bool ExperimentReady { get; private set; }

    private List<InputManager.Handler> handlersNeeded;
    private List<InputManager.ContiunousHandler> contiunousHandlersNeeded;

    protected abstract void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded);
    protected abstract void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded);
    protected abstract void Init();
    protected abstract void Run();
    protected abstract void BindEvents();
    protected abstract void UnBindEvents();

    private bool _initialized = false;

    void SetActiveSceneGroup()
    {
        if (sceneGroupsToLoad != null && sceneGroupsToLoad.Count > 0)
            scenesToLoad = sceneGroupsToLoad[Index].scenesToLoad;
    }

    public void MoveNextSceneGroup()
    {
        if (Index < sceneGroupsToLoad.Count - 1)
        {
            Index++;
            SetSceneGroup();
        }
        else
        {
            if (OnExperimentEnded != null)
                OnExperimentEnded.Invoke();
        }
    }
    void SetSceneGroup()
    {
        if (Index < 0 || Index > sceneGroupsToLoad.Count - 1) return;//invalid

        SetActiveSceneGroup();
        LoadScenarioScenes();
    }

    void LoadScenarioScenes()
    {

        ExperimentReady = false;
        List<Scene> allScenesInHierarcy = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            allScenesInHierarcy.Add(SceneManager.GetSceneAt(i));
        }

        //Retrive active scene list
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string foundSceneName =
                System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

            //It will be check when we load it below.
            //If the scene is already placed do not add it to sceneDictionary for not to adding the scene
            //if (allScenesInHierarcy.Exists(x => x.name == foundSceneName)) continue;

            
            if (!sceneDictionary.ContainsKey(foundSceneName))
            {
                sceneDictionary.Add(foundSceneName, i);
            }
        }
        

        foreach (string sceneName in scenesToLoad)
        {
            if (sceneDictionary.ContainsKey(sceneName))
            {
                //if (!SceneManager.GetSceneByBuildIndex(sceneDictionary[sceneName]).isLoaded)
                if (!allScenesInHierarcy.Contains(SceneManager.GetSceneByBuildIndex(sceneDictionary[sceneName])))
                    SceneManager.LoadSceneAsync(sceneDictionary[sceneName], LoadSceneMode.Additive);
            }
        }

        StartCoroutine(CheckAllScenesLoaded());

        //Clear previous scenes
        if (PrevIndex > -1)
        {
            foreach (string sceneName in sceneGroupsToLoad[PrevIndex].scenesToLoad)
            {
                if (sceneDictionary.ContainsKey(sceneName))
                {
                    if (SceneManager.GetSceneByBuildIndex(sceneDictionary[sceneName]).isLoaded)
                        SceneManager.UnloadSceneAsync(sceneDictionary[sceneName]);
                }
            }
        }

    }

    void Awake()
    {
        Index = startIndex;

        SetSceneGroup();
    }


    void Start()
    {

        if (setAsActiveScene && sceneDictionary.ContainsKey(this.GetType().Name))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneDictionary[this.GetType().Name]));
        }

        Init();

        SelectDiscreateHandlers(out handlersNeeded);
        SelectContiunousHandlers(out contiunousHandlersNeeded);

        if (handlersNeeded != null && handlersNeeded.Count > 0)
            InputManager.Instance.RegisterScene(this, ref handlersNeeded);
        if (contiunousHandlersNeeded != null && contiunousHandlersNeeded.Count > 0)
            InputManager.Instance.RegisterScene(this, ref contiunousHandlersNeeded);

        if (handlersNeeded == null)
        {
            Debug.LogError("Handlers not initiated.");
            gameObject.SetActive(false);
        }

        BindEvents();

        _initialized = true;
    }

    
    IEnumerator CheckAllScenesLoaded()
    {
        yield return new WaitUntil(() =>
        {
            bool allScenesLoaded = true;

            foreach (string sceneName in scenesToLoad)
            {
                if (sceneDictionary.ContainsKey(sceneName))
                {
                    if (!SceneManager.GetSceneByBuildIndex(sceneDictionary[sceneName]).isLoaded)
                        allScenesLoaded = false;
                    break;
                }
            }

            return allScenesLoaded;
        });

        if (OnExperimentLoaded != null && !ExperimentReady)
        {
            ExperimentReady = true;
            OnExperimentLoaded.Invoke();
        }
    }

    void Update()
    {
        Run();
    }

    void OnEnabled()
    {
        if (_initialized)
        {
            BindEvents();
            InputManager.Instance.RegisterScene(this, ref handlersNeeded);
        }
    }
    void OnDisabled()
    {
        UnBindEvents();
        InputManager.Instance.UnRegisterScene(this);
    }

    void OnDestroy()
    {
        if (enabled)
            OnDisabled();
    }
    
}
