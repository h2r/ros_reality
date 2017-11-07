using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TeleopManager : NetworkBehaviour {
    [SyncVar]
    public int player_num = 1;

    [Command]
    public void CmdIncrement() {
        if (!isServer) {
            return;
        }
        player_num++;
    }

}
