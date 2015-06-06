using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	// Orbit variables.
	public float orbitSpeed;
	public float distance;
	public Transform target;
	
	// Firing variables.
	public Transform[] patternObjects;
	public float delay;
	
	public Vector2 _angularPos;
	public Vector2 angularVel;
	
	// Use this for initialization
	void Start ()
	{
		StartCoroutine(Orbit());
		StartCoroutine(Shoot());
	}
	
	void Update ()
	{
		_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(MainGame.Radius);
	}
	
	// Update is called once per frame
	IEnumerator Shoot ()
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			
			if (patternObjects.Length > 0)
			{
				GameObject pattern = RecycleController.Spawn(patternObjects[0].gameObject, transform.position, Quaternion.identity);
				pattern.GetComponent<BulletPatterns>()._angularPos = _angularPos;
			}
		}
	}
	
	IEnumerator Orbit ()
	{
		float count = 0;
		
		while (true)
		{
			//if (count < 1)
			//	count += Time.deltaTime * orbitSpeed;
			//else
			//	count = 0;
			//
			//float angle = 360 * count;
			//Vector3 position = Vector3.zero;
			//
			//position.x = target.position.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
			//position.y = target.position.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad);
			//position.z = transform.position.z;
			
			//transform.position = _angularPos.ToWorld(MainGame.Radius);
			
			yield return null;
		}
	}
}
