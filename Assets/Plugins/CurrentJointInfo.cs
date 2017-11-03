using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrentJointInfo
{
    public string name;
    public List<float> pos;
    public List<float> rot;

    public CurrentJointInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<CurrentJointInfo>(jsonString);
    }
}

