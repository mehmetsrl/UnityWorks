using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using UnityEngine;
using System.Linq;

public class UnityInputs : MonoBehaviour
{

    static UnityInputs _instance;

    public static UnityInputs Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }

    public Queue<KeyCode> KeysPressed
    {
        get { return keysPressed; }
    }

    Queue<KeyCode> keysPressed = new Queue<KeyCode>();

    public Queue<int> JoystickInputs
    {
        get { return joystickInputCodes; }
    }

    Queue<int> joystickInputCodes = new Queue<int>();

    public Queue<float> ThrottleInputs
    {
        get { return throttleQueue; }
    }

    Queue<float> throttleQueue = new Queue<float>();


    public Queue<Vector2> StickInputs
    {
        get { return stickQueue; }
    }

    Queue<Vector2> stickQueue = new Queue<Vector2>();

    public List<GameObject> colObj = new List<GameObject>();
    private Dictionary<GameObject,GameObject> collidedObjects = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, GameObject> CollidedObjects
    {
        get { return collidedObjects; }
    }

    public bool CollisionListUpdated
    {
        get { return collisionListUpdated; }
    }
    private bool collisionListUpdated = false;
    

    void Awake()
    {
        Instance = this;
    }


    void FixedUpdate()
    {

        joystickInputCodes.Clear();
        throttleQueue.Clear();
        stickQueue.Clear();
    }

    void Update()
    {
        //if (Input.GetKeyDown("joystick button " + 14))
        //{
        //    GazeInputs.Instance.TriggerPress();
        //}

        ////if (Input.GetKeyDown(KeyCode.U))
        //if (Input.GetKeyUp("joystick button " + 14))
        //{
        //    GazeInputs.Instance.TriggerUnPress();
        //}

        //float stickAxisHorizontal = Input.GetAxis("Stick HrorizontalAxis");

        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                joystickInputCodes.Enqueue(i);
            }
        }


        throttleQueue.Enqueue(Input.GetAxis("Throttle"));

        stickQueue.Enqueue(new Vector2(Input.GetAxis("Stick HorizontalAxis"), Input.GetAxis("Stick VerticalAxis")));
    }

    public void NotifyCollisionEnter(GameObject reporterObject,GameObject collidedObject)
    {
        if(collidedObjects.ContainsKey(reporterObject))return;
        
        collidedObjects.Add(reporterObject, reporterObject);
        colObj.Add(reporterObject);

        collisionListUpdated = true;
    }
    public void NotifyCollisionExit(GameObject reporterObject, GameObject collidedObject)
    {
        if (collidedObjects.ContainsKey(reporterObject))
        {
            collidedObjects.Remove(reporterObject);

        }

        if (colObj.Contains(reporterObject))
            colObj.Remove(reporterObject);

        //Debug.Log(collidedObjects.Count);
        
        collisionListUpdated = true;
    }



    void LateUpdate () {
        keysPressed.Clear();
        keysPressed =new Queue<KeyCode>();

        collisionListUpdated = false;
    }

    void OnGUI()
    {
        keysPressed.Enqueue(Event.current.keyCode);
    }
}
