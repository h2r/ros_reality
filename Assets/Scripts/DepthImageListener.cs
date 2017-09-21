using UnityEngine;
using System.Collections;

public class DepthImageListener : MonoBehaviour {

	private WebsocketClient wsc;
	string topic;
	public int framerate = 1;
	public string compression = "none"; //"png" is the other option, haven't tried it yet though
	string message;

	public Material material;
	Texture2D texture;

	int width = 640;
	int height = 480;
	//byte[] image;
	Renderer rend;
	// Use this for initialization
	void Start () {
		texture = new Texture2D(width, height, TextureFormat.ARGB4444, false);
		rend = GetComponent<Renderer> ();
		rend.material.mainTexture = texture;
		rend.material.SetTextureScale ("_MainTex", new Vector2 (-1, 1));
		material.mainTexture = texture;
		//image = new byte[85151];
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		topic = "openni/depth_registered/hw_registered/image_rect_raw";
		wsc.Subscribe (topic, "sensor_msgs/Image", compression, framerate);

		InvokeRepeating ("RenderTexture", .5f, 1.0f/framerate);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void RenderTexture() {
		material.mainTexture = texture;

		message = wsc.messages[topic];
		byte[] image = System.Convert.FromBase64String(message);
		Debug.Log (image.Length);
		texture.LoadRawTextureData (image);
		texture.Apply ();

		material.SetPass(0);
		Graphics.DrawProcedural (MeshTopology.Points, width * height, 1);
	}
}
