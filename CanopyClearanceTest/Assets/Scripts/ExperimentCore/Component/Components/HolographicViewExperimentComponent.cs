using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolographicViewExperimentComponent : ExperimentComponent
{
    private GameObject viewObject;
    public GameObject GlassBellGO;
    public GameObject CenterPivot;

    private Quaternion CenterPivotInitialRotation = Quaternion.identity;

    [Range(0.1f, 2)] public float ViewRadious = 0.2f;
    [Range(0, 0.1f)] public float ModalDistanceToGlass = 0.05f;
    [Range(-0.1f, 0.1f)] public float VerticalAllignment = 0.03f;

    Vector3 prevContactPos = Vector3.zero;
    private SphereCollider col;
    private Material wireframeMat;
    public ButtonExperimentComponent closeButton;
    public Texture2D scanLineTexture;
    public List<TextExperimentComponent> infoTexts = new List<TextExperimentComponent>();

    private bool firstInit = true;

    public bool IsDisplayingObject
    {
        get { return (viewObject != null); }
    }

    void Start()
    {

        if (CenterPivot == null)
        {
            CenterPivot = new GameObject("CenterPivot");
            CenterPivot.transform.parent = transform;
        }
        CenterPivot.transform.localPosition = Vector3.zero;
        CenterPivot.transform.localScale = Vector3.one;


        if (Type == ComponentType.Static3D)
        {
            GlassBellGO.transform.parent = null;
            GlassBellGO.transform.localScale = Vector3.one * ViewRadious;
            GlassBellGO.transform.parent = transform;
            GlassBellGO.transform.localPosition = Vector3.zero;
            GlassBellGO.SetActive(false);
            GlassBellGO.transform.parent = CenterPivot.transform;
            
            closeButton.gameObject.SetActive(false);


            col = GetComponent<SphereCollider>();
            if (col == null)
                col = gameObject.AddComponent<SphereCollider>();

            col.center = Vector3.zero;
            col.radius = ViewRadious / 2;
            col.enabled = false;


            //Implayer.IgnoreGrasping(false);

        }

        

        wireframeMat = new Material(Shader.Find("Zololgo/Sci-Fi Hologram Moving"));
        SetDefaultSciFiSettings();


        if (closeButton != null)
        {
            closeButton.onPress.AddListener(() =>
            {
                AnimateTweens(false,()=>
                {
                    HideObject();
                });
            });
        }

        OnItemUnhold += () =>
        {
            prevContactPos = Vector3.zero;
        };


        CenterPivotInitialRotation = CenterPivot.transform.localRotation;
        
    }

    void SetDefaultSciFiSettings()
    {

        wireframeMat.SetFloat("_Brightness", 4f);
        wireframeMat.SetFloat("_Fade", .25f);
        wireframeMat.SetFloat("_RimStrenght", 2f);
        wireframeMat.SetFloat("_RimFalloff", 0f);
        
        wireframeMat.SetTexture("_Scanlines", scanLineTexture);
        wireframeMat.SetTextureScale("_Scanlines",new Vector2(.05f,.8f));
        wireframeMat.SetTextureOffset("_Scanlines",new Vector2(.2f,-.3f));

        wireframeMat.SetFloat("_ScanStr", 1);

    }

    void Update()
    {
        //TODO iyileştirme yapılacak
        //if (CurrentInteraction == InteractionState.Holded)
        //{
        //    Vector3 contactPos = Implayer.GetContactItemPosition();
        //    if (contactPos == Vector3.positiveInfinity){ prevContactPos = Vector3.zero; return;}

        //    if (prevContactPos != Vector3.zero)
        //    {

        //        Vector3 movementVector = contactPos - CenterPivot.transform.position;
        //        Vector3 prevMovementVector = prevContactPos - CenterPivot.transform.position;
                
        //        float rotationY = Vector3.SignedAngle(prevMovementVector, movementVector,Vector3.up);
        //        //Debug.Log("movementY: " + rotationY);
        //        CenterPivot.transform.Rotate(Vector3.up, rotationY);

        //    }

        //    prevContactPos = contactPos;
        //}



        if (rotationAxis != Vector3.zero && rotationSpeed != 0)
            CenterPivot.transform.Rotate(rotationAxis, rotationSpeed);
    }


    public void SetRimColor(Color color)
    {
        wireframeMat.SetColor("_RimColor",color);
    }

    public void SetTintColor(Color color)
    {
        wireframeMat.SetColor("_Color", color);

    }
    public void SetWireColor(Color color)
    {
        //wireframeMat.SetColor("_WireColor", color);
        SetRimColor(color);
        SetTintColor(color);
    }

    public void DisplayObject(GameObject viewObject,string holoInfo ="",float scaleFactor =1f,Vector3 offset = default(Vector3) )
    {
        if (infoTexts != null&& infoTexts.Count>0)
        {
            foreach (var text in infoTexts)
            {
                text.Text = holoInfo;
            }
        }

        if (this.viewObject != null)
            Destroy(this.viewObject.gameObject);

        if (viewObject == null)
        {
            if (Type == ComponentType.Static3D)
            {
                GlassBellGO.SetActive(false);
                closeButton.gameObject.SetActive(false);
                col.enabled = false;
            }

            return;
        }

        //ResetRotation
        rotationAxis = Vector3.zero;
        rotationSpeed = 0f;

        if (Type == ComponentType.Static3D)
        {
            GlassBellGO.SetActive(true);
            closeButton.gameObject.SetActive(true);
            col.enabled = true;
            closeButton.SetVisibility(true);

            SetAutoRotation(Vector3.up, 1f);
        }



        Dictionary<MonoBehaviour, bool> lastStatus = new Dictionary<MonoBehaviour, bool>();

        foreach (var goComponents in viewObject.GetComponentsInChildren<MonoBehaviour>())
        {
            lastStatus.Add(goComponents,goComponents.enabled);
            goComponents.enabled = false;
        }


        this.viewObject = Instantiate(viewObject,CenterPivot.transform);
        this.viewObject.gameObject.name = "ViewObject";

        foreach (var goComponents in viewObject.GetComponentsInChildren<MonoBehaviour>())
        {
            if (lastStatus.ContainsKey(goComponents))
            {
                goComponents.enabled = lastStatus[goComponents];
            }
        }

        foreach (var goComponents in this.viewObject.GetComponentsInChildren<MonoBehaviour>())
        {
            DestroyImmediate(goComponents);
        }
        foreach (var goCollider in this.viewObject.GetComponentsInChildren<Collider>())
        {
            Destroy(goCollider);
        }
        foreach (var particle in this.viewObject.GetComponentsInChildren<ParticleSystem>())
        {
            Destroy(particle.gameObject);
        }
        foreach (var cam in this.viewObject.GetComponentsInChildren<Camera>())
        {
            cam.targetTexture = null;
            Destroy(cam.gameObject);
        }
        foreach (var childs in this.viewObject.GetComponentsInChildren<Transform>())
        {
            if(childs!=null)
                if (childs.tag == "Legent")
                    DestroyImmediate(childs.gameObject);
        }

        MeshFilter mesh = CombineMesh(ref this.viewObject);
        //Vector3 centerOfObj = GetCenter(ref this.viewObject);

        //TODO
        float biggestBound = Math.Max(mesh.mesh.bounds.size.x,
            Math.Max(mesh.mesh.bounds.size.y, mesh.mesh.bounds.size.z));
        if (biggestBound == 0)
            biggestBound = Single.Epsilon;

        float scaleRatio = ((ViewRadious / 2 - ModalDistanceToGlass) * transform.lossyScale.x) /
                            (biggestBound * 1 / this.viewObject.transform.lossyScale.x);


        if (Type == ComponentType.UI)
        {

            foreach (var mRend in this.viewObject.GetComponentsInChildren<MeshRenderer>())
            {
                mRend.materials = new Material[1];
                mRend.material = wireframeMat;
            }
        }

        

        this.viewObject.transform.parent = null;
        this.viewObject.transform.localScale = Vector3.one * scaleRatio * scaleFactor;
        this.viewObject.transform.parent = CenterPivot.transform;

        this.viewObject.transform.localRotation = Quaternion.identity;

        if (Type == ComponentType.UI)
        {
            this.viewObject.transform.Rotate(Vector3.right, 180f);
        }

        //this.viewObject.transform.Rotate(Vector3.right, 180f);
        //if (Type == ComponentType.UI && firstInit)
        //{
        //    this.viewObject.transform.Rotate(Vector3.left, 180f);
        //       firstInit = false;
        //}
        this.viewObject.transform.localPosition = offset + Vector3.up * VerticalAllignment;

    }
    public void SetCustomViewRotation(Quaternion rotation)
    {
        CenterPivot.transform.localRotation = CenterPivotInitialRotation * rotation;
    }

    Vector3 rotationAxis=Vector3.zero;
    float rotationSpeed=0f;
    public void SetAutoRotation(Vector3 axis,float speed)
    {
        rotationAxis = axis;
        rotationSpeed = speed;
    }
    

    public void HideObject()
    {
        //Destroy(this.viewObject.gameObject);
        if (viewObject != null)
            Destroy(this.viewObject.gameObject);
        this.viewObject = null;

        if (Type == ComponentType.Static3D)
        {
            GlassBellGO.SetActive(false);
            closeButton.gameObject.SetActive(false);
            col.enabled = false;
            closeButton.SetVisibility(false);
        }
    }
    
    //Not Wirking correctly
    private Vector3 GetCenter(ref GameObject viewObject)
    {
        Vector3 center=Vector3.zero;
        Transform[] childTransforms = viewObject.GetComponentsInChildren<Transform>();
        foreach (var childTransform in childTransforms)
        {
            if(childTransform==viewObject.transform) continue;
            center += childTransform.localPosition;
        }

        center /= childTransforms.Length;
        return center;
    }


    MeshFilter CombineMesh(ref GameObject obj)
    {
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        
        MeshFilter objMeshFilter = obj.GetComponent<MeshFilter>();

        if (objMeshFilter == null)
        {
            objMeshFilter = obj.AddComponent<MeshFilter>();
        }

        objMeshFilter.mesh = new Mesh();
        objMeshFilter.mesh.CombineMeshes(combine);

        return objMeshFilter;
    }

    protected override void HandleVisibility(bool visible)
    {

        CenterPivot.SetActive(visible);
        if (Type == ComponentType.Static3D)
        {
            GlassBellGO.SetActive(visible);
            closeButton.gameObject.SetActive(visible);
        }
        base.HandleVisibility(visible);
    }

}
