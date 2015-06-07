using Sleepy.UnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Menu : MonoBehaviour
{
    public static Menu S;
    public PlayerCamera PlayerCamera;

    public void Awake()
    {
        S = this;
        TinyCoro.SpawnNext(DoGame);
    }
    public void Update()
    {
        TinyCoro.StepAllCoros();
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

        yield break;
    }
}
