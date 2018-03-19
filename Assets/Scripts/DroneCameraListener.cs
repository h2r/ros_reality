using UnityEngine;

// The CameraLister object subscribes to a CompressedImage ROS topic and renders the image onto a 2D texture.
// We have attached the texture to a plane floating next to the controller.
public class DroneCameraListener : MonoBehaviour {

    // string of which arm to control. Valid values are "left" and "right"

    //websocket client connected to ROS network
    private WebsocketClient wsc;
    //ROS topic that is streaming RGB video form Baxter's end effector
    string topic;
    //frame rate of the video in Unity
    public int framerate = 15;

    Renderer rend;
    Texture2D texture;

    void Start() {
        //rendering texture for the RGB feed
        rend = GetComponent<Renderer>();
        texture = new Texture2D(2, 2);
        rend.material.mainTexture = texture;

        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
        topic = "pidrone/picamera/image_compressed/compressed";
        wsc.Subscribe(topic, "sensor_msgs/CompressedImage", framerate);

        InvokeRepeating("RenderTexture", .5f, 1.0f / framerate);
    }

    // Converts the CompressedImage message from base64 into a byte array, and loads the array into texture
    void RenderTexture() {
        try {
            string message = wsc.messages[topic];
            byte[] image = System.Convert.FromBase64String(message);
            texture.LoadImage(image);
        } catch(System.FormatException) {
            Debug.Log("Compressed camera image corrupted in transmission");
        }
    }
}
