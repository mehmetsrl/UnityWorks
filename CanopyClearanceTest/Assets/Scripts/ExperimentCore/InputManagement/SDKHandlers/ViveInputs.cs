using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ViveInputs : MonoBehaviour
{
    static ViveInputs _instance;

    public static ViveInputs Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }
    

    #region HandAccesors
    //-------------------------------------------------
    // Get the number of active Hands.
    //-------------------------------------------------
    public int handCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < hands.Length; i++)
                if (hands[i].gameObject.activeInHierarchy)
                    count++;
            return count;
        }
    }
    //-------------------------------------------------
    // Get the i-th active Hand.
    //
    // i - Zero-based index of the active Hand to get
    //-------------------------------------------------
    public Hand GetHand(int i)
    {
        for (int j = 0; j < hands.Length; j++)
        {
            if (!hands[j].gameObject.activeInHierarchy)
                continue;
            if (i > 0)
            {
                i--;
                continue;
            }
            return hands[j];
        }

        return null;
    }
    //-------------------------------------------------
    public Hand leftHand
    {
        get
        {
            for (int j = 0; j < hands.Length; j++)
            {
                if (!hands[j].gameObject.activeInHierarchy)
                    continue;
                if (hands[j].GuessCurrentHandType() != Hand.HandType.Left)
                    continue;
                return hands[j];
            }
            return null;
        }
    }
    //-------------------------------------------------
    public Hand rightHand
    {
        get
        {
            for (int j = 0; j < hands.Length; j++)
            {
                if (!hands[j].gameObject.activeInHierarchy)
                    continue;
                if (hands[j].GuessCurrentHandType() != Hand.HandType.Right)
                    continue;
                return hands[j];
            }
            return null;
        }
    }
    //-------------------------------------------------
    public SteamVR_Controller.Device leftController
    {
        get
        {
            Hand h = leftHand;
            if (h) return h.controller;
            return null;
        }
    }
    //-------------------------------------------------
    public SteamVR_Controller.Device rightController
    {
        get
        {
            Hand h = rightHand;
            if (h)
                return h.controller;
            return null;
        }
    }
    #endregion


    public Hand[] hands;
    [SerializeField]
    private GameObject rightTagHolder, leftTagHolder;
    public TextMesh rightHandTag, leftHandTag;
    private List<SteamVR_Controller.Device> devices;
    public SteamVR_Controller.Device activeDevice;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hands = FindObjectsOfType<Valve.VR.InteractionSystem.Hand>();
        activeDevice = null;

        devices=new List<SteamVR_Controller.Device>();

        //Collect hands
        for (int i = 0; i < hands.Length; i++)
        {
            StartCoroutine(CollectControllers(i));
        }

        InputManager.Instance.RootEvents.OnViveButtonUp.AddListener(OnButtonUp);

    }

   void  OnDestroy()
    {
        InputManager.Instance.RootEvents.OnViveButtonUp.RemoveAllListeners();
    }

    private void OnButtonUp(EventRegistery.InputData inputData)
    {
        EventRegistery.ViveInputData viveInputData = inputData as EventRegistery.ViveInputData;
        if (viveInputData != null)
        {
            if (viveInputData.button == EVRButtonId.k_EButton_Grip)
                SetAsActiveDevice(viveInputData.device);
        }
    }

    /// <summary>
    /// Try to gets calibration device till it awakes
    /// </summary>
    /// <param name="index">Device index</param>
    /// <returns></returns>
    IEnumerator CollectControllers(int index)
    {
        yield return new WaitWhile(() => { return hands[index].controller == null; });
        devices.Add(hands[index].controller);
    }




    /// <summary>
    /// Set new device as calibration device
    /// </summary>
    /// <param name="device">New device object</param>
    void SetAsActiveDevice(SteamVR_Controller.Device device)
    {
        if (activeDevice != null)
        {
            if (device != activeDevice)
                activeDevice = device;
        }
        else
        {
            activeDevice = device;
        }
    }

    /// <summary>
    /// It checks updates calibration device
    /// </summary>
    void UpdateCalibrationDevice()
    {
        if (devices != null)
        {
            //if calibration device is null or not connected select new calibration devices from candidates
            if (activeDevice == null || (activeDevice != null && !activeDevice.connected))
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].connected)
                    {
                        activeDevice = devices[i];
                        break;
                    }
                }
            }
        }
    }

    void UpdateHandTagPositions()
    {
        bool rightHandPlaced = false, leftHandPlaced = false;
        for (int i = 0; i < hands.Length; i++)
        {
            if (!hands[i].gameObject.activeInHierarchy)
                continue;
            if (hands[i].GuessCurrentHandType() == Hand.HandType.Right)
            {
                rightTagHolder.transform.parent = hands[i].transform;
                rightTagHolder.transform.localPosition=Vector3.zero;
                rightTagHolder.transform.localRotation=Quaternion.identity;
                rightHandPlaced =true;
                rightTagHolder.gameObject.SetActive(true);
                continue;
            }
            if (hands[i].GuessCurrentHandType() == Hand.HandType.Left)
            {
                leftTagHolder.transform.parent = hands[i].transform;
                leftTagHolder.transform.localPosition = Vector3.zero;
                leftTagHolder.transform.localRotation = Quaternion.identity;
                leftHandPlaced = true;
                leftTagHolder.gameObject.SetActive(true);
                continue;
            }

        }

        if(!leftHandPlaced) leftTagHolder.gameObject.SetActive(false);
        if(!rightHandPlaced) rightTagHolder.gameObject.SetActive(false);

    }
    
    void Update()
    {
        UpdateCalibrationDevice();

        UpdateHandTagPositions();

    }

}
