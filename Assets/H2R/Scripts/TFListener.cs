using UnityEngine;
using System;
using System.Collections;

public class TFListener : MonoBehaviour
{


	private WebsocketClient wsc;
	string topic = "ros_unity";

	public float scale = 1f;

	// Use this for initialization
	void Start ()
	{
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		wsc.Subscribe (topic, "std_msgs/String", "none", 0);

		//Attach table stuff
		GameObject basePivot = GameObject.Find ("basePivot"); // replace with hashmap
		GameObject table = GameObject.Find ("Table");
		table.transform.SetParent (basePivot.transform);
		table.transform.localPosition = new Vector3 (0f, -.15f, 0f);
		table.transform.localScale = new Vector3 (0.2123f, 1f, 0.2123f);


	}
	
	// Update is called once per frame
	//void Update () {
	
	//}

	void FixedUpdate () // maybe should be fixed update
	{
		try {
			string message = wsc.messages[topic];
			//Debug.Log (message);
			string[] dataPairs = message.Split (';');

			if (dataPairs.Length > 0) {
				for (int i = 0; i < dataPairs.Length; i++) {
					string[] dataPair = dataPairs [i].Split (':');
					GameObject cur = GameObject.Find (dataPair [0] + "Pivot"); // replace with hashmap
					if (cur != null/* && dataPair [0] != "screen"*/) {

						string[] tmp = dataPair [1].Split (')');
						string pos = tmp [0];
						string rot = tmp [1];
						pos = pos.Substring (1, pos.Length - 1);
						rot = rot.Substring (1, rot.Length - 1);

						string[] poses = pos.Split (',');
						float pos_x = float.Parse (poses [0]);
						float pos_y = float.Parse (poses [1]);
						float pos_z = float.Parse (poses [2]);

						Vector3 curPos = new Vector3 (pos_x, pos_y, pos_z);


						string[] rots = rot.Split (',');
						float rot_x = float.Parse (rots [0]);
						float rot_y = float.Parse (rots [1]);
						float rot_z = float.Parse (rots [2]);
						float rot_w = float.Parse (rots [3]);


						Quaternion curRot = new Quaternion (rot_x, rot_y, rot_z, rot_w);

						cur.transform.position = Vector3.Lerp(scale * RosToUnityPositionAxisConversion (curPos), cur.transform.position, 0.7f);
						cur.transform.rotation = Quaternion.Slerp(RosToUnityQuaternionConversion (curRot), cur.transform.rotation, 0.7f);
						if (!cur.name.Contains("kinect")) {
							cur.transform.localScale = new Vector3(scale, scale, scale);
						} else {
							cur.transform.localScale = new Vector3(-scale, scale, -scale);
						}
						//cur.transform.position = RosToUnityPositionAxisConversion (curPos);
						//cur.transform.rotation = RosToUnityQuaternionConversion (curRot);

					}
				}
			}
		} catch (Exception e) {
			
		}
	}

	Vector3 RosToUnityPositionAxisConversion (Vector3 rosIn)
	{
		return new Vector3 (-rosIn.x, rosIn.z, -rosIn.y);	
	}

	Quaternion RosToUnityQuaternionConversion (Quaternion rosIn)
	{
		return new Quaternion (rosIn.x, -rosIn.z, rosIn.y, rosIn.w);
	}


		

	//Vector3 RosToUnityRotationAxisConversion(Quaternion rosIn) OLD, I don't think this ever worked
	//{
	//	float roll = Mathf.Atan2 (2 * rosIn.y * rosIn.w + 2 * rosIn.x * rosIn.z, 1 - 2 * rosIn.y * rosIn.y - 2 * rosIn.z * rosIn.z);
	//	float pitch = Mathf.Atan2 (2 * rosIn.x * rosIn.w + 2 * rosIn.y * rosIn.z, 1 - 2 * rosIn.x * rosIn.x - 2 * rosIn.z * rosIn.z);
	//	float yaw = Mathf.Asin (2 * rosIn.x * rosIn.y + 2 * rosIn.z * rosIn.w);
	//	return new Vector3 (-rosIn.x, rosIn.z, -rosIn.y);
	//}


}
