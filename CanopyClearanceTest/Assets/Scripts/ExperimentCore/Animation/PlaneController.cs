using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[SerializeField]
public class ConstaintView
{
    [SerializeField]
    public RigidbodyConstraints movementConstraints;
}

public class PlaneController : MonoBehaviour
{
    private static PlaneController _instance;

    public static PlaneController Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance == null)
            {
                _instance = value;
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }

    [SerializeField] private RigidbodyConstraints movementConstraints;
    public Transform roll, pitch, yaw;
    [Range(0.01f,1)]
    public float rotationMultiplyer = .3f;
    [Range(50f,300f)]
    public float speedMultiplyer = 200f;

    public ConstaintView cons;


    private iTweenPath path;
    private Quaternion rollQuaternion = Quaternion.identity;
    private bool initialRotation = true;

    [SerializeField]
    float rollMultiplyer = 1.5f;
    [SerializeField]
    [Range(0f, .01f)]
    float correctionAmount = .005f;

    [Range(0f, 1f)] [SerializeField]
    private float time = 0;

    public float AnimTime
    {
        private get {return time;}
        set
        {
            if (value < 0) time = 0;
            else if (value > 1) time = 1;
            else time = value;

            if(OnAnimTimeUpdated!=null)
                OnAnimTimeUpdated.Invoke(time);
        }
    }

    public Action<float> OnAnimTimeUpdated;

    public float estimatedPathLength;

    void Awake ()
    {
        Instance = this;
        path = GetComponentInChildren<iTweenPath>();


    }

    void Start()
    {
        Vector3 firstPoint = iTween.PointOnPath(iTweenPath.GetPath(path.pathName), 0);
        Vector3 secondPoint = iTween.PointOnPath(iTweenPath.GetPath(path.pathName), 0.001f);
        float dist = Vector3.Distance(firstPoint, secondPoint);
        estimatedPathLength = dist * 1000;
    }

    public void Roll(float amount)
    {
        if (movementConstraints != RigidbodyConstraints.FreezeRotationZ)
            roll.localRotation =
                Quaternion.Euler(new Vector3(roll.localRotation.eulerAngles.x + amount * rotationMultiplyer*-1,0,0));
        //roll.rotation *= Quaternion.AngleAxis(amount * rotationMultiplyer, roll.forward);
    }

    public void Pitch(float amount)
    {
        if (movementConstraints != RigidbodyConstraints.FreezeRotationX)
            pitch.localRotation =
                Quaternion.Euler(new Vector3(0,0,pitch.localRotation.eulerAngles.z + amount * rotationMultiplyer));
        //pitch.rotation *= Quaternion.AngleAxis(amount * rotationMultiplyer, pitch.right);
    }

    public void Yaw(float amount)
    {
        if (movementConstraints != RigidbodyConstraints.FreezeRotationY)
            yaw.localRotation =
                Quaternion.Euler(new Vector3(0, yaw.localRotation.eulerAngles.y + amount * rotationMultiplyer, 0));
        //yaw.rotation *= Quaternion.AngleAxis(amount * rotationMultiplyer, yaw.up);
    }

    public void AdvanceOnPath(float amount)
    {
        if(estimatedPathLength==0) return;
        
        AnimTime += amount * speedMultiplyer * Time.deltaTime / estimatedPathLength;

        if (AnimTime >= 1 || AnimTime <= 0) return;

        iTween.PutOnPath(gameObject, iTweenPath.GetPath(path.pathName), AnimTime);

        Vector3 lookAtPoint =
            iTween.PointOnPath(iTweenPath.GetPath(path.pathName), (AnimTime + .001f));

        Vector3 lookAtDirection = new Vector3((lookAtPoint - transform.position).x, 0,
            (lookAtPoint - transform.position).z).normalized;

        float turnAmount = Vector3.SignedAngle(new Vector3(transform.forward.x, 0, transform.forward.z),
            lookAtDirection, Vector3.up);

        if (!initialRotation)
            rollQuaternion *= Quaternion.AngleAxis(turnAmount * rollMultiplyer, Vector3.back);



        transform.LookAt(lookAtPoint);
        transform.localRotation *= rollQuaternion;
        initialRotation = false;

        rollQuaternion = Quaternion.Slerp(rollQuaternion, Quaternion.identity, correctionAmount);
    }

    void Update()
    {
        ////AdvanceOnPath(0.001f);
    }

}
