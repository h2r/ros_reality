using UnityEngine;
using System.Collections;

public class ArmController : MonoBehaviour
{
	public string arm;
	public Transform tf;
	private WebsocketClient wsc;

	// Use this for initialization
	void Start ()
	{
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();
		wsc.Advertise ("ein/" + arm + "/forth_commands", "std_msgs/String");
		InvokeRepeating ("sendControls", .1f, .1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void sendControls()
	{

		float x = tf.position.x;
		float y = tf.position.y;
		float z = tf.position.z;

		float tw = tf.rotation.w;
		float tx = tf.rotation.x;
		float ty = tf.rotation.y;
		float tz = tf.rotation.z;

		Vector3 inRot = tf.eulerAngles;
		Vector3 outRot = new Vector3(inRot.z, inRot.y, inRot.x);
		Quaternion outQuat = Quaternion.Euler(outRot);

		float qw = outQuat.w;
		float qx = outQuat.x;
		float qy = outQuat.y;
		float qz = outQuat.z;

		float x0 = 1;
		float y0 = 0;
		float z0 = 0;
		float w0 = 0;

		string message = x + " " + z + " " + y + " " + w0 + " " + x0 + " " + y0 + " " + z0 + " moveToEEPose"; // correct, always down facing
		// message = x + " " + z + " " + y + " " + qw + " " + qx + " " + qy + " " + qz + " moveToEEPose";


		VRTK.VRTK_ControllerEvents controller = this.GetComponent<VRTK.VRTK_ControllerEvents>();

		if (controller.IsButtonPressed(VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Click))
		{
			message += " openGripper";
			//Debug.Log ("openGripper");
		}
		else
		{
			//Debug.Log ("closeGripper");
			message += " closeGripper";
		}

		wsc.SendEinMessage(message, arm);
		Debug.Log(arm+":"+message);
	}

}

