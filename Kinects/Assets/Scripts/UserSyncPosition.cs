using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class UserSyncPosition : NetworkBehaviour
{

    [SyncVar] private Vector3 syncPos;
    [SyncVar] private Vector3 syncRot;

    [SyncVar] public bool Offset;
    [SyncVar] private Color userColor;
    [SyncVar] private string name;

    [SerializeField] private Transform myTransform;
    [SerializeField] private float lerpRate = 15;

    public bool MirroredMovement;

    private KinectHandler kinectHandler;

    // Use this for initialization
    void Start ()
    {
        kinectHandler = KinectHandler.handlerInstance;
    }

    // Update is called once per frame
    [ClientCallback]
    void FixedUpdate () {
	    TransmitPosition();
        LerpPosition();
	}

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime*lerpRate);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(syncRot), Time.deltaTime * lerpRate);
            transform.GetComponent<MeshRenderer>().material.color = userColor;
            transform.name = name;
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos, Vector3 rot)
    {
        syncPos = pos;
        syncRot = rot;
    }

    [Command]
    void Cmd_ChangeIdentity(Color col, string name)
    {
        this.name = name;
        userColor = col;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && kinectHandler && kinectHandler.IsInitialized())
        {
            MoveWithUser();
            RotateWithUser();
            CmdProvidePositionToServer(myTransform.position, myTransform.rotation.eulerAngles);
            // Initialize User
            
        }
        if (isLocalPlayer)
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            CmdProvidePositionToServer(myTransform.position, myTransform.rotation.eulerAngles);
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        string name = "User " + (GetComponent<NetworkIdentity>().netId.Value -1);
        //PlayerController.playerControllerId;
        userColor = new Color(Random.value, Random.value, Random.value);
        transform.GetComponent<MeshRenderer>().material.color = userColor;
        transform.name = name;
        Cmd_ChangeIdentity(userColor, name);
    }

    [Client]
    private void MoveWithUser()
    {
        uint playerID = kinectHandler != null ? kinectHandler.GetPlayer1ID() : 0;
        Vector3 posPointMan = kinectHandler.GetUserPosition(playerID);

        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;
        posPointMan.x *= 1;

        if (Offset)
        {
            Quaternion direction = Quaternion.AngleAxis(kinectHandler.rotationOffset.y, Vector3.up);
            transform.position = (direction*posPointMan) != Vector3.zero ? (direction*posPointMan) : posPointMan;
        }
    }

    [Client]
    private void RotateWithUser()
    {
        if (kinectHandler.IsUserDetected())
        {
            uint userId = kinectHandler.GetPlayer1ID();

            if (kinectHandler.IsJointTracked(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft) &&
                kinectHandler.IsJointTracked(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight))
            {
                Vector3 posLeftShoulder = kinectHandler.GetJointPosition(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
                Vector3 posRightShoulder = kinectHandler.GetJointPosition(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);

                posLeftShoulder.z = -posLeftShoulder.z;
                posRightShoulder.z = -posRightShoulder.z;

                Vector3 dirLeftRight = posRightShoulder - posLeftShoulder;
                dirLeftRight -= Vector3.Project(dirLeftRight, Vector3.up);

                Quaternion rotationShoulders = Quaternion.FromToRotation(Vector3.right, dirLeftRight);

                myTransform.rotation = rotationShoulders;
            }
        }
    }
}
