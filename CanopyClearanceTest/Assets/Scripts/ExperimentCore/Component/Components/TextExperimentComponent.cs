using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextExperimentComponent : ExperimentComponent
{
    public enum FontType
    {
        Arial,
        ArialBD,
        ArialBI,
        ArialI,
        AriBLK,
        Imagine,
        InconsolataRegular,
        InconsolataBold
    }
    static Dictionary<FontType,Font> fontList;
    private Material fontMaterial;
    private Renderer textRenderer;

    [SerializeField]
    private FontType selectedFontType;
    protected new void Awake()
    {
        base.Awake();
        if (fontList == null)
        {
            fontList = new Dictionary<FontType, Font>();
            fontList.Add(FontType.Arial, Resources.Load("Fonts/FontData/ARIAL") as Font);
            fontList.Add(FontType.ArialBD, Resources.Load("Fonts/FontData/ARIALBD") as Font);
            fontList.Add(FontType.ArialBI, Resources.Load("Fonts/FontData/ARIALBI") as Font);
            fontList.Add(FontType.ArialI, Resources.Load("Fonts/FontData/ARIALI") as Font);
            fontList.Add(FontType.AriBLK, Resources.Load("Fonts/FontData/ARIBLK") as Font);
            fontList.Add(FontType.Imagine, Resources.Load("Fonts/FontData/Imagine") as Font);
            fontList.Add(FontType.InconsolataRegular, Resources.Load("Fonts/FontData/InconsolataRegular") as Font);
            fontList.Add(FontType.InconsolataBold, Resources.Load("Fonts/FontData/InconsolataBold") as Font);
        }

        fontMaterial = new Material(Shader.Find("GUI/3DTextShaderOneSided"));
        fontMaterial.SetTexture("_MainTex", fontList[selectedFontType].material.GetTexture("_MainTex"));

        text.font = fontList[selectedFontType];

        textRenderer = text.GetComponent<Renderer>();
        
        if (textRenderer != null)
        {
            fontMaterial.renderQueue = textRenderer.material.renderQueue;
            textRenderer.material = fontMaterial;
            
        }
    }

    public void SetFont(FontType font)
    {
        if (textRenderer == null) return;

        selectedFontType = font;
        text.font = fontList[selectedFontType];
        fontMaterial.SetTexture("_MainTex", fontList[selectedFontType].material.GetTexture("_MainTex"));
    }

    public void SetColor(Color fontColor)
    {
        if (textRenderer == null) return;
        fontMaterial.SetColor("_Color", fontColor);

    }

    [SerializeField]
    private TextMesh text;

    public string Text
    {
        get { return text.text;}
        set { text.text = value; }
    }

    public void SetTextSize(float size) { text.fontSize=(int)size; }

    public void SetTextAllignment(TextAlignment allignment){text.alignment = allignment;}
    public void SetTextAnchor(TextAnchor anchor){text.anchor = anchor; }

    [SerializeField]
    private float minVal=default(float), maxVal=default(float);
    
    [SerializeField] private int precision = 0;
    public void SetRandomRangeNumber(float minVal,float maxVal, int precision = 0)
    {
        this.minVal = minVal;
        this.maxVal = maxVal;
        this.precision = precision;
    }

    [SerializeField] private float changeAmount = default(float);
    public void SetRandomRelativeNumber(float minVal, float maxVal,float changeAmount, int precision = 0)
    {
        this.minVal = minVal;
        this.maxVal = maxVal;
        this.changeAmount = changeAmount;
        this.precision = precision;
    }

    [SerializeField] private float textUpdateTime = 1f;
    public void SetTextUpdatePeriodLength(float textUpdateTime)
    {
        this.textUpdateTime = textUpdateTime;
    }

    public float LineSpacing
    {
        get { return text.lineSpacing; }
        set { text.lineSpacing = value; }
    }

    float val = 0;
    private float changeTime = 0f;
    void Update()
    {
        if (changeTime > textUpdateTime)
        {
            changeTime = 0f;
            if (minVal != default(float) && maxVal != default(float))
            {

                if (changeAmount != default(float))
                {

                    val = Random.Range(minVal, maxVal);

                    val = Mathf.Round(val * Mathf.Pow(10, precision)) / Mathf.Pow(10, precision);
                }
                else
                {
                    if (val == 0)
                        val = minVal;

                    bool isIncrease = Random.Range(0, 2) > 1;

                    if (isIncrease)
                    {
                        if (val + changeAmount <= maxVal)
                            val += changeAmount;
                        else
                            val -= changeAmount;
                    }
                    else
                    {
                        if (val - changeAmount >= minVal)
                            val -= changeAmount;
                        else
                            val += changeAmount;
                    }
                }

                Text = val.ToString();
            }
        }
        else
        {
            changeTime += Time.deltaTime;
        }
    }


    protected override void HandleRenderQueueLevelAssignment(RenderQueueLevel order)
    {
        if (fontMaterial != null)
            fontMaterial.renderQueue = (int)order;
    }
    protected override void HandleVisibility(bool visible)
    {
        if (textRenderer != null)
            textRenderer.enabled = visible;
    }
}
