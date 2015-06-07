using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float speedX = 1f;
	public float speedY = 1f;
	public float speedRotate = 1f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		Vector3 pos = transform.position;

		float dx = Input.GetAxis ("Horizontal");
		float dy = Input.GetAxis ("Vertical");

		float cosa = Mathf.Cos (Mathf.PI * transform.eulerAngles.y / 180f);
		float sina = Mathf.Sin (Mathf.PI * transform.eulerAngles.y / 180f);

		pos.x += (cosa * dx + dy * sina) * speedX;
		pos.z += (-sina * dx + dy * cosa) * speedY;

		transform.position = pos;

		if (Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q))
		{
			transform.RotateAround(transform.position, Vector3.up, speedRotate);
		}
		else if (!Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Q))
		{
			transform.RotateAround(transform.position, Vector3.up, -speedRotate);
		}
	}

	void OnGUI () 
	{
		int w = Screen.width;
		int h = Screen.height;

		GUI.Box(new Rect (20, 20, 250, 100), "");
		GUI.Label(new Rect (30, 30, 230, 80), "W,A,S,D - move camera\r\nQ, E - rotate camera\r\n\r\nUse left click of mouse on highligthed tile for move capsule");
    }
}
