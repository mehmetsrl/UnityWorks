using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.InputManagement;
using Leap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class HandlerRegistary
{
    public InputHandler handler;
    public TempScene scene;

    public HandlerRegistary(InputHandler handler, TempScene scene)
    {
        this.handler = handler;
        this.scene = scene;
    }
}

/// <summary>
/// Mediator class to handle detected events
/// </summary>
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    [FormerlySerializedAs("RootEvents")]
    [SerializeField]
    private EventRegistery _rootEvents = new EventRegistery();

    #region Accesors
    public static InputManager Instance
    {
        get { return _instance;}
        private set
        {
            if (_instance == null)
            {
                _instance = value;
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }
    public EventRegistery RootEvents { get { return _rootEvents; } }

    public static ViveInputs ViveInputs;
    public static LeapInputs LeapInputs;
    public static UnityInputs UnityInputs;
    public static GazeInputs GazeInputs;

    private bool initiated = false;

    public bool Initiated
    {
        get { return initiated; }
    }

    public List<TempScene> scenesRegistered=new List<TempScene>();
    #endregion


    public InputHandler handler;
    public List<InputHandler> contiunousHandlerList = new List<InputHandler>();
    public Dictionary<TempScene,InputHandler> customContiunousHandlerList = new Dictionary<TempScene,InputHandler>();

    public enum Handler
    {
        ShortcutHandler,
        LeapComponentHandler,
        LeapContactHandler,
        LeapGesturetHandler,
        ViveDiscreateHandler,
        LookDirectionDiscreateHandler,
        JoystickHandler,
        CollisionHandler
    }
    public enum ContiunousHandler
    {
        ViveContiunousHandler,
        LookDirectionContiunousHandler,
        JoystickAxisHandler
    }

    private Dictionary<TempScene, List<Handler>> sceneHandlerDictionary;
    private Dictionary<Handler, InputHandler> handlerDictionary;

    private Dictionary<TempScene, List<ContiunousHandler>> sceneHandlerDictionaryCont;
    private Dictionary<ContiunousHandler, InputHandler> handlerDictionaryCont;

    void Awake()
    {
        Instance = this;

        sceneHandlerDictionary = new Dictionary<TempScene, List<Handler>>();
        handlerDictionary = new Dictionary<Handler, InputHandler>();

        handlerDictionary.Add(Handler.ShortcutHandler, new ShortcutHandler(null));
        handlerDictionary.Add(Handler.LeapComponentHandler, new LeapComponentHandler(null));
        handlerDictionary.Add(Handler.LeapContactHandler, new LeapContactHandler(null));
        handlerDictionary.Add(Handler.LeapGesturetHandler, new LeapGestureHandler(null));
        handlerDictionary.Add(Handler.ViveDiscreateHandler, new ViveDiscreateHandler(null));
        handlerDictionary.Add(Handler.LookDirectionDiscreateHandler, new LookDirectionDiscreateHandler(null));
        handlerDictionary.Add(Handler.JoystickHandler, new JoystickHandler(null));
        handlerDictionary.Add(Handler.CollisionHandler, new CollisionHandler(null));

        sceneHandlerDictionaryCont = new Dictionary<TempScene, List<ContiunousHandler>>();
        handlerDictionaryCont = new Dictionary<ContiunousHandler, InputHandler>();

        handlerDictionaryCont.Add(ContiunousHandler.ViveContiunousHandler, new ViveContiunousHandler(null));
        handlerDictionaryCont.Add(ContiunousHandler.LookDirectionContiunousHandler, new LookDirectionContiunousHandler(null));
        handlerDictionaryCont.Add(ContiunousHandler.JoystickAxisHandler, new JoystickAxisHandler(null));
        
    }
    
    public void AddContiunousHandler(TempScene scene, InputHandler handler)
    {
        customContiunousHandlerList.Add(scene, handler);
    }

    // Use this for initialization
    void Start ()
    {
        ViveInputs = ViveInputs.Instance;//GetComponentInChildren<ViveInputs>();
        LeapInputs = LeapInputs.Instance;//GetComponentInChildren<LeapInputs>();
        UnityInputs = UnityInputs.Instance;//GetComponentInChildren<UnityInputs>();
        GazeInputs = GazeInputs.Instance;//GetComponentInChildren<GazeInputs>();

        initiated = true;
    }

    public void RegisterScene(TempScene scene,ref List<Handler> handlersNeeded)
    {
        if(!scenesRegistered.Contains(scene))
            scenesRegistered.Add(scene);

        sceneHandlerDictionary.Add(scene, handlersNeeded);
        
        UpdateSelectedHandler();
    }
    public void UnRegisterScene(TempScene scene)
    {
        if (scenesRegistered.Contains(scene))
            scenesRegistered.Remove(scene);

        sceneHandlerDictionary.Remove(scene);
        sceneHandlerDictionaryCont.Remove(scene);

        UpdateSelectedHandler();
    }

    public void RegisterScene(TempScene scene, ref List<ContiunousHandler> handlersNeeded)
    {
        if (!scenesRegistered.Contains(scene))
            scenesRegistered.Add(scene);

        sceneHandlerDictionaryCont.Add(scene, handlersNeeded);

        UpdateSelectedHandler();
    }

    private void UpdateSelectedHandler()
    {
        if (handler != null)
        {
            handler.Dispose();
            handler = null;
        }

        if ((sceneHandlerDictionary == null || sceneHandlerDictionary.Count == 0) &&
            (sceneHandlerDictionaryCont == null || sceneHandlerDictionaryCont.Count == 0)) return;

        if (sceneHandlerDictionary != null && sceneHandlerDictionary.Count > 0)
        {
            List<int> handlerList = new List<int>();

            foreach (var sceneHandlers in sceneHandlerDictionary)
            {
                foreach (var handler in sceneHandlers.Value)
                {
                    if (!handlerList.Contains((int) handler))
                    {
                        handlerList.Add((int) handler);
                    }
                }
            }


            if (handlerList.Count == 0) return;

            handlerList.OrderBy(x => x);

            handler = handlerDictionary[(Handler) handlerList[0]];
            for (int i = 1; i < handlerList.Count; i++)
            {
                handler.Add(handlerDictionary[(Handler) handlerList[i]]);
            }
        }


        if (sceneHandlerDictionaryCont != null && sceneHandlerDictionaryCont.Count > 0)
        {
            List<int> handlerList = new List<int>();

            foreach (var sceneHandlers in sceneHandlerDictionaryCont)
            {
                foreach (var handler in sceneHandlers.Value)
                {
                    if (!handlerList.Contains((int)handler))
                    {
                        handlerList.Add((int)handler);
                    }
                }
            }


            if (handlerList.Count == 0) return;

            handlerList.OrderBy(x => x);

            contiunousHandlerList.Clear();
            contiunousHandlerList=new List<InputHandler>();

            for (int i = 0; i < handlerList.Count; i++)
            {
                contiunousHandlerList.Add(handlerDictionaryCont[(ContiunousHandler)handlerList[i]]);
            }
        }

    }
    
	
	// Update is called once per frame
    void Update()
    {
        if (!Initiated) return;
        
        if (handler != null)
        {
            //Detect event inputData and execute it
            IEventInvoker[] eventInvokers = handler.GetInput();
            if (eventInvokers != null)
            {
                foreach (var inv in eventInvokers)
                {
                    if (inv != null)
                    {
                        foreach (var scene in scenesRegistered)
                        {
                            inv.Execute(scene.Events);
                        }
                    }
                }
            }
        }


        if (contiunousHandlerList != null && contiunousHandlerList.Count > 0)
        {
            foreach (var handler in contiunousHandlerList)
            {
                IEventInvoker[] eventInvokers = handler.GetInput();
                if (eventInvokers != null)
                {
                    foreach (var inv in eventInvokers)
                    {
                        if (inv != null)
                        {
                            foreach (var scene in scenesRegistered)
                            {
                                inv.Execute(scene.Events);
                            }
                        }
                    }
                }
            }

        }
    }

}