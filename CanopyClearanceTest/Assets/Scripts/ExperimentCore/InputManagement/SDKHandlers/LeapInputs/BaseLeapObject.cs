using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using Leap;
using Leap.Unity;
using Leap.Unity.Attachments;
using UnityEngine;
using System;


[System.Serializable]
public class BaseLeapObject : InteractionBehaviour
{

    public enum InteractionHands
    {
        Both,
        Right,
        Left
    }
    public InteractionHands ClickableHands = InteractionHands.Both;

    public Hand ClosestHand
    {
        get
        {
            Hand h = null;
            if (ClickableHands == InteractionHands.Both)
            {
                h = closestHoveringHand;
                return h;
            }
            else if (ClickableHands == InteractionHands.Left){
                h = Hands.Get(Chirality.Left);
                if (h != null && h.IsLeft)
                    return h;
            }
            else
            {
                h = Hands.Get(Chirality.Right);
                if (h!=null && h.IsRight)
                    return h;
            }
            return null;
        }
    }

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        manager = TAI_LeapInteractionManager.Instance;
    }

    void OnDestroy()
    {
        OnContactBegin = null;
        OnContactStay = null;
        OnContactEnd = null;
    }
    
}
