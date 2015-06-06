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
	
	public Vector3 originalScale;
	
	//Vector2 _angularPos;
	// Use this for initialization
	void Awake ()
	{
		transform.position = _angularPos.ToWorld(MainGame.Radius);
		originalScale = transform.localScale;
	}
	
	void OnEnable ()
	{
		transform.localScale = originalScale;
		_angularPos = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//transform.Translate(transform.up * speed * Time.deltaTime);
		_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(MainGame.Radius);
		
		//_angularPos += new Vector2(0, 1) * Time.deltaTime;
		////while(_angularPos.x > Mathf.PI * 2f)
		////	_angularPos.x -= Mathf.PI * 2f;
		////while(_angularPos.x < Mathf.PI * 2f)
		////	_angularPos.x += Mathf.PI * 2f;
		//
		//var sphereRadius = 50f;
		if (MainGame.S.PlayerCamera != null)
			transform.rotation = MainGame.S.PlayerCamera.Transform.rotation;
		//
		//float x = Mathf.Cos(_angularPos.x) * sphereRadius;
		//float y = Mathf.Sin(_angularPos.y) * sphereRadius;
		//float z = Mathf.Sin(_angularPos.x) * sphereRadius;
		//
		//transform.position = new Vector3(x,y,z);
		lifespan -= Time.deltaTime;
		
		if (lifespan < 1f)
			transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, lifespan);
		
		if (lifespan <= 0)
			RecycleController.Recycle(gameObject);
	}
}
