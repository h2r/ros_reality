using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerID : NetworkBehaviour {

    [SyncVar] public string playerUniqueIdentity;
    private int playerNetID;
    
	public override void OnStartLocalPlayer() {
        GetNetIdentity();
        SetIdentity();
    }

    void Update() {
        if (this.name == "" || this.name == "Player(clone)") {
            SetIdentity();
        }
    }

    [Client]
    void GetNetIdentity() {
        playerNetID = (int) GetComponent<NetworkIdentity>().netId.Value;
        playerNetID /= 3;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    void SetIdentity() {
        if(!isLocalPlayer) {
            this.name = playerUniqueIdentity;
        } else {
            this.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity() {
        return "Player " + playerNetID.ToString();
    }

    [Command]
    void CmdTellServerMyIdentity(string name) {
        playerUniqueIdentity = name;
    }
}
