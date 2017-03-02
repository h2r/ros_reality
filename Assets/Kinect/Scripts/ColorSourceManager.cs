using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceManager : MonoBehaviour 
{
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }

    private KinectSensor _Sensor;
    private ColorFrameReader _Reader;
    private Texture2D _Texture;
    private byte[] _Data;
    
    public Texture2D GetColorTexture()
    {
        return _Texture;
    }
    
    void Start()
    {
        _Sensor = KinectSensor.GetDefault ();
        
        if (_Sensor != null) 
        {
            _Reader = _Sensor.ColorFrameSource.OpenReader();
            
            var frameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = frameDesc.Width;
            ColorHeight = frameDesc.Height;
			Debug.Log (ColorWidth);
			Debug.Log (ColorHeight);
            
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
			_Data = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];
			
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }
    
    unsafe void Update () 
    {
        if (_Reader != null) 
        {
            var frame = _Reader.AcquireLatestFrame();
            
            if (frame != null)
            {
				fixed (byte* pData = _Data)
				{
					frame.CopyConvertedFrameDataToIntPtr(new System.IntPtr(pData), (uint)_Data.Length, ColorImageFormat.Rgba);
				}
				frame.Dispose();
				frame = null;
				
				_Texture.LoadRawTextureData(_Data);
				_Texture.Apply();
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null) 
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null) 
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
