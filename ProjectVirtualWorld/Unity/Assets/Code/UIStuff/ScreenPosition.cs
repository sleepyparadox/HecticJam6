using UnityEngine;
using System.Collections;

public enum SnapPosition
{
	Center,
	CenterLeft,
	CenterRight,
	TopCenter,
	TopLeft,
	TopRight,
	BottomCenter,
	BottomLeft,
	BottomRight
}

public class ScreenPosition : MonoBehaviour
{
	public float posX = 0;
	public float posY = 0;
	public SnapPosition snapPosition;
	
	private float posZ = 0;
	private Vector3 pos;
	private new Camera camera;
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(ZPosition());
	}
	IEnumerator ZPosition ()
	{
		while (true)
		{
			if (MainGame.S.PlayerCamera != null)
			{
				posZ = transform.position.z + MainGame.S.PlayerCamera.Transform.position.z;
				camera = MainGame.S.PlayerCamera.Transform.gameObject.GetComponent<Camera>();
				transform.parent = camera.transform;
				break;
			}
			yield return null;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (camera)
		{
			switch (snapPosition)
			{
				case SnapPosition.Center:
					pos = new Vector3(Screen.width / 2f, Screen.height / 2f, posZ);
					break;
				case SnapPosition.CenterLeft:
					pos = new Vector3(0, Screen.height / 2f, posZ);
					break;
				case SnapPosition.CenterRight:
					pos = new Vector3(Screen.width, Screen.height / 2f, posZ);
					break;
				case SnapPosition.TopCenter:
					pos = new Vector3(Screen.width / 2f, Screen.height, posZ);
					break;
				case SnapPosition.TopLeft:
					pos = new Vector3(0, Screen.height, posZ);
					break;
				case SnapPosition.TopRight:
					pos = new Vector3(Screen.width, Screen.height, posZ);
					break;
				case SnapPosition.BottomCenter:
					pos = new Vector3(Screen.width / 2f, 0, posZ);
					break;
				case SnapPosition.BottomLeft:
					pos = new Vector3(0, 0, posZ);
					break;
				case SnapPosition.BottomRight:
					pos = new Vector3(Screen.width, 0, posZ);
					break;
				default:
					pos = new Vector3(Screen.width / 2f, Screen.height / 2f, posZ);
					break;
			}
			
			transform.position = new Vector3(posX + camera.ScreenToWorldPoint(pos).x, 
											posY + camera.ScreenToWorldPoint(pos).y,
											camera.ScreenToWorldPoint(pos).z);
			
			transform.rotation = camera.transform.rotation;
		}
	}
}
