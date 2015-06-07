using UnityEngine;
using System;
using System.Collections;

public class Timer : MonoBehaviour
{
	public float TimeLimit;
	private TextMesh text;
	private MainGame mainGame;
	
	// Use this for initialization
	void Start ()
	{
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.timeScale > 0 
            && MainGame.S != null)
		{
            if (MainGame.S.GameState == GameState.Lose)
            {
                text.text = "YOU LOSE\ntap to continue";
            }
            else if (MainGame.S.GameState == GameState.Ready)
            {
                text.text = "HAXAGON\ntap to continue";
            }
            else if (MainGame.S.GameState == GameState.Playing)
            {
                if (TimeLimit <= 0)
                    text.text = "\r\n" + Mathf.Floor(MainGame.S.TimeLimit / 60).ToString("00") + "." + (MainGame.S.TimeLimit % 60).ToString("00");
                else
                    text.text = (Convert.ToInt32(TimeLimit - MainGame.S.TimeLimit) / 60).ToString() + "m " + (Convert.ToInt32(TimeLimit - MainGame.S.TimeLimit) % 60).ToString() + "s";
            }
		}
	}
}
