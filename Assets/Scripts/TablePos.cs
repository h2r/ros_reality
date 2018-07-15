using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePos : MonoBehaviour {

    private GameObject head;
	public float delta3;
	private WebsocketClient wsc;
	// Use this for initialization
	void Start () {
        head = GameObject.Find("Camera (eye)");
		GameObject wso = GameObject.FindWithTag("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient>();

		wsc.Advertise("/follow_vr_head", "std_msgs/String");
	}

	// Update is called once per frame
	void Update () {
		head = GameObject.Find("Camera (eye)");
		Debug.Log (head.transform.position + Vector3.forward);
		transform.position = head.transform.position + head.transform.forward*delta3;
		transform.position = intersect_gaze_with_plane ();

		Vector3 outPos = UnityToRosPositionAxisConversion(transform.position);

		string msg = outPos.x.ToString () + " " + outPos.y.ToString () + " " + outPos.z.ToString ();

	    wsc.Publish("/follow_vr_head", msg);

	}
	Vector3 intersect_gaze_with_plane (){
		float t = -head.transform.position.y / head.transform.forward.y;
		Vector3 i = head.transform.position + head.transform.forward * t;
		Debug.Log (i);
		return i;
	}

	Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn) {
		return new Vector3(-rosIn.x, -rosIn.z, rosIn.y);
	}
		
}
