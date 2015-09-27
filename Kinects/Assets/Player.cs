using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class Player : NetworkBehaviour {

    KinectManager kinectManager;
    PlayerController myPlayer;
    public OffsetCalculator server;
    [SyncVar] public string playerUniqueIdentity;
    private NetworkInstanceId playerNetID;

    public Vector3[] Hands;
    private Vector3 rHandOff;
    private Vector3 lHandOff;

    [ClientRpc]
    public void RpcSetOffset(Vector3 rHandOff, Vector3 lHandOff) {
        this.rHandOff = rHandOff;
        this.lHandOff = lHandOff;
    }

    void Start () {
        server = GameObject.FindGameObjectWithTag("Server").GetComponent<OffsetCalculator>();
        
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
    private Vector3[] hands;


    [Client]
    void Update()
    {
        if ((transform.name == "" || transform.name == "Player(Clone)"))
        {
            transform.name = playerUniqueIdentity;
        }
        CmdSetJointArray(hands, int.Parse(playerNetID.ToString())-1); // Testing line of code remove later
        kinectManager = KinectManager.Instance;
        uint playerID = kinectManager != null ? kinectManager.GetPlayer1ID() : 0;

        if (kinectManager.IsPlayerCalibrated(playerID))
        {
            Vector3 userPos = kinectManager.GetUserPosition(playerID);
            Quaternion userRot = kinectManager.GetUserOrientation(playerID, true);
            if (bothJointsTracked(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft))
            {
                hands = getBothJointsPos(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
                CmdSetJointArray(hands, int.Parse(playerNetID.ToString())-1); //Minus one because the OffsetCalculator has the number 1 netId
                CmdMoveCube(userPos, userRot);
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();

        Debug.Log("Im connected!");
        print("Player ID is " + int.Parse(playerNetID.ToString()));
    }

    public override void OnStartClient()
    {
        //server = GameObject.FindGameObjectWithTag("Server").GetComponent<OffsetCalculator>();
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
        return "Player " + (int.Parse(playerNetID.ToString())-1);
    }

    [Command]
    public void CmdMoveCube(Vector3 position, Quaternion Rotation) {
        transform.eulerAngles = Rotation.eulerAngles;
        transform.position = new Vector3(position.x, position.y, position.z);
    }

    [Command]
    public void CmdSetJointArray(Vector3[] joints, int index) {

        //if statements irellevant during proper tests
        if (index == 1)
        {
            //this.joints[0] = joints[0];
            //this.joints[1] = joints[1];
            this.Hands[0] = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
            this.Hands[1] = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
            //this.joints1 = joints;
        }
        if (index == 2)
        {
            //this.joints[0] = joints[0];
            //this.joints[1] = joints[1];
            this.Hands[0] = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
            this.Hands[1] = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
            //this.joints2 = joints;
        }
    }
}
