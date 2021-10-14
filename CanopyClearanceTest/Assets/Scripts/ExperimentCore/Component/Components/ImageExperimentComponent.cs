using System.Collections;
using System.Collections.Generic;
using Leap.Unity.GraphicalRenderer;
using UnityEngine;

public class ImageExperimentComponent : ExperimentComponent {

    [Header("Frame Variables")]
    public GameObject background;
    private Renderer imgRenderer;

    [SerializeField]
    private Material material;

    new void Awake()
    {
        base.Awake();
        if (material == null)
            material = new Material(Shader.Find("UI/Unlit/Transparent"));
        if (material != null)
        {
            MainColor = new Color(1, 1, 1, 1);
            //material.renderQueue = 5000;
        }

        if (background == null && transform.childCount > 0)
            background = transform.GetChild(0).gameObject;

        if (background == null)
        {
            background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            background.transform.parent = transform;
            background.transform.position = Vector3.zero;
            background.transform.rotation = Quaternion.identity;
        }


        imgRenderer = background.GetComponent<Renderer>();

        if (imgRenderer == null)
            imgRenderer = background.AddComponent<MeshRenderer>();



        Texture2D prevTexture = null;
        Color prevMainColor = default(Color);
        if (imgRenderer.material != null)
        {
            RenderQueue = imgRenderer.material.renderQueue;
            prevTexture = imgRenderer.material.GetTexture("_MainTex") as Texture2D;
            prevMainColor = imgRenderer.material.GetColor("_Color");
        }

        if (prevTexture != null && texture == null)
            Texture = prevTexture;
        if (prevMainColor != default(Color) && color == default(Color))
            MainColor = prevMainColor;

        if (material != null && imgRenderer != null)
            imgRenderer.material = material;

        if (texture != null)
            Texture = texture;
        else if (rendTexture != null)
            RendTexture = rendTexture;
        
    }

    void Update()
    {
        if (material != null)
        {
            Vector2 materialOffset = material.GetTextureOffset("_MainTex");
            if (materialOffset.x > 1)
                materialOffset = new Vector2(materialOffset.x - 1, materialOffset.y);
            if (materialOffset.y > 1)
                materialOffset = new Vector2(materialOffset.x, materialOffset.y - 1);

            material.SetTextureOffset("_MainTex", (materialOffset + offsetMotionAmount));
        }
    }

    [SerializeField] private Color color = default(Color);
    [SerializeField] private Texture2D texture = null;

    [SerializeField]
    private Vector2 offsetMotionAmount = Vector2.zero;
    public void SetOffsetMotion(Vector2 motionAmount=default(Vector2))
    {
        offsetMotionAmount = motionAmount;
    }

    public Texture2D Texture
    {
        get
        {
            return texture;
        }
        set
        {
            texture = value;
            if (material != null)
                material.SetTexture("_MainTex", texture);
        }
    }
    public Color MainColor
    {
        get { return color; }
        set
        {
            color = value;
            material.SetColor("_Color", color);
        }
    }

    [SerializeField] private RenderTexture rendTexture = null;

    public RenderTexture RendTexture
    {
        get { return rendTexture; }
        set
        {
            rendTexture = value;
            if (material != null)
                material.SetTexture("_MainTex", rendTexture);
        }
    }

    public int RenderQueue
    {
        set {
            if (material!=null)
            {
                material.renderQueue = value;
            }
        }
        get {
            if (material!=null)
            {
                return material.renderQueue;
            }

            return 0;
        }
    }

    protected override void  HandleRenderQueueLevelAssignment(RenderQueueLevel order)
    {
        RenderQueue = (int) order;
    }


    public Material Material
    {
        get { return material; }
        set
        {
            if ( value!=null && imgRenderer != null)
            {
                material = value;
                imgRenderer.material = material;
            }
        }
    }
    
}
