using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


public class TweenEventArgs : EventArgs
{
    public TweenHandler tweenObject;
    public Hashtable tweenSettings;
    public TweenEventArgs(TweenHandler tweenObject, Hashtable tweenSettings = null)
    {
        this.tweenObject = tweenObject;
        this.tweenSettings = tweenSettings;
    }
}

public class TweenHandler : MonoBehaviour
{
    private Vector3 localPosition;
    private Quaternion localRotation;

    private Hashtable itweenHashtable;
    private Hashtable tweenToAnimate = new Hashtable();
    public Action<TweenEventArgs> OnAnimStart, OnAnimComplete;
    public Action<float, TweenEventArgs> OnAnimUpdate;

    public GameObject AnimatedObj
    {
        get
        {
            if (transform.childCount != 0)
                return transform.GetChild(0).gameObject;
            return null;
        }
    }

    public void SetTween(iTween.EaseType easeType, float time = 1f, float delay = 0f)
    {
        itweenHashtable = iTween.Hash(
            "onStart", "onAnimStart", "onStartTarget", gameObject, "onStartParams", this,
            "onComplete", "onAnimComplete", "onCompleteTarget", gameObject, "onCompleteParams", this,
            "onUpdate", "onAnimUpdate", "onUpdateTarget", gameObject);

        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
    }


    public void SetTween(Action<float, TweenEventArgs> whatToDoAction, iTween.EaseType easeType, float time = 1f,
        float delay = 0f)
    {
        itweenHashtable = iTween.Hash(
            "onStart", "onAnimStart", "onStartTarget", gameObject, "onStartParams", this,
            "onComplete", "onAnimComplete", "onCompleteTarget", gameObject, "onCompleteParams", this,
            "onUpdate", (Action<object>) (
                valParam =>
                {
                    TweenEventArgs args = new TweenEventArgs(this, tweenToAnimate);
                    whatToDoAction.Invoke((float) valParam, args);
                    onAnimUpdate(valParam);
                }
            ),
            "onUpdateTarget", gameObject);
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
    }

    public void ResetAnimation()
    {
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }

    void onAnimStart(object anim)
    {
        if (OnAnimStart != null)
            OnAnimStart.Invoke(new TweenEventArgs(this, tweenToAnimate));
    }

    void onAnimComplete(object anim)
    {
        if (OnAnimComplete != null)
            OnAnimComplete.Invoke(new TweenEventArgs(this, tweenToAnimate));
    }
    
    void onAnimUpdate(object anim)
    {
        if (OnAnimUpdate != null)
        {
            OnAnimUpdate.Invoke((float)anim, new TweenEventArgs(this, tweenToAnimate));
        }
    }
    
    public void PlayTween(float from,float to)
    {
        tweenToAnimate.Clear();
        tweenToAnimate = (Hashtable) itweenHashtable.Clone();
        tweenToAnimate.Add("from",from);
        tweenToAnimate.Add("to",to);

        AnimateTween(tweenToAnimate);
    }
    public void PlayTween(float from, float to, float time)
    {
        tweenToAnimate.Clear();
        tweenToAnimate = (Hashtable)itweenHashtable.Clone();
        tweenToAnimate.Add("from", from);
        tweenToAnimate.Add("to", to);
        tweenToAnimate.Add("time", time);

        AnimateTween(tweenToAnimate);
    }
    public void PlayTween(float from, float to, float time, float delay)
    {
        tweenToAnimate.Clear();
        tweenToAnimate = (Hashtable)itweenHashtable.Clone();
        tweenToAnimate.Add("from", from);
        tweenToAnimate.Add("to", to);
        tweenToAnimate.Add("time", time);
        tweenToAnimate.Add("delay", delay);

        AnimateTween(tweenToAnimate);
    }
    public void PlayWholeTween(float time)
    {
        tweenToAnimate.Clear();
        tweenToAnimate = (Hashtable)itweenHashtable.Clone();
        tweenToAnimate.Add("from", 0);
        tweenToAnimate.Add("to", 1);
        tweenToAnimate.Add("time", time);

        AnimateTween(tweenToAnimate);
    }
    public void PlayWholeTween(float time, float delay)
    {
        tweenToAnimate.Clear();
        tweenToAnimate = (Hashtable)itweenHashtable.Clone();
        tweenToAnimate.Add("from", 0);
        tweenToAnimate.Add("to", 1);
        tweenToAnimate.Add("time", time);
        tweenToAnimate.Add("delay", delay);

        AnimateTween(tweenToAnimate);
    }

    void AnimateTween(Hashtable tweenToAnimate)
    {
        iTween.ValueTo(gameObject,tweenToAnimate);
    }

    public void StopTween()
    {
        iTween.Stop(gameObject);
    }

}
