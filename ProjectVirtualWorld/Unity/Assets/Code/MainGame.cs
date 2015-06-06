using Sleepy.UnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MainGame : MonoBehaviour
{
    public const float Radius = 3f;
    public static MainGame S;
    public PlayerCamera PlayerCamera;
    public UnityObject PlayerShip;

    public void Awake()
    {
        S = this;
        TinyCoro.SpawnNext(DoGame);
    }
    public void Update()
    {
        TinyCoro.StepAllCoros();
    }

    public IEnumerator DoGame()
    {
        new BulletTest() { lVel = new Vector2(-1, 0) };
        new BulletTest() { lVel = new Vector2(1, 0) };
        new BulletTest() { lVel = new Vector2(0, -1) };
        new BulletTest() { lVel = new Vector2(0, 1f) };

        //if(Application.isEditor)
        //{
        //    PlayerCamera = new EditorCamera();
        //}
        //else
        {
            PlayerCamera = new GearVRCamera();
        }
        PlayerCamera.WorldPosition = Vector3.zero;

        PlayerShip = new UnityObject(GameObject.CreatePrimitive(PrimitiveType.Cube));
        PlayerShip.Transform.localScale = Vector3.one * 0.3f;
        PlayerShip.GameObject.name = "PlayerShip";
        PlayerShip.UnityUpdate += (me) =>
        {
            var hitLatLon = (PlayerCamera.LookDirection * Radius).ToLatLon(Radius);
            PlayerShip.WorldPosition = hitLatLon.ToWorld(Radius);

            Debug.Log("Dir " + PlayerCamera.LookDirection + ", latLon " + hitLatLon + ", world " + PlayerShip.WorldPosition);
        };

        yield break;
    }
}
