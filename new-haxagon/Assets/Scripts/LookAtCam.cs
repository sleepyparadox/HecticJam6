using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LookAtCam : NetworkBehaviour
{
	void Update ()
	{
		if (Camera.main)
			transform.rotation = Camera.main.transform.rotation;
	}
}
