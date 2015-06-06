using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// *** The Recycling Controller, responsible for handling the recycling of Game Objects. ***
public class RecycleController : MonoBehaviour
{
	[SerializeField]
	private List<RecycleObject> displayRecycleList;					// Test list to view & cache recycle objects in the Inspector.
	public static List<RecycleObject> recycleList { get; set; }		// Global list of recycle objects.
	private static RecycleController rc { get; set; }				// Reference to itself for use within methods calling on coroutines.
	
	void Awake ()
	{
		recycleList = displayRecycleList;			// Assigns pre-cached objects to the recycle list
		StartCoroutine(UpdateList());
		rc = this;
	}
	
	// Coroutine for updating the recycle list in the inspector.
	IEnumerator UpdateList ()
	{
		while (true)
		{
			yield return new WaitForSeconds(1.0f);		// Delays the check in order to lower the overhead.
			displayRecycleList = recycleList;			// Updates the test variable to display the same items as the recycle array.
		}
	}
	
	// Method for spawning game objects.
	public static GameObject Spawn (GameObject spawnObj, Vector3 spawnPos = default(Vector3), Quaternion spawnRot = default(Quaternion), string name = "")
	{
		RecycleObject recycleObject = null;
		
		// Assigns the received game object's name if no name is supplied.
		if (string.IsNullOrEmpty(name))
			name = spawnObj.name;
		
		// Searches for a recycleObject with the same name as the recycleName variable
		for (int i = 0; i < recycleList.Count; i++)
		{
			if (recycleList[i].recycleName == name)
			{
				recycleObject = recycleList[i];
				break;
			}
		}
		
		// Creates a recycleObject for storing recycled objects if one doesn't already exist
		if (recycleObject == null)
		{
			recycleObject = new RecycleObject(name);
			recycleList.Add(recycleObject);
		}
		
		// Calls the Check method to see if there are any objects in the list.
		if (!recycleObject.Check())
		{
			// Creates a new object ONLY if the array is currently empty.
			GameObject obj = Instantiate(spawnObj, spawnPos, spawnRot) as GameObject;
			obj.name = name;
			return obj;
		}
		else
		{
			// Takes a recycled object if the list is not empty.
			spawnObj = recycleObject.Pull();
			spawnObj.transform.position = spawnPos;
			spawnObj.transform.rotation = spawnRot;
			spawnObj.SetActive(true);
			
			return spawnObj;
		}
	}
	
	// Method for calling the coroutine responsible for recycling game objects.
	public static void Recycle (GameObject spawnObj, float time = 0, string name = "")
	{
		rc.StartCoroutine(rc.PerformRecycle(spawnObj, time, name));
	}
	
	// Coroutine for recycling game objects.
	IEnumerator PerformRecycle (GameObject spawnObj, float time = 0, string name = "")
	{
		RecycleObject recycleObject = null;
		
		// Assigns the received game object's name if no name is supplied.
		if (string.IsNullOrEmpty(name))
			name = spawnObj.name;
		
		// Searches for a recycleObject with the same name as the recycleName variable
		for (int i = 0; i < recycleList.Count; i++)
		{
			if (recycleList[i].recycleName == name)
			{
				recycleObject = recycleList[i];
				break;
			}
		}
		
		// Creates a recycleObject for storing recycled objects if one doesn't already exist
		if (recycleObject == null)
		{
			recycleObject = new RecycleObject(name);
			recycleList.Add(recycleObject);
		}
		
		// Recycles the object if the object is not null
		if (spawnObj != null)
		{
			yield return new WaitForSeconds(time);
			spawnObj.SetActive(false);
			recycleObject.Push(spawnObj);
		}
	}
}

// *** The Recycle Object, responsible for holding the recycled objects of a given type ***
[System.Serializable]
public class RecycleObject
{
	public List<GameObject> objectList = new List<GameObject>();	// The recycling list of Game Objects.
	public string recycleName;		// Name of the Recycling Object. Used to check if the Object belongs here.
	
	// Class Constructor
	public RecycleObject (string recycleName)
	{
		this.recycleName = recycleName;
	}

	// Method for inserting an object into the list.
	public void Push (GameObject obj)
	{
		objectList.Add(obj);
	}

	// Method for removing an object from the list.
	public GameObject Pull ()
	{
		GameObject obj = objectList[objectList.Count - 1];
		objectList.RemoveAt(objectList.Count - 1);
		
		return obj;
	}

	// Method for checking if the list is empty or not.
	public bool Check ()
	{
		if (objectList.Count > 0)
			return true;
		else
			return false;
	}
}