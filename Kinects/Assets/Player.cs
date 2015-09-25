using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class Player : NetworkBehaviour {

    KinectManager kinectManager;
    NetworkView myView;
    NetworkClient myClient;
    NetworkIdentity myIdentity;
    PlayerController myPlayer;
    public OffsetCalculator server;
    [SyncVar]
    public string playerUniqueIdentity;
    private NetworkInstanceId playerNetID;

    //public Vector3[] joints1;
    //public Vector3[] joints2;

    public Vector3[] joints;
    // Use this for initialization
    void Start () {
        myIdentity = GetComponent<NetworkIdentity>();
        myClient = new NetworkClient();
    }
    public bool bothJointsTracked(uint userID, int Joint1, int Joint2)
    {
        if(kinectManager.IsJointTracked(userID, Joint1) && kinectManager.IsJointTracked(userID, Joint2))
        {
            return true;
        }
        return false;
    }
    public Vector3[] getBothJointsPos(uint userID, int Joint1, int Joint2)
    {
        Vector3[] output = new Vector3[2];
        output[0] = kinectManager.GetJointPosition(userID, Joint1);
        output[1] = kinectManager.GetJointPosition(userID, Joint2);
        return output;
    }
    // Update is called once per frame

    public Vector3[] hands;

    [Client]
    void Update()
    {
        if ((transform.name == "" || transform.name == "Player(Clone)")) {
            transform.name = playerUniqueIdentity;
        }
        CmdSetJointArray(hands, int.Parse(playerNetID.ToString()));
        kinectManager = KinectManager.Instance;
        uint playerID = kinectManager != null ? kinectManager.GetPlayer1ID() : 0;

        if (kinectManager.IsPlayerCalibrated(playerID))
        {
            Vector3 posPointMan = kinectManager.GetUserPosition(playerID);
            Quaternion rotCube = kinectManager.GetUserOrientation(playerID, true);
            if (bothJointsTracked(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft))
            {
                hands = getBothJointsPos(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
                CmdSetJointArray(hands, int.Parse(playerNetID.ToString()));
                CmdMoveCube(posPointMan, rotCube);
                //transform.eulerAngles = rotCube.eulerAngles;
                //transform.position = new Vector3(posPointMan.x * 20, posPointMan.y, posPointMan.z * 20);
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();

        Debug.Log("Im connected!");
        print("Player ID is " + int.Parse(playerNetID.ToString()));
        //myClient = new NetworkClient();
    }

    public override void OnStartClient()
    {
        server = GameObject.FindGameObjectWithTag("Server").GetComponent<OffsetCalculator>();
    }
    private void SetIdentity()
    {
        if (isLocalPlayer) {
            transform.name = playerUniqueIdentity;
        }
        else
        {
            transform.name = MakeUniqueIdentity();
        }
    }
    private void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    [Command]
    private void CmdTellServerMyIdentity(string name)
    {
        playerUniqueIdentity = name;
    }
    private string MakeUniqueIdentity()
    {
        return "Player " + playerNetID.ToString();
    }

    [Command]
    public void CmdMoveCube(Vector3 position, Quaternion Rotation) {
        transform.eulerAngles = Rotation.eulerAngles;
        transform.position = new Vector3(position.x, position.y, position.z);
    }

    [Command]
    public void CmdSetJointArray(Vector3[] joints, int index) {
        if (index == 1)
        {
            //this.joints[0] = joints[0];
            //this.joints[1] = joints[1];
            this.joints[0] = new Vector3(1, 1, 1);
            this.joints[1] = new Vector3(2, 2, 2);
            //this.joints1 = joints;
        }
        if (index == 2)
        {
            //this.joints[0] = joints[0];
            //this.joints[1] = joints[1];
            this.joints[0] = new Vector3(3, 3, 3);
            this.joints[1] = new Vector3(4, 4, 4);
            //this.joints2 = joints;
        }

        //ClientSetJoints(joints, index);
    }
}
