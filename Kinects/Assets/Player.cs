using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	KinectManager kinectManager;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		kinectManager = KinectManager.Instance;
		uint playerID = kinectManager != null ? kinectManager.GetPlayer1ID() : 0;
		Debug.Log(kinectManager.IsPlayerCalibrated(playerID));

		if(kinectManager.IsPlayerCalibrated(playerID)){
		
		Vector3 posPointMan = kinectManager.GetUserPosition(playerID);
		Quaternion rotCube = kinectManager.GetUserOrientation(playerID, true);

		transform.eulerAngles =  rotCube.eulerAngles;
		transform.position =  new Vector3(posPointMan.x*20, posPointMan.y, posPointMan.z*20);
		}
	}
}
