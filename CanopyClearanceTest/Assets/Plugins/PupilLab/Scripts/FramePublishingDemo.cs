using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramePublishingDemo : MonoBehaviour
{
    FramePublishing publisher;

    void OnEnable()
    {
        PupilTools.OnConnected += StartFramePublishing;
        PupilTools.OnDisconnecting += StopFramePublishing;


        PupilSettings.Instance.currentCamera = GetComponentInChildren<Camera>();
    }

    void StartFramePublishing()
    {


        if (publisher == null)
            publisher = gameObject.AddComponent<FramePublishing>();
        else
            publisher.enabled = true;

        if (publisher == null)
            Debug.Log("publisher is null in Frame Publishing");

    }

    void StopFramePublishing()
    {
        if (publisher != null)
            publisher.enabled = false;
    }

    void OnDisable()
    {
        PupilTools.OnConnected -= StartFramePublishing;
        PupilTools.OnDisconnecting -= StopFramePublishing;
    }
}
