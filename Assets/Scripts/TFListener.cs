using UnityEngine;
using System;
using System.Collections;

public class TFListener : MonoBehaviour
{


	private WebsocketClient wsc;
	public GameObject websocketgo;
	public string topic = "ros_unity";
	public GameObject myrobot;

    public float scale; //= 1f;

	// Use this for initialization
	void Start ()
	{
        // get the websocket from the inspector
		wsc = websocketgo.GetComponent<WebsocketClient>(); //GameObject.Find("WebsocketClient"+gameObject.name[gameObject.name.Length-1]).GetComponent<WebsocketClient>();
		wsc.Subscribe (topic, "std_msgs/String", 0);
        print("TF Listener script called: "+wsc.ip_address);
		//Attach table stuff
		//GameObject basePivot = GameObject.Find ("basePivot"); // replace with hashmap
		//GameObject table = GameObject.Find ("Table");
		//table.transform.SetParent (basePivot.transform); //make table the parent object of the base pivot
		//table.transform.localPosition = new Vector3 (0f, -.15f, 0f); //new local position
		//table.transform.localScale = new Vector3 (0.2123f, 1f, 0.2123f);//local scaling of robot
	}

	void Update () 
	{
		string message = wsc.messages[topic]; //get newest robot state data (from transform)
		string[] tfElements = message.Split (';'); //split the message into each joint/link data pair
        //Debug.Log(string.Join(", ", tfElements));
        foreach (string tfElement in tfElements) {
            Debug.Log(tfElement);
            //continue;
            //print(tfElement.Split(':'));
			string[] dataPair = tfElement.Split (':');
            //print("?: "+myrobot.GetComponent<GetRobotJoint>().num.ToString());
            //print(dataPair[0] + "Pivot" + myrobot.GetComponent<GetRobotJoint>().num.ToString());
			GameObject cur = GameObject.Find (dataPair [0] + "Pivot"); // replace with hashmap
			//GameObject cur = transform.find (dataPair [0] + "Pivot"); // replace with hashmap
			//GameObject cur = myrobot.GetComponent<GetRobotJoint>().getJoint(dataPair [0] + "Pivot");
			if (cur != null) {

				string[] tmp = dataPair [1].Split ('^'); //seperate position from rotation data
				string pos = tmp [0]; //position data
				string rot = tmp [1]; //rotation data
				pos = pos.Substring (1, pos.Length - 2);
				rot = rot.Substring (1, rot.Length - 2);
                string[] poses = pos.Split (',');
				float pos_x = float.Parse (poses [0]); //x position
				float pos_y = float.Parse (poses [1]); //y position
				float pos_z = float.Parse (poses [2]); //z position


                Vector3 curPos = new Vector3 (pos_x, pos_y, pos_z); //save current position
				string[] rots = rot.Split (',');
                //save rotation as quaternions
				float rot_x = float.Parse (rots [0]); 
				float rot_y = float.Parse (rots [1]);
				float rot_z = float.Parse (rots [2]);
				float rot_w = float.Parse (rots [3]);


				Quaternion curRot = new Quaternion (rot_x, rot_y, rot_z, rot_w);
                //Debug.Log(cur);
                //Debug.Log(curPos);

                cur.transform.position = Vector3.Lerp(scale * RosToUnityPositionAxisConversion (curPos), cur.transform.position, 0.7f); //convert ROS coordinates to Unity coordinates and scale for position vector
				//if (myrobot.GetComponent<GetRobotJoint> ().num.ToString () == "1") 
				//{
					cur.transform.position += new Vector3 (1, 0, 1);
				//}
                cur.transform.rotation = Quaternion.Slerp(RosToUnityQuaternionConversion (curRot), cur.transform.rotation, 0.7f); //convert ROS quaternions to Unity quarternions
				if (!cur.name.Contains("kinect")) { //rescaling direction of kinect point cloud
					cur.transform.localScale = new Vector3(scale, scale, scale);
				} else {
					cur.transform.localScale = new Vector3(-scale, scale, -scale);
				}
				//cur.transform.position = RosToUnityPositionAxisConversion (curPos);
				//cur.transform.rotation = RosToUnityQuaternionConversion (curRot);
			}
		}
	}

    //convert ROS position to Unity Position
	Vector3 RosToUnityPositionAxisConversion (Vector3 rosIn)
	{
		return new Vector3 (-rosIn.x, rosIn.z, -rosIn.y);	
	}

    //Convert ROS quaternion to Unity Quaternion
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
