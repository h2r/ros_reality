using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerID : NetworkBehaviour {

    [SyncVar] public string playerUniqueIdentity;
    private NetworkInstanceId playerNetID;
    
	public override void OnStartLocalPlayer() {
        GetNetIdentity();
        SetIdentity();
    }

    void Update() {
        if (this.name == "" || this.name == "PlayerPosition(clone)") {
            SetIdentity();
        }
    }

    [Client]
    void GetNetIdentity() {
        playerNetID = GetComponent<NetworkIdentity>().netId;
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
