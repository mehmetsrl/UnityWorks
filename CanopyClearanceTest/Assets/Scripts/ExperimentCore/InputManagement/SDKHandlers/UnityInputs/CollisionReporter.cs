using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour {
    
    void OnTriggerEnter(Collider c)
    {
        UnityInputs.Instance.NotifyCollisionEnter(gameObject,c.gameObject);
    }

    void OnTriggerExit(Collider c)
    {
        UnityInputs.Instance.NotifyCollisionExit(gameObject, c.gameObject);
    }
}
