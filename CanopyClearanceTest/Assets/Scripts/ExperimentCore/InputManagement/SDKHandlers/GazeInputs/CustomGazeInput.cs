using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGazeInput : MonoBehaviour
{
    public float sensivity = .003f;
    private Vector2 viewportCoords = Vector2.one/2;
    public Vector2 ViewportCoords
    {
        get
        {
            if (viewportCoords.x > .7f) viewportCoords = new Vector2(.7f, viewportCoords.y);
            if (viewportCoords.x < .3f) viewportCoords = new Vector2(.3f, viewportCoords.y);

            if (viewportCoords.y > .7f) viewportCoords = new Vector2(viewportCoords.x, .7f);
            if (viewportCoords.y < .3f) viewportCoords = new Vector2(viewportCoords.x, .3f);

            return viewportCoords;
        }
        set { viewportCoords = value; }
    }

    private bool isCoursorMoving = false;
    public bool IsCoursorMoving
    {
        get { return isCoursorMoving; }
    }

    void Update()
    {
        //for (int i = 0; i < 20; i++)
        //{
        //    if (Input.GetKeyDown("joystick button " + i))
        //    {
        //        Debug.Log("Pressed button: " + i);
        //    }
        //}


        //if (Input.GetKeyDown(KeyCode.P))//
        if(Input.GetKeyDown("joystick button " + 14))
        {
            GazeInputs.Instance.TriggerPress();
        }

        //if (Input.GetKeyDown(KeyCode.U))
        if (Input.GetKeyUp("joystick button " + 14))
        {
            GazeInputs.Instance.TriggerUnPress();
        }

        float stickAxisHorizontal = Input.GetAxis("Stick HorizontalAxis");

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //    stickAxisHorizontal = 0.05f;
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    stickAxisHorizontal = -0.05f;

        //if (stickAxisHorizontal != 0)
        //{
        //    Debug.Log("HrorizontalAxis " + stickAxisHorizontal);
        //}





        float stickAxisVertical = Input.GetAxis("Stick VerticalAxis");

        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    stickAxisVertical = 0.05f;
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //    stickAxisVertical = -0.05f;

        //if (stickAxisVertical != 0)
        //{
        //    Debug.Log("VerticalAxis " + stickAxisVertical);
        //}

        Vector2 coursorMovements = new Vector2((stickAxisHorizontal != 0) ? stickAxisHorizontal : 0,
                                       (stickAxisVertical != 0) ? stickAxisVertical : 0) * sensivity;

        isCoursorMoving = (coursorMovements.magnitude > 0);

        if (isCoursorMoving)
        {
            viewportCoords += coursorMovements;
        }
    }

}
