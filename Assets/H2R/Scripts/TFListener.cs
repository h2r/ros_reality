using UnityEngine;
using System.Collections;

public class TFListener : MonoBehaviour {


	private WebsocketClient wsc;


	// Use this for initialization
	void Start () {
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		wsc.Subscribe ("ros_unity", "std_msgs/String");
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}

	void FixedUpdate() {
		string message = wsc.message;
		string[] dataPairs = message.Split(';');
		if (dataPairs.Length > 0)
		{
			for (int i = 0; i < dataPairs.Length; i++)
			{
				string[] dataPair = dataPairs[i].Split(':');
				GameObject cur = GameObject.Find (dataPair [0]); // replace with hashmap
				if (cur != null && dataPair [0] != "screen")
				{

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

					//Quaternion curRot = new Quaternion (rot_x, rot_y, rot_z, rot_w);
					Quaternion curRot = new Quaternion (rot_x, -1*rot_z, rot_y, rot_w);

					cur.transform.position = RosToUnityPositionAxisConversion(curPos);
					cur.transform.rotation = curRot;
					//cur.transform.eulerAngles = RosToUnityRotationAxisConversion(curRot);

					//Debug.Log (dataPair [0] + "******" + pos  + "&&&&" + rot);
				}
			}
		}
	}

	Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn) 
	{
		return new Vector3 (-rosIn.x, rosIn.z + 0.5f, -rosIn.y);	
	}

	Vector3 RosToUnityRotationAxisConversion(Quaternion rosIn) 
	{
		float roll = Mathf.Atan2 (2 * rosIn.y * rosIn.w + 2 * rosIn.x * rosIn.z, 1 - 2 * rosIn.y * rosIn.y - 2 * rosIn.z * rosIn.z);
		float pitch = Mathf.Atan2 (2 * rosIn.x * rosIn.w + 2 * rosIn.y * rosIn.z, 1 - 2 * rosIn.x * rosIn.x - 2 * rosIn.z * rosIn.z);
		float yaw = Mathf.Asin (2 * rosIn.x * rosIn.y + 2 * rosIn.z * rosIn.w);
		return new Vector3 (-rosIn.x, rosIn.z, -rosIn.y);	
	}


}
