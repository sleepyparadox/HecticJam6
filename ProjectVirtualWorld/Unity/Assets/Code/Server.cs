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
        : base(GameObject.CreatePrimitive(PrimitiveType.Cube))
    {
        GameObject.name = "Server at " + serverPos;
        Transform.localScale = Vector3.one * 0.1f;
        _renderer = GameObject.GetComponent<Renderer>();

        WorldPosition = LatLon.ToWorld(serverPos, MainGame.Radius);
        GameObject.SetActive(false);
        UnityUpdate += (me) => Transform.forward = MainGame.S.PlayerCamera.LookDirection;
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
