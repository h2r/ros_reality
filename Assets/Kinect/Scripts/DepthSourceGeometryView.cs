using UnityEngine;
using System.Collections;

public class DepthSourceGeometryView : MonoBehaviour {
	public GameObject DepthSourceManager;

	public Material Material;

	private DepthSourceManager _DepthManager;
	Matrix4x4 m;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (DepthSourceManager == null)
		{
			return;
		}
		
		_DepthManager = DepthSourceManager.GetComponent<DepthSourceManager>();
		if (_DepthManager == null)
		{
			return;
		}	
	}

	void OnRenderObject() 
	{
		Material.mainTexture = _DepthManager.GetDepthTexture ();
		Material.SetVector ("_OriginOffset", this.transform.position);
		Material.SetPass(0);

		m = Matrix4x4.TRS (this.transform.position, this.transform.rotation, this.transform.localScale);
		Material.SetMatrix ("transformationMatrix", m);

		if (Camera.current.name == "Depth Camera" || true) 
		{
			Graphics.DrawProcedural (MeshTopology.Points, _DepthManager.GetDepthWidth () * _DepthManager.GetDepthHeight (), 1);
		}

		//Debug.Log (_DepthManager.GetDepthWidth ());
		//Debug.Log (_DepthManager.GetDepthHeight ());
	}
}