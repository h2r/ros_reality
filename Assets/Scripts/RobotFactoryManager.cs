using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFactoryManager : MonoBehaviour {
    private int numRobots = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int getRobotNumber() {
        this.numRobots = this.numRobots + 1;
        return numRobots;
    }
}
