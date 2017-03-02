using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class URDFConverter : MonoBehaviour {

	Dictionary<string, Link> linkDict;
	Dictionary<string, Joint> jointDict;
	Dictionary<string, GameObject> robotParts;

	public string xmlPath;

	//string relativePath = "C:/Users/robot/Documents/ROS_Unity/Assets/H2R/URDFs/";
	string relativePath = "URDFs/Baxter/";

	void Start () 
	{
		//Load all links and joints from URDF
		//Link dictionary
		linkDict = loadLinkDict(xmlPath); //load in all the links
		foreach (KeyValuePair<string, Link> pair in linkDict) {
			//Debug.Log(((Link)pair.Value).toString());
		}
		jointDict = loadJointDict(xmlPath); //load in all the joints
		foreach (KeyValuePair<string, Joint> pair in jointDict) {
			//Debug.Log(((Joint)pair.Value).toString());
		}

		//Instaniate all the gameobject links
		char[] delims = { '/' };
		char[] delims2 = { '.' };
		foreach (KeyValuePair<string, Link> pair in linkDict) {
			if (pair.Value.getMeshFile () != null) {
				string meshfile = relativePath + pair.Value.getMeshFile ().Split (delims, 5) [4].Split (delims2) [0];
				GameObject instance = Instantiate (Resources.Load (meshfile, typeof(GameObject))) as GameObject;
				instance.name = pair.Key;
				//instance.transform.position += new Vector3 (0f, 1.0f, 0f);
				//foreach (Transform meshObject in instance.transform) {
				//	meshObject.rotation = Quaternion.identity;
				//}
			} else {
				//GameObject instance = new GameObject (pair.Key); // we do this because there are some empty links that only serve to attach other things together (ex: right_arm_mount)
			}



			//Rigidbody rb = instance.AddComponent<Rigidbody> () as Rigidbody;
			//rb.useGravity = false;


		}

		//Create parent heirachy for gameobjects based on joints

		foreach (KeyValuePair<string, Joint> pair in jointDict) {
			Joint curJoint = pair.Value;
			GameObject child = GameObject.Find (curJoint.getChildLink ().getName ());
			GameObject parent = GameObject.Find (curJoint.getParentLink ().getName ());
//			Debug.Log (((Joint)pair.Value).toString());
	//		child.transform.SetParent (parent.transform);
//			Debug.Log("initial LP:" + child.transform.localPosition.ToString() + " joint LP:" + RosToUnityPositionAxisConversion(curJoint.getXyz ()).ToString() + " link LP:" + RosToUnityPositionAxisConversion(curJoint.getChildLink ().getXyz ()).ToString());
//			child.transform.localPosition = RosToUnityPositionAxisConversion(curJoint.getXyz ());
//			child.transform.localPosition += RosToUnityPositionAxisConversion(curJoint.getChildLink ().getXyz ());
//			Debug.Log("Final LP:" + child.transform.localPosition.ToString());
//			Debug.Log("initial LEA:" + child.transform.localEulerAngles.ToString() + " joint EA:" + RosToUnityRotationAxisConversion(curJoint.getRpy ()).ToString() + " link EA:" + RosToUnityRotationAxisConversion(curJoint.getChildLink ().getRpy ()).ToString());
//			child.transform.localEulerAngles = RosToUnityRotationAxisConversion(curJoint.getRpy ());
//			child.transform.localEulerAngles += RosToUnityRotationAxisConversion(curJoint.getChildLink ().getRpy ());
//			Debug.Log("Final LEA:" + child.transform.localEulerAngles.ToString());
//
//
//
		}
	}

	Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn) 
	{
		return new Vector3 (-rosIn.x, rosIn.z, -rosIn.y);	
	}

	Vector3 RosToUnityRotationAxisConversion(Vector3 rosIn) 
	{
		Vector3 inDeg = rosIn * 180f / 3.14159f;
		if (rosIn.y > 0.01) {
			return new Vector3 (inDeg.x, -inDeg.z, inDeg.y);
		} else {
			return new Vector3 (inDeg.x, -inDeg.z, 0);
		}
	}

	//TODO: Deal with different types of joints (ie revolute)
	Dictionary<string, Joint> loadJointDict(string path)
	{
		Dictionary<string, Joint> jointDict = new Dictionary<string, Joint>(); //instaniate a dictionary to hold all joints

		//holds values for each loop of going through joints
		string jname = null;
		string jtype = null;
		Vector3 jo_rpy = new Vector3();
		Vector3 jo_xyz = new Vector3();
		Link jparentLink = null;
		Link jchildLink = null;

		bool jo_rpyF;
		bool jo_xyzF;
		bool jparentF;
		bool jchildF;


		XmlDocument xmlDoc = new XmlDocument(); 
		//Absolute path to the xml file
		xmlDoc.Load(path);
		//xmlDoc.Load("C:\\Users\\robot\\Documents\\ROS_Unity\\Assets\\H2R\\URDFs\\R2D2\\r2d2.xml");
		XmlElement root = xmlDoc.DocumentElement;

		char[] delims = { ' ' };

		//Load all of the links
		XmlNodeList jointList = root.SelectNodes ("/robot/joint");
		foreach (XmlNode joint in jointList) 
		{
			jname = null;
			jtype = null;
			jo_rpyF = false;
			jo_xyzF = false;
			jparentF = false;
			jchildF = false;

			jparentLink = null;
			jchildLink = null;

			jname = joint.Attributes["name"].Value;
			jtype = joint.Attributes["type"].Value;

			XmlNodeList jointComponents = joint.ChildNodes; //get components of the link

			foreach (XmlNode jointComp in jointComponents) {
				if (jointComp.Name == "origin") { //Found the visual component
					//Debug.Log("origin found!");
					string[] rpyVals = jointComp.Attributes ["rpy"].Value.Split (delims);
					jo_rpy = new Vector3 (float.Parse (rpyVals [0]), float.Parse (rpyVals [1]), float.Parse (rpyVals [2]));
					jo_rpyF = true;

					//lo_rpy = visualComp.Attributes ["rpy"].Value;
					//Debug.Log (visualComp.Attributes ["xyz"].Value);
					string[] xyzVals = jointComp.Attributes ["xyz"].Value.Split (delims);
					jo_xyz = new Vector3 (float.Parse (xyzVals [0]), float.Parse (xyzVals [1]), float.Parse (xyzVals [2]));
					jo_xyzF = true;
				} else if (jointComp.Name == "parent") {
					//Debug.Log ("parent found!");
					string name = jointComp.Attributes["link"].Value;
					jparentF = linkDict.TryGetValue (name, out jparentLink);
				} else if (jointComp.Name == "child") {
					string name = jointComp.Attributes["link"].Value;
					jchildF = linkDict.TryGetValue (name, out jchildLink);
				}
			}

			if (jname != null && jtype != null && jo_rpyF == true && jo_xyzF == true && jparentF == true && jchildF == true)
			{
				//Debug.Log (jname);
				jointDict.Add (jname, new Joint (jname, jtype, jo_rpy, jo_xyz, jparentLink, jchildLink));
			}
		}

		return jointDict;
	}

	Dictionary<string, Link> loadLinkDict(string path)
	{
		Dictionary<string, Link> linkDict = new Dictionary<string, Link>(); //instantiate a dictionary to hold all links

		//holds values for each loop of going through links
		string lname = null;
		Vector3 lo_rpy = new Vector3();
		Vector3 lo_xyz = new Vector3 ();
		string lmeshfile = null;
		Vector4 lmaterial = new Vector4 ();

		bool lo_rpyF;
		bool lo_xyzF;
		bool lmaterialF;

		XmlDocument xmlDoc = new XmlDocument(); 
		//Absolute path to the xml file
		xmlDoc.Load(path);
		//xmlDoc.Load("C:\\Users\\robot\\Documents\\ROS_Unity\\Assets\\H2R\\URDFs\\R2D2\\r2d2.xml");
		XmlElement root = xmlDoc.DocumentElement;

		char[] delims = { ' ' };

		//Load all of the links
		XmlNodeList linkList = root.SelectNodes ("/robot/link");
		foreach (XmlNode link in linkList) 
		{
			lname = null;
			lo_rpyF = false;
			lo_xyzF = false;
			lmeshfile = null;
			lmaterialF = false;

			XmlNodeList linkComponents = link.ChildNodes; //get components of the link
			lname = link.Attributes["name"].Value;
			foreach (XmlNode linkComp in linkComponents)  //loop through components of link: Probably visual, collision and intertia
			{
				if (linkComp.Name == "visual") //Found the visual component
				{
					//Debug.Log (link.Attributes["name"].Value); //Name of the link

					XmlNodeList visualComponents = linkComp.ChildNodes; //get components of the visual component

					foreach (XmlNode visualComp in visualComponents) //loop through visual components: origin and geometr 
					{
						if (visualComp.Name == "origin") {
							//Debug.Log (visualComp.Attributes ["rpy"].Value);
							string[] rpyVals = visualComp.Attributes ["rpy"].Value.Split (delims);
							lo_rpy = new Vector3 (float.Parse (rpyVals [0]), float.Parse (rpyVals [1]), float.Parse (rpyVals [2]));
							lo_rpyF = true;

							//lo_rpy = visualComp.Attributes ["rpy"].Value;
							//Debug.Log (visualComp.Attributes ["xyz"].Value);
							string[] xyzVals = visualComp.Attributes ["xyz"].Value.Split (delims);
							lo_xyz = new Vector3 (float.Parse (xyzVals [0]), float.Parse (xyzVals [1]), float.Parse (xyzVals [2]));
							lo_xyzF = true;
							//lo_xyz = visualComp.Attributes ["xyz"].Value;
						} else if (visualComp.Name == "geometry") { //get inside mesh
							foreach (XmlNode child in visualComp) {
								if (child.Name == "mesh") {
									//Debug.Log (mesh.Attributes ["filename"].Value);
									lmeshfile = child.Attributes ["filename"].Value;
								}
							}
						}
						else if (visualComp.Name == "material") 
						{
							//WAS HERE BEFORE
							//XmlNode materialColor = visualComp.FirstChild;

							//Debug.Log (materialColor.Attributes ["rgba"].Value);
							//lmaterial = materialColor.Attributes ["rgba"].Value;
							//WAS HERE BEFORE
							//string[] rgbaVals = materialColor.Attributes ["rgba"].Value.Split (delims);
							//lmaterial = new Vector4 (float.Parse (rgbaVals [0]), float.Parse (rgbaVals [1]), float.Parse (rgbaVals [2]),float.Parse (rgbaVals [3]));
							//WAS HERE BEFORE
							lmaterial = new Vector4(1,1,1,1);
							lmaterialF = true;
						}
					}
				}
			}

			if (lname != null) // && lo_rpyF == true && lo_xyzF == true && lmeshfile != null) //valid link, instantiate and add to dictionary
			{
				Link temp;
				if (lmaterialF == true) { //has a material vector as well
					temp = new Link (lname,lo_rpy,lo_xyz,lmeshfile,lmaterial);
				} else {
					temp = new Link (lname, lo_rpy, lo_xyz, lmeshfile);
				}
				linkDict.Add(lname,temp);
			}
		}

		//Debug.Log (linkDict.Count);
		//Debug.Log (nodeList.Item(0).InnerXml);

		return linkDict;
	}

	public class Link 
	{
		string name;
		Vector3 o_rpy;
		Vector3 o_xyz;
		string meshfile;
		Vector4 material;

		public Link(string name, Vector3 o_rpy, Vector3 o_xyz, string meshfile, Vector4 material) 
		{
			this.name = name;
			this.o_rpy = o_rpy;
			this.o_xyz = o_xyz;
			this.meshfile = meshfile;
			this.material = material;
		}

		public string toString()
		{
			string retString = "";
			retString += "Link Name:" + this.name + "\n";
			retString += "RPY:" + this.o_rpy.x + " " + this.o_rpy.y + " " + this.o_rpy.z + "\n";
			retString += "XYZ:" + this.o_xyz.x + " " + this.o_xyz.y + " " + this.o_xyz.z + "\n";
			retString += "mesh file:" + this.meshfile + "\n";
			retString += "Material:" + + this.material.x + " " + this.material.y + " " + this.material.z + " " + this.material.w;
			return retString;
		}

		public Link(string name, Vector3 o_rpy, Vector3 o_xyz, string meshfile) 
		{
			this.name = name;
			this.o_rpy = o_rpy;
			this.o_xyz = o_xyz;
			this.meshfile = meshfile;
			this.material = new Vector4(1,1,1,1);
		}

		public string getName()
		{
			return this.name;
		}

		public Vector3 getRpy()
		{
			return this.o_rpy;
		}

		public Vector3 getXyz()
		{
			return this.o_xyz;
		}

		public string getMeshFile()
		{
			return this.meshfile;
		}

		public Vector4 getMaterial()
		{
			return this.material;
		}

	}

	public class Joint
	{
		string name;
		string type;
		Vector3 o_rpy;
		Vector3 o_xyz;
		Link parentLink;
		Link childLink;

		public Joint(string name, string type, Vector3 o_rpy, Vector3 o_xyz, Link parentLink, Link childLink) 
		{
			this.name = name;
			this.type = type;
			this.o_rpy = o_rpy;
			this.o_xyz = o_xyz;
			this.parentLink = parentLink;
			this.childLink = childLink;
		}

		public string getName()
		{
			return this.name;
		}

		public string getType()
		{
			return this.type;
		}

		public Vector3 getRpy()
		{
			return this.o_rpy;
		}

		public Vector3 getXyz()
		{
			return this.o_xyz;
		}

		public Link getParentLink()
		{
			return this.parentLink;
		}

		public Link getChildLink()
		{
			return this.childLink;
		}

		public string toString()
		{
			string retString = "";
			retString += "Joint Name:" + this.name + "\n";
			retString += "type:" + this.type + "\n";
			retString += "RPY:" + this.o_rpy.x + " " + this.o_rpy.y + " " + this.o_rpy.z + "\n";
			retString += "XYZ:" + this.o_xyz.x + " " + this.o_xyz.y + " " + this.o_xyz.z + "\n";
			retString += "parent:" + this.getParentLink ().getName () + "\n";
			retString += "child:" + this.getChildLink ().getName () + "\n";
			return retString;
		}
	}
}
	
