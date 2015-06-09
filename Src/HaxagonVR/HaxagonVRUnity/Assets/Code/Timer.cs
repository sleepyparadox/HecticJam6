using UnityEngine;
using System;
using System.Collections;

public class Timer : UnityObject
{
	public float TimeLimit;
	private TextMesh _text;
	
	// Use this for initialization
	public Timer()
        : base(Assets.Prefabs.TimerPrefab)
	{
		_text = GameObject.GetComponent<TextMesh>();
        UnityUpdate += Update;
	}
	
	// Update is called once per frame
	void Update (UnityObject me)
	{
		if (Time.timeScale > 0 
            && MainGame.S != null)
		{
            if (MainGame.S.GameState == GameState.Lose)
            {
                _text.text = "you hacked " + MainGame.S.NodesHacked + " nodes\ntap to continue";
            }
            else if (MainGame.S.GameState == GameState.Ready)
            {
                _text.text = "HaxagoN VR\ntap to continue";
            }
            else if (MainGame.S.GameState == GameState.Playing)
            {
                if (TimeLimit <= 0)
                    _text.text = "\r\n" + Mathf.Floor(MainGame.S.TimeLimit / 60).ToString("00") + "." + (MainGame.S.TimeLimit % 60).ToString("00");
                else
                    _text.text = (Convert.ToInt32(TimeLimit - MainGame.S.TimeLimit) / 60).ToString() + "m " + (Convert.ToInt32(TimeLimit - MainGame.S.TimeLimit) % 60).ToString() + "s";
            }
		}
	}
}
