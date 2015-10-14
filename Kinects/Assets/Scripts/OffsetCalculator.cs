using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OffsetCalculator : NetworkBehaviour {

    public Vector3 offset;

    public GameObject[] players;
    private float kinect1Angle;
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

            SetOffset();            
        }
	}

    private void SetOffset()
    {
        players[1].GetComponent<CubemanController>().offset = this.offset;
    }

}
