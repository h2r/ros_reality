using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSync : NetworkBehaviour {

    GameObject camera_rig;
    Transform head_tf;
    Transform left_tf;
    Transform right_tf; 

    void OnStartLocalPlayer() {

        camera_rig = GameObject.Find("[CameraRig]");
        
    }
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
            return;
        }
        if (camera_rig == null) {
            camera_rig = GameObject.Find("[CameraRig]");
        }
        head_tf = this.transform.Find("Head");
        left_tf = this.transform.Find("Left Controller");
        right_tf = this.transform.Find("Right Controller");

        
        //head_tf = camera_rig.transform.Find("Camera (eye)");

    
        //left_tf.position = camera_rig.transform.Find("Controller (left)").position;
        //left_tf.rotation = camera_rig.transform.Find("Controller (left)").rotation;

        right_tf.position = camera_rig.transform.Find("Controller (right)").position;
        //right_tf.rotation = camera_rig.transform.Find("Controller (right)").rotation;
    }
}
