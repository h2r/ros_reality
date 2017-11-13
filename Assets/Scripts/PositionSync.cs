using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PositionSync : NetworkBehaviour {

    GameObject camera_rig;
    Transform head_tf;
    Transform left_tf;
    Transform right_tf;

    public override void OnStartLocalPlayer() {
        camera_rig = GameObject.Find("[CameraRig]");
        head_tf = this.transform.Find("Head");
        left_tf = this.transform.Find("left Controller");
        right_tf = this.transform.Find("right Controller");
        //foreach MeshRenderer mr in transform.GetComponentsInChildren<MeshRenderer>() {
        //    mr.material.color = Color.blue;
        //}
    }
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
            return;
        }
        head_tf.position = camera_rig.transform.Find("Camera (eye)").position;
        head_tf.rotation = camera_rig.transform.Find("Camera (eye)").rotation;

    
        left_tf.position = camera_rig.transform.Find("Controller (left)").position;
        left_tf.rotation = camera_rig.transform.Find("Controller (left)").rotation;

        right_tf.position = camera_rig.transform.Find("Controller (right)").position;
        right_tf.rotation = camera_rig.transform.Find("Controller (right)").rotation;
    }
}
