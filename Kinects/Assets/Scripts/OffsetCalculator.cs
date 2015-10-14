using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OffsetCalculator : NetworkBehaviour {

    public Vector3 offset;

    public CubemanController[] players;
    private float kinect1Angle;
    private float kinect2Angle;

	void Start () {
	
	}


	// Update is called once per frame
    [Server]
	void Update () {
        GameObject[] playersGameObject = GameObject.FindGameObjectsWithTag("Player");

       

        if (players.Length >= 2) {
            for (int i = 0; i < playersGameObject.Length; i++)
            {
                players[i] = playersGameObject[i].GetComponent<CubemanController>();
            }
            offset = players[0].transform.position - players[1].transform.position;
            kinect1Angle = players[0].AngleFromKinect;
            kinect2Angle = players[1].AngleFromKinect;

            SetOffset();
            //SetHorizontalAngle();
            
        }
	}

    private void SetOffset()
    {
        players[1].GetComponent<CubemanController>().offset = this.offset;
    }

    /*private float CubeMenAngles()
    {
        float angle = 0;
        foreach (CubemanController player in players)
        {
            angle += player.GetComponent<CubemanController>().AngleFromKinect;
        }
        return angle;
    }*/

    public float HorizontalAngle;

    private void SetHorizontalAngle()
    {
        HorizontalAngle = AngleBetweenGameObjects(players[0].transform, players[1].transform);
        players[1].angleOffset = HorizontalAngle;
    }

    private float AngleBetweenGameObjects(Transform t1, Transform t2)
    {
        return Vector3.Angle(t1.position, t2.position);
    }

}
