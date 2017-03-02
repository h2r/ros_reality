using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMouseController : MonoBehaviour {

	public float translationSensitivity = 2;
	public float zoomSensitiviy = 10;

	public float rotationSensitiviry = 2;

	public string mouseHorizontalAxisName = "Mouse X";
	public string mouseVerticalAxisName = "Mouse Y";
	public string scrollAxisName = "Mouse ScrollWheel";

	Camera _camera;

	void Awake(){
		_camera = GetComponent<Camera>();
	}

	void Update () {
		//  translation
		float translateX = 0;
		float translateY = 0;

		if (Input.GetMouseButton(2)) {
			translateY = Input.GetAxis(mouseVerticalAxisName) * translationSensitivity;
			translateX = Input.GetAxis(mouseHorizontalAxisName) * translationSensitivity;
		}


		float zoom = Input.GetAxis(scrollAxisName) * zoomSensitiviy;

		transform.Translate(-translateX, -translateY, zoom);

		// rotation

		float rotationX = 0;
		float rotationY = 0;

		if (Input.GetMouseButton(1)) {
			rotationX = Input.GetAxis(mouseVerticalAxisName) * rotationSensitiviry;
			rotationY = Input.GetAxis(mouseHorizontalAxisName) * rotationSensitiviry;
		}

		transform.Rotate(0, rotationY, 0, Space.World);
		transform.Rotate(-rotationX, 0, 0 );

//		// Focus
//		if (Input.GetKeyDown(KeyCode.F)) {
//			Vector3 mp = Input.mousePosition;
//			Ray ray = _camera.ScreenPointToRay(mp);
//			RaycastHit hit;
//			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
//				FocusCameraOnGameObject(Camera.main, hit.transform.gameObject);
//			}
//		}
	}

	// http://answers.unity3d.com/questions/13267/how-can-i-mimic-the-frame-selected-f-camera-move-z.html
	Bounds CalculateBounds(GameObject go) {
		Bounds b = new Bounds(go.transform.position, Vector3.zero);
		Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
		foreach (Renderer r in rList) {
			b.Encapsulate(r.bounds);
		}
		return b;
	}

	void FocusCameraOnGameObject(Camera c, GameObject go) {
		Bounds b = CalculateBounds(go);
		Vector3 max = b.size;
		float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
		float dist = radius / (Mathf.Sin(c.fieldOfView * Mathf.Deg2Rad / 2f));
		c.transform.position = go.transform.position + transform.rotation * Vector3.forward * -dist;
	}
}