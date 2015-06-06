using UnityEngine;
using System.Collections;

public class BulletParticle : MonoBehaviour
{
	public enum MoveModifier {Straight, Curve, Sine, Loop, ZigZag}
	public MoveModifier modifier = MoveModifier.Straight;
	
	public float speed;
	public float lifespan = 99;
	
	public Vector2 _angularPos;
	public Vector2 angularVel;
	public float angle;
	
	public Vector3 originalScale;
	
	//Vector2 _angularPos;
	// Use this for initialization
	void Awake ()
	{
		originalScale = transform.localScale;
	}
	
	void OnEnable ()
	{
		transform.position = _angularPos.ToWorld(MainGame.Radius);
		transform.localScale = originalScale;
		StartCoroutine(Modify());
	}
	
	public void Line (float value)
	{
		StartCoroutine(ToLine(value));
	}
	IEnumerator ToLine (float value)
	{
		float count = 0;
		float x = _angularPos.x;
		
		while (count < 1f)
		{
			_angularPos.x = Mathf.Lerp(x, x + value, count);
			count += Time.deltaTime * 4;
			yield return null;
		}
	}
	
	IEnumerator Modify ()
	{
		float count = 0;
		while (true)
		{
			if (modifier == MoveModifier.Straight)
			{
				var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				angularVel = direction * speed;
			}
			else if (modifier == MoveModifier.Curve)
			{
				count += Time.deltaTime * 2;
				var sineAngle = angle + count;
				var direction = new Vector2(Mathf.Sin(sineAngle), Mathf.Cos(sineAngle));
				angularVel = direction * speed;
			}
			yield return null;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(MainGame.Radius);
		
		if (MainGame.S.PlayerCamera != null)
			transform.rotation = MainGame.S.PlayerCamera.LookRotation;
		
		lifespan -= Time.deltaTime;
		
		if (lifespan < 1f)
			transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, lifespan);
		
		if (lifespan <= 0)
			RecycleController.Recycle(gameObject);
	}
}
