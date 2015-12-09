﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserTracking : MonoBehaviour {

    GameObject[] cubes;
    List<GameObject> players;
    bool Calibrated;

    List<GameObject[]> calibratedPlayers;
	// Use this for initialization
	void Start () {
        Calibrated = false;
        cubes = GameObject.FindGameObjectsWithTag("Player");
        addToPlayers(cubes);
        
    }
	
	// Update is called once per frame
	void Update () {

	
	}
    public void addToPlayers(GameObject[] cubes)
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            players.Add(cubes[i]);
        }
            
    }

    public void checkForUsers()
    {
        int i = 0;
        while(players.Count >= 1){
            for (int j = 0; j < players.Count; j++)
            {
                if(i != j)
                {
                    if(players[i].transform.position.magnitude - players[j].transform.position.magnitude < 0.2)
                    {
                        GameObject[] player = new GameObject[] { players[i], players[j] };
                        calibratedPlayers.Add(player);
                        players.Remove(players[i]);
                        players.Remove(players[j]);
                        break;
                    }
                }
                else if(players.Count == 1)
                {
                    GameObject[] player = new GameObject[] { players[i], players[j] };
                    calibratedPlayers.Add(player);
                    players.Remove(players[i]);
                    break;
                }
            }
        }
    }

}
