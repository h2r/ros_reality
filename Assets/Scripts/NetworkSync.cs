using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSync : NetworkBehaviour {

    GameObject camera_rig;
    Transform head_tf;
    Transform left_tf;
    Transform right_tf;

    public override void OnStartLocalPlayer() {
        camera_rig = GameObject.Find("[CameraRig]");
        head_tf = this.transform.Find("Head");
        left_tf = this.transform.Find("Left Controller");
        right_tf = this.transform.Find("Right Controller");
        GameObject tm = GameObject.Find("TeleopManager");
        int num = tm.GetComponent<TeleopManager>().player_num;
        tm.GetComponent<TeleopManager>().Increment();
        this.name = "Player" + num.ToString();

    }
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
            return;
        }
        //if (camera_rig == null) {
        //    camera_rig = GameObject.Find("[CameraRig]");
        //}


        
        head_tf.position = camera_rig.transform.Find("Camera (eye)").position;
        head_tf.rotation = camera_rig.transform.Find("Camera (eye)").rotation;

    
        left_tf.position = camera_rig.transform.Find("Controller (left)").position;
        left_tf.rotation = camera_rig.transform.Find("Controller (left)").rotation;

        right_tf.position = camera_rig.transform.Find("Controller (right)").position;
        right_tf.rotation = camera_rig.transform.Find("Controller (right)").rotation;
    }
}
