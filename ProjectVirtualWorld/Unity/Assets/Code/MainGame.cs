﻿using Sleepy.UnityTools;
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
        if(Application.isEditor)
        {
            PlayerCamera = new EditorCamera();
        }
        else
        {
            PlayerCamera = new GearVRCamera();
        }

        var globe = new Globe(Radius);

        yield return TinyCoro.Wait(1f);

        var hackNodes = globe.ServerLocations.Select(latLon => new Node(latLon)).ToArray();
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
