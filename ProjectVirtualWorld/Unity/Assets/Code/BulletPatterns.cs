using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPatterns : MonoBehaviour
{
	public Vector2 _angularPos;
	public Vector2 angularVel;
	
	public enum BasicPattern {Single, Line, Arc, Circle, ZigZag}
	public BasicPattern pattern = BasicPattern.Single;
	
	public GameObject bullet;
	public float speed;
	public int count;
	public int waves;
	public float delay;
	public float spacing;
	public float arc;
	
	// Use this for initialization
	void Start ()
	{
		transform.position = _angularPos.ToWorld(10);
		StartCoroutine(Spawn());
	}
	
	void Update ()
	{
		_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(10);
	}
	
	// Update is called once per frame
	IEnumerator Spawn ()
	{
		if (pattern == BasicPattern.Single)
		{
			GameObject bulletObj = Instantiate(bullet, transform.position, transform.rotation) as GameObject;
			bulletObj.GetComponent<BulletParticle>().speed = speed;
		}
		else if (pattern == BasicPattern.Line)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					GameObject bulletObj = Instantiate(bullet, CalculateLinePos(x, spacing, count), transform.rotation) as GameObject;
					bulletObj.GetComponent<BulletParticle>().speed = speed;
				}
				yield return new WaitForSeconds(delay);
			}
		}
		//else if (pattern == BasicPattern.Arc)
		//{
		//	for (int i = 0; i < waves; i++)
		//	{
		//		for (int x = 0; x < count; x++)
		//		{
		//			GameObject bulletObj = Instantiate(bullet, transform.position, Quaternion.AngleAxis(360f / count * x, Vector3.forward)) as GameObject;
		//			bulletObj.GetComponent<BulletParticle>().speed = speed;
		//		}
		//		yield return new WaitForSeconds(delay);
		//	}
		//}
		else if (pattern == BasicPattern.Circle)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					var angle = ((float)x / count) * (Mathf.PI * 2);
					var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var bulletObj = Instantiate(bullet) as GameObject;
					bulletObj.GetComponent<BulletParticle>().angularVel = direction * speed;
				}
				yield return new WaitForSeconds(delay);
			}
		}
	}
	
	public Vector3 CalculateCirclePos (int index, float distance, int total)
	{
		float angle = (360f / total) * index;
		Vector3 position = Vector3.zero;
		
		position.x = Vector3.zero.x + (distance * total) * Mathf.Cos(angle * Mathf.Deg2Rad);
		position.z = Vector3.zero.z + (distance * total) * Mathf.Sin(angle * Mathf.Deg2Rad);
		
		return position;
	}
	
	public Vector3 CalculateLinePos (int index, float space, int total)
	{
		Vector3 position = Vector3.zero;
		position.x = Vector3.zero.x - (space * total / 2f) + (space * index);
		
		return position;
	}
	
	public Vector3 CalculateCircleVector (int index, int total)
	{
		var vector = Quaternion.AngleAxis(360f / total * index, Vector3.forward) * Vector3.up;
		
		return vector;
	}
}
