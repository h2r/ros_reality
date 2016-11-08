using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using System.Threading;


public class H2R_SocketPublisher : MonoBehaviour
{

    private Transform tf;
    private float yAxis = 0;
    private int counter = 3;
    private WebSocket ws;
    private bool gripperOpen = true;

    // Use this for initialization
    void Start()
    {
        tf = GetComponent<Transform>();
        ws = new WebSocket("ws://138.16.160.16:9090");

        ws.OnOpen += OnOpenHandler;
        ws.OnMessage += OnMessageHandler;
        ws.OnClose += OnCloseHandler;

        ws.ConnectAsync();
        Thread.Sleep(1000);
       // Debug.Log("hello");


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = new Vector3(0, yAxis * 10, 0);

        tf.rotation = Quaternion.Euler(temp);

        if (!EditorApplication.isPlayingOrWillChangePlaymode &&
              EditorApplication.isPlaying)
        {
            //Debug.Log("Exiting playmode.");
            ws.CloseAsync();
        }

        Quaternion q = gameObject.transform.rotation;
        float x = gameObject.transform.position.x;
        float y = gameObject.transform.position.y;
        float z = gameObject.transform.position.z;

        //float x0 = gameObject.transform.rotation.x;
        //float y0 = gameObject.transform.rotation.y;
        //float z0 = gameObject.transform.rotation.z;
        //float w0 = gameObject.transform.rotation.w;

        float x0 = 0;
        float y0 = 1;
        float z0 = 0;
        float w0 = 0;

       

        //string message = x + " " + y + " " + z + " " + x0 + " " + y0 + " " + z0 + " " + w0 + " moveToEEPose";
        string message = x + " " + z + " " + y + " " + x0 + " " + y0 + " " + z0 + " " + w0 + " moveToEEPose";
        Debug.Log(message);
        SendEinMessage(message);
        VRTK.VRTK_ControllerEvents controller = this.GetComponent<VRTK.VRTK_ControllerEvents>();
        if(controller.IsButtonPressed(VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Click))
        {
            if (gripperOpen)
            {
                SendEinMessage("closeGripper");
                gripperOpen = false;
            }
            else
            {
                SendEinMessage("openGripper");
                gripperOpen = true;
            }
        }
    }

    private void OnOpenHandler(object sender, System.EventArgs e)
    {
        //Debug.Log("WebSocket connected!");
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
        //Debug.Log("WebSocket closed with reason: " + e.Reason);
    }

    private void OnSendComplete(bool success)
    {
        //Debug.Log("Message sent successfully? " + success);
    }

}
