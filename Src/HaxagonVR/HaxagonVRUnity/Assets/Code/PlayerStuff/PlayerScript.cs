using UnityEngine;
using Sleepy.UnityTools;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;
    public Vector2 CurrentPos;

    float _speed = 3f;

    void Awake()
    {
        S = this;
    }

	void Update ()
	{
		if (MainGame.S.PlayerCamera != null)
		{
			var hitPos = (MainGame.S.PlayerCamera.LookDirection * MainGame.Radius).ToLatLon(MainGame.Radius);
            //transform.position = hitLatLon.ToWorld(MainGame.Radius);
            var diff = LatLon.GetClosestDist(CurrentPos, hitPos);


            CurrentPos = Vector2.Lerp(CurrentPos, CurrentPos + diff, Time.deltaTime * 10f);

            Debug.DrawLine(CurrentPos.ToWorld(MainGame.Radius), (CurrentPos + diff).ToWorld(MainGame.Radius), Color.green);
            //Debug.DrawLine(_currentPos.ToWorld(MainGame.Radius), (hitPos).ToWorld(MainGame.Radius), Color.green);

            transform.position = CurrentPos.ToWorld(MainGame.Radius);
            transform.rotation = MainGame.S.PlayerCamera.LookRotation;
		}

        if (Input.GetKeyUp(KeyCode.Escape))
            Death();
	}
	
    public void Respawn()
    {
        gameObject.SetActive(true);
        CurrentPos = (MainGame.S.PlayerCamera.LookDirection * MainGame.Radius).ToLatLon(MainGame.Radius);
        transform.position = CurrentPos.ToWorld(MainGame.Radius);
        transform.rotation = MainGame.S.PlayerCamera.LookRotation;
    }

	public void Death ()
	{
        MainGame.S.Lose();
        gameObject.SetActive(false);
	}
}
