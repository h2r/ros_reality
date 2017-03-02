using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	public Vector2 PanSensitivity = new Vector2(2.0f, 2.0f);
	public Vector2 RotateSensitivity = new Vector2(4.0f, 4.0f);
	public float ZoomSensititity = 15.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector3(0.0f, 0.0f, Input.GetAxis("Mouse ScrollWheel") * ZoomSensititity));

		//pan
		if (Input.GetMouseButton(2))
		{
			transform.Translate(new Vector3(Input.GetAxis("Mouse X") * PanSensitivity.x,
			                                Input.GetAxis("Mouse Y") * PanSensitivity.y,
			                                0.0f));
		}
		//rotate
		else if (Input.GetMouseButton(1))
		{
			transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y") * RotateSensitivity.y,
			                                      Input.GetAxis("Mouse X") * RotateSensitivity.x,
			                                      0.0f);
		}
	}
}
