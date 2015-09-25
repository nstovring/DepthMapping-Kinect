using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OffsetCalculator : MonoBehaviour {

    public Vector3[] hands1;
    public Vector3[] hands2;
    public Vector3 offsetVectorRightHand;
    public Vector3 offsetVectorLeftHand;

    GameObject[] players;
	void Start () {
	
	}

    public void SetJoints(Vector3[] joints, int player)
    {
        if (player == 1)
        {
            hands1 = joints;
        }
        if (player == 2)
        {
            hands2 = joints;
        }
    }

    Vector3 CalcOffset(Vector3 v1, Vector3 v2) {
        return v2 - v1;
    }

	// Update is called once per frame
	void Update () {
       players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 1) {
            offsetVectorRightHand = CalcOffset(hands1[0], hands1[0]);
            offsetVectorLeftHand = CalcOffset(hands2[1], hands2[1]);
        }

        for( int i = 0; i< players.Length; i++) {
            if(i == 0)
            hands1 = players[i].GetComponent<Player>().joints;
            if(i == 1)
            hands2 = players[i].GetComponent<Player>().joints;
        }
    }
}
