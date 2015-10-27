using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class CubeController : NetworkBehaviour
{
    //Public 
    public bool MoveVertically = false;
    public bool MirroredMovement = false;

    [SyncVar]
    private float yRotationOffset = 0;

    //Sync Vars
    [SyncVar]
    public Vector3 positionOffset;
    [SyncVar]
    public float angleOffset;
    [SyncVar]
    public float otherAngleFromKinect;
    [SyncVar]
    public float angleFromKinect;
    [SyncVar]
    public float angleBetweenKinects;

    //Private 
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialPosOffset = Vector3.zero;
    private uint initialPosUserID = 0;
    private KinectManager manager;
    private bool isCalibrated;
    [SyncVar] private Color userColor;

    public float YRotationOffset
    {
        get
        {
            return yRotationOffset;
        }

        set
        {
            yRotationOffset = value;
        }
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }
    }

    public override void OnStartLocalPlayer()
    {
        userColor = new Color(Random.value, Random.value, Random.value);
        transform.GetComponent<MeshRenderer>().material.color = userColor;
        transform.name = "User " + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - 1);
    }
    // Update is called once per frame
    [Client]
    void Update()
    {
        if (isLocalPlayer)
        {
            if (manager == null)
            {
                manager = KinectManager.Instance;
            }
            else
            {
                angleBetweenKinects = GetAngleBetweenKinects();
                angleFromKinect = GetAngleBetweenForwardAxisAndTrackedUser();
                angleOffset = Mathf.Abs(angleBetweenKinects) + Mathf.Abs(angleFromKinect) +
                              Mathf.Abs(otherAngleFromKinect);
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log(angleFromKinect + " Angle from kinect");
                    Debug.Log(angleBetweenKinects + " Angle Between Cameras");
                    CalibratePosition();
                    isCalibrated = true;
                }
                if (isCalibrated)
                {
                    Debug.Log("Current Pos " + transform.position);
                    isCalibrated = false;
                }
                //MoveSkeleton();
                ApplyRotationOffset();
            }
        }
    }

    [Client]
    void ApplyRotationOffset()
    {
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;
        Vector3 posPointMan = manager.GetUserPosition(playerID);

        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;
        posPointMan.x *= 1;
        Quaternion direction = Quaternion.AngleAxis(YRotationOffset, Vector3.up);

        transform.position = (direction * posPointMan) != Vector3.zero ? (direction * posPointMan) : posPointMan;
        RotateWithUser();
        //Apply direction to movement of cube
    }

    private void CalibratePosition()
    {
        manager.kinectToWorld.SetTRS(new Vector3(positionOffset.x, positionOffset.y + 1, positionOffset.z), Quaternion.identity, Vector3.one);
    }

    [Client]
    public float GetAngleBetweenForwardAxisAndTrackedUser()
    {
        Vector3 targetDir = transform.position - Vector3.zero;
        Vector3 forward = Vector3.left;

        return Vector3.Angle(targetDir, forward) - 90;

    }

    private float GetAngleBetweenKinects()
    {
        Vector3 positionVector3 = transform.position;
        Vector3 v0Vector3 = Vector3.zero;
        Vector3 vOffsetVector3 = positionOffset;
        Vector3 r1Vector3 = positionVector3 - vOffsetVector3;
        Vector3 r2Vector3 = positionVector3 - v0Vector3;
        return Vector3.Angle(r1Vector3, r2Vector3);
    }

    [Client]
    private void RotateWithUser()
    {
        if (manager && manager.IsInitialized())
        {
            if (manager.IsUserDetected())
            {
                uint userId = manager.GetPlayer1ID();

                if (manager.IsJointTracked(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft) &&
                   manager.IsJointTracked(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight))
                {
                    Vector3 posLeftShoulder = manager.GetJointPosition(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
                    Vector3 posRightShoulder = manager.GetJointPosition(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);

                    posLeftShoulder.z = -posLeftShoulder.z;
                    posRightShoulder.z = -posRightShoulder.z;

                    Vector3 dirLeftRight = posRightShoulder - posLeftShoulder;
                    dirLeftRight -= Vector3.Project(dirLeftRight, Vector3.up);

                    Quaternion rotationShoulders = Quaternion.FromToRotation(Vector3.right, dirLeftRight);

                    transform.rotation = rotationShoulders;
                }
            }
        }
    }
}
