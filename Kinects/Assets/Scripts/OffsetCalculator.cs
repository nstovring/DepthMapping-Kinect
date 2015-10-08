using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OffsetCalculator : NetworkBehaviour {

    public Vector3[] hands1;
    public Vector3[] hands2;
    public Vector3 offsetVectorRightHand;
    public Vector3 offsetVectorLeftHand;

    GameObject[] players;
	void Start () {
	
	}

    [Command]
    public void CmdSetOffsets(GameObject[] players)
    {
        foreach (GameObject player in players) {
            player.GetComponent<Player>().RpcSetOffset(offsetVectorRightHand, offsetVectorLeftHand);
        }
    }

    Vector3 CalcOffset(Vector3 v1, Vector3 v2) {
        return v2 - v1;
    }

	// Update is called once per frame
    [Server]
	void Update () {
       players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length >= 2) {
            offsetVectorRightHand = CalcOffset(hands1[0], hands2[0]);
            offsetVectorLeftHand = CalcOffset(hands1[1], hands2[1]);
            CmdSetOffsets(players);
        }
        for( int i = 0; i< players.Length; i++) {
            if(i == 0)
            hands1 = players[i].GetComponent<Player>().Hands;
            if(i == 1)
            hands2 = players[i].GetComponent<Player>().Hands;
        }
    }
}
