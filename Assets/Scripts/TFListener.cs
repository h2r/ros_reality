using UnityEngine;
using System;
using System.Collections;

public class TFListener : MonoBehaviour {


    private WebsocketClient wsc;
    public string topic = "ros_unity";

    public float scale = 1f;

    // Use this for initialization
    void Start() {
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
        wsc.Subscribe(topic, "std_msgs/String", 0);
    }

    void Update() {
        string message = wsc.messages[topic]; //get newest robot state data (from transform)
        string[] tfElements = message.Split(';'); //split the message into each joint/link data pair
        foreach (string tfElement in tfElements) {
            string[] dataPair = tfElement.Split(':');
            GameObject cur = GameObject.Find(dataPair[0] + "Pivot"); // replace with hashmap
            if (cur != null) {
                string[] tmp = dataPair[1].Split('^'); //seperate position from rotation data
                string pos = tmp[0]; //position data
                string rot = tmp[1]; //rotation data
                pos = pos.Substring(1, pos.Length - 2);
                rot = rot.Substring(1, rot.Length - 2);
                string[] poses = pos.Split(',');
                float pos_x = float.Parse(poses[0]); //x position
                float pos_y = float.Parse(poses[1]); //y position
                float pos_z = float.Parse(poses[2]); //z position


                Vector3 curPos = new Vector3(pos_x, pos_y, pos_z); //save current position
                string[] rots = rot.Split(',');
                //save rotation as quaternions
                float rot_x = float.Parse(rots[0]);
                float rot_y = float.Parse(rots[1]);
                float rot_z = float.Parse(rots[2]);
                float rot_w = float.Parse(rots[3]);


                Quaternion curRot = new Quaternion(rot_x, rot_y, rot_z, rot_w);
                if (!cur.name.Contains("kinect")) { //rescaling direction of kinect point cloud
                    cur.transform.localScale = new Vector3(scale, scale, scale);
                }
                else {
                    cur.transform.localScale = new Vector3(-scale, scale, -scale);
                    continue;
                }
                cur.transform.position = Vector3.Lerp(scale * RosToUnityPositionAxisConversion(curPos), cur.transform.position, 0.7f); //convert ROS coordinates to Unity coordinates and scale for position vector
                cur.transform.rotation = Quaternion.Slerp(RosToUnityQuaternionConversion(curRot), cur.transform.rotation, 0.7f); //convert ROS quaternions to Unity quarternions
                
            }
        }
    }

    //convert ROS position to Unity Position
    Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn) {
        return new Vector3(-rosIn.x, rosIn.z, -rosIn.y);
    }

    //Convert ROS quaternion to Unity Quaternion
    Quaternion RosToUnityQuaternionConversion(Quaternion rosIn) {
        return new Quaternion(rosIn.x, -rosIn.z, rosIn.y, rosIn.w);
    }
}
