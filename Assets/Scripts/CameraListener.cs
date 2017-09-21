using UnityEngine;
using System.Collections;

public class CameraListener : MonoBehaviour {

	public string arm;

	private WebsocketClient wsc;
	string topic;
	public int framerate = 15;
	public string compression = "none"; //"png" is the other option, haven't tried it yet though

	Renderer rend;
	Texture2D texture;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
		texture = new Texture2D(2, 2);
		rend.material.mainTexture = texture;

		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		topic = "cameras/" + arm + "_hand_camera/image_compressed/compressed";
		wsc.Subscribe (topic, "sensor_msgs/CompressedImage", compression, framerate);

		InvokeRepeating ("renderTexture", .5f, 1.0f/framerate);

	}
	
	// Update is called once per frame
	void Update () {
		//renderTexture ();
	}

	void renderTexture() {
		string message = wsc.messages[topic];
		byte[] image = System.Convert.FromBase64String(message);
		texture.LoadImage (image);
	}
}
