using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OffsetCalculator : NetworkBehaviour {

    [SyncVar] public Vector3 offset;
    [SyncVar] public float angleOffset;

    public GameObject[] players;
    [SyncVar] private float kinectAngle;
    private float kinect2Angle;

	void Start () {
	
	}


	// Update is called once per frame
    [Server]
	void Update () {
        players = GameObject.FindGameObjectsWithTag("Player");

        if (this.players.Length >= 2) {
            
            offset = players[0].transform.position - players[1].transform.position;
            //kinect1Angle = this.players[0].AngleFromKinect;
            //kinect2Angle = this.players[1].AngleFromKinect;
            kinectAngle = players[0].transform.GetComponent<CubemanController>().AngleFromKinect;

            CubemanController player2Controller = players[1].transform.GetComponent<CubemanController>();

            angleOffset = kinectAngle + player2Controller.AngleFromKinect + player2Controller.angleBetweenCameras;
            SetOffset();            
        }
	}

    private void SetOffset()
    {
        players[1].GetComponent<CubemanController>().offset = this.offset;
        players[1].GetComponent<CubemanController>().angleOffset = this.angleOffset;
    }

}
