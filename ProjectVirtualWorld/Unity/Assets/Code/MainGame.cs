using Sleepy.UnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MainGame : MonoBehaviour
{
    public const float Radius = 7f;
    public static MainGame S;
    public PlayerCamera PlayerCamera;
    public float timeLimit = 600;

    public void Awake()
    {
        S = this;
        TinyCoro.SpawnNext(DoGame);
    }
    public void Update()
    {
        TinyCoro.StepAllCoros();
		timeLimit -= Time.deltaTime;
    }
	
	// Initialises the game and core game loop.
    public IEnumerator DoGame()
    {
        new BulletTest() { lVel = new Vector2(-1, 0) };
        new BulletTest() { lVel = new Vector2(1, 0) };
        new BulletTest() { lVel = new Vector2(0, -1) };
        new BulletTest() { lVel = new Vector2(0, 1f) };

        if(Application.isEditor)
        {
            PlayerCamera = new EditorCamera();
        }
        else
        {
            PlayerCamera = new GearVRCamera();
        }
		
		PlayerCamera.WorldPosition = Vector3.zero;

        yield break;
    }
}
