using UnityEngine;

public class EricArmController : MonoBehaviour {
    // string of which arm to control. Valid values are "left" and "right"
    public string arm;
    //websocket client connected to ROS network
    private WebsocketClient wsc;
    private VRTK.VRTK_ControllerEvents controller;
    GameObject target_cube;
    TFListener TFListener;
    //scale represents how resized the virtual robot is
    float scale;
    bool recording = false;

    void Start() {
        // Get the live websocket client
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();

        target_cube = GameObject.Find("FollowCube");
        //Renderer rend = target_cube.GetComponent<Renderer>();
        //rend.material.SetColor("_Color", Color.red);

        // Get the live TFListener
        TFListener = GameObject.Find("TFListener").GetComponent<TFListener>();

        // Get the controller componenet of this gameobject
        controller = GetComponent<VRTK.VRTK_ControllerEvents>();

        // Create publisher to the Baxter's arm topic (uses Ein)
        wsc.Advertise("ein/" + arm + "/forth_commands", "std_msgs/String");
        // Asychrononously call sendControls every .1 seconds
        InvokeRepeating("SendControls", .1f, .1f);
    }

    private void Update() {
        if (controller.triggerPressed) {
            recording = true;
            Renderer rend = target_cube.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.green);
        }
        else {
            recording = false;
            Renderer rend = target_cube.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
        }
    }

    void SendControls() {
        if (recording == true) {
            //Convert the Unity position of the hand controller to a ROS position (scaled)
            Debug.Log("hey");
        }
        /*
        scale = TFListener.scale;

        //Convert the Unity position of the hand controller to a ROS position (scaled)
        Vector3 outPos = UnityToRosPositionAxisConversion(GetComponent<Transform>().position) / scale;
        //Convert the Unity rotation of the hand controller to a ROS rotation (scaled, quaternions)
        Quaternion outQuat = UnityToRosRotationAxisConversion(GetComponent<Transform>().rotation);
        //construct the Ein message to be published
        string message = "";
        //Allows movement control with controllers if menu is disabled

        //if deadman switch held in, move to new pose
        if (controller.gripPressed) {
            //construct message to move to new pose for the robot end effector 
            message = outPos.x + " " + outPos.y + " " + outPos.z + " " +
            outQuat.x + " " + outQuat.y + " " + outQuat.z + " " + outQuat.w + " moveToEEPose";
            //if touchpad is pressed (Crane game), incrementally move in new direction
        }
        else if (controller.touchpadPressed) {
            //get the angle contact point on touch pad
            float angle = controller.GetTouchpadAxisAngle();

            //Con
            if (angle >= 45 && angle < 135) // touching right
                message += " yDown ";
            else if (angle >= 135 && angle < 225) // touching bottom
                message += " xDown ";
            else if (angle >= 225 && angle < 315) // touching left
                message += " yUp ";
            else //touching top
                message += " xUp ";
        }

        //Send the message to the websocket client (i.e: publish message onto ROS network)
        wsc.SendEinMessage(message, arm);

        //Debug.Log(arm+":"+message);
        */
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

