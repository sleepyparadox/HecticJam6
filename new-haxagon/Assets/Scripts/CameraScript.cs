using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CameraScript : NetworkBehaviour
{

	// Update is called once per frame
	void Update ()
	{
		if (!isLocalPlayer)
			return;

		transform.Rotate(-Input.GetAxis("Vertical") * 2, Input.GetAxis("Horizontal") * 2, 0);

	}
}
