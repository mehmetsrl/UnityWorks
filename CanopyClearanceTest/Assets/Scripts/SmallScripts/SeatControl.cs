using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatControl : MonoBehaviour {
    public bool seatAllocationModeEnabled = false;
    public Vector3 LastSeatPosition = Vector3.zero, LastPlayerEyePosition = Vector3.zero;
    public GameObject playerEye;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.K))
        {
            seatAllocationModeEnabled = !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            if (seatAllocationModeEnabled)
            {
                LastSeatPosition = transform.position;
                LastPlayerEyePosition = playerEye.transform.position;
            }
        }

        if (seatAllocationModeEnabled)
        {
            transform.position = new Vector3(LastSeatPosition.x, LastSeatPosition.y + (playerEye.transform.position - LastPlayerEyePosition).y, LastSeatPosition.z);
        }
	}
}
