using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sleepy.UnityTools;

public class Enemy : MonoBehaviour
{
	// Orbit variables.
	public float orbitSpeed;
	public float distance;
	public Transform target;
	public PlayerScript player;
    float deathRadians = 0.0005f;

	// Firing variables.
	
	public float delay;
	public int bulletLimit = 1000;
	public BulletPattern[] patterns;
	public Vector2 _angularPos;
	public Vector2 angularVel;
	public Vector2 targetPos;
    public ParticleSystem particles;

    private int index = 0;

    Dictionary<TinyCoro, List<Bullet>> _activePatterns = new Dictionary<TinyCoro, List<Bullet>>();
	// Use this for initialization
	void Start ()
	{
        particles = transform.FindChild("ParticleChild").gameObject.GetComponent<ParticleSystem>();
		player = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
		StartCoroutine(Orbit());
		StartCoroutine(Shoot(this));
	}

    void OnEnable()
    {
        index = Random.Range(0, patterns.Length);
    }
	
	void Update ()
	{
        foreach(var col in _activePatterns.Values)
        {
            foreach(var b in col)
            {
                b.Update();

                if (LatLon.GetClosestDist(b._angularPos, player.currentPos).sqrMagnitude < deathRadians)
                {
                    player.Death();
                }
            }
        }

		//_angularPos += angularVel * Time.deltaTime;
		transform.position = _angularPos.ToWorld(MainGame.Radius);

        particles.transform.position = MainGame.S.PlayerCamera.LookSource + (MainGame.S.PlayerCamera.LookDirection * 100f);

		if (MainGame.S.PlayerCamera != null)
			transform.rotation = MainGame.S.PlayerCamera.LookRotation;
	}
	
    void FixedUpdate()
    {
        var particlePositions = new List<ParticleSystem.Particle>();
        foreach (var col in _activePatterns.Values)
        {
            col.RemoveAll(b => b.lifespan < 0f);
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
        particles.SetParticles(pArray, pArray.Length);

        particles.time = 0;
    }

	// Update is called once per frame
	IEnumerator Shoot (Enemy enemy)
	{
		while (player != null)
		{
            if (enemy._activePatterns.Values.Sum(x => x.Count) < bulletLimit)
			{
				//Debug.Log("Start pattern");
				var bulletsForPattern = new List<Bullet>();
                var coro = TinyCoro.SpawnNext(() => BulletPattern.DoSpawn(bulletsForPattern, patterns[index], this));

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

            yield return new WaitForSeconds(delay - (MainGame.S.nodesHacked / 5f));
		}

        //Debug.Log("Finished shooting");
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
                int count = pattern.count + MainGame.S.nodesHacked;

                var angle = ((float)i / pattern.waves) * (Mathf.PI * 2) + (pattern.forwardAngle / 360f * Mathf.PI * 2f);
                var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                var bulletScript = new Bullet()
                {
                    speed = pattern.speed,
                    lifespan = pattern.lifespan,
                    _angularPos = enemy._angularPos,
                };
                _bullets.Add(bulletScript);

                bulletScript.partColor = pattern.partColor;
                bulletScript.angle = angle;
                bulletScript._angularPos = enemy._angularPos;
                bulletScript.angularVel = direction * pattern.speed;
                bulletScript.speed = pattern.speed + MainGame.S.nodesHacked / 10f;
                bulletScript.lifespan = pattern.lifespan;
                bulletScript.modifier = pattern.modifier;
                bulletScript.modifierIntensity = pattern.modifierIntensity;
                bulletScript.modifierTimeScale = pattern.modifierTimeScale;

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
                int count = pattern.count + MainGame.S.nodesHacked;

                for (int x = 0; x < count; x++)
                {
                    var angle = ((float)x / (count - 1)) * (Mathf.PI * 2) / (360f / pattern.arc) - ((Mathf.PI * 2) / (360f / pattern.arc)) / 2f + (pattern.forwardAngle / 360f * Mathf.PI * 2f);
                    var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    //Debug.Log("Do spawn bulletScript");
                    var bulletScript = new Bullet()
                    {
                        speed = pattern.speed,
                        lifespan = pattern.lifespan,
                        _angularPos = enemy._angularPos,
                    };
                    _bullets.Add(bulletScript);

                    bulletScript.partColor = pattern.partColor;
                    bulletScript.angle = angle;
                    bulletScript._angularPos = enemy._angularPos;
                    bulletScript.angularVel = direction * pattern.speed;
                    bulletScript.speed = pattern.speed + MainGame.S.nodesHacked / 10f;
                    bulletScript.lifespan = pattern.lifespan;
                    bulletScript.modifier = pattern.modifier;
                    bulletScript.modifierIntensity = pattern.modifierIntensity;
                    bulletScript.modifierTimeScale = pattern.modifierTimeScale;
                }
                yield return TinyCoro.Wait(pattern.delay);
            }
        }
        else if (pattern.shape == BasicShape.Circle)
        {
            for (int i = 0; i < pattern.waves; i++)
            {
                int count = pattern.count + MainGame.S.nodesHacked;
                
                for (int x = 0; x < count; x++)
                {
                    var angle = ((float)x / count) * (Mathf.PI * 2) + (pattern.forwardAngle / 360f * Mathf.PI * 2f);
                    var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    //Debug.Log("Do spawn bulletScript");
                    var bulletScript = new Bullet()
                    {
                        speed = pattern.speed,
                        lifespan = pattern.lifespan,
                        _angularPos = enemy._angularPos,
                    };
                    _bullets.Add(bulletScript);

                    bulletScript.partColor = pattern.partColor;
                    bulletScript.angle = angle;
                    bulletScript._angularPos = enemy._angularPos;
                    bulletScript.angularVel = direction * pattern.speed;
                    bulletScript.speed = pattern.speed + MainGame.S.nodesHacked / 10f;
                    bulletScript.lifespan = pattern.lifespan;
                    bulletScript.modifier = pattern.modifier;
                    bulletScript.modifierIntensity = pattern.modifierIntensity;
                    bulletScript.modifierTimeScale = pattern.modifierTimeScale;
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
