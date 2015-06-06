using UnityEngine;
using UnityEngine.UI;
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
		if (Time.timeScale > 0 && MainGame.S != null)
		{
			if (TimeLimit <= 0)
				text.text = Mathf.Floor(MainGame.S.TimeLimit / 60).ToString("00") + "." + (MainGame.S.TimeLimit % 60).ToString("00.00");
			else
				text.text = (Convert.ToInt32(TimeLimit - MainGame.S.TimeLimit) / 60).ToString() + "m " + (Convert.ToInt32(TimeLimit - MainGame.S.TimeLimit) % 60).ToString() + "s";
		}
	}
}
