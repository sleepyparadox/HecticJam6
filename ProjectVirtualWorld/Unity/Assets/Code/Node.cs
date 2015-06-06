using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Node : UnityObject
{
    public Node NextNode;
    UnityObject _boss;
    public event Action OnDefeated;
    public float TimeHacked;
    public float TotalHackTime;
    float HitRadius = 0.35f;
    Renderer _renderer;
    private Vector2 _posLatLon;

    public Node(Vector2 serverPos)
        : base(Assets.Prefabs.ServerPrefab)
    {
        _posLatLon = serverPos;
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
        _boss = new UnityObject(Assets.Prefabs.EasyEnemyPrefab);
        var bossPosLatLon = _posLatLon + new Vector2(0f, Mathf.PI * 0.1f);
        _boss.GameObject.GetComponent<Enemy>()._angularPos = bossPosLatLon;
        _boss.WorldPosition = LatLon.ToWorld(bossPosLatLon, MainGame.Radius);
    }

    void CheckForPlayer(UnityObject me)
    {
        if (PlayerScript.S != null
            && (PlayerScript.S.transform.position - WorldPosition).sqrMagnitude < (HitRadius * HitRadius))
        {
            SetActive(false);
            UnityUpdate -= CheckForPlayer;

            _boss.Dispose();

            if (NextNode != null)
                NextNode.BecomeTarget();

            if (OnDefeated != null)
                OnDefeated();
        }
    }
}
