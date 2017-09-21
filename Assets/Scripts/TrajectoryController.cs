using UnityEngine;
using System.Collections;
using System.Threading;

public class TrajectoryController : MonoBehaviour {
    public string arm;
    public GameObject targetModel;
    Transform tf;
    private WebsocketClient wsc;
    private VRTK.VRTK_ControllerEvents controller;
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

        GameObject wso = GameObject.FindWithTag("WebsocketTag");
        wsc = wso.GetComponent<WebsocketClient>();

        wsc.Advertise("ein/" + arm + "/forth_commands", "std_msgs/String");
        controller = GetComponent<VRTK.VRTK_ControllerEvents>();
        TFListener = GameObject.Find("TFListener").GetComponent<TFListener>();
        tf = GetComponent<Transform>();
        lastControllerPosition = tf.position;
        lastControllerRotation = tf.rotation;
        Invoke("FindArm", 1f);
        InvokeRepeating("sendMessage", 1.2f, .1f);
        targetTransform = targetModel.GetComponent<Transform>();
    }

    void FindArm() {
        lastArmTF = GameObject.Find(arm + "_gripper_base").GetComponent<Transform>();
        lastArmPosition = lastArmTF.position;
        lastArmRotation = lastArmTF.rotation;
        //Debug.Log(lastArmPosition);
    }

    void sendMessage() {
        wsc.SendEinMessage(message, arm);
    }

    void Update() {
        scale = TFListener.scale;

        Vector3 deltaPos = tf.position - lastControllerPosition;
        lastControllerPosition = tf.position;

        Quaternion deltaRot = tf.rotation * Quaternion.Inverse(lastControllerRotation);
        lastControllerRotation = tf.rotation;

        //
        message = "";


        //Allows movement control with controllers if menu is disabled
        if (controller.gripPressed) {
            lastArmPosition = lastArmPosition + deltaPos;
            lastArmRotation = deltaRot * lastArmRotation;

            if ((Vector3.Distance(new Vector3(0f, 0f, 0f), lastArmPosition)) < 1.5) {
                targetTransform.position = lastArmPosition;
            }
            targetTransform.rotation = lastArmRotation;

            //Vector3 outPos = UnityToRosPositionAxisConversion(lastArmTF.position + deltaPos) / scale;
            Vector3 outPos = UnityToRosPositionAxisConversion(lastArmPosition) / scale;
            //Quaternion outQuat = UnityToRosRotationAxisConversion(deltaRot * lastArmTF.rotation);
            Quaternion outQuat = UnityToRosRotationAxisConversion(lastArmRotation);

            message = outPos.x + " " + outPos.y + " " + outPos.z + " " + outQuat.x + " " + outQuat.y + " " + outQuat.z + " " + outQuat.w + " moveToEEPose";
        }
        else if (controller.touchpadPressed) {
            float angle = controller.GetTouchpadAxisAngle();

            if (angle >= 45 && angle < 135) // touching right
                message += " yDown ";
            else if (angle >= 135 && angle < 225) // touching bottom
                message += " xDown ";
            else if (angle >= 225 && angle < 315) // touching left
                message += " yUp ";
            else //touching top
                message += " xUp ";
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

