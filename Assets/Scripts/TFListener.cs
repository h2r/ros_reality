using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TFListener : MonoBehaviour
{


	private WebsocketClient wsc;
	public string topic = "ros_unity";

	public float scale = 1f;

    public class CurrentJointInfo
    {
        public string name { get; set; }
        public List<float> pos { get; set; }
        public List<float> rot { get; set; }
    }

	// Use this for initialization
	void Start ()
	{
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
		wsc.Subscribe (topic, "std_msgs/String", 0);

		//Attach table stuff
		GameObject basePivot = GameObject.Find ("basePivot"); // replace with hashmap
		GameObject table = GameObject.Find ("Table");
		table.transform.SetParent (basePivot.transform); //make table the parent object of the base pivot
		table.transform.localPosition = new Vector3 (0f, -.15f, 0f); //new local position
		table.transform.localScale = new Vector3 (0.2123f, 1f, 0.2123f);//local scaling of robot
	}

	void Update () 
	{
		string message = wsc.messages[topic]; //get newest robot state data (from transform)
        Debug.Log(message);
        //Debug.Log("\"Hi\"");
		string[] tfElements = message.Split (';'); //split the message into each joint/link data pair
        
        //Debug.Log(string.Join(", ", tfElements));
        foreach (string tfElement in tfElements) {
            //continue;
            //var currentJoint = JsonUtility.FromJson<CurrentJointInfo>(tfElement);
            var currentJoint = JsonUtility.FromJson<CurrentJointInfo>(tfElement);
            Debug.Log(currentJoint.name);
            GameObject cur = GameObject.Find (currentJoint.name + "Pivot"); // replace with hashmap
			if (cur != null) {
                
				//position
                Vector3 curPos = new Vector3 (currentJoint.pos[0], currentJoint.pos[1], currentJoint.pos[2]); //save current position
			
                //rotation
				Quaternion curRot = new Quaternion (currentJoint.rot[0], currentJoint.rot[1], currentJoint.rot[2], currentJoint.rot[3]);

				cur.transform.position = Vector3.Lerp(scale * RosToUnityPositionAxisConversion (curPos), cur.transform.position, 0.7f); //convert ROS coordinates to Unity coordinates and scale for position vector
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
