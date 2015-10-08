using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class Player : NetworkBehaviour {

    public KinectManager kinectManager;
    PlayerController myPlayer;
    public OffsetCalculator offsetCalc;
    [SyncVar] public string playerUniqueIdentity;
    private NetworkInstanceId playerNetID;

    public Vector3[] Hands;
    private Vector3 rHandOff;
    private Vector3 lHandOff;


    uint playerID;

    //Assigns the calculated offset to the r and lHandOff variables on the clients
    [ClientRpc]
    public void RpcSetOffset(Vector3 rHandOff, Vector3 lHandOff) {
        this.rHandOff = rHandOff;
        this.lHandOff = lHandOff;
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

    //This update function will only run on clients
    [Client]
    void Update()
    {
        kinectManager = GetComponentInChildren<KinectManager>();
        uint playerID = kinectManager != null ? kinectManager.GetPlayer1ID() : 0;

        //Change the name of the spawned Player gameObject --> Move this to start or awake
        if ((transform.name == "" || transform.name == "Player(Clone)"))
        {
            transform.name = playerUniqueIdentity;
        }
        if (kinectManager != null && kinectManager.AllPlayersCalibrated1)
        {
            MoveCube(playerID);
        }
        else {
            transform.position = MatrixFunk.ExtractTranslationFromMatrix(ref kinectManager.kinectToWorld);
            transform.rotation = MatrixFunk.ExtractRotationFromMatrix(ref kinectManager.kinectToWorld);
        }
    }
    //[Client]
    private void MoveCube(uint playerID) {
       //kinectManager = GetComponent<KinectManager>();

            Vector3 userPos = kinectManager.GetUserPosition(playerID);
            Quaternion userRot = kinectManager.GetUserOrientation(playerID, false);
            Debug.Log("Move Here! " + userPos);
            //if (bothJointsTracked(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft))
            hands = getBothJointsPos(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
            //KinectWrapper.MapSkeletonPointToDepthPoint <----- Remember this!
            //CmdSetJointArray(hands, int.Parse(playerNetID.ToString()) - 1); //Minus one because the OffsetCalculator has the number 1 netId
            transform.eulerAngles = userRot.eulerAngles;
            transform.position = (new Vector3(userPos.x, userPos.y, userPos.z))*5;
    }

    //Called on the client when connected to a server
    public override void OnStartLocalPlayer()
    {
        //kinectManager = GameObject.FindGameObjectWithTag("KinectManager").GetComponent<KinectManager>();
        //offsetCalc = GameObject.FindGameObjectWithTag("Server").GetComponent<OffsetCalculator>();
        //kinectManager = GetComponent<KinectManager>();
        kinectManager.StartKinect();
        GetNetIdentity();
        SetIdentity();
        Debug.Log("Im connected!");
        print("Player ID is " + (int.Parse(playerNetID.ToString())-1));
    }

    //Checks if current player is host
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
    //Acquires the netID from the NetworkIdentity Component and sends name to server
    private void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }
    //Tell the server what this gameObjects name is
    [Command]
    private void CmdTellServerMyIdentity(string name)
    {
        playerUniqueIdentity = name;
    }

    //Simply a nice string to represent which player is which
    private string MakeUniqueIdentity()
    {
        return "Player " + (int.Parse(playerNetID.ToString())-1); //Minus one because the OffsetCalculator has the number 1 netId
    }
    
    //Tells the server which joints are being tracked
    [Command]
    public void CmdSetJointArray(Vector3[] Hands, int playerID) {

        if (playerID == 1)
        {
            this.Hands[0] = Hands[0];
            this.Hands[1] = Hands[1];
        }
        if (playerID == 2)
        {
            this.Hands[0] = Hands[0];
            this.Hands[1] = Hands[1];
        }
    }
}
