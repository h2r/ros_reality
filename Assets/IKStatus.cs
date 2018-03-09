using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKStatus : MonoBehaviour {

    private WebsocketClient wsc;
    string ik_topic = "ros_reality_ik_status";
    Renderer rend;

    void Start () {
        // Get the live websocket client
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
        wsc.Subscribe(ik_topic, "std_msgs/String", 0);
        rend = GetComponent<Renderer>();
        InvokeRepeating("CheckIK", 0.1f, 0.1f);
    }

    void CheckIK() {
        if (wsc.messages.ContainsKey(ik_topic) && wsc.messages[ik_topic].Equals("f")) {
            //Debug.Log("Ik Failed");
            wsc.messages[ik_topic] = "";
            rend.material.color = Color.red;
            StartCoroutine("Fade");
        } 
    }

    IEnumerator Fade() {
        for (float f = 1f; f >= 0; f -= 0.05f) {
            yield return null;
        }
        for (float f = 1f; f >= 0; f -= 0.01f) {
            if (f < 0.05f) {
                f = 0f;
            }
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return null;
        }
    }
}
