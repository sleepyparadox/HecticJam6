using UnityEngine;
using System.Collections;

public class BulletParticle : MonoBehaviour
{
	public enum MoveModifier {Straight, Curve, Sine, Loop, ZigZag}
	public MoveModifier modifier = MoveModifier.Straight;
	
	public float speed;
	
	public Vector2 _angularPos;
	public Vector2 angularVel;
	
	//Vector2 _angularPos;
	// Use this for initialization
	void Start ()
	{
		transform.position = _angularPos.ToWorld(10);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//transform.Translate(transform.up * speed * Time.deltaTime);
		_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(10);
		
		//_angularPos += new Vector2(0, 1) * Time.deltaTime;
		////while(_angularPos.x > Mathf.PI * 2f)
		////	_angularPos.x -= Mathf.PI * 2f;
		////while(_angularPos.x < Mathf.PI * 2f)
		////	_angularPos.x += Mathf.PI * 2f;
		//
		//var sphereRadius = 50f;
		transform.rotation = Camera.main.transform.rotation;
		//
		//float x = Mathf.Cos(_angularPos.x) * sphereRadius;
		//float y = Mathf.Sin(_angularPos.y) * sphereRadius;
		//float z = Mathf.Sin(_angularPos.x) * sphereRadius;
		//
		//transform.position = new Vector3(x,y,z);
	}
}
