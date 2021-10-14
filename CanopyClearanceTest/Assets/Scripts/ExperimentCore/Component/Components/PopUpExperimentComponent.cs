using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpExperimentComponent : ExperimentComponent
{

    public InteractiveScreenExperimentComponent screenExperiment;

    [SerializeField]
    private bool hold = false;

    [SerializeField]
    private bool placed = false;
    public bool IsPlaced
    {
        get { return PlacedSlot != null; }
    }

    private SlotExperimentComponent placedSlot = null;
    private SlotExperimentComponent placedOutSlot = null;
    public SlotExperimentComponent PlacedSlot
    {
        get { return placedSlot;}
        set
        {
            placedSlot = value;
            placed = (placedSlot != null);
        }
    }

    public ImageExperimentComponent display;
    public GameObject Borders;
    private Renderer[] bordeRenderers;

    [SerializeField]
    private Vector3 initialScale=default(Vector3), finalScale=default(Vector3);

    //Temprary Solution for rigidbody errors
    public GameObject resetGameObject;

    public Texture2D popOutMask, placedMask;

    private Material displayMaterial;

    public Collider hideCollider, showCollider;

    // Use this for initialization
    void Start ()
    {
        if (Borders != null)
            bordeRenderers = Borders.GetComponentsInChildren<MeshRenderer>();

        OnItemHold += HoldItem;

        OnItemUnhold += UnholdItem;

        OnItemMove += MoveItem;



        if (initialScale != finalScale)
        {
            SetScaleTween(transform, finalScale, initialScale);
        }

        displayMaterial = new Material(Shader.Find("Custom/MaskedTexture"));

        if (placed)
        {
            PlacedSlot = screenExperiment.InserItem(this);
        }
        else
        {
            placedOutSlot = screenExperiment.RemoveItem(this);
        }
        UnholdItem();
    }

    void UpdateDisplay()
    {
        if (display != null)
        {
            RenderTexture prevRenderTexture = display.RendTexture;
            if (prevRenderTexture == null)
            {
                Texture2D prevTexture2D = display.Texture;
                display.Material = displayMaterial;
                display.Texture = prevTexture2D;

            }
            else
            {
                display.Material = displayMaterial;
                display.RendTexture = prevRenderTexture;
            }

        }
    }

    void HoldItem()
    {
        hold = true;
        if (PlacedSlot != null)
        {
            PlacedSlot.RemoveItem();
            PlacedSlot = null;
        }
        if (placedOutSlot != null)
        {
            placedOutSlot.RemoveItem();
            placedOutSlot = null;
        }
        PopUpView(true);
    }

    void UnholdItem()
    {
        hold = false;
        if (screenExperiment.IsIn(transform.position))
        {
            if(!placed)
            PlacedSlot = screenExperiment.InserItem(this);
        }
        else
        {
            placedOutSlot = screenExperiment.RemoveItem(this);
        }

        PopUpView(!IsPlaced);
    }

    private bool popedOutPrev = true;

    void PopUpView(bool isPoped = true)
    {
        if (isPoped != popedOutPrev)
        {
            SetBorderVisibility(isPoped);
            if (isPoped)
            {
                Mask = popOutMask;
                display.RenderQueue = (int)ExperimentComponent.RenderQueueLevel.Base;
                UpdateDisplay();
                AnimateTweens(false);
                hideCollider.gameObject.SetActive(false);
                showCollider.gameObject.SetActive(true);
            }
            else
            {
                display.RenderQueue = (int)ExperimentComponent.RenderQueueLevel.Level1;

                AnimateTweens(true, () =>
                {
                    Mask = placedMask;
                    display.RenderQueue = (int)ExperimentComponent.RenderQueueLevel.Level1;
                    UpdateDisplay();
                    display.RenderQueue = (int)ExperimentComponent.RenderQueueLevel.Level1;

                    hideCollider.gameObject.SetActive(true);
                    showCollider.gameObject.SetActive(false);
                });
                display.RenderQueue = (int)ExperimentComponent.RenderQueueLevel.Level1;
            }

            popedOutPrev = isPoped;
        }
    }


    void MoveItem(Vector3 movePos)
    {
        transform.position = movePos; /*transform.rotation=Implayer.*//*Debug.Log(Implayer.GetContactItem().name);*/
    }

    void SetBorderVisibility(bool enabled = true)
    {
        if(bordeRenderers!=null)
            foreach (var rend in bordeRenderers)
            {
                rend.enabled = enabled;
            }
    }

    private Vector3 offsetPos = Vector3.zero;
    private Quaternion offsetRot = Quaternion.identity;

    //TODO: 
	// Update is called once per frame
	void Update () {
        if (CurrentInteraction == InteractionState.Holded)
        {
            GameObject ContactItem = Implayer.GetContactItem();
            if (ContactItem == null) return;

            if (offsetPos == Vector3.positiveInfinity)
                offsetPos = transform.position - ContactItem.transform.position;

            //if (offsetRot == Quaternion.identity)
            //    offsetRot = ContactItem.transform.rotation * Quaternion.Inverse(transform.rotation);

            transform.position = ContactItem.transform.position - offsetPos;
            //transform.rotation = ContactItem.transform.rotation;// * offsetRot;
            //transform.LookAt(Camera.main.transform);

            //Debug.Log(ContactItem.name);
        }
        else
        {
            offsetPos = Vector3.zero;
            //offsetRot = Quaternion.identity;
        }
    }

    void LateUpdate()
    {
        if (resetGameObject != null)
            resetGameObject.transform.localRotation = Quaternion.identity;
    }


    private Texture2D Mask
    {
        get { return displayMaterial.GetTexture("_Mask") as Texture2D; }
        set { displayMaterial.SetTexture("_Mask", value); }
    }
    
}
