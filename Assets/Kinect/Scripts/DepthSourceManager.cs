using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;

public class DepthSourceManager : MonoBehaviour
{   
    private KinectSensor _Sensor;
	private DepthFrameReader _Reader;

	private Texture2D _Texture;
	public Texture2D GetDepthTexture()
	{
		return _Texture;
	}
	
	private byte[] _Data;
	private ushort[] ushortData;
	public ushort[] GetData()
	{
		Buffer.BlockCopy (_Data, 0, ushortData, 0, _Data.Length);
		return ushortData;
	}
	
	private int _DepthWidth;
	public int GetDepthWidth()
	{
		return _DepthWidth;
	}
	
	private int _DepthHeight;
	public int GetDepthHeight()
	{
		return _DepthHeight;
	}

    void Start () 
	{
		_Sensor = KinectSensor.GetDefault();
		
		if (_Sensor != null) 
        {
			_Reader = _Sensor.DepthFrameSource.OpenReader();

			var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

			_DepthWidth = frameDesc.Width;
			_DepthHeight = frameDesc.Height;
			
			// Use ARGB4444 as there's no handier 16 bit texture format readily available
			_Texture = new Texture2D(_DepthWidth, _DepthHeight, TextureFormat.R16, false);
			_Data = new byte[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels * _Sensor.DepthFrameSource.FrameDescription.BytesPerPixel];
			ushortData = new ushort[_Data.Length];

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
					frame.CopyFrameDataToIntPtr(new System.IntPtr(pData), (uint)_Data.Length);
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
