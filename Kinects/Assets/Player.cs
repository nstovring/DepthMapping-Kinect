using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	KinectManager kinectManager;
	// Use this for initialization
	void Start () {
	
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
    void Update () {
		kinectManager = KinectManager.Instance;
		uint playerID = kinectManager != null ? kinectManager.GetPlayer1ID() : 0;
		Debug.Log(kinectManager.IsPlayerCalibrated(playerID));

		if(kinectManager.IsPlayerCalibrated(playerID)){
		
		Vector3 posPointMan = kinectManager.GetUserPosition(playerID);
		Quaternion rotCube = kinectManager.GetUserOrientation(playerID, true);
            Debug.Log(kinectManager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight));
            if(bothJointsTracked(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft))
            {
                Vector3[] hands = getBothJointsPos(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
            }


		transform.eulerAngles =  rotCube.eulerAngles;
		transform.position =  new Vector3(posPointMan.x*20, posPointMan.y, posPointMan.z*20);
		}
	}
}
