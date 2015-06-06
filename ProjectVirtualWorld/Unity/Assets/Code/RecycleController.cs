using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// *** The Recycling Controller, responsible for handling the recycling of Game Objects. ***
public class RecycleController : MonoBehaviour
{
	private static RecycleController rc { get; set; }				// Reference to itself for use within methods calling on coroutines.
	
    //Not static so this collection will be lost when the scene reloads
    private Dictionary<string, List<GameObject>> _collections = new Dictionary<string,List<GameObject>>();

	void Awake ()
	{
		rc = this;
	}

	// Method for spawning game objects.
	public static GameObject Spawn (GameObject srcObject, Vector3 spawnPos = default(Vector3), Quaternion spawnRot = default(Quaternion), string name = null)
	{
		// Assigns the received game object's name if no name is supplied.
		if (string.IsNullOrEmpty(name))
			name = srcObject.name;
        //Debug.Log("Spawn() " + name);

        var objCollection = rc.GetCollection(name);

        var obj = objCollection.FirstOrDefault();

		if(obj == null)
        {
            //Debug.Log("Spawned " + name);
            obj = Instantiate(srcObject) as GameObject;
            obj.name = name;
        }
        else
        {
           // Debug.Log("Retrieved " + name);
            objCollection.Remove(obj);
        }

        obj.name = name;
        obj.transform.position = spawnPos;
        obj.transform.rotation = spawnRot;
        obj.SetActive(true);

        return obj;
	}
	
	// Method for calling the coroutine responsible for recycling game objects.
	public static void Recycle (GameObject spawnObj, string name = null)
	{
        ////HACK
        ////GameObject.Destroy(spawnObj);
        //spawnObj.SetActive(false);
        //return;

		// Assigns the received game object's name if no name is supplied.
		if (string.IsNullOrEmpty(name))
			name = spawnObj.name;

        //Debug.Log("Recycle() " + name);

        var objCollection = rc.GetCollection(name);
        objCollection.Add(spawnObj);

        spawnObj.SetActive(false);
        ////Set active / inactive has a cost on mobile, so fly it far far away
        //spawnObj.transform.position = new Vector3(100000f, 100000f, 100000f);
    }

    private List<GameObject> GetCollection(string name)
    {
        List<GameObject> objCollection;
        if (_collections.ContainsKey(name))
        {
            objCollection = _collections[name];
        }
        else
        {
            objCollection = new List<GameObject>();
            _collections.Add(name, objCollection);
        }
        return objCollection;
    }
}