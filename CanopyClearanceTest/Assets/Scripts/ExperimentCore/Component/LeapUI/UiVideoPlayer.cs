using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Video;

public class UiVideoPlayer : UIComponent
{
    [Header("Player Variables")]
    public GameObject displayGO;
    public GameObject background;
    public GameObject controllerObject;

    public LeapButton playPauseButton;
    public LeapSlider slider;

    public VideoPlayer player;
    public Vector2 videoSize = new Vector2(1920, 1080);

    private RenderTexture rt;
    private MeshRenderer renderer;

    private bool _initialized = false;
    public bool IsActive
    {
        get { return isActive; }
    }

    private bool isActive = true;

    void Start()
    {
        if (displayGO != null)
        {
            renderer = displayGO.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                rt = new RenderTexture((int) videoSize.x, (int) videoSize.y, 0);

                if (player == null)
                    player = GetComponent<VideoPlayer>();

                if (player == null)
                    return;
                
                player.targetTexture = rt;

                renderer.material = new Material(Shader.Find("Unlit/Texture"));
                renderer.material.SetTexture("_MainTex", rt);
                

                _initialized = true;
            }
        }

    }

    void OnDestroy()
    {
    }

    protected override void HandleVisibility(bool visible)
    {
        if (visible)
        {
            displayGO.SetActive(visible);
            background.SetActive(visible);
            controllerObject.SetActive(visible);
            if (_initialized)
            {
                BindEvents();
            }
        }
        else
        {
            UnbindEvents();

            displayGO.SetActive(visible);
            background.SetActive(visible);
            controllerObject.SetActive(visible);
        }

        this.isActive = visible;
    }
    
    void BindEvents()
    {
        if (playPauseButton != null)
            playPauseButton._onPress.AddListener(OnPlayPauseClicked);

        if (slider != null)
        {
            slider._onPress.AddListener(OnSliderPress);
            slider._onUnpress.AddListener(OnSliderUnPress);
            slider._horizontalSlideEvent.AddListener(OnSlide);
        }
    }


    void UnbindEvents()
    {
        if (playPauseButton != null)
            playPauseButton._onPress.RemoveAllListeners();

        if (slider != null)
        {
            slider._onPress.RemoveAllListeners();
            slider._onUnpress.RemoveAllListeners();
            slider._horizontalSlideEvent.RemoveAllListeners();
        }
    }

    private void OnSliderUnPress()
    {
        player.Play();
    }

    private void OnSliderPress()
    {
        if (player.isPlaying)
            player.Pause();
    }
    
    private void OnSlide(float value)
    {
        SetVideoTime(value);
    }

    private void OnPlayPauseClicked()
    {
        //Debug.Log("Video -> OnPlayed/OnPaused");

        if (player.isPlaying)
        {
            player.Pause();
        }
        else
        {
            //player.OnPlayed();
            if (player.isPrepared)
                player.Play();
            else
                StartCoroutine(PrepareAndPlayVideo());
        }
    }

    IEnumerator PrepareAndPlayVideo()
    {
        player.Prepare();
        yield return new WaitUntil(() => player.isPrepared);
        player.Play();
    }

    float GetVideoTime()
    {
        return (float)((double)player.frame / (double)player.frameCount);
    }

    void SetVideoTime(float timeInterval=0f)
    {
        player.time = timeInterval / GetVideoTime() * player.time;
    }

    void LateUpdate()
    {
        if (player.isPlaying)
        {
            if (player.frameCount > 0)
            {
                slider.HorizontalSliderValue = GetVideoTime();
            }
        }
    }

}
