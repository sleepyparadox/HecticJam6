using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	// Orbit variables.
	public float orbitSpeed;
	public float distance;
	[HideInInspector] public Transform target;
	[HideInInspector] public Transform player;
	
	// Firing variables.
	
	public float delay;
	public PatternObjects[] phases;
	public Vector2 _angularPos;
	public Vector2 angularVel;
	public Vector2 targetPos;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindWithTag("Player").transform;
		StartCoroutine(Orbit());
		StartCoroutine(Shoot());
	}
	
	void Update ()
	{
		//_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(MainGame.Radius);
		
		if (MainGame.S.PlayerCamera != null)
			transform.rotation = MainGame.S.PlayerCamera.LookRotation;
	}
	
	// Update is called once per frame
	IEnumerator Shoot ()
	{
		while (player != null)
		{
			for (int i = 0; i < phases.Length; i++)
			{
				if ((player.position - transform.position).magnitude <= phases[i].range)
				{
					for (int x = 0; x < phases[i].patternObjects.Length; x++)
					{
						if ((player.position - transform.position).magnitude > phases[i].range)
							break;
						
						GameObject pattern = RecycleController.Spawn(phases[i].patternObj, transform.position, Quaternion.identity);
						
						phases[i].patternObjects[x].pattern = pattern.GetComponent<Pattern>();
						phases[i].patternObjects[x].InitialiseValues(pattern, _angularPos);
						
						yield return new WaitForSeconds(delay);
					}
					break;
				}
			}
			yield return null;
		}
	}
	
	IEnumerator Orbit ()
	{
		float count = 0;
		
		while (true)
		{
			if (target && target.gameObject.activeSelf)
			{
				if (count < 1)
					count += Time.deltaTime * orbitSpeed;
				else
					count = 0;
				
				float angle = 360 * count;
				
				_angularPos.x = targetPos.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
				_angularPos.y = targetPos.y + distance * Mathf.Sin(angle * Mathf.Deg2Rad);
			}
			
			yield return null;
		}
	}
}

[System.Serializable]
public class PatternObjects
{
	public float range;
	[HideInInspector] public GameObject patternObj = Assets.Prefabs.PatternPrefab.Prefab;
	public BulletPattern[] patternObjects;
}

[System.Serializable]
public class BulletPattern
{
	[HideInInspector] public Pattern pattern;
	
	public BasicShape shape = BasicShape.Single;
	public MoveModifier modifier = MoveModifier.Straight;
	public float modifierIntensity;
	
	public float speed;
	public float lifespan;
	public int count;
	public int waves;
	public float delay;
	public float length;
	public float arc;
	
	public void InitialiseValues (GameObject patternObj, Vector2 pos)
	{
		pattern = patternObj.GetComponent<Pattern>();
		pattern._angularPos = pos;
		pattern.pattern = shape;
		pattern.modifier = modifier;
		pattern.modifierIntensity = modifierIntensity;
		pattern.speed = speed;
		pattern.lifespan = lifespan;
		pattern.count = count;
		pattern.waves = waves;
		pattern.delay = delay;
		pattern.length = length;
		pattern.arc = arc;
	}
}
