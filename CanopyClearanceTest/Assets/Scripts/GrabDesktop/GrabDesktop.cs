using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScreenImageCapture.Utility;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using CielaSpike;
using System.Threading;

public class GrabDesktop : MonoBehaviour, ILazy
{
    
    public string windowName;
    private string wasWindowName = "";
    private int nWinHandle;
    private GameObject surface;
    public int top = 0;
    public int left = 0;
    public int height = 500;
    public int width = 500;

    private Texture2D tex;
    private Renderer renderMat;
    private bool yFlipped = false;

    ScreenCaptureImage sc = new ScreenCaptureImage();
    IntPtr iH = (IntPtr)null;
    bool threadRunning = false;
    Bitmap bitmap = null;
    Thread _thread;
    
    static ReStartHelper helper;
    static GrabDesktop instance;
    public static GrabDesktop Instance
    {
        get { return instance; }
        private set
        {
            if(instance==null) instance = value;
        }
    }
    static ReStartHelper Helper
    {
        get { return helper; }
        set
        {
            if (helper == null) helper = value;
            else
            {
                Destroy(value.gameObject);
            }
        }
    }


    // Use this for initialization
    void Start() {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;

        Instance = this;
        Helper = new GameObject("LazyHelper").AddComponent<ReStartHelper>();

        tex = new Texture2D(256, 256);
        if (surface == null)
        {
            renderMat = GetComponent<Renderer>();
        }
        else
        {
            renderMat = surface.GetComponent<Renderer>();
        }

        StartCoroutine(NinjaThread());

        helper.Lazy(Instance, 0.5f);


    }

    bool lazyStarted = false;
    public void LazyMethod(bool activated)
    {
        if (lazyStarted)
        {
            if (activated)
            {
                gameObject.SetActive(true);
                GameObject.Destroy(helper.gameObject);
            }
        }
        else
        {
            if (activated)
            {
                lazyStarted = true;
                gameObject.SetActive(false);
                helper.Lazy(Instance, 5f);
            }
        }
    }
    

    IEnumerator NinjaThread()
    {
        Task task;
        this.StartCoroutineAsync(GrabDesktopWork(), out task);
        yield return StartCoroutine(task.Wait());
        CheckWindowName();
        StartCoroutine(NinjaThread());
    }
    IEnumerator GrabDesktopWork()
    {
        GrabDesktopImp();
        yield return Ninja.JumpToUnity;
        SetTextureImp();
    }

    void GrabDesktoThreadpWork()
    {
        threadRunning = true;
        bool workDone = false;
        while (threadRunning && !workDone)
        {
            GrabDesktopImp();
        }
        threadRunning = false;
    }

    void CheckWindowName()
    {
        iH = (IntPtr)null;
        // Grab selected screenExperiment image
        if (windowName.Length > 0)
            iH = sc.WinGetHandle(windowName); // If there is one, use Window name to find window
    }

    byte[] byteArrayDesktopImage = null;
    void GrabDesktopImp()
    {
        if (renderMat != null)
        {
            if (iH != (IntPtr)null || (width * height > 0))
            {
                bitmap = sc.GetScreenshot(iH, top, left, height, width);  // Get window contents, using window handle to ident window or screenExperiment rect area
                if (bitmap != null)
                {
                    // Wrap screenExperiment image onto object surface, as albedo colour wrap
                    byteArrayDesktopImage = sc.BitmapToByteArray(bitmap);
                }
                else
                {
                    DefaultPic();
                }
            }
            else
            {
                DefaultPic();
            }
        }
        else
        {
            DefaultPic();
        }
    }

    void SetTextureImp()
    {
        if (byteArrayDesktopImage != null)
        {
            //Debug.Log(bitmap.Width);
            tex.Resize(bitmap.Width, bitmap.Height, TextureFormat.BGRA32, false);
            tex.LoadRawTextureData(byteArrayDesktopImage);
            tex.Apply();

            renderMat.material.SetTexture("_MainTex", tex);

            if (!yFlipped)
                FlipY();
        }
        else
        {
            DefaultPic();
        }
        bitmap.Dispose();
    }

    void DefaultPic()
    {
        if (yFlipped)
        {
            renderMat.material.SetTexture("_MainTex", Resources.Load("No Window") as Texture2D);
            FlipY();
            nWinHandle = 0;
        }
    }

    void FlipY()
    {
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        yFlipped = !yFlipped;
    }

}





