using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class BulletTest : UnityObject
{
    public BulletTest()
        : base(GameObject.CreatePrimitive(PrimitiveType.Cube))
    {
        Transform.localScale = Vector3.one * 0.2f;
        UnityUpdate += Update;
    }

    void Update (UnityObject me)
    {
        lPos += lVel * Time.deltaTime;
        WorldPosition = lPos.ToWorld(MainGame.Radius);
        //Debug.Log(string.Format("pos {0}, vel {1}, wPos {2}", lPos, lVel, WorldPosition));
    }
    public Vector2 lPos;
    public Vector2 lVel;
}