using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OffsetCalculator : NetworkBehaviour {

    public Vector3 offset;

    public GameObject[] players;
	void Start () {
	
	}


	// Update is called once per frame
    [Server]
	void Update () {
       players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length >= 2) {

            offset = players[0].transform.position - players[1].transform.position;
            SetHorizontalAngle();
            SetOffset();
        }
	}

    private void SetOffset()
    {
        players[1].GetComponent<CubemanController>().offset = this.offset;
    }

    private float CubeMenAngles()
    {
        float angle = 0;
        foreach (GameObject player in players)
        {
            angle += player.GetComponent<CubemanController>().GetAngleFromKinect();
        }
        return angle;
    }

    public float HorizontalAngle;

    private void SetHorizontalAngle()
    {
        HorizontalAngle = CubeMenAngles() + AngleBetweenGameObjects(players[0].transform, players[1].transform);
        players[1].GetComponent<CubemanController>().angleOffset = HorizontalAngle;
    }

    private float AngleBetweenGameObjects(Transform t1, Transform t2)
    {
        return Vector3.Angle(t1.position, t2.position);
    }

}
