using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Globe : UnityObject 
{
    public List<Vector2> ServerLocations;
    public Globe(float radius)
        : base(GameObject.Find("Globe"))
    {
        var meshFilter = GameObject.GetComponent<MeshFilter>();
        var mesh = meshFilter.mesh;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;

        Transform.localScale = Vector3.one * radius * 1.6f/*Fudge*/; 
        ServerLocations = new List<Vector2>();
        for(var i = 0; i < Transform.childCount; ++i)
        {
            var child = Transform.GetChild(i);
            var latLon = LatLon.ToLatLon(child.transform.position, MainGame.Radius);
            //latLon.x += Mathf.PI * 0.5f;

            ServerLocations.Add(latLon);

            //Disable
            child.gameObject.SetActive(false);
        }
    }
}
