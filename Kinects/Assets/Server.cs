using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

    public Vector3[] hands1;
    public Vector3[] hands2;


    GameObject[] players;
	// Use this for initialization
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

	// Update is called once per frame
    //[Server]
	void Update () {
       players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);
        for( int i = 0; i< players.Length; i++) {
            if(i == 0)
            hands1 = players[i].GetComponent<Player>().joints;
            if(i == 1)
            hands2 = players[i].GetComponent<Player>().joints;
        }
    }
}
