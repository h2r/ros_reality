# ROS Reality

The Windows/Unity portion of our ROS Reality package. Works in conjunction with our other package [ros_reality_bridge](https://github.com/h2r/ros_reality_bridge).

## Dependencies

Currently only tested on Windows 10. The only large change needed to make this MacOS or Linux compatible is finding a new websocket library.

## Installation
1. Download and install [ros_reality_bridge](https://github.com/h2r/ros_reality_bridge) on your robot computer. Follow instructions there.
2. Download and install Unity 2017.1
3. Clone this repo
4. Open repo as new project

## Scenes
There are 4 scenes that show off different ways to interact with the robot.
1. Crane Game Control - The touchpad controls the movement of the robot
2. Menu Demo - Demo showing how to build a menu
3. Position Control - The robot gripper is controller with the trigger, and the position of the end effector moves to the position of the controller with the controller side buttons (grips) are pressed down.
4. Trajectory Control -  The robot gripper is controller with the trigger, and the position of the end effector is controlled by the trajectory of the controller when the side buttons are pressed down.
