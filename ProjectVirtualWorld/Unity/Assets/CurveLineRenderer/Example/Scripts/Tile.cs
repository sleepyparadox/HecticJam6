using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
    public static int nextId = 0;

	// Position parameters
	public int x = 0;
	public int y = 0;
	public int z = 0;

	// Properties
	public int id = 0;
	public float tmpDistance = 0f;
	public int idx = 0;
	public int prevId = -1;
	public int prevCount = 0;

	void Awake () 
	{
		Vector3 position = transform.position;
		x = (int) position.x;
		y = Mathf.CeilToInt(position.y * 10f / 3f);
		z = (int) position.z;
	}

	void Update () 
	{
	
	}

    public class PositionComparer : IComparer<Tile>
    {
        public int Compare (Tile t1, Tile t2)
        {
            int zdiff = t1.z.CompareTo(t2.z);
            if (zdiff == 0)
                return t1.x.CompareTo(t2.x);
            else
                return zdiff;
        }
    }
}
