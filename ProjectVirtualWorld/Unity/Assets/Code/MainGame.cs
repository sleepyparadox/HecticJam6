using Sleepy.UnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public enum GameState
{
    Ready,
    Playing,
    Lose,
}

public class MainGame : MonoBehaviour
{
    public int nodesHacked = 0;
    public const float Radius = 15f;
    public const float TimeGainedPerNode = 30;

    public static MainGame S;
    public PlayerCamera PlayerCamera;
    public const float DefaultTimeLimit = 60 * 2f;
    public float TimeLimit = DefaultTimeLimit;

	
    public List<Vector3> nodePoints = new List<Vector3>();
    private TinyCoro _doGame;
    List<Node> hackNodes;
    private Globe globe;
    private PlayerScript player;
    public GameState GameState { get; private set; }

    ParticleSystem _lineParticleSystem;
    public void Awake()
    {
        S = this;
        _lineParticleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();

        if (Application.isEditor)
        {
            PlayerCamera = new EditorCamera();
        }
        else
        {
            PlayerCamera = new GearVRCamera();
        }

        globe = new Globe(Radius);
        player = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();

        GameState = GameState.Ready;
    }

    public void Update()
    {
        TinyCoro.StepAllCoros();
		TimeLimit -= Time.deltaTime;

        if(TimeLimit <= 0)
        {
            Lose();
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (GameState == global::GameState.Ready)
            {
                GameState = GameState.Playing;
                _doGame = TinyCoro.SpawnNext(DoGame);
            }
            if (GameState == global::GameState.Lose)
            {
                GameState = global::GameState.Ready;
            }
        }
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Lose();
            GameState = global::GameState.Ready;
        }

        if(GameState == global::GameState.Playing
            && hackNodes != null && hackNodes.Count >= 2)
        {
            var next = hackNodes.FirstOrDefault(h => h.IsTarget);
            Node prev = null;
            if (next == hackNodes.First())
                prev = hackNodes[hackNodes.Count - 2];
            else
                prev = hackNodes[hackNodes.IndexOf(next) - 1];

            var startPos = prev._posLatLon;
            var midPos = player.currentPos;
            var endPos = next._posLatLon;
            
            var lineOne = LatLon.GetClosestDist(startPos, midPos);
            var lineTwo = LatLon.GetClosestDist(startPos, endPos);

            var distOne = lineOne.magnitude;
            var distTwo = lineOne.magnitude;

            var particles = new List<ParticleSystem.Particle>();
            var step = 0.1f;
            for (float i = (Time.time % 1) * step; i < distOne + distTwo; i += step)
            {
                if(i < distOne)
                {
                    if (MainGame.S.nodesHacked > 0)
                    {
                        //Drawing first line
                        var point = Vector2.Lerp(startPos, midPos, i / distOne);
                        particles.Add(new ParticleSystem.Particle()
                        {
                            color = Color.white * 0.35f,
                            position = point.ToWorld(MainGame.Radius),
                            size = 0.5f,
                            lifetime = 1000,
                            startLifetime = 500,
                        });
                    }
                }
                else
                {
                    //Drawing second
                    var point = Vector2.Lerp(midPos, endPos, (i - distOne) / distTwo);
                    particles.Add(new ParticleSystem.Particle()
                    {
                        color = Color.white * 0.5f,
                        position = point.ToWorld(MainGame.Radius),
                        size = 0.5f,
                        lifetime = 1000,
                        startLifetime = 500,
                    });
                }
            }
            _lineParticleSystem.SetParticles(particles.ToArray(), particles.Count);
        }
        else
        {
            _lineParticleSystem.SetParticles(null, 0);
        }
    }

    public void Lose()
    {
        if (_doGame != null && _doGame.Alive)
            _doGame.Kill();

        foreach(var node in hackNodes)
        {
            if (node.GameObject != null)
            {
                node.Dispose();
            }
        }

        GameState = global::GameState.Lose;
    }
	
    public void Start()
    {
        player.gameObject.SetActive(true);
    }

    public IEnumerator DoGame()
    {
        nodesHacked = 0;
        TimeLimit = DefaultTimeLimit;
        player.Respawn();
        yield return TinyCoro.Wait(0.5f);

        hackNodes = globe.ServerLocations.Select(latLon => new Node(latLon)).ToList();
        hackNodes[0].BecomeTarget();
        for (var i = 0; i < hackNodes.Count; ++i)
        {
            if (i < hackNodes.Count - 1)
            {
                hackNodes[i].NextNode = hackNodes[i + 1];
            }
            else
            {
                hackNodes[i].NextNode = hackNodes[0];
            }
        }

        yield break;
    }
}
