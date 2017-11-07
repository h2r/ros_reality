using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TeleopManager : NetworkBehaviour {
    [SyncVar]
    public int player_num = 1;

    public void Increment() {
        if (!isServer) {
            return;
        }
        player_num++;
    }

}
