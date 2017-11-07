using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Connect : MonoBehaviour
{
	NetworkManager myManager;

	// Use this for initialization
	void Start ()
	{
		myManager = GetComponent<NetworkManager>();
		myManager.StartHost();
		//myManager.StartClient();
	}
}
