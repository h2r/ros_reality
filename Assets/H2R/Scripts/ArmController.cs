using UnityEngine;
using System.Collections;
using System.Threading;

public class ArmController : MonoBehaviour
{
	public string arm;
	public Transform tf;
	private WebsocketClient wsc;
	private VRTK.VRTK_ControllerEvents controller;
	TFListener TFListener;
	float scale;

	// Use this for initialization
	void Start ()
	{

		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();

		wsc.Advertise ("ein/" + arm + "/forth_commands", "std_msgs/String");
		InvokeRepeating ("sendControls", .1f, .1f);
		controller = this.GetComponent<VRTK.VRTK_ControllerEvents>();

		TFListener = GameObject.Find ("TFListener").GetComponent<TFListener> ();
	}

	void sendControls()
	{
		scale = TFListener.scale;
        
		Vector3 outPos = UnityToRosPositionAxisConversion (tf.position) / scale;
		Quaternion outQuat = UnityToRosRotationAxisConversion (tf.rotation);
		string message = "";
		//Allows movement control with controllers if menu is disabled

			if (controller.gripPressed) {
				message = outPos.x + " " + outPos.y + " " + outPos.z + " " + 
                outQuat.x + " " + outQuat.y + " " + outQuat.z + " " + outQuat.w + " moveToEEPose";
			} else if (controller.touchpadPressed) {
				float angle = controller.GetTouchpadAxisAngle ();

				if (angle >= 45 && angle < 135) // touching right
					message += " yDown ";
				else if (angle >= 135 && angle < 225) // touching bottom
					message += " xDown ";
				else if (angle >= 225 && angle < 315) // touching left
					message += " yUp ";
				else //touching top
					message += " xUp ";
			}
			if (controller.triggerPressed) {
				message += " openGripper ";
			} else {
				message += " closeGripper ";
			}

			wsc.SendEinMessage (message, arm);
		
		//Debug.Log(arm+":"+message);
	}

	Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn) 
	{
		return new Vector3 (-rosIn.x, -rosIn.z, rosIn.y);	
	}

	Quaternion UnityToRosRotationAxisConversion(Quaternion qIn) 
	{
		//return rosIn;
		//return new Quaternion (rosIn.y, -rosIn.x, rosIn.w, rosIn.z);
		//return new Quaternion (-rosIn.w, rosIn.z, rosIn.y, rosIn.x);
		//Quaternion curRot = new Quaternion (rot_x, -1*rot_z, rot_y, rot_w);
		//BELOW IS WORKING
		//return new Quaternion (qIn.y, qIn.z, -qIn.w, qIn.x); 
		//return new Quaternion (-qIn.y, qIn.z, -qIn.w, qIn.x);
		//return new Quaternion (qIn.z, qIn.y, -qIn.w, qIn.x);
		//Working a little more..
		//return new Quaternion (qIn.y, -qIn.z, -qIn.w, qIn.x);
		//return new Quaternion (qIn.y, -qIn.z, qIn.w, qIn.x);
		//return new Quaternion (qIn.y, -qIn.z, -qIn.w, -qIn.x);
		//return new Quaternion (qIn.z, -qIn.y, -qIn.w, qIn.x);
		//return new Quaternion (qIn.w, -qIn.z, -qIn.y, qIn.x);
		//return new Quaternion (qIn.y, -qIn.z, -qIn.x, qIn.w);
		//return new Quaternion (qIn.y, -qIn.z, -qIn.w, qIn.x);
		//90 rotation
		//return ((new Quaternion (qIn.x, qIn.z, -qIn.y, qIn.w))*(new Quaternion(0,1,0,0))); BEST SO FAR
		Quaternion temp = (new Quaternion (qIn.x, qIn.z, -qIn.y, qIn.w))*(new Quaternion(0,1,0,0));
		//temp = temp * new Quaternion (0, 0, (float)3.14159/2, 0);
		//return (temp*(new Quaternion(0.5f,0,0,0))); //zeus sticker on oppose side
		//temp = (temp*(new Quaternion(0,0,1,0)));
		//return (temp*(new Quaternion(0,0,0.5f,0))); //zeus sticker on oppose side
		return temp;
		//return ((new Quaternion (qIn.x, qIn.z, -qIn.y, qIn.w))*(new Quaternion(0,1,0,0))); // EVEN BETTER?
		//return new Quaternion (qIn.x, qIn.z, -qIn.y, qIn.w); good
		//return new Quaternion (qIn.x, -qIn.z, qIn.y, qIn.w);

		//return new Quaternion (qIn.x, -qIn.z, qIn.y, qIn.w);
	}

}

