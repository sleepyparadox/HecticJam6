using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sleepy.UnityTools;

public class Enemy : MonoBehaviour
{
    const float DeathRadiusRadians = 0.0005f;
    const int BulletLimit = 1000;

	// Orbit variables.
	public float OrbitSpeed;
	public float Distance;
	public Transform Target;

	// Firing variables.
	public float Delay;
	public BulletPattern[] patterns; //Preserving prefab data
	public Vector2 AngularPos;
	public Vector2 AngularVel;
	public Vector2 TargetPos;
    public ParticleSystem Particles;

    int _index = 0;
    Dictionary<TinyCoro, List<Bullet>> _activePatterns = new Dictionary<TinyCoro, List<Bullet>>();
	
    // Use this for initialization
	void Start ()
	{
        Particles = transform.FindChild("ParticleChild").gameObject.GetComponent<ParticleSystem>();
		StartCoroutine(Orbit());
		StartCoroutine(Shoot(this));
	}

    void OnEnable()
    {
        _index = Random.Range(0, patterns.Length);
    }
	
	void Update ()
	{
        foreach(var col in _activePatterns.Values)
        {
            foreach(var b in col)
            {
                b.Update();

                if (LatLon.GetClosestDist(b.AngularPos, MainGame.S.Player.CurrentPos).sqrMagnitude < DeathRadiusRadians)
                {
                    MainGame.S.Player.Death();
                }
            }
        }

		//_angularPos += angularVel * Time.deltaTime;
		transform.position = AngularPos.ToWorld(MainGame.Radius);

        Particles.transform.position = MainGame.S.PlayerCamera.LookSource + (MainGame.S.PlayerCamera.LookDirection * 100f);

		if (MainGame.S.PlayerCamera != null)
			transform.rotation = MainGame.S.PlayerCamera.LookRotation;
	}
	
    void FixedUpdate()
    {
        var particlePositions = new List<ParticleSystem.Particle>();
        foreach (var col in _activePatterns.Values)
        {
            col.RemoveAll(b => b.Lifespan < 0f);
            particlePositions.AddRange(col.Select(p => p.GetParticle()));
        }

        //if(particlePositions.Count > 0)
        //    Debug.Log("Bullets " + particlePositions.Count);

        //for (var i = -100; i <= 100; ++i)
        //{
        //    particlePositions.Add(new ParticleSystem.Particle()
        //    {
        //        size = 1,
        //        position = new Vector3(0, i, 0),
        //        lifetime = 1000,
        //        startLifetime = 500,
        //        color = Color.white,
        //    });
        //    particlePositions.Add(new ParticleSystem.Particle()
        //    {
        //        size = 1,
        //        position = new Vector3(i, 0, 0),
        //        lifetime = 1000,
        //        startLifetime = 500,
        //        color = Color.white,
        //    });
        //}
        var pArray = particlePositions.ToArray();
        Particles.SetParticles(pArray, pArray.Length);

        Particles.time = 0;
    }

	// Update is called once per frame
	IEnumerator Shoot (Enemy enemy)
	{
		while (MainGame.S.Player != null)
		{
            if (enemy._activePatterns.Values.Sum(x => x.Count) < BulletLimit)
			{
				//Debug.Log("Start pattern");
				var bulletsForPattern = new List<Bullet>();
                var coro = TinyCoro.SpawnNext(() => BulletPattern.DoSpawn(bulletsForPattern, patterns[_index], this));

				_activePatterns.Add(coro, bulletsForPattern);
				coro.OnFinished += (c, r) =>
				{
					if(r == TinyCoroFinishReason.Killed)
					{
						//Debug.Log("end pattern");
						_activePatterns.Remove(coro);
					}
				};

                while (coro.Alive)
                    yield return null;
			}
			else
                yield return null;

            yield return new WaitForSeconds(Delay - (MainGame.S.NodesHacked / 5f));
		}

        //Debug.Log("Finished shooting");
	}
	
	IEnumerator Orbit ()
	{
		float count = 0;
		
		while (true)
		{
			if (Target && Target.gameObject.activeSelf)
			{
				if (count < 1)
					count += Time.deltaTime * OrbitSpeed;
				else
					count = 0;
				
				float angle = 360 * count;
				
				AngularPos.x = TargetPos.x + Distance * Mathf.Cos(angle * Mathf.Deg2Rad);
				AngularPos.y = TargetPos.y + Distance * Mathf.Sin(angle * Mathf.Deg2Rad);
			}
			
			yield return null;
		}
	}

    public void End()
    {
        foreach (var coro in _activePatterns.Keys)
            coro.Kill();
    }
}

public enum BasicShape { Single, Line, Arc, Circle }
[System.Serializable]
public class BulletPattern
{
	public BasicShape shape = BasicShape.Single;
	public MoveModifier modifier = MoveModifier.Straight;
	public float modifierIntensity;
    public float modifierTimeScale;

	public Color partColor = Color.red;
	public float speed;
	public float lifespan;
	public int count;
	public int waves;
	public float delay;
	public float arc;
	public float forwardAngle;

