using UnityEngine;
using Sleepy.UnityTools;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;
	public int lives = 3;
    public Vector2 currentPos;
    public float speed = 3f;
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
            var diff = LatLon.GetClosestDist(currentPos, hitPos);


            currentPos = Vector2.Lerp(currentPos, currentPos + diff, Time.deltaTime * 10f);

            Debug.DrawLine(currentPos.ToWorld(MainGame.Radius), (currentPos + diff).ToWorld(MainGame.Radius), Color.green);
            //Debug.DrawLine(_currentPos.ToWorld(MainGame.Radius), (hitPos).ToWorld(MainGame.Radius), Color.green);

            transform.position = currentPos.ToWorld(MainGame.Radius);
            transform.rotation = MainGame.S.PlayerCamera.LookRotation;
		}

        if (Input.GetKeyUp(KeyCode.Escape))
            Death();
	}
	
	public void Death ()
	{
		Application.LoadLevel("GameOver");
	}
}
