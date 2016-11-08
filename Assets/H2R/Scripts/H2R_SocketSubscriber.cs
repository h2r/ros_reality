using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using System.Threading;


public class H2R_SocketSubscriber : MonoBehaviour
{

    private Transform tf;
    private float yAxis = 0;
    private int counter = 3;
    private WebSocket ws;

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
        Debug.Log("hello");

        //SendEinMessage("goHome");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = new Vector3(0, yAxis * 10, 0);
        
        tf.rotation = Quaternion.Euler(temp);

        if (!EditorApplication.isPlayingOrWillChangePlaymode &&
              EditorApplication.isPlaying)
        {
            Debug.Log("Exiting playmode.");
            ws.CloseAsync();
        }
    }

    private void OnOpenHandler(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connected!");
        ws.SendAsync("{\"op\":\"subscribe\",\"id\":\"subscribe:/ros_unity:1\",\"type\":\"std_msgs/String\",\"topic\":\"/ros_unity\",\"compression\":\"none\",\"throttle_rate\":0,\"queue_length\":0}", OnSendComplete);
        ws.SendAsync("{\"op\":\"advertise\",\"id\":\"advertise:/ein/left/forth_commands:2\",\"type\":\"std_msgs/String\",\"topic\":\"/ein/left/forth_commands\",\"latch\":false,\"queue_size\":1}", OnSendComplete);
       
    }

    private void SendEinMessage(string message)
    {
        string msg = "{\"op\":\"publish\",\"id\":\"publish:/ein/left/forth_commands:" + counter + "\",\"topic\":\"/ein/left/forth_commands\",\"msg\":{\"data\":\"" + message + "\"},\"latch\":false}";
        ws.SendAsync(msg, OnSendComplete);
        counter++;
        Debug.Log("SendEin is happening");
    }
    private void OnMessageHandler(object sender, MessageEventArgs e)
    {
        //Debug.Log("WebSocket server said: " + e.Data);
        setBaxterTransform(e.Data);
        //Thread.Sleep(10);
        
    }

    private void OnCloseHandler(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket closed with reason: " + e.Reason);
    }

    private void OnSendComplete(bool success)
    {
        Debug.Log("Message sent successfully? " + success);
    }

    private void setBaxterTransform(string baxterMessage)
    {
        string[] dataPairs = baxterMessage.Split(',');
        if (dataPairs.Length > 0)
        {
            for (int i = 0; i < dataPairs.Length; i++)
            {
                string[] dataPair = dataPairs[i].Split(':');
                if (dataPair[0] == "r_6")
                {
                    yAxis = (2.5f * float.Parse(dataPair[1])) + 5;
                    //Debug.Log(yAxis);
                }
            }
        }
        // Vector3 temp = new Vector3(float.Parse(entries[0]),float.Parse(entries[1]),float.Parse(entries[2]));
        //tf.position = temp;
        //Debug.Log("s");
    }

}
