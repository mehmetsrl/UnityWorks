using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class CircularPositionCreator : MonoBehaviour
{

    private Dictionary<float, TweenHandler> positionObjects = new Dictionary<float, TweenHandler>();
    public List<Vector2> ignoredAngleIntervals = new List<Vector2>();
    public List<int> validPositionIndexes = new List<int>();
    private List<int> animationPool;

    public int numberOfFighters = 0;
    public float angleDif = 0;
    public int precision = 3;

    public float radious = 0;

    public GameObject FighterModal;
    public int forwardBackwardMoveLimit = 250;
    public float forwardMovementMultiplyer = 5f;
    [Range(0.001f, 0.1f)]
    public float animSpeed = .05f;

    public iTween.EaseType animEaseType = iTween.EaseType.linear;
    
    public Action<int> OnAnimationPlayed;

    public bool fromInfiniteToOurPlane = false;

    public float AngleFromIndex(int ind)
    {
        return (ind + 1) * angleDif; 
    }
    // Use this for initialization
    void Start()
    {
        if (angleDif == 0 && numberOfFighters == 0)
            return;
        if (angleDif == 0)
            angleDif = (float)360 / (float)numberOfFighters;
        else
            numberOfFighters = (int)(360 / angleDif);


        for (int i = 0; i < ignoredAngleIntervals.Count; i++)
        {
            ignoredAngleIntervals[i]=new Vector2(ignoredAngleIntervals[i].x- angleDif, ignoredAngleIntervals[i].y + angleDif);
        }


        for (int i = 0; i < numberOfFighters; i++)
        {
            float angle = (i + 1) * angleDif;
            TweenHandler fighterPosition = new GameObject("FighterPosition_" + i +"_"+ angle).AddComponent<TweenHandler>();
            fighterPosition.transform.parent = transform;

            positionObjects.Add(angle, fighterPosition);

            Vector3 pos = GetCirclePoint(radious, angle);
            fighterPosition.transform.position =
                transform.TransformDirection(pos - new Vector3(0, 0, forwardBackwardMoveLimit)) + transform.position;

            Vector3 initialPos = fighterPosition.transform.position;
            Vector3 targetPos = fighterPosition.transform.position +
                                transform.TransformDirection(new Vector3(0, 0,
                                    forwardBackwardMoveLimit * forwardMovementMultiplyer));

            //fighterPosition.SetTween(AnimateFighter, animEaseType);
            fighterPosition.SetTween((float time, TweenEventArgs args) => AnimateFighterToSpesificPosition(time, args, initialPos,targetPos), animEaseType);

            
            if (IsValidPosition(angle))
                validPositionIndexes.Add(i);

            //For rotations if needed 
            //positionObjects[i].transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);


            //To virtualize all positions
            //GameObject f16 = Instantiate(FighterModal, fighterPosition.transform.position, FighterModal.transform.rotation);
            //f16.transform.parent = fighterPosition.transform;
        }

        FillAnimationPool(ref validPositionIndexes);

        isAnimating = false;

        //ForTesting
        //index = GetClosestValidAngleIndex(0);
        //prevIndex = index;
        //PlaceFighterAt(index);

    }


    public void FillAnimationPool(ref List<int> positionIndexes)
    {
        animationPool = new List<int>(positionIndexes);
        animationPool = positionIndexes.ToList();
    }

    public bool PlayRandomAnimationFromPool(float animDelay = 0,bool reloadPoolIfEmpty=false)
    {

        if (animationPool != null)
        {
            if (animationPool.Count > 0)
            {

                int randIndex = Random.Range(0, animationPool.Count - 1);

                index = animationPool[randIndex];
                PlaceFighterAt(index);
                PlayAnimation(animDelay);
                return true;
            }
            else if(reloadPoolIfEmpty)
            {
                FillAnimationPool(ref validPositionIndexes);
                return PlayRandomAnimationFromPool(animDelay, false);
            }
        }

        return false;
    }

    float GetClosestValidAngle(float desiredAngle)
    {
        //bool angleHandled = false;
        for (int i = 0; i < ignoredAngleIntervals.Count; i++)
        {

            if (ignoredAngleIntervals[i].y < ignoredAngleIntervals[i].x)
                ignoredAngleIntervals[i] = new Vector2(ignoredAngleIntervals[i].y, ignoredAngleIntervals[i].x);


            //TODO: Check desiredAngle is in angle intervals
            if (!(desiredAngle > ignoredAngleIntervals[i].x && desiredAngle < ignoredAngleIntervals[i].y))
            {
                continue;
            }


            Vector2 unsignedIntervals = ignoredAngleIntervals[i];

            if (ignoredAngleIntervals[i].x < 0 || ignoredAngleIntervals[i].y < 0)
            {
                unsignedIntervals = new Vector2(
                    ignoredAngleIntervals[i].x < 0
                        ? 360 + ignoredAngleIntervals[i].x
                        : ignoredAngleIntervals[i].x,
                    ignoredAngleIntervals[i].y < 0
                        ? 360 + ignoredAngleIntervals[i].y
                        : ignoredAngleIntervals[i].y);
            }



            if (Mathf.Abs(ignoredAngleIntervals[i].y - desiredAngle) >
                Mathf.Abs(desiredAngle - ignoredAngleIntervals[i].x))
                desiredAngle = (Mathf.Ceil(unsignedIntervals.x / angleDif + 1)) * angleDif;
            else
                desiredAngle = (Mathf.Ceil(unsignedIntervals.y / angleDif + 1)) * angleDif;

            //angleHandled = true;
        }

        //if (angleHandled)
            return desiredAngle;
        //else
        //    return float.NaN;
    }

    public float GetClosestAngle(float angle)
    {
        float desiredVal = angle + 360;
        foreach (float angles in positionObjects.Keys)
        {
            if (Mathf.Abs(Mathf.Abs(angles) - Mathf.Abs(angle)) < Mathf.Abs(Mathf.Abs(desiredVal) - Mathf.Abs(angle)))
                desiredVal = angles;
        }

        return desiredVal;
    }

    int GetClosestValidAngleIndex(int desiredAngleIndex)
    {
        float desiredAngle = GetClosestValidAngle((desiredAngleIndex + 1) * angleDif);

        return ((int)(desiredAngle / angleDif)) - 1;
    }

    bool IsValidPosition(float desiredAngle)
    {
        return (Mathf.Abs(desiredAngle - GetClosestValidAngle(desiredAngle)) == 0);
    }



    void AnimateFighter(float time, TweenEventArgs args)
    {
        GameObject fighter = args.tweenObject.gameObject;

        fighter.transform.localPosition = Vector3.Lerp(fighter.transform.localPosition,
            fighter.transform.localPosition + (new Vector3(0, 0, forwardBackwardMoveLimit)), time);
    }
    void AnimateFighterToSpesificPosition(float time, TweenEventArgs args, Vector3 initialPos, Vector3 targetPos)
    {
        TweenHandler fighter = args.tweenObject;
        Vector3 initialPos1 = initialAnimPos;
        Vector3 targetPos1 = initialAnimPos +
                            transform.TransformDirection(new Vector3(0, 0,
                                forwardBackwardMoveLimit * forwardMovementMultiplyer));


        //GameObject fighter = args.tweenObject.gameObject;

        if (fromInfiniteToOurPlane)
        {
            fighter.transform.position = Vector3.Lerp(targetPos1, initialPos1, time);
            fighter.AnimatedObj.transform.LookAt(initialPos1);
        }
        else
        {
            fighter.transform.position = Vector3.Lerp(initialPos1, targetPos1, time);
            fighter.AnimatedObj.transform.LookAt(targetPos1);
        }

    }

    public bool isAnimating { get; private set; }
    int prevIndex;
    private Vector3 initialAnimPos = Vector3.zero;
    public void PlayAnimation(float animDelay = 0)
    {
        if(OnAnimationPlayed!=null)
            OnAnimationPlayed.Invoke(index);

        if (isAnimating)
        {
            positionObjects.ElementAt(index).Value.StopTween();
            OnFighterPassed(new TweenEventArgs(positionObjects.ElementAt(index).Value, null));
        }

        initialAnimPos = positionObjects.ElementAt(index).Value.transform.position;

        isAnimating = true;
        positionObjects.ElementAt(index).Value.OnAnimComplete += OnFighterPassed;
        float animTime = 1f / animSpeed;
        positionObjects.ElementAt(index).Value.PlayWholeTween(animTime, animDelay);
    }

    public void PlaySpesificAnimation(int posIndex, float animDelay = 0)
    {
        
        positionObjects.ElementAt(index).Value.StopTween();
        OnFighterPassed(new TweenEventArgs(positionObjects.ElementAt(index).Value, null));
        
        index = posIndex;
        PlaceFighterAt(index);
        PlayAnimation(animDelay);
    }


    public void PlayClosestAngleAnimation(float angle, float animDelay = 0)
    {
        float desiredAngle = GetClosestAngle(angle);
        TweenHandler anim;
        if (positionObjects.TryGetValue(desiredAngle, out anim))
        {
            PlaySpesificAnimation(positionObjects.Keys.ToList().IndexOf(desiredAngle));
        }
    }

    private void OnFighterPassed(TweenEventArgs obj)
    {
        obj.tweenObject.StopTween();
        obj.tweenObject.OnAnimComplete -= OnFighterPassed;
        obj.tweenObject.ResetAnimation();
        isAnimating = false;
    }


    public int index = 0;
    public float viewAngle = 0f;
    public float forwardIndex = 0;

    public void RotateFighterForward()
    {
        prevIndex = index;
        index++;
        if (index >= positionObjects.Count) index = 0;
        PlaceFighterAt(index);
    }
    public void RotateFighterBackward()
    {
        prevIndex = index;
        index--;
        if (index < 0) index = positionObjects.Count - 1;
        PlaceFighterAt(index);
    }

    public void MoveFighterForward(float amount = 1f)
    {
        forwardIndex += amount;
        if (forwardIndex >= forwardBackwardMoveLimit) forwardIndex = forwardBackwardMoveLimit;
        ForwardFighter(forwardIndex);
    }
    public void MoveFighterBackward(float amount = 1f)
    {
        forwardIndex -= amount;
        if (forwardIndex < -forwardBackwardMoveLimit) forwardIndex = -forwardBackwardMoveLimit;
        ForwardFighter(forwardIndex);
    }



    public void PlaceFighterAt(int order)
    {
        viewAngle = order * angleDif;
        if (isAnimating)
        {
            positionObjects.ElementAt(prevIndex).Value.StopTween();
            OnFighterPassed(new TweenEventArgs(positionObjects.ElementAt(prevIndex).Value, null));
        }

        FighterModal.transform.parent = positionObjects.ElementAt(order).Value.transform;
        FighterModal.transform.localPosition = offset;

    }


    private Vector3 offset = Vector3.zero;

    public void ForwardFighter(float amount)
    {
        if (isAnimating)
        {
            positionObjects.ElementAt(index).Value.StopTween();
            OnFighterPassed(new TweenEventArgs(positionObjects.ElementAt(index).Value, null));
        }

        offset = transform.TransformDirection(new Vector3(0, 0, amount));
        FighterModal.transform.localPosition = offset;
    }

    Vector3 GetCirclePoint(float radious, float angle = 0)
    {
        return new Vector3(
            radious * Mathf.Sin((angle * Mathf.Deg2Rad)),
            radious * Mathf.Cos((angle * Mathf.Deg2Rad)),
            0);
    }

}
