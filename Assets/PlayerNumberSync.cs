using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNumberSync : NetworkBehaviour {

    [SyncVar]
    public int playerNumber = 1;

    public void AssignName() {
        if (!isServer) {
            return;
        }


    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
