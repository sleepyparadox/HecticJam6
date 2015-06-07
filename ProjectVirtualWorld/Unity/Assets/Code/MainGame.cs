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
    public const float Radius = 15f;
    public static MainGame S;
    public PlayerCamera PlayerCamera;
    public float TimeLimit = 60 * 2f;
	
    public List<Vector3> nodePoints = new List<Vector3>();
    private TinyCoro _doGame;
    Node[] hackNodes;
    private Globe globe;
    private PlayerScript player;
    public GameState GameState { get; private set; }
    
    public void Awake()
    {
        S = this;

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
    }
	
	public void UpdateLine()
	{
        while(nodePoints.Count >= 3)
        {
            nodePoints.RemoveAt(0);
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
        player.Respawn();
        yield return TinyCoro.Wait(0.5f);

        hackNodes = globe.ServerLocations.Select(latLon => new Node(latLon)).ToArray();
        hackNodes[0].BecomeTarget();
        for (var i = 0; i < hackNodes.Length; ++i)
        {
            if (i < hackNodes.Length - 1)
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
