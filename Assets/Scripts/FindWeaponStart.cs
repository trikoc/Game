using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
public class FindWeaponStart : NetworkBehaviour
{

    public static string GetPath(GameObject obj)
    {
        var rootName = obj.transform.root.gameObject.name;
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {

            obj = obj.transform.parent.gameObject;
            if (!obj.name.Equals(rootName))
            {
                path = "/" + obj.name + path;
            }
        }
        return path;
    }

    public static Transform WeaponPositionsInitialization(string tag, Transform currentTransform)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
        {
            if (go.transform.root == currentTransform.root)
            {
                return go.transform;
            }
        }
        return null;
    }
}
