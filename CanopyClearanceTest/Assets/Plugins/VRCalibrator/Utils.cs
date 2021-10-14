using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static string sceneExtention = ".unity";
    public static void ReadSceneNames(out List<string> sceneNames)
    {
        sceneNames = new List<string>();
#if UNITY_EDITOR
        foreach (UnityEditor.EditorBuildSettingsScene sc in UnityEditor.EditorBuildSettings.scenes)
        {
            string name = sc.path.Substring(sc.path.LastIndexOf('/') + 1);
            if (name.Length > sceneExtention.Length)
            {
                name = name.Substring(0, name.Length - sceneExtention.Length);
                sceneNames.Add(name);
            }
        }
#else

#endif
    }
}

public static class ObjectStateExtensions
{

    public static IStateListener GetStateListener(this GameObject obj)
    {
        return obj.GetComponent<ObjectStateListener>() ?? obj.AddComponent<ObjectStateListener>();
    }
    public interface IStateListener
    {
        event Action Enabled;
        event Action Disabled;
    }
    class ObjectStateListener : MonoBehaviour, IStateListener
    {
        public event Action Enabled;
        public event Action Disabled;
        void Awake()
        {
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
        }
        void OnEnable()
        {
            TryInvoke(Enabled);
        }
        void OnDisable()
        {
            TryInvoke(Disabled);
        }


        private void TryInvoke(Action action)
        {
            if (action != null)
            {
                action();
            }
        }
    }
}