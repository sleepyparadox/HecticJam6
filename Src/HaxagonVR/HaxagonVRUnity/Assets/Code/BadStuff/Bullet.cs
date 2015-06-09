using UnityEngine;
using System;
using System.Collections;

public enum MoveModifier {Straight, Curve, Sine, Loop, ZigZag}

public class Bullet
{
	public MoveModifier Modifier = MoveModifier.Straight;
    public float ModifierTimeScale = 1f;
    public float ModifierIntensity = 1f;
	
	public Color PartColor = Color.red;
	public float Speed;
	public float Lifespan = 99;
    public float Currentscale = 1f;
	
	public Vector2 AngularPos;
	public Vector2 AngularVel;
	public float Angle;

    private float Count = 0;
	
	// Update is called once per frame
    public void Update()
	{
		AngularPos += AngularVel * Time.deltaTime;
		
		Lifespan -= Time.deltaTime;

        if (Modifier == MoveModifier.Straight)
        {
            var direction = new Vector2(Mathf.Sin(Angle), Mathf.Cos(Angle));
            AngularVel = direction * Speed;
        }
        else if (Modifier == MoveModifier.Curve)
        {
            Count += Time.deltaTime * ModifierTimeScale;
            var curveAngle = Angle + Count * ModifierIntensity;
            var direction = new Vector2(Mathf.Sin(curveAngle), Mathf.Cos(curveAngle));
            AngularVel = direction * Speed;
        }
        else if (Modifier == MoveModifier.Sine)
        {
            Count += Time.deltaTime * ModifierTimeScale;
            var sineAngle = Angle + Mathf.Sin(Count) * ModifierIntensity;
            var direction = new Vector2(Mathf.Sin(sineAngle), Mathf.Cos(sineAngle));
            AngularVel = direction * Speed;
        }
        else if (Modifier == MoveModifier.ZigZag)
        {
            Count += Time.deltaTime * ModifierTimeScale;
            var zzAngle = Angle + Mathf.RoundToInt(Mathf.PingPong(Count, 1)) * ModifierIntensity;
            var direction = new Vector2(Mathf.Sin(zzAngle), Mathf.Cos(zzAngle));
            AngularVel = direction * Speed;
        }
	}

    public ParticleSystem.Particle GetParticle()
    {
        return new ParticleSystem.Particle()
        {
            color = PartColor,
            position = AngularPos.ToWorld(MainGame.Radius),
            size = Currentscale,
            lifetime = 1000,
            startLifetime = 500,
        };
    }
}
