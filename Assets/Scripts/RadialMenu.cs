using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour {

    public WebsocketClient wsc;

    private SteamVR_TrackedController controller;
    private DepthRosGeometryView drgv;
    private float timeOfLastPress;
    private float debounceAmount = 0.1f;
    private bool pressedLastUpdate = false;

    // Use this for initialization
    void Start () {
        controller = GetComponent<SteamVR_TrackedController>();
        drgv = GameObject.Find("kinect2_linkPivot"+).GetComponent<DepthRosGeometryView>();
        timeOfLastPress = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
		if (controller.padPressed && Time.time - timeOfLastPress > debounceAmount && !pressedLastUpdate) {
            drgv.updatePointCloud = !drgv.updatePointCloud;
            timeOfLastPress = Time.time;
        }
        if (controller.padPressed) {
            pressedLastUpdate = true;
        } else {
            pressedLastUpdate = false;
        }
	}
}
