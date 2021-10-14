using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerExperimentComponent : ExperimentComponent {

    [Header("Player Variables")]
    public GameObject displayGO;
    public GameObject background;
    public GameObject controllerObject;

    public ButtonExperimentComponent playPauseButtonExperiment;
    public SliderExperimentComponent sliderExperiment;

    public VideoPlayer player;
    public Vector2 videoSize = new Vector2(1920, 1080);

    private RenderTexture rt;
    private MeshRenderer renderer;
    private double videoTime = 0;

    public Texture2D playIcon, pauseIcon;

    public Action OnPlayed,OnPaused;
    public Action<float> OnSlided;


    private bool isPlaying = false;
    public bool IsPlaying
    {
        get { return isPlaying; }
        private set { isPlaying = value; }
    }

    private bool _initialized = false;
    public bool IsActive
    {
        get { return isActive; }
    }

    public bool isActive = true;
    private bool playerOverrided = false;

    void Start()
    {
        if(_initialized) return;
        
        if (displayGO != null)
        {
            renderer = displayGO.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                if (rt == null)
                {
                    rt = new RenderTexture((int) videoSize.x, (int) videoSize.y, 0);

                    if (player == null)
                        player = GetComponent<VideoPlayer>();

                    if (player == null)
                        return;

                    player.targetTexture = rt;
                }

                renderer.
                    material = 
                    new Material(
                        Shader.Find("Unlit/Texture"));
                renderer.material.SetTexture("_MainTex", rt);
                
                sliderExperiment.HorizontalSliderValue = GetVideoTime();
                IsPlaying = false;
                _initialized = true;
            }
        }

        playPauseButtonExperiment.Texture = playIcon;

    }

    public void SetRenderer(RenderTexture rt)
    {
        this.rt = rt;
        if(!_initialized)Start();
        playerOverrided = true;
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Unlit/Texture"));
            renderer.material.SetTexture("_MainTex", rt);
        }
    }

    void OnDestroy()
    {
        OnPlayed = null;
        OnPaused = null;
        OnSlided = null;
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
        if (playPauseButtonExperiment != null)
            playPauseButtonExperiment.onPress.AddListener(OnPlayPauseClicked);

        if (sliderExperiment != null)
        {
            sliderExperiment.onPress.AddListener(OnSliderPress);
            sliderExperiment.onUnpress.AddListener(OnSliderUnPress);
            sliderExperiment._horizontalSlideEvent.AddListener(OnSlide);
        }
    }


    void UnbindEvents()
    {
        if (playPauseButtonExperiment != null)
            playPauseButtonExperiment.onPress.RemoveAllListeners();

        if (sliderExperiment != null)
        {
            sliderExperiment.onPress.RemoveAllListeners();
            sliderExperiment.onUnpress.RemoveAllListeners();
            sliderExperiment._horizontalSlideEvent.RemoveAllListeners();
        }
    }

    private void OnSliderUnPress()
    {
        IsPlaying = true;
        player.Play();
    }

    private void OnSliderPress()
    {
        if (player.isPlaying)
        {
            IsPlaying = false;
            player.Pause();
        }
    }

    private void OnSlide(float value)
    {
        SetVideoTime(value);

        if (OnSlided != null)
            OnSlided.Invoke(value);
    }

    private void OnPlayPauseClicked()
    {
        //Debug.Log("Video -> OnPlayed/OnPaused");

        if (IsPlaying)
        {
            IsPlaying = false;
            player.Pause();
            if (OnPaused != null)
                OnPaused.Invoke();
            playPauseButtonExperiment.Texture = playIcon;
        }
        else
        {
            if (player.isPrepared)
            {
                IsPlaying = true;
                player.Play();
            }
            else
                StartCoroutine(PrepareAndPlayVideo());

            if (OnPlayed != null)
                OnPlayed.Invoke();

            playPauseButtonExperiment.Texture = pauseIcon;
        }
    }

    public void Pause()
    {
        if (IsPlaying)
        {
            IsPlaying = false;
            player.Pause();
            if (OnPaused != null)
                OnPaused.Invoke();
            playPauseButtonExperiment.Texture = playIcon;
        }
    }

    IEnumerator PrepareAndPlayVideo()
    {
        player.Prepare();
        yield return new WaitUntil(() => player.isPrepared);
        IsPlaying = true;
        player.Play();
    }

    float GetVideoTime()
    {
        double time = ((double) player.frame / (double) player.frameCount);
        //Debug.Log();
        videoTime = time * ((double) player.frameCount / (double) player.frameRate);
        return (float)time;
    }

    void SetVideoTime(float timeInterval = 0f)
    {
        player.time = timeInterval / GetVideoTime() * player.time;
        videoTime = player.time;
    }

    public void SetSliderValue(float perc)
    {
        sliderExperiment.HorizontalSliderValue = perc;
    }
    
    void LateUpdate()
    {
        if(!playerOverrided)
        if (IsPlaying)
        {
            if (player.frameCount > 0)
            {
                sliderExperiment.HorizontalSliderValue = GetVideoTime();
            }
        }
        else
        {
            if (player.isPlaying)
            {
                player.Pause();
                player.time = videoTime;
            }
        }
    }
}
