using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class LatLon
{
    const float LonOffset = Mathf.PI * 0.5f;

    /// <summary>
    /// north / south, .y
    /// </summary>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static float GetLat(this Vector2 vec2)
    {
        return vec2.y;
    }

    /// <summary>
    /// east / west, .x
    /// </summary>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static float GetLon(this Vector2 vec2)
    {
        return vec2.x;
    }

    /// <summary>
    /// north / south, .y
    /// </summary>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static void SetLat(ref Vector2 vec2, float lat)
    {
        vec2.y = lat;
    }

    /// <summary>
    /// east / west, .x
    /// </summary>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static void SetLon(ref Vector2 vec2, float lon)
    {
        vec2.x = lon;
    }

    public static Vector3 ToWorld(this Vector2 vec2, float worldRadius )
    {
        var x = worldRadius * Mathf.Cos(vec2.GetLat()) * Mathf.Cos((vec2.GetLon() * -1f) + LonOffset);
        var y = worldRadius * Mathf.Cos(vec2.GetLat()) * Mathf.Sin((vec2.GetLon() * -1f) + LonOffset);
        var z = worldRadius * Mathf.Sin(vec2.GetLat());
        return new Vector3(x, z, y);
    }

    public static Vector2 ToLatLon(this Vector3 vec3, float worldRadius)
    {
        var result = new Vector2();
        SetLat(ref result, Mathf.Asin(vec3.y / worldRadius));
        SetLon(ref result, (Mathf.Atan2(vec3.z, vec3.x) - LonOffset) * -1f);
        //Debug.Log("Lat " + Mathf.Asin(vec3.z / worldRadius) + ", lon " + Mathf.Atan2(vec3.y, vec3.x) + ", result " + result);
        return result;
    }

    public static Vector2 GetClosestDist(Vector2 a, Vector2 b)
    {
        var classicDist = b - a;

        //Alternative check
        a.x = a.x < Mathf.PI ? a.x + Mathf.PI + Mathf.PI : a.x;
        a.y = a.y < Mathf.PI ? a.y + Mathf.PI + Mathf.PI : a.y;
        b.x = b.x < Mathf.PI ? b.x + Mathf.PI + Mathf.PI : b.x;
        b.y = b.y < Mathf.PI ? b.y + Mathf.PI + Mathf.PI : b.y;

        var altDist = b - a;

        return new Vector2(Mathf.Abs(classicDist.x) < Mathf.Abs(altDist.x) ? classicDist.x : altDist.x, Mathf.Abs(classicDist.y) < Mathf.Abs(altDist.y) ? classicDist.y : altDist.y);
    }
}
