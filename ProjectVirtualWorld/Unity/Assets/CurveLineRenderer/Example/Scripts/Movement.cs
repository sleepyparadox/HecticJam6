using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour 
{
	public MovementCallback movementCallback;
	private int state = 0;
	private List<Vector3> path;
	private int targetPos;	
	public float movementSpeed = 8f;
	private Vector3 startPosition;

	void Start ()
	{
	}

	void Update () 
	{
		if (state == 1)
		{
			Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			Vector3 target = path[targetPos];
			if (Mathf.Abs (position.x - target.x) < 0.01f && 
			    Mathf.Abs (position.z - target.z) < 0.01f)
			{
				transform.position = new Vector3 (target.x, target.y+0.5f, target.z);
				startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				targetPos++;
				if (targetPos == path.Count)
				{
					targetPos = -1;
					state = 0;

					StopMove ();
					return;
				}

				target = path[targetPos];
			}
			Vector3 targetPosition = new Vector3 (target.x, target.y+0.5f, target.z);
			transform.position = Vector3.Lerp(position, targetPosition, 
			                                  Time.deltaTime * movementSpeed / (targetPosition - startPosition).magnitude);
		}
	}

	public void MoveWithPath (List<Vector3> p, MovementCallback callback)
	{
		movementCallback = callback;
		path = p;
		path.Reverse ();

		StartMove ();
	}

	private void StartMove ()
	{
		if (movementCallback != null)
			movementCallback.OnStart ();

		startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		state = 1;
		targetPos = 0;
	}

	private void StopMove ()
	{
		if (movementCallback != null)
			movementCallback.OnStop ();

		state = 0;
	}

	public interface MovementCallback
	{
		void OnStart ();
		void OnStop ();
	}
}
