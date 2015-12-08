using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//This class is a simplified version of the KinectManager developed for enabling network support for this project


public class KinectHandler : KinectManager
{
    //Gameobject representing the the user which calibration will be derived from
    public GameObject[] CalibrationUserGameObjects;
    public CubeController CubeController;
    public OffsetCalculator OffsetCalculator;

    //public Transform[] 
    public static KinectHandler handlerInstance;

    [SyncVar] public Vector3 positionOffset;
    [SyncVar] public Vector3 rotationOffset;

    // Use this for initialization
    void Start ()
    {
        handlerInstance = this;
    }
	
	// Update is called once per frame
	void Update () {
	    if (isLocalPlayer && Input.GetKeyUp(KeyCode.S))
	    {
            //Start kinect will initialize all vairables and setting from the kinectwrapper through the KinectManagers methods
	        StartKinect();
	    }
	    if (isServer && Input.GetKeyUp(KeyCode.C))
	    {
	        CalibrationUserGameObjects = GameObject.FindGameObjectsWithTag("Player");
	        positionOffset = OffsetCalculator.GetPositionOffset();
	        rotationOffset = OffsetCalculator.GetRotationOffset();
	    }
        if (isServer && Input.GetKeyUp(KeyCode.O) && CalibrationUserGameObjects[1] != null)
        {
            CalibrationUserGameObjects[1].transform.GetComponent<UserSyncPosition>().Offset = true;
        }
    }
}
