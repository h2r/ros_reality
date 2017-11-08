using UnityEngine;
using UnityEngine.Networking;

public class ArmController : NetworkBehaviour {
    // string of which arm to control. Valid values are "left" and "right"
    public string arm;
    //websocket client connected to ROS network
    private WebsocketClient wsc;
    private SteamVR_TrackedController controller;
    TFListener TFListener;
    //scale represents how resized the virtual robot is
    float scale;

    void Start() {
        // Get the live websocket client
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();

        // Get the live TFListener
        TFListener = GameObject.Find("TFListener").GetComponent<TFListener>();

        // Get the controller componenet of this gameobject
        controller = GameObject.Find("Controller (" + arm + ")").GetComponent<SteamVR_TrackedController>();

        // Create publisher to the Baxter's arm topic (uses Ein)
        wsc.Advertise("ein/" + arm + "/forth_commands", "std_msgs/String");
        // Asychrononously call sendControls every .1 seconds
        InvokeRepeating("SendControls", .1f, .1f);
    }

    [Client]
    void SendControls() {
        if (!isLocalPlayer) {
            return;
        }
        if ((this.transform.parent.name == "Player 3" && this.arm == "left") || (this.transform.parent.name == "Player 4" && this.arm == "right")) {
            Debug.Log(this.transform.parent.name + "   "  + this.arm);
            scale = TFListener.scale;

            //Convert the Unity position of the hand controller to a ROS position (scaled)
            Vector3 outPos = UnityToRosPositionAxisConversion(GetComponent<Transform>().position) / scale;
            //Convert the Unity rotation of the hand controller to a ROS rotation (scaled, quaternions)
            Quaternion outQuat = UnityToRosRotationAxisConversion(GetComponent<Transform>().rotation);
            //construct the Ein message to be published
            string message = "";
            //Allows movement control with controllers if menu is disabled

            //if deadman switch held in, move to new pose
            if (controller.gripped) {
                //construct message to move to new pose for the robot end effector 
                message = outPos.x + " " + outPos.y + " " + outPos.z + " " +
                outQuat.x + " " + outQuat.y + " " + outQuat.z + " " + outQuat.w + " moveToEEPose";
                //if touchpad is pressed (Crane game), incrementally move in new direction
            }

            //If trigger pressed, open the gripper. Else, close gripper
            if (controller.triggerPressed) {
                message += " openGripper ";
            }
            else {
                message += " closeGripper ";
            }

            //Send the message to the websocket client (i.e: publish message onto ROS network)
            //Debug.Log(message);
            wsc.SendEinMessage(message, arm);

            //Debug.Log(arm+":"+message);
        }
    }

    //Convert 3D Unity position to ROS position 
    Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn) {
        return new Vector3(-rosIn.x, -rosIn.z, rosIn.y);
    }

    //Convert 4D Unity quaternion to ROS quaternion
    Quaternion UnityToRosRotationAxisConversion(Quaternion qIn) {

        Quaternion temp = (new Quaternion(qIn.x, qIn.z, -qIn.y, qIn.w)) * (new Quaternion(0, 1, 0, 0));
        return temp;
    }

}

