using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using System.Threading;


public class H2R_SocketPublisher : MonoBehaviour
{

    public Transform tf;
    private int counter = 3;
    private WebSocket ws;
    private bool gripperOpen = true;

    // Use this for initialization
    void Start()
    {
        ws = new WebSocket("ws://138.16.160.16:9090");

        ws.OnOpen += OnOpenHandler;
        ws.OnMessage += OnMessageHandler;
        ws.OnClose += OnCloseHandler;

        ws.ConnectAsync();
        Thread.Sleep(1000);

        InvokeRepeating("sendControls", .1f, .1f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!EditorApplication.isPlayingOrWillChangePlaymode &&
              EditorApplication.isPlaying)
        {
            //Debug.Log("Exiting playmode.");
            ws.CloseAsync();
        }

    }

    void sendControls()
    {
        //float x = gameObject.transform.position.x;
        //float y = gameObject.transform.position.y;
        //float z = gameObject.transform.position.z;

        float x = tf.position.x;
        float y = tf.position.y;
        float z = tf.position.z;

        //float qw = tf.rotation.w;
        //float qx = tf.rotation.x;
        //float qy = tf.rotation.y;
        //float qz = tf.rotation.z;

        Vector3 inRot = tf.eulerAngles;
        Vector3 outRot = new Vector3(inRot.z, inRot.y, inRot.x);
        Quaternion outQuat = Quaternion.Euler(outRot);

        float qw = outQuat.w;
        float qx = outQuat.x;
        float qy = outQuat.y;
        float qz = outQuat.z;
        //Debug.Log(tf.eulerAngles.ToString());

        float x0 = 1;
        float y0 = 0;
        float z0 = 0;
        float w0 = 0;

        string message = x + " " + z + " " + y + " " + w0 + " " + x0 + " " + y0 + " " + z0 + " moveToEEPose"; // correct, always down facing
        // message = x + " " + z + " " + y + " " + qw + " " + qx + " " + qy + " " + qz + " moveToEEPose";
        //Debug.Log(message);
        //SendEinMessage(message);
        VRTK.VRTK_ControllerEvents controller = this.GetComponent<VRTK.VRTK_ControllerEvents>();
        //if (controller.IsButtonPressed(VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Click))
        //{
        //    if (gripperOpen)
        //    {
        //        SendEinMessage("closeGripper");
        //        gripperOpen = false;
        //    }
        //   else
        //   {
        //        SendEinMessage("openGripper");
        //       gripperOpen = true;
        //   }
        //}
        if (controller.IsButtonPressed(VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Click))
        {
			message += " openGripper";
        }
        else
        {
			message += " closeGripper";
        }
		Debug.Log(message);
		SendEinMessage(message);
    }

    private void OnOpenHandler(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connected!");
        // ws.SendAsync("{\"op\":\"subscribe\",\"id\":\"subscribe:/ros_unity:1\",\"type\":\"std_msgs/String\",\"topic\":\"/ros_unity\",\"compression\":\"none\",\"throttle_rate\":0,\"queue_length\":0}", OnSendComplete);
        ws.SendAsync("{\"op\":\"advertise\",\"id\":\"advertise:/ein/left/forth_commands:2\",\"type\":\"std_msgs/String\",\"topic\":\"/ein/left/forth_commands\",\"latch\":false,\"queue_size\":1}", OnSendComplete);

    }

    private void SendEinMessage(string message)
    {
        string msg = "{\"op\":\"publish\",\"id\":\"publish:/ein/left/forth_commands:" + counter + "\",\"topic\":\"/ein/left/forth_commands\",\"msg\":{\"data\":\"" + message + "\"},\"latch\":false}";
        ws.SendAsync(msg, OnSendComplete);
        counter++;
        //Debug.Log("SendEin is happening");
    }
    private void OnMessageHandler(object sender, MessageEventArgs e)
    {
        //Debug.Log("WebSocket server said: " + e.Data);
        //setBaxterTransform(e.Data);
        //Thread.Sleep(10);

    }

    private void OnCloseHandler(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket closed with reason: " + e.Reason);
    }

    private void OnSendComplete(bool success)
    {
        //Debug.Log("Message sent successfully? " + success);
    }

}
