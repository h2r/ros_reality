using UnityEngine;
using System.Collections;
using System;

public class DepthRosGeometryView : MonoBehaviour {

	private WebsocketClient wsc;
	string depthTopic;
	string colorTopic;
	public int framerate = 30;
	public string compression = "none"; //"png" is the other option, haven't tried it yet though
	string depthMessage;
	string colorMessage;

	public Material Material;
	Texture2D depthTexture;
	Texture2D colorTexture;

	int width = 512;
	int height = 424;

	Matrix4x4 m;

	// Use this for initialization
	void Start () 
	{
		depthTexture = new Texture2D(width, height, TextureFormat.R16, false);
		colorTexture = new Texture2D(2, 2);
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		depthTopic = "kinect2/sd/image_depth_rect";
		colorTopic = "kinect2/sd/image_color_rect/compressed";
		wsc.Subscribe (depthTopic, "sensor_msgs/Image", compression, framerate);
		wsc.Subscribe (colorTopic, "sensor_msgs/CompressedImage", compression, framerate);
		InvokeRepeating ("UpdateTexture", 0.1f, 0.1f);

	
		//this.transform.f
	}
	
	// Update is called once per frame
	void UpdateTexture ()
	{
		//Debug.Log("hello");

		try {
			depthMessage = wsc.messages[depthTopic];
			byte[] depthImage = System.Convert.FromBase64String(depthMessage);

			depthTexture.LoadRawTextureData (depthImage);
			//depthTexture.LoadImage(depthImage);
			depthTexture.Apply ();
			//Debug.Log(depthTexture.GetType());

		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
			
		try {
			colorMessage = wsc.messages[colorTopic];
			byte[] colorImage = System.Convert.FromBase64String(colorMessage);
			colorTexture.LoadImage (colorImage);
			colorTexture.Apply ();
		} catch {
			return;
		}



		//Debug.Log (image.Length);

	}

	void OnRenderObject() 
	{
		
		Material.SetTexture ("_MainTex", depthTexture);
		Material.SetTexture ("_ColorTex", colorTexture);
		//Material.SetVector ("_OriginOffset", this.transform.position);
		Material.SetPass(0);

		m = Matrix4x4.TRS (this.transform.position, this.transform.rotation, this.transform.localScale);
		Material.SetMatrix ("transformationMatrix", m);


		Graphics.DrawProcedural (MeshTopology.Points, 512 * 424, 1);

		//Debug.Log (_DepthManager.GetDepthWidth ());
		//Debug.Log (_DepthManager.GetDepthHeight ());
	}
}