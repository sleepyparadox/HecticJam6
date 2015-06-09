using UnityEngine;
using Sleepy.UnityTools;
using System.Collections;

public class Player : UnityObject
{
    public Vector2 CurrentPos;

    public Player()
        : base(Assets.Prefabs.PlayerPrefab)
    {
        UnityUpdate += Update;
    }

	void Update (UnityObject me)
	{
		if (MainGame.S.PlayerCamera != null)
		{
			var hitPos = (MainGame.S.PlayerCamera.LookDirection * MainGame.Radius).ToLatLon(MainGame.Radius);
            //transform.position = hitLatLon.ToWorld(MainGame.Radius);
            var diff = LatLon.GetClosestDist(CurrentPos, hitPos);

            CurrentPos = Vector2.Lerp(CurrentPos, CurrentPos + diff, Time.deltaTime * 10f);

            Debug.DrawLine(CurrentPos.ToWorld(MainGame.Radius), (CurrentPos + diff).ToWorld(MainGame.Radius), Color.green);
            //Debug.DrawLine(_currentPos.ToWorld(MainGame.Radius), (hitPos).ToWorld(MainGame.Radius), Color.green);

            Transform.position = CurrentPos.ToWorld(MainGame.Radius);
            Transform.rotation = MainGame.S.PlayerCamera.LookRotation;
		}

        if (Input.GetKeyUp(KeyCode.Escape))
            Death();
	}
	
    public void Respawn()
    {
        SetActive(true);
        CurrentPos = (MainGame.S.PlayerCamera.LookDirection * MainGame.Radius).ToLatLon(MainGame.Radius);
        Transform.position = CurrentPos.ToWorld(MainGame.Radius);
        Transform.rotation = MainGame.S.PlayerCamera.LookRotation;
    }

	public void Death ()
	{
        MainGame.S.Lose();
        GameObject.SetActive(false);
	}
}
