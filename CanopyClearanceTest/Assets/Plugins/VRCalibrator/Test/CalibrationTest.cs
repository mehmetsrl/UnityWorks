using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationTest : MonoBehaviour {


    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };

    public Configs config;

    public GameObject virPointVirtualizationRootDummy, phyPointVirtualizationRootDummy;

	private void Start()
    {
        if (CalibrationTransporter.Instance != null)
        {
            ConfigHandler.Instance.Initiate(ref config, true);

            ConfigHandler.Instance.virVirtualizationRoot = virPointVirtualizationRootDummy;
            ConfigHandler.Instance.phyVirtualizationRoot = phyPointVirtualizationRootDummy;

            ConfigHandler.Instance.UpdateVirtualPoints();
            ConfigHandler.Instance.UpdatePhysicalPoints();
        }
	}

	// Update is called once per frame
	void Update () {
        if (CalibrationTransporter.Instance != null)
        {
            if ((Input.GetKey(KeyCode.T)))
            {
                for (int numberPressed = 0; numberPressed < keyCodes.Length; numberPressed++)
                {
                    if (Input.GetKeyDown(keyCodes[numberPressed]))
                    {
                        CalibrationTransporter.Instance.Calibrate(numberPressed);
                    }
                }
            }
        }
	}
}
