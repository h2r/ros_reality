using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRobotJoint : MonoBehaviour {

	public int num;
	// Use this for initialization
	void Start () {
		
		foreach (Transform child in transform) 
		{
			child.gameObject.name = child.gameObject.name + num.ToString();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
}
