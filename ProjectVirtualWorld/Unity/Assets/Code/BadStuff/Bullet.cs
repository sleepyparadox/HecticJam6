using UnityEngine;
using System.Collections;

public enum MoveModifier {Straight, Curve, Sine, Loop, ZigZag}

public class Bullet
{
	public MoveModifier modifier = MoveModifier.Straight;
	public float modifierIntensity = 1f;
	
	public float speed;
	public float lifespan = 99;
    public float currentscale = 1f;
	
	public Vector2 _angularPos;
	public Vector2 angularVel;
	public float angle;

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
				count += Time.deltaTime * modifierIntensity;
				var curveAngle = angle + count;
				var direction = new Vector2(Mathf.Sin(curveAngle), Mathf.Cos(curveAngle));
				angularVel = direction * speed;
			}
			else if (modifier == MoveModifier.Sine)
			{
				count += Time.deltaTime * modifierIntensity;
				var sineAngle = angle + Mathf.Sin(count);
				var direction = new Vector2(Mathf.Sin(sineAngle), Mathf.Cos(sineAngle));
				angularVel = direction * speed;
			}
			yield return null;
		}
	}
	
	// Update is called once per frame
    public void Update()
	{
		_angularPos += angularVel * Time.deltaTime;
		
		lifespan -= Time.deltaTime;
	}

    public ParticleSystem.Particle GetParticle()
    {
        return new ParticleSystem.Particle()
        {
            color = Color.red,
            position = _angularPos.ToWorld(MainGame.Radius),
            size = currentscale,
            lifetime = 1000,
            startLifetime = 500,
        };
    }
}
