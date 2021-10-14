using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance != null)
            {
                Destroy(value);
            }
            else
            {
                _instance = value;
            }
        }
    }
    static GameManager _instance;

    [SerializeField]
    Camera arCam;
    CustomInput mouseInput, touchInput;

    public GameObject cubeInfo;
    public Text debugText;
    void Awake()
    {
        Instance = this;

        mouseInput = new MouseInput(this, MouseInput.MouseKey.left);
        mouseInput.OnClick += OnClick;
        mouseInput.OnDrag += OnDrag;
        mouseInput.OnDragEnd += OnDragEnd;

        touchInput = new TouchInput(this);
        touchInput.OnClick += OnClick;
        touchInput.OnDrag += OnDrag;
        touchInput.OnDragEnd += OnDragEnd;

        touchInput.OnRotate += OnRotate;
        touchInput.OnZoom += OnZoom;
        touchInput.OnPan += OnPan;
    }

    private void OnDestroy()
    {
        mouseInput.OnClick = null;
        mouseInput.OnDrag = null;
        mouseInput.OnDragEnd = null;
        mouseInput.OnRotate = null;
        mouseInput.OnZoom = null;
        mouseInput.OnPan = null;


        touchInput.OnClick = null;
        touchInput.OnDrag = null;
        touchInput.OnDragEnd = null;
        touchInput.OnRotate = null;
        touchInput.OnZoom = null;
        touchInput.OnPan = null;
    }

    private void OnPan(InputEventArgs eventArgs)
    {
        if (hit.transform != null)
            hit.transform.Translate(eventArgs.coords * eventArgs.amount*Time.deltaTime*0.01f, Space.World);
    }

    private void OnRotate(InputEventArgs eventArgs)
    {
        inputRay = arCam.ScreenPointToRay(eventArgs.coords);
        if (hit.transform != null)
            hit.transform.RotateAround(inputRay.direction * Vector3.Distance(inputRay.origin, hit.transform.position), Vector3.forward, eventArgs.amount);

    }

    private void OnZoom(InputEventArgs eventArgs)
    {
        if (hit.transform != null)
            hit.transform.localScale += Vector3.one * eventArgs.amount * Time.deltaTime * 0.1f;

        //hit.transform.Translate((arCam.transform.position - hit.transform.position).normalized * eventArgs.amount * Time.deltaTime * 0.1f,Space.World);
        //debugText.text= "Zoom: "+eventArgs.amount;
    }

    Ray inputRay;
    RaycastHit hit;
    Vector2 previousPos = Vector2.zero;

    private void OnDrag(InputEventArgs eventArgs)
    {
        inputRay = arCam.ScreenPointToRay(eventArgs.coords);

        if (Physics.Raycast(inputRay, out hit) && previousPos != Vector2.zero)
        {
            Vector2 difference = eventArgs.coords - previousPos;
            hit.transform.RotateAround(hit.transform.position, Vector3.down, difference.x * Time.deltaTime * 5f);
            hit.transform.RotateAround(hit.transform.position, Vector3.right, difference.y * Time.deltaTime * 5f);

        }

        Debug.DrawRay(inputRay.origin, inputRay.direction * 100, Color.red);

        previousPos = eventArgs.coords;
    }

    private void OnDragEnd(InputEventArgs obj)
    {
        previousPos = Vector2.zero;
    }


    private void OnClick(InputEventArgs eventArgs)
    {
        inputRay = arCam.ScreenPointToRay(eventArgs.coords);

        if (Physics.Raycast(inputRay, out hit))
        {
            cubeInfo.SetActive(!cubeInfo.activeInHierarchy);
        }

        Debug.DrawRay(inputRay.origin, inputRay.direction * 100, Color.red);


    }

}
