using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EinMenuScript : MonoBehaviour {
	public Button gh_l;
	public Button hp_l;
	public Button sp_l;
	public Button cp_l;

	public Button gh_r;
	public Button hp_r;
	public Button sp_r;
	public Button cp_r;

	private WebsocketClient wsc;
	// Use this for initialization
	void Start () 
	{
		GameObject wso = GameObject.FindWithTag ("WebsocketTag");
		wsc = wso.GetComponent<WebsocketClient> ();

		Button ghl = gh_l.GetComponent<Button> ();
		Button hpl = hp_l.GetComponent<Button> ();
		Button spl = sp_l.GetComponent<Button> ();
		Button cpl = cp_l.GetComponent<Button> ();

		Button ghr = gh_r.GetComponent<Button> ();
		Button hpr = hp_r.GetComponent<Button> ();
		Button spr = sp_r.GetComponent<Button> ();
		Button cpr = cp_r.GetComponent<Button> ();
	
		ghl.onClick.AddListener (goHome_left);
		hpl.onClick.AddListener (handingPose_left);
		spl.onClick.AddListener (shrugPose_left);
		cpl.onClick.AddListener (cranePose_left);

		ghr.onClick.AddListener (goHome_right);
		hpr.onClick.AddListener (handingPose_right);
		spr.onClick.AddListener (shrugPose_right);
		cpr.onClick.AddListener (cranePose_right);
	}

	void goHome_left()
	{
		Debug.Log ("cheecker");
		wsc.SendEinMessage ("goHome", "left");
	}

	void handingPose_left()
	{
		wsc.SendEinMessage ("assumeHandingPose", "left");
	}

	void shrugPose_left()
	{
		wsc.SendEinMessage ("assumeShrugPose", "left");
	}

	void cranePose_left()
	{
		wsc.SendEinMessage ("assumeCrane1", "left");
	}

	void goHome_right()
	{
		wsc.SendEinMessage ("goHome", "right");
	}

	void handingPose_right()
	{
		wsc.SendEinMessage ("assumeHandingPose", "right");
	}

	void shrugPose_right()
	{
		wsc.SendEinMessage ("assumeShrugPose", "right");
	}

	void cranePose_right()
	{
		wsc.SendEinMessage ("assumeCrane1", "right");
	}



}
