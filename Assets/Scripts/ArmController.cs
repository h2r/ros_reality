using UnityEngine;

public class ArmController : MonoBehaviour {
    // string of which arm to control. Valid values are "left" and "right"
    public string arm;

    private string grip_label;
    private string trigger_label;
    //websocket client connected to ROS network
    private WebsocketClient wsc;
    TFListener TFListener;
    //scale represents how resized the virtual robot is
    float scale;

    void Start() {
        // Get the live websocket client
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();

        // Get the live TFListener
        TFListener = GameObject.Find("TFListener").GetComponent<TFListener>();

        // Create publisher to the Baxter's arm topic (uses Ein)
        wsc.Advertise("ein/" + arm + "/forth_commands", "std_msgs/String");
        // Asychrononously call sendControls every .1 seconds
        InvokeRepeating("SendControls", .1f, .1f);

        if (arm == "left") {
            grip_label = "Left Grip";
            trigger_label = "Left Trigger";
        }
        else if (arm == "right") {
            grip_label = "Right Grip";
            trigger_label = "Right Trigger";
        }
        else
            Debug.LogError("arm variable is not set correctly");
    }

    void SendControls() {
        scale = TFListener.scale;

        //Convert the Unity position of the hand controller to a ROS position (scaled)
        Vector3 outPos = UnityToRosPositionAxisConversion(GetComponent<Transform>().position) / scale;
        //Convert the Unity rotation of the hand controller to a ROS rotation (scaled, quaternions)
        Quaternion outQuat = UnityToRosRotationAxisConversion(GetComponent<Transform>().rotation);
        //construct the Ein message to be published
        string message = "";
        //Allows movement control with controllers if menu is disabled

		if (arm == "right") {
			//Debug.Log (transform.name);
			//Debug.Log (transform.localPosition);
			Debug.Log ("arm: " + GetComponent<Transform> ().position);
			Debug.Log ("arm ros: " + outPos.x + " " + outPos.y + " " + outPos.z);
		}
        //if deadman switch held in, move to new pose
        if (Input.GetAxis(grip_label) > 0.5f) {
            //construct message to move to new pose for the robot end effector 
            message = outPos.x + " " + outPos.y + " " + outPos.z + " " +
            outQuat.x + " " + outQuat.y + " " + outQuat.z + " " + outQuat.w + " moveToEEPose";
			
            //if touchpad is pressed (Crane game), incrementally move in new direction
        }

        //If trigger pressed, open the gripper. Else, close gripper
        if (Input.GetAxis(trigger_label) > 0.5f) {
            message += " openGripper ";
        }
        else {
            message += " closeGripper ";
        }

        //Send the message to the websocket client (i.e: publish message onto ROS network)
        wsc.SendEinMessage(message, arm);
    }

    //Convert 3D Unity position to ROS position 
    Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn) {
        return new Vector3(-rosIn.x, -rosIn.z, rosIn.y);
    }

    //Convert 4D Unity quaternion to ROS quaternion
    Quaternion UnityToRosRotationAxisConversion(Quaternion qIn) {

        Quaternion temp = (new Quaternion(qIn.x, qIn.z, -qIn.y, qIn.w)) * (new Quaternion(0, 1, 0, 0));
        return temp;

        //return new Quaternion(-qIn.z, qIn.x, -qIn.w, -qIn.y);
        //return new Quaternion(-qIn.z, qIn.w, -qIn.x, -qIn.y);
        //return new Quaternion(-qIn.z, qIn.w, -qIn.x, -qIn.y);
        //return new Quaternion(-qIn.z, qIn.x, qIn.w, qIn.y);
    }

}

