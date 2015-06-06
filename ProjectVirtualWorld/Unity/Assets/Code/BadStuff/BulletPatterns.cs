using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPatterns : MonoBehaviour
{
	public Vector2 _angularPos;
	public Vector2 angularVel;
	
	public enum BasicPattern {Single, Line, Arc, Circle}
	public BasicPattern pattern = BasicPattern.Single;
	
	public GameObject bullet;
	public float speed;
	public float lifespan;
	public int count;
	public int waves;
	public float delay;
	public float length;
	public float arc;
	
	// Use this for initialization
	void Start ()
	{
		transform.position = _angularPos.ToWorld(MainGame.Radius);
		StartCoroutine(Spawn());
	}
	
	void Update ()
	{
		_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(MainGame.Radius);
	}
	
	// Update is called once per frame
	IEnumerator Spawn ()
	{
		if (pattern == BasicPattern.Single)
		{
			GameObject bulletObj = RecycleController.Spawn(bullet,transform.position, transform.rotation);
			bulletObj.GetComponent<BulletParticle>().speed = speed;
			bulletObj.GetComponent<BulletParticle>().lifespan = lifespan;
			bulletObj.GetComponent<BulletParticle>()._angularPos = _angularPos;
		}
		else if (pattern == BasicPattern.Line)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					GameObject bulletObj = RecycleController.Spawn(bullet, transform.position, transform.rotation);
					Debug.Log(CalculateLinePos(x, length, count));
					var angle = 0;
					var bulletScript = bulletObj.GetComponent<BulletParticle>();
					
					bulletScript.angle = angle;
					bulletScript.speed = speed;
					bulletScript.lifespan = lifespan;
					bulletScript._angularPos = _angularPos;
					bulletScript.Line(CalculateLinePos(x, length, count));
				}
				yield return new WaitForSeconds(delay);
			}
		}
		else if (pattern == BasicPattern.Arc)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					var angle = ((float)x / (count - 1)) * (Mathf.PI * 2) / (360f / arc);
					var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var bulletObj = RecycleController.Spawn(bullet, transform.position);
					var bulletScript = bulletObj.GetComponent<BulletParticle>();
					
					bulletScript.angle = angle;
					bulletScript._angularPos = _angularPos;
					bulletScript.angularVel = direction * speed;
					bulletScript.speed = speed;
					bulletScript.lifespan = lifespan;
				}
				yield return new WaitForSeconds(delay);
			}
		}
		else if (pattern == BasicPattern.Circle)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					var angle = ((float)x / count) * (Mathf.PI * 2);
					var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var bulletObj = RecycleController.Spawn(bullet, transform.position);
					var bulletScript = bulletObj.GetComponent<BulletParticle>();
					
					bulletScript.angle = angle;
					bulletScript._angularPos = _angularPos;
					bulletScript.angularVel = direction * speed;
					bulletScript.speed = speed;
					bulletScript.lifespan = lifespan;
				}
				yield return new WaitForSeconds(delay);
			}
		}
		RecycleController.Recycle(gameObject);
	}
	
	public Vector3 CalculateCirclePos (int index, float distance, int total)
	{
		float angle = (360f / total) * index;
		Vector3 position = Vector3.zero;
		
		position.x = Vector3.zero.x + (distance * total) * Mathf.Cos(angle * Mathf.Deg2Rad);
		position.z = Vector3.zero.z + (distance * total) * Mathf.Sin(angle * Mathf.Deg2Rad);
		
		return position;
	}
	
	public float CalculateLinePos (int index, float space, int total)
	{
		float x = _angularPos.x;
		x += (space / (total - 1f) * index) - space / 2f;
		
		return x;
	}
	
	public Vector3 CalculateCircleVector (int index, int total)
	{
		var vector = Quaternion.AngleAxis(360f / total * index, Vector3.forward) * Vector3.up;
		
		return vector;
	}
}
