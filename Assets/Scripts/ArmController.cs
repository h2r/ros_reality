using UnityEngine;
using System.Collections;
using System.Threading;

public class ArmController : MonoBehaviour
{
	public string arm;
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
		controller = GetComponent<VRTK.VRTK_ControllerEvents>();

		TFListener = GameObject.Find ("TFListener").GetComponent<TFListener> ();
	}

	void sendControls()
	{
		scale = TFListener.scale;
        
		Vector3 outPos = UnityToRosPositionAxisConversion(GetComponent<Transform>().position) / scale;
		Quaternion outQuat = UnityToRosRotationAxisConversion (GetComponent<Transform>().rotation);
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

		Quaternion temp = (new Quaternion (qIn.x, qIn.z, -qIn.y, qIn.w))*(new Quaternion(0,1,0,0));
		return temp;
	}

}

