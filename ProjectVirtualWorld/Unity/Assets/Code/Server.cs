using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Server : UnityObject
{
    public Server NextNode;
    public event Action OnDefeated;
    public float TimeHacked;
    public float TotalHackTime;
    float HitRadius = 0.15f;
    Renderer _renderer;

    public Server(Vector2 serverPos)
        : base(Assets.Prefabs.ServerPrefab)
    {
        GameObject.name = "Server at " + serverPos;
        _renderer = GameObject.GetComponent<Renderer>();

        WorldPosition = LatLon.ToWorld(serverPos, MainGame.Radius);
        GameObject.SetActive(false);
        UnityUpdate += (me) => Transform.rotation = MainGame.S.PlayerCamera.LookRotation;
    }

    public void BecomeTarget()
    {
        GameObject.SetActive(true);
        UnityUpdate += CheckForPlayer;
    }

    void CheckForPlayer(UnityObject me)
    {
        if (PlayerScript.S != null
            && (PlayerScript.S.transform.position - WorldPosition).sqrMagnitude < (HitRadius * HitRadius))
        {
            SetActive(false);
            UnityUpdate -= CheckForPlayer;

            if (NextNode != null)
                NextNode.BecomeTarget();

            if (OnDefeated != null)
                OnDefeated();
        }
    }
}