    // Update is called once per frame
    public static IEnumerator DoSpawn(List<Bullet> _bullets, BulletPattern pattern, Enemy enemy)
    {
		//Debug.Log(string.Format("Do spawn pattern {0} w{1} c{2} l{3}", pattern.shape, pattern.waves, pattern.count, pattern.lifespan));
        if (pattern.shape == BasicShape.Single)
        {
            for (int i = 0; i < pattern.waves; i++)
            {
                int count = pattern.count + MainGame.S.NodesHacked;

                var angle = ((float)i / pattern.waves) * (Mathf.PI * 2) + (pattern.forwardAngle / 360f * Mathf.PI * 2f);
                var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                var bulletScript = new Bullet()
                {
                    Speed = pattern.speed,
                    Lifespan = pattern.lifespan,
                    AngularPos = enemy.AngularPos,
                };
                _bullets.Add(bulletScript);

                bulletScript.PartColor = pattern.partColor;
                bulletScript.Angle = angle;
                bulletScript.AngularPos = enemy.AngularPos;
                bulletScript.AngularVel = direction * pattern.speed;
                bulletScript.Speed = pattern.speed + MainGame.S.NodesHacked / 10f;
                bulletScript.Lifespan = pattern.lifespan;
                bulletScript.Modifier = pattern.modifier;
                bulletScript.ModifierIntensity = pattern.modifierIntensity;
                bulletScript.ModifierTimeScale = pattern.modifierTimeScale;

                yield return TinyCoro.Wait(pattern.delay);
            }
        }
        //else if (pattern.shape == BasicShape.Line)
        //{
        //    for (int i = 0; i < pattern.waves; i++)
        //    {
        //        for (int x = 0; x < pattern.count; x++)
        //        {
        //            GameObject bulletObj = RecycleController.Spawn(Assets.Prefabs.BulletPrefab.Prefab, transform.position, transform.rotation);
        //            Debug.Log(CalculateLinePos(x, length, count));
        //            var angle = 0;
        //            var bulletScript = bulletObj.GetComponent<Bullet>();
        //
        //            bulletScript.angle = angle;
        //            bulletScript.speed = pattern.speed;
        //            bulletScript.lifespan = pattern.lifespan;
        //            bulletScript._angularPos = enemy._angularPos;
        //            bulletScript.modifier = pattern.modifier;
        //            bulletScript.modifierIntensity = pattern.modifierIntensity;
        //            bulletScript.Line(CalculateLinePos(x, length, count));
        //        }
        //        yield return new WaitForSeconds(pattern.delay);
        //    }
        //}
        else if (pattern.shape == BasicShape.Arc)
        {
            for (int i = 0; i < pattern.waves; i++)
            {
                int count = pattern.count + MainGame.S.NodesHacked;

                for (int x = 0; x < count; x++)
                {
                    var angle = ((float)x / (count - 1)) * (Mathf.PI * 2) / (360f / pattern.arc) - ((Mathf.PI * 2) / (360f / pattern.arc)) / 2f + (pattern.forwardAngle / 360f * Mathf.PI * 2f);
                    var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    //Debug.Log("Do spawn bulletScript");
                    var bulletScript = new Bullet()
                    {
                        Speed = pattern.speed,
                        Lifespan = pattern.lifespan,
                        AngularPos = enemy.AngularPos,
                    };
                    _bullets.Add(bulletScript);

                    bulletScript.PartColor = pattern.partColor;
                    bulletScript.Angle = angle;
                    bulletScript.AngularPos = enemy.AngularPos;
                    bulletScript.AngularVel = direction * pattern.speed;
                    bulletScript.Speed = pattern.speed + MainGame.S.NodesHacked / 10f;
                    bulletScript.Lifespan = pattern.lifespan;
                    bulletScript.Modifier = pattern.modifier;
                    bulletScript.ModifierIntensity = pattern.modifierIntensity;
                    bulletScript.ModifierTimeScale = pattern.modifierTimeScale;
                }
                yield return TinyCoro.Wait(pattern.delay);
            }
        }
        else if (pattern.shape == BasicShape.Circle)
        {
            for (int i = 0; i < pattern.waves; i++)
            {
                int count = pattern.count + MainGame.S.NodesHacked;
                
                for (int x = 0; x < count; x++)
                {
                    var angle = ((float)x / count) * (Mathf.PI * 2) + (pattern.forwardAngle / 360f * Mathf.PI * 2f);
                    var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    //Debug.Log("Do spawn bulletScript");
                    var bulletScript = new Bullet()
                    {
                        Speed = pattern.speed,
                        Lifespan = pattern.lifespan,
                        AngularPos = enemy.AngularPos,
                    };
                    _bullets.Add(bulletScript);

                    bulletScript.PartColor = pattern.partColor;
                    bulletScript.Angle = angle;
                    bulletScript.AngularPos = enemy.AngularPos;
                    bulletScript.AngularVel = direction * pattern.speed;
                    bulletScript.Speed = pattern.speed + MainGame.S.NodesHacked / 10f;
                    bulletScript.Lifespan = pattern.lifespan;
                    bulletScript.Modifier = pattern.modifier;
                    bulletScript.ModifierIntensity = pattern.modifierIntensity;
                    bulletScript.ModifierTimeScale = pattern.modifierTimeScale;
                }
                yield return TinyCoro.Wait(pattern.delay);
            }
        }
    }

    public float CalculateLinePos(int index, float space, int total, Vector2 center)
    {
        float x = center.x;
        x += (space / (total - 1f) * index) - space / 2f;

        return x;
    }

    public Vector3 CalculateCircleVector(int index, int total)
    {
        var vector = Quaternion.AngleAxis(360f / total * index, Vector3.forward) * Vector3.up;

        return vector;
    }
}
