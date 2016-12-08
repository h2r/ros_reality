using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using System.Threading;


public class WebsocketClient : MonoBehaviour
{

	private WebSocket ws;
	private int counter = 1;
	public string message;
	// Use this for initialization
	void Start ()
	{
		ws = new WebSocket("ws://138.16.160.16:9090");

		ws.OnOpen += OnOpenHandler;
		ws.OnMessage += OnMessageHandler;
		ws.OnClose += OnCloseHandler;

		ws.ConnectAsync();
		Thread.Sleep(1000);
		//Subscribe ("ros_unity", "std_msgs/String");
	}

	public void Subscribe(string topic, string type)
	{
		string message = "{\"op\":\"subscribe\",\"id\":\"subscribe:/" + topic + ":" + counter + "\",\"type\":\"" + type + "\",\"topic\":\"/" + topic + "\",\"compression\":\"none\",\"throttle_rate\":0,\"queue_length\":0}";
		ws.SendAsync(message, OnSendComplete);
		counter++;
	}

	public void Advertise(string topic, string type)
	{
		string message = "{\"op\":\"advertise\",\"id\":\"advertise:/" + topic + ":" + counter + "\",\"type\":\"" + type + "\",\"topic\":\"/" + topic + "\",\"latch\":false,\"queue_size\":0}";
		Debug.Log (message);
		ws.SendAsync(message, OnSendComplete);
		counter++;

	}

	public void Publish(string topic, string message)
	{
		string msg = "{\"op\":\"publish\",\"id\":\"publish:/" + topic + ":" + counter + "\",\"topic\":\"/" + topic + "\",\"msg\":{\"data\":\"" + message + "\"},\"latch\":false}";
		ws.SendAsync(msg, OnSendComplete);
		counter++;
	}

	public void SendEinMessage(string message, string arm)
	{
		Publish ("ein/" + arm + "/forth_commands", message);
	}

	private void OnMessageHandler(object sender, MessageEventArgs e)
	{
		//Debug.Log("WebSocket server said: " + e.Data);
		message = e.Data;
		//setBaxterTransform(e.Data);
		//Thread.Sleep(10);
	}

	private void OnOpenHandler(object sender, System.EventArgs e)
	{
		Debug.Log("WebSocket connected!");
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

