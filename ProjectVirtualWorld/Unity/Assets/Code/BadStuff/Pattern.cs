using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BasicShape {Single, Line, Arc, Circle}

public class Pattern : MonoBehaviour
{
	public Vector2 _angularPos;
	public Vector2 angularVel;
	
	[HideInInspector] public BasicShape pattern = BasicShape.Single;
	[HideInInspector] public MoveModifier modifier = MoveModifier.Straight;
	[HideInInspector] public float modifierIntensity = 1f;
	
	[HideInInspector] public float speed;
	[HideInInspector] public float lifespan;
	[HideInInspector] public int count;
	[HideInInspector] public int waves;
	[HideInInspector] public float delay;
	[HideInInspector] public float length;
	[HideInInspector] public float arc;
	
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
		if (pattern == BasicShape.Single)
		{
			GameObject bulletObj = RecycleController.Spawn(Assets.Prefabs.BulletPrefab.Prefab, transform.position, transform.rotation);
			bulletObj.GetComponent<Bullet>().speed = speed;
			bulletObj.GetComponent<Bullet>().lifespan = lifespan;
			bulletObj.GetComponent<Bullet>()._angularPos = _angularPos;
		}
		else if (pattern == BasicShape.Line)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					GameObject bulletObj = RecycleController.Spawn(Assets.Prefabs.BulletPrefab.Prefab, transform.position, transform.rotation);
					Debug.Log(CalculateLinePos(x, length, count));
					var angle = 0;
					var bulletScript = bulletObj.GetComponent<Bullet>();
					
					bulletScript.angle = angle;
					bulletScript.speed = speed;
					bulletScript.lifespan = lifespan;
					bulletScript._angularPos = _angularPos;
					bulletScript.modifier = modifier;
					bulletScript.modifierIntensity = modifierIntensity;
					bulletScript.Line(CalculateLinePos(x, length, count));
				}
				yield return new WaitForSeconds(delay);
			}
		}
		else if (pattern == BasicShape.Arc)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					var angle = ((float)x / (count - 1)) * (Mathf.PI * 2) / (360f / arc) - ((Mathf.PI * 2) / (360f / arc)) / 2f;
					var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var bulletObj = RecycleController.Spawn(Assets.Prefabs.BulletPrefab.Prefab, transform.position);
					var bulletScript = bulletObj.GetComponent<Bullet>();
					
					bulletScript.angle = angle;
					bulletScript._angularPos = _angularPos;
					bulletScript.angularVel = direction * speed;
					bulletScript.speed = speed;
					bulletScript.lifespan = lifespan;
					bulletScript.modifier = modifier;
					bulletScript.modifierIntensity = modifierIntensity;
				}
				yield return new WaitForSeconds(delay);
			}
		}
		else if (pattern == BasicShape.Circle)
		{
			for (int i = 0; i < waves; i++)
			{
				for (int x = 0; x < count; x++)
				{
					var angle = ((float)x / count) * (Mathf.PI * 2);
					var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var bulletObj = RecycleController.Spawn(Assets.Prefabs.BulletPrefab.Prefab, transform.position);
					var bulletScript = bulletObj.GetComponent<Bullet>();
					
					bulletScript.angle = angle;
					bulletScript._angularPos = _angularPos;
					bulletScript.angularVel = direction * speed;
					bulletScript.speed = speed;
					bulletScript.lifespan = lifespan;
					bulletScript.modifier = modifier;
					bulletScript.modifierIntensity = modifierIntensity;
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
