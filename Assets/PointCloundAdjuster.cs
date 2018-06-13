using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloundAdjuster : MonoBehaviour {

    public Transform PointCloudRoot;
    public string ActivationButton;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

	void Start () {
        UpdateLastPose();
    }
	
	void Update () {
        if(Input.GetButton(ActivationButton)) {
            MovePointCloud();
        }
        UpdateLastPose();
	}

    void UpdateLastPose() {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void MovePointCloud() {
        PointCloudRoot.position += transform.position - lastPosition;
        PointCloudRoot.rotation = (transform.rotation * Quaternion.Inverse(lastRotation)) * PointCloudRoot.rotation;
    }
}
