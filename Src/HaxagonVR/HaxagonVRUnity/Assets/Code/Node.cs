using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Node : UnityObject
{
    public bool IsTarget;
    public Node NextNode;
    public event Action OnDefeated;

    const float HitRadius = 0.35f;

    Renderer _renderer;
    public Vector2 _posLatLon;
    UnityObject _boss;

    public Node(Vector2 serverPos)
        : base(Assets.Prefabs.ServerPrefab)
    {
        _posLatLon = serverPos;
        GameObject.name = "Server at " + serverPos;
        _renderer = GameObject.GetComponent<Renderer>();

        WorldPosition = LatLon.ToWorld(serverPos, MainGame.Radius);
        GameObject.SetActive(false);
        UnityUpdate += (me) => Transform.rotation = MainGame.S.PlayerCamera.LookRotation;

        OnDispose += (me) =>
        {
            if (_boss != null)
                _boss.Dispose();
        };
    }

    public void BecomeTarget()
    {
        IsTarget = true;
        MainGame.S.NodePoints.Add(Transform.position);
        GameObject.SetActive(true);
        UnityUpdate += CheckForPlayer;
        _boss = new UnityObject(Assets.Prefabs.EasyEnemyPrefab);
        var bossPosLatLon = _posLatLon + new Vector2(0f, Mathf.PI * 0.1f);
        _boss.GameObject.GetComponent<Enemy>().AngularPos = bossPosLatLon;
        _boss.WorldPosition = LatLon.ToWorld(bossPosLatLon, MainGame.Radius);
		
        _boss.GameObject.GetComponent<Enemy>().Target = Transform;
        _boss.GameObject.GetComponent<Enemy>().TargetPos = _posLatLon;
    }

    void CheckForPlayer(UnityObject me)
    {
        if (PlayerScript.S != null
            && (PlayerScript.S.transform.position - WorldPosition).sqrMagnitude < (HitRadius * HitRadius))
        {
            MainGame.S.TimeLimit += MainGame.TimeGainedPerNode;
            MainGame.S.NodesHacked += 1;

            IsTarget = false;
            SetActive(false);
            UnityUpdate -= CheckForPlayer;

            if (_boss != null)
                _boss.Dispose();

            if (NextNode != null)
                NextNode.BecomeTarget();

            if (OnDefeated != null)
                OnDefeated();
        }
    }
}
