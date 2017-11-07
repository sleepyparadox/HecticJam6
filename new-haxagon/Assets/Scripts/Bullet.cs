using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour
{
	public float Angle;
	public Vector3 OriginVector;
	public float RotationModifier;
	public float ModifierSpeed = 1;
	public float Speed = 1;
	public float Clamp;
	public ModifierType ModifierType;

	public Vector3 RandomVector;
	public float RandomFloat;
	
	private float _count;
	private Quaternion _rotation = Quaternion.identity;
	private float _timer;
	private IEnumerator _coro;

	void Start ()
	{
		CalculateWorldPosition();
	}

	void Update ()
	{
		_count += Speed * Time.deltaTime;
		_timer += ModifierSpeed * Time.deltaTime;

		if (ModifierType == ModifierType.Sine)
		{
			//RotationModifier = Mathf.Sin(_timer * 8) * 16;
			RandomFloat = Mathf.Sin(_timer * 8) * 16;
		}
		else if (ModifierType == ModifierType.Spiral)
		{
			//RotationModifier = _timer;
			RandomFloat = _timer;
		}
		else if (ModifierType == ModifierType.ZigZag)
		{
			if (_coro == null)
			{
				_coro = ZigZag();
				StartCoroutine(_coro);
			}
			//RotationModifier = _timer;
			RandomFloat = _timer;
		}

		CalculateWorldPosition();
	}

	void CalculateWorldPosition ()
	{
		var forwardVector = Quaternion.AngleAxis(Angle, Vector3.forward) * Vector3.up;
		var horizontalVector = Quaternion.AngleAxis(Angle, Vector3.forward) * Vector3.right;

		if (OriginVector != Vector3.zero)
			_rotation = Quaternion.Inverse(Quaternion.LookRotation(OriginVector));
		else
			_rotation = Quaternion.identity;
		_rotation *= Quaternion.Euler(0, 0, RotationModifier);							// Polar rotation pattern modifier.
		_rotation *= Quaternion.AngleAxis(_count, forwardVector);						// Speed & forward axis angle.
		_rotation *= Quaternion.AngleAxis(RandomFloat, horizontalVector);				// Movement pattern modifier.

		// Set
		transform.position = Vector3.MoveTowards(transform.position, _rotation * Vector3.forward * 2.5f, Clamp);
	}

	IEnumerator ZigZag ()
	{
		//RotationModifier = ModifierSpeed;
		int oscillator = -1;

		while (true)
		{
			if (ModifierType == ModifierType.ZigZag)
			{
				ModifierSpeed = ModifierSpeed * oscillator;
				oscillator = -oscillator;
			}
			yield return new WaitForSeconds(.125f);
		}
	}
}

public enum ModifierType
{
	Straight,
	Spiral,
	ZigZag,
	Sine
}