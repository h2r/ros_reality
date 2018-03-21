using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFactoryManager : MonoBehaviour {
    private int numRobots = 0;
	// Use this for initialization

    public int assignRobotNumber() {
        this.numRobots = this.numRobots + 1;
        return numRobots;
    }
}
