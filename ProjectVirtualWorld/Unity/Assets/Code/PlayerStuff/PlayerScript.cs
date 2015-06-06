﻿using UnityEngine;
using Sleepy.UnityTools;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;
	public int lives = 3;
	
    void Awake()
    {
        S = this;
    }

	void Update ()
	{
		if (MainGame.S.PlayerCamera != null)
		{
			var hitLatLon = (MainGame.S.PlayerCamera.LookDirection * MainGame.Radius).ToLatLon(MainGame.Radius);
			transform.position = hitLatLon.ToWorld(MainGame.Radius);
			transform.rotation = MainGame.S.PlayerCamera.LookRotation;
		}
	}
	
	public void Death ()
	{
		Debug.Log("DIE!");
	}
	
	void OnParticleCollision ()
	{
		Death();
	}
	
	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.tag == "Enemy")
			Death();
		else if (collision.gameObject.tag == "Node")
			Death();
	}
}
