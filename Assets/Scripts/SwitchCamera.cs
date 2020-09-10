using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Globals;

public class SwitchCamera : MonoBehaviour {

    public Camera fpsCam;
    public Camera tpsCam;
    public bool switchCam = true;



    // Use this for initialization
    void Start () {
        fpsCam.enabled = !switchCam;
        tpsCam.enabled = switchCam;
	}
	
	// Update is called once per frame
	void Update () {


        //obvezno pregledat za AIM stil

        if (Input.GetKeyUp("v"))
        {
            switchCam = !switchCam;
            fpsCam.enabled = !switchCam;
            tpsCam.enabled = switchCam;
        }
    }

    public Camera GetActiveCamera()
    {
        if (switchCam)
        {
            return tpsCam;
        }
        else
        {
            return fpsCam;
        }
    }
}
