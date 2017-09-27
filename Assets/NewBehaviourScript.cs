using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
        private WebsocketClient wsc;
        string topic = "ros_unity";
	// Use this for initialization
	void Start () {
        // Get the live websocket client

        

        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
		wsc.Subscribe(topic, "std_msgs/String", "none", 0);

       

    }

    // Update is called once per frame
    void Update () {
		
	}
}
