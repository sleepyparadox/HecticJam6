using Sleepy.UnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    public const float Radius = 15f;
    public static MainGame S;
    public PlayerCamera PlayerCamera;
    public float TimeLimit = 605;
	
    public List<Vector3> nodePoints = new List<Vector3>();
    public CurveLineRenderer line;
    private TinyCoro _doGame;
    Node[] hackNodes;
    private Globe globe;

    public void Awake()
    {
        S = this;
		line = GetComponent<CurveLineRenderer>();

        if (Application.isEditor)
        {
            PlayerCamera = new EditorCamera();
        }
        else
        {
            PlayerCamera = new GearVRCamera();
        }

        globe = new Globe(Radius);

        _doGame = TinyCoro.SpawnNext(DoGame);
    }
    public void Update()
    {
        TinyCoro.StepAllCoros();
		TimeLimit -= Time.deltaTime;
    }
	
	public void UpdateLine()
	{
        while(nodePoints.Count >= 3)
        {
            nodePoints.RemoveAt(0);
        }
        if (nodePoints.Count > 0)
        {
            line.vertices = nodePoints;
        }
	}

    public void Lose()
    {
        _doGame.Kill();

        foreach(var node in hackNodes)
        {
            node.Dispose();
        }
    }
	
	// Initialises the game and core game loop.
    public IEnumerator DoGame()
    {
        yield return TinyCoro.Wait(1f);

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
