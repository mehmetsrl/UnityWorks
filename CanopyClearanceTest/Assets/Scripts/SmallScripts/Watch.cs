using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : MonoBehaviour {

    TextMesh tm;

	// Use this for initialization
	void Start () {
        tm = GetComponentInChildren<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        tm.text = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
	}
}
