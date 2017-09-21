using UnityEngine;
using System.Collections;

public class DepthRosTextureView : MonoBehaviour {

	private WebsocketClient wsc;
	string topic;
	public int framerate = 30;
	public string compression = "none"; //"png" is the other option, haven't tried it yet though
	string message;

	Texture2D texture;

	int width = 512;
	int height = 424;

	Matrix4x4 m;
	Renderer rend;

	// Use this for initialization
	void Start () 
	{
		texture = new Texture2D(width, height, TextureFormat.RGB565, false);
		GetComponent<Renderer> ().material.mainTexture = texture;
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		topic = "kinect2/sd/image_depth_rect";
		wsc.Subscribe (topic, "sensor_msgs/Image", compression, framerate);
	}
	
	// Update is called once per frame
	void Update ()
	{
		try {
			message = wsc.messages[topic];
		} catch {
			return;
		}
		byte[] image = System.Convert.FromBase64String(message);
		texture.LoadRawTextureData (image);
		texture.Apply ();

		Debug.Log(texture.GetPixel(200,200));

		float max = 0;
		float min = 100000;
		float cur;
		Color c;
		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < width; j++) {
				c = texture.GetPixel (i, j);
				cur = (float) DepthFromPackedRGB565(c);
				if (cur > max)
				{
					max = cur;
				}
				if (cur < min)
				{
					min = cur;
				}
			}
		}
		Debug.Log ("max = " + max);
		Debug.Log ("min = " + min);
	}

	Vector4 ConvertColor(Color c) {
		return new Vector4(c.r / 255, c.g /255, c.b /255, c.a /255);
	}
	float DepthFromPacked4444(Vector4 packedDepth)
	{
		// convert from [0,1] to [0,15]
		packedDepth *= 15.01f;
		//packedDepth /= 255.01f;
		return ((int) packedDepth.w) * 4096 + ((int) packedDepth.x) * 256 + ((int) packedDepth.y) * 16 + ((int) packedDepth.z);				
	}

	uint DepthFromPackedRGB565(Color enc) {
		uint depth = (uint)enc.r * 31 << 11 | (uint)enc.g * 63 << 5 | (uint)enc.b * 31;
		return depth;
	}

}