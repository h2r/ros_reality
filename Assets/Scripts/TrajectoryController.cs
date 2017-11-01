using UnityEngine;
using System.Collections;
using System.Threading;

public class TrajectoryController : MonoBehaviour {

    public string arm;
    Transform tf;
    private WebsocketClient wsc;
    private VRTK.VRTK_ControllerEvents controller;
    TFListener TFListener;
    float scale;
    string message;
    public GameObject followTarget;

    // Use this for initialization
    void Start() {

        GameObject wso = GameObject.FindWithTag("WebsocketTag");
        wsc = wso.GetComponent<WebsocketClient>();

        wsc.Advertise("ein/" + arm + "/forth_commands", "std_msgs/String");
        wsc.Advertise("/demonstrations", "std_msgs/String");
        controller = GetComponent<VRTK.VRTK_ControllerEvents>();
        TFListener = GameObject.Find("TFListener").GetComponent<TFListener>();
        tf = GetComponent<Transform>();

        //InvokeRepeating("myUpdate", 1.2f, 1f); //send message to move arm by displacement of current controller position/rotation with previous position/rotation
    }



    void Update() {
        scale = TFListener.scale;

        //message to be sent over ROs network
        message = "";

        if (controller.triggerPressed) { //moves the arm to where the cube is
            followTarget.GetComponent<Renderer>().material.color = Color.green;

            Vector3 outPos = UnityToRosPositionAxisConversion(followTarget.GetComponent<Transform>().position) / scale;

            message = outPos.x + " " + outPos.y + " " + outPos.z + " " + 0 + " " + 1 + " " + 0 + " " + 0 + " moveToEEPose";

            wsc.SendEinMessage(message, arm);
            wsc.Publish("/demonstrations", message);

            StartCoroutine(waiter());

        }
        else {
            followTarget.GetComponent<Renderer>().material.color = Color.red;
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

    IEnumerator waiter() 
        {
            yield return new WaitForSeconds(1f);
        }

}

