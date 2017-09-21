using UnityEngine;
using System.Collections;

public class keylogger : MonoBehaviour {
	private WebsocketClient wsc;
	TFListener TFListener;

	// Use this for initialization
	void Start () {

		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		//while (!wsc.IsConnected ()) {

		//}
		wsc.Advertise ("ein/" + "right" + "/forth_commands", "std_msgs/String");
		InvokeRepeating ("sendControls", .1f, .1f);

		TFListener = GameObject.Find ("TFListener").GetComponent<TFListener> ();
	}
	
	// Update is called once per frame
	void sendControls () {
		string message = "";
		if (Input.GetKey("left shift") && Input.GetKey ("w") ) {
			message += " oXUp ";
		}
		else if (Input.GetKey("left shift") && Input.GetKey ("a"))
			message += " oYUp ";
		else if (Input.GetKey("left shift") && Input.GetKey ("s"))
			message += " oXDown ";
		else if (Input.GetKey("left shift") && Input.GetKey ("d"))
			message += " oYDown ";
		else if (Input.GetKey("left shift") && Input.GetKey ("e"))
			message += " oZUp ";
		else if (Input.GetKey("left shift") && Input.GetKey ("q"))
			message += " oZDown ";
		else if (Input.GetKey ("w")) {
			message += " xUp ";
		}
		else if (Input.GetKey ("a"))
			message += " yUp ";
		else if (Input.GetKey ("s"))
			message += " xDown ";
		else if (Input.GetKey ("d"))
			message += " yDown ";
		else if (Input.GetKey ("e"))
			message += " zUp ";
		else if (Input.GetKey ("q"))
			message += " zDown ";
		else if (Input.GetKey ("r"))
			message += " openGripper ";
		else if (Input.GetKey ("f"))
			message += " closeGripper ";
	
		wsc.SendEinMessage (message, "right");
	}
}
