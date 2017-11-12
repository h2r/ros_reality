using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class URDFParser : MonoBehaviour {

    Dictionary<string, Link> linkDict;
    Dictionary<string, Joint> jointDict;
    Dictionary<string, GameObject> robotParts;

    public string xmlPath;
    string relativePath = "URDFs/Nao/";

    public GameObject root;


    void Start() {
        // Load all links and joints from URDF
        linkDict = loadLinkDict(xmlPath); //load in all the links into link dictionary
        jointDict = loadJointDict(xmlPath); //load in all the joints into joint dictionary


        //Instaniate all the gameobject links
        char[] delims = { '/' };
        char[] delims2 = { '.' };
        foreach (KeyValuePair<string, Link> pair in linkDict) {
            GameObject instance;
            if (pair.Value.getMeshFile() != null) {
                string meshfile = relativePath + pair.Value.getMeshFile().Split(delims, 5)[4].Split(delims2)[0]; // NORMAL
                                                                                                                 //string meshfile = relativePath + pair.Value.getMeshFile().Split(delims, 5)[4].Split(delims2)[0] + "_decimate2"; // DECIMATED
                //Debug.Log(meshfile);
                instance = Instantiate(Resources.Load(meshfile, typeof(GameObject))) as GameObject;
                //Debug.Log(pair.Value.getName());
                //Debug.Log(pair.Value.getScale());
                instance.name = pair.Key;
            }
            else {
                instance = new GameObject(pair.Key); // we do this because there are some empty links that only serve to attach other things together (ex: right_arm_mount)
            }
            GameObject origin = new GameObject(pair.Key + "Origin");
            GameObject pivot = new GameObject(pair.Key + "Pivot");
            instance.transform.SetParent(origin.transform); // might have 'false' as second argument here
            origin.transform.SetParent(pivot.transform);
            origin.transform.localPosition = RosToUnityPositionAxisConversion(pair.Value.getXyz());
            origin.transform.localEulerAngles = RosToUnityRotationAxisConversion(pair.Value.getRpy());
            origin.transform.localScale = pair.Value.getScale();

            pivot.transform.SetParent(root.transform);
        }

        //Create parent heirachy for gameobjects based on joints

        //foreach (KeyValuePair<string, Joint> pair in jointDict) {
        //	Joint curJoint = pair.Value;
        //	GameObject child = GameObject.Find (curJoint.getChildLink ().getName () + "Pivot");
        //	GameObject parent = GameObject.Find (curJoint.getParentLink ().getName () + "Pivot");
        //	child.transform.SetParent (parent.transform);
        //	child.transform.localPosition = RosToUnityPositionAxisConversion(curJoint.getXyz ());
        //	child.transform.localEulerAngles = RosToUnityRotationAxisConversion(curJoint.getRpy ());
        //}
    }

    Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn) {
        return new Vector3(-rosIn.x, rosIn.z, -rosIn.y);
    }

    Vector3 RosToUnityRotationAxisConversion(Vector3 rosIn) {
        Vector3 inDeg = rosIn * Mathf.Rad2Deg;// 180f / Mathf.PI;
        return new Vector3(inDeg.x, -inDeg.z, inDeg.y);
    }

    //TODO: Deal with different types of joints (ie revolute)
    Dictionary<string, Joint> loadJointDict(string path) {
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
        XmlNodeList jointList = root.SelectNodes("/robot/joint");
        foreach (XmlNode joint in jointList) {
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
                    string[] rpyVals = jointComp.Attributes["rpy"].Value.Split(delims);
                    jo_rpy = new Vector3(float.Parse(rpyVals[0]), float.Parse(rpyVals[1]), float.Parse(rpyVals[2]));
                    jo_rpyF = true;

                    //lo_rpy = visualComp.Attributes ["rpy"].Value;
                    //Debug.Log (visualComp.Attributes ["xyz"].Value);
                    string[] xyzVals = jointComp.Attributes["xyz"].Value.Split(delims);
                    jo_xyz = new Vector3(float.Parse(xyzVals[0]), float.Parse(xyzVals[1]), float.Parse(xyzVals[2]));
                    jo_xyzF = true;
                }
                else if (jointComp.Name == "parent") {
                    //Debug.Log ("parent found!");
                    string name = jointComp.Attributes["link"].Value;
                    jparentF = linkDict.TryGetValue(name, out jparentLink);
                }
                else if (jointComp.Name == "child") {
                    string name = jointComp.Attributes["link"].Value;
                    jchildF = linkDict.TryGetValue(name, out jchildLink);
                }
            }

            if (jname != null && jtype != null && jo_rpyF == true && jo_xyzF == true && jparentF == true && jchildF == true) {
                //Debug.Log (jname);
                jointDict.Add(jname, new Joint(jname, jtype, jo_rpy, jo_xyz, jparentLink, jchildLink));
            }
        }

        return jointDict;
    }

    Dictionary<string, Link> loadLinkDict(string path) {
        Dictionary<string, Link> linkDict = new Dictionary<string, Link>(); //instantiate a dictionary to hold all links

        //holds values for each loop of going through links

        string lname = null;
        Vector3 lo_rpy = new Vector3();
        Vector3 lo_xyz = new Vector3();
        Vector3 lscale_v = new Vector3(1f,1f,1f);
        string lmeshfile = null;
        Vector4 lmaterial = new Vector4();

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
        XmlNodeList linkList = root.SelectNodes("/robot/link");
        foreach (XmlNode link in linkList) {
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
                            string[] rpyVals = visualComp.Attributes["rpy"].Value.Split(delims);
                            lo_rpy = new Vector3(float.Parse(rpyVals[0]), float.Parse(rpyVals[1]), float.Parse(rpyVals[2]));
                            lo_rpyF = true;

                            //lo_rpy = visualComp.Attributes ["rpy"].Value;
                            //Debug.Log (visualComp.Attributes ["xyz"].Value);
                            string[] xyzVals = visualComp.Attributes["xyz"].Value.Split(delims);
                            lo_xyz = new Vector3(float.Parse(xyzVals[0]), float.Parse(xyzVals[1]), float.Parse(xyzVals[2]));
                            lo_xyzF = true;
                            //lo_xyz = visualComp.Attributes ["xyz"].Value;
                        }
                        else if (visualComp.Name == "geometry") { //get inside mesh
                            foreach (XmlNode child in visualComp) {
                                if (child.Name == "mesh") {
                                    //Debug.Log (mesh.Attributes ["filename"].Value);
                                    lmeshfile = child.Attributes["filename"].Value;
                                    try {
                                        string[] lscale = child.Attributes["scale"].Value.Split(delims);
                                        lscale_v = new Vector3(float.Parse(lscale[0]), float.Parse(lscale[1]), float.Parse(lscale[2]));

                                    }
                                    catch { }
                                }
                            }
                        }
                        else if (visualComp.Name == "material") {
                            //WAS HERE BEFORE
                            //XmlNode materialColor = visualComp.FirstChild;

                            //Debug.Log (materialColor.Attributes ["rgba"].Value);
                            //lmaterial = materialColor.Attributes ["rgba"].Value;
                            //WAS HERE BEFORE
                            //string[] rgbaVals = materialColor.Attributes ["rgba"].Value.Split (delims);
                            //lmaterial = new Vector4 (float.Parse (rgbaVals [0]), float.Parse (rgbaVals [1]), float.Parse (rgbaVals [2]),float.Parse (rgbaVals [3]));
                            //WAS HERE BEFORE
                            lmaterial = new Vector4(1, 1, 1, 1);
                            lmaterialF = true;
                        }
                    }
                }
            }

            if (lname != null) // && lo_rpyF == true && lo_xyzF == true && lmeshfile != null) //valid link, instantiate and add to dictionary
            {
                Link temp;
                if (lmaterialF == true) { //has a material vector as well
                    temp = new Link(lname, lo_rpy, lo_xyz, lmeshfile, lmaterial, lscale_v);
                }
                else {
                    temp = new Link(lname, lo_rpy, lo_xyz, lmeshfile, lscale_v);
                }
                linkDict.Add(lname, temp);
            }
        }

        //Debug.Log (linkDict.Count);
        //Debug.Log (nodeList.Item(0).InnerXml);

        return linkDict;
    }

    public class Link {
        string name;
        Vector3 o_rpy;
        Vector3 o_xyz;
        string meshfile;
        Vector4 material;
        Vector3 scale;

        public Link(string name, Vector3 o_rpy, Vector3 o_xyz, string meshfile, Vector4 material, Vector3 scale) {
            this.name = name;
            this.o_rpy = o_rpy;
            this.o_xyz = o_xyz;
            this.meshfile = meshfile;
            this.material = material;
            this.scale = scale;
        }

        public string toString() {
            string retString = "";
            retString += "Link Name:" + this.name + "\n";
            retString += "RPY:" + this.o_rpy.x + " " + this.o_rpy.y + " " + this.o_rpy.z + "\n";
            retString += "XYZ:" + this.o_xyz.x + " " + this.o_xyz.y + " " + this.o_xyz.z + "\n";
            retString += "mesh file:" + this.meshfile + "\n";
            retString += "Material:" + +this.material.x + " " + this.material.y + " " + this.material.z + " " + this.material.w;
            retString += "Scale:" + +this.scale.x + " " + this.scale.y + " " + this.scale.z;
            return retString;
        }

        public Link(string name, Vector3 o_rpy, Vector3 o_xyz, string meshfile, Vector3 scale) {
            this.name = name;
            this.o_rpy = o_rpy;
            this.o_xyz = o_xyz;
            this.meshfile = meshfile;
            this.material = new Vector4(1, 1, 1, 1);
            this.scale = scale;
        }

        public string getName() {
            return this.name;
        }

        public Vector3 getRpy() {
            return this.o_rpy;
        }

        public Vector3 getXyz() {
            return this.o_xyz;
        }

        public string getMeshFile() {
            return this.meshfile;
        }

        public Vector4 getMaterial() {
            return this.material;
        }
        
        public Vector3 getScale() {
            return this.scale;
        }

    }

    public class Joint {
        string name;
        string type;
        Vector3 o_rpy;
        Vector3 o_xyz;
        Link parentLink;
        Link childLink;

        public Joint(string name, string type, Vector3 o_rpy, Vector3 o_xyz, Link parentLink, Link childLink) {
            this.name = name;
            this.type = type;
            this.o_rpy = o_rpy;
            this.o_xyz = o_xyz;
            this.parentLink = parentLink;
            this.childLink = childLink;
        }

        public string getName() {
            return this.name;
        }

        public string getType() {
            return this.type;
        }

        public Vector3 getRpy() {
            return this.o_rpy;
        }

        public Vector3 getXyz() {
            return this.o_xyz;
        }

        public Link getParentLink() {
            return this.parentLink;
        }

        public Link getChildLink() {
            return this.childLink;
        }

        public string toString() {
            string retString = "";
            retString += "Joint Name:" + this.name + "\n";
            retString += "type:" + this.type + "\n";
            retString += "RPY:" + this.o_rpy.x + " " + this.o_rpy.y + " " + this.o_rpy.z + "\n";
            retString += "XYZ:" + this.o_xyz.x + " " + this.o_xyz.y + " " + this.o_xyz.z + "\n";
            retString += "parent:" + this.getParentLink().getName() + "\n";
            retString += "child:" + this.getChildLink().getName() + "\n";
            return retString;
        }
    }
}
