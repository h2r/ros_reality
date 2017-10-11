using UnityEngine;
using System.Collections;
using System.Threading;

public class TrajectoryController : MonoBehaviour {

    public string arm;
    GameObject targetModel;
    Transform tf;
    private WebsocketClient wsc;
    private SteamVR_TrackedController controller;
    TFListener TFListener;
    float scale;
    Vector3 lastControllerPosition;
    Vector3 lastArmPosition;
    Quaternion lastControllerRotation;
    Quaternion lastArmRotation;
    Transform lastArmTF;
    Transform targetTransform;
    string message;

    // Use this for initialization
    void Start() {
        Debug.Log("help meh");
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();

        wsc.Advertise("ein/" + arm + "/forth_commands", "std_msgs/String");
        
        
        

        if (arm == "left") {
            targetModel = GameObject.Find("LeftTargetModel");
        } else if (arm == "right") {
            targetModel = GameObject.Find("RightTargetModel");
        } else {
            Debug.LogError("arm must be left or right");
        }

        //last positions/rotation of the controller (calculate relative displacement of controller at each update)
        lastControllerPosition = tf.position;
        lastControllerRotation = tf.rotation;
        Invoke("FindArm", 1f); //update position of lastArm position and rotation
        InvokeRepeating("sendMessage", 1.2f, .1f); //send message to move arm by displacement of current controller position/rotation with previous position/rotation
        targetTransform = targetModel.GetComponent<Transform>();
    }

    void FindArm() { //update the lastArm with the current position/rotation of the controller
        lastArmTF = GameObject.Find(arm + "_gripper_base").GetComponent<Transform>();
        lastArmPosition = lastArmTF.position;
        lastArmRotation = lastArmTF.rotation;
        //Debug.Log(lastArmPosition);
    }

    void sendMessage() { //send an ein message to arm
        wsc.SendEinMessage(message, arm);
    }

    void Update() {
        TFListener = GameObject.Find("TFListener").GetComponent<TFListener>();
        tf = GetComponent<Transform>();
        scale = TFListener.scale;
        controller = GameObject.Find("Controller (" + arm + ")").GetComponent<SteamVR_TrackedController>();


        Vector3 deltaPos = tf.position - lastControllerPosition; //displacement of current controller position to old controller position
        lastControllerPosition = tf.position;

        Quaternion deltaRot = tf.rotation * Quaternion.Inverse(lastControllerRotation); //delta of current controller rotation to old controller rotation
        lastControllerRotation = tf.rotation;
        
        //message to be sent over ROs network
        message = "";


        //Allows movement control with controllers if menu is disabled
        if (controller.gripped) { //deadman switch being pressed
            lastArmPosition = lastArmPosition + deltaPos; //new arm position
            lastArmRotation = deltaRot * lastArmRotation; //new arm rotation

            if ((Vector3.Distance(new Vector3(0f, 0f, 0f), lastArmPosition)) < 1.5) { //make sure that the trajectory 
                targetTransform.position = lastArmPosition;
            }
            targetTransform.rotation = lastArmRotation;

            //Vector3 outPos = UnityToRosPositionAxisConversion(lastArmTF.position + deltaPos) / scale;
            Vector3 outPos = UnityToRosPositionAxisConversion(lastArmPosition) / scale;
            //Quaternion outQuat = UnityToRosRotationAxisConversion(deltaRot * lastArmTF.rotation);
            Quaternion outQuat = UnityToRosRotationAxisConversion(lastArmRotation);

            message = outPos.x + " " + outPos.y + " " + outPos.z + " " + outQuat.x + " " + outQuat.y + " " + outQuat.z + " " + outQuat.w + " moveToEEPose";
        }
        if (controller.triggerPressed) {
            message += " openGripper ";
        }
        else {
            message += " closeGripper ";
        }

        //Debug.Log(message);
        //Debug.Log(lastArmPosition);
    }

    Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn) {
        return new Vector3(-rosIn.x, -rosIn.z, rosIn.y);
    }

    Quaternion UnityToRosRotationAxisConversion(Quaternion qIn) {
        Quaternion temp = (new Quaternion(qIn.x, qIn.z, -qIn.y, qIn.w)) * (new Quaternion(0, 1, 0, 0));
        return temp;
    }

}

