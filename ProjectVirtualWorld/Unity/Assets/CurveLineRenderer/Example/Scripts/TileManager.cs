using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour, Movement.MovementCallback
{
	public int width = 32;
	public int height = 32;
	public float maxDistance = 1.0f;

	public static Color32 defaultColor = new Color32 (0, 204, 0, 0);
	public static Color32 focusedColor = new Color32 (0, 0, 153, 0);
	public static Color32 selectedColor = new Color32 (0, 153, 0, 0);

	public GameObject tilePrefab;
	private List<GameObject> tiles;
	private CurveLineRenderer splineRenderer;

	public int[] rates;
	public Color32[] colors;

	public List<Tile> tileList;
	public int[][] edgeMatrix;

	private GameObject focusedTile = null;
	private List<GameObject> focusedTiles = null;

	public Movement capsule;
	public List<Vector3> path;
	public bool handleCapsule = true;

	void Start ()
	{
		splineRenderer = GetComponent<CurveLineRenderer> ();
		rates = new int[6];
		rates [0] = -1;
		rates [1] = 1;
		rates [2] = 2;
		rates [3] = 3;
		rates [4] = 1;
		rates [5] = 1;

		tiles = new List<GameObject> ();
		path = new List<Vector3> ();

		//tileList = new List<Tile>(width * height);
		edgeMatrix = new int[width * height][];

		Tile[] tileArray = FindObjectsOfType<Tile> ();
        tileList.AddRange(tileArray);
		tileList.Sort(new Tile.PositionComparer());

		colors = new Color32[6];
		colors[0] = tileList[0].gameObject.GetComponent<Renderer>().material.color;

		int nextTileId = 0;
		for (int iii = 0; iii < tileList.Count; ++iii) 
		{
			Tile t = tileList[iii];
			t.id = nextTileId++;
			tiles.Add (t.gameObject);
			edgeMatrix[t.id] = new int[8];
		
			t.gameObject.GetComponent<Renderer>().material.color = t.gameObject.GetComponent<Renderer>().material.color;

			int i = t.id % width;
			int j = t.id / height;
			int k = 0;
			int y = t.y;

			for (int jj = j - 1; jj < j + 2; ++jj)
			{	for (int ii = i - 1; ii < i + 2; ++ii)
				{
					if (ii == i && jj == j)
						continue;
					
					if (ii > -1 && ii < width && 
					    jj > -1 && jj < height ) 
					{
                        int yy = (tileList[jj * width + ii]).y;

                        if (Mathf.Abs(yy - y) < 1.5f)
                        {
                            edgeMatrix[t.id][k] = jj * width + ii;
                        }
                        else
                            edgeMatrix[t.id][k] = -1;
                    }
					else
						edgeMatrix[t.id][k] = -1;
					
					k++;
				}
			}
		}

		for (int i = 0; i < tileList.Count; ++i)
		{
			Tile t = tileList[i];
			int y = t.y;

			if (y == 1) 
			{
				int y1 = tileList[edgeMatrix[t.id][1]].y;
				int y2 = tileList[edgeMatrix[t.id][6]].y;

				int v1 = 0;
				int v2 = 0;

				if (y1 == y2)
				{
					v1 = edgeMatrix[t.id][1];
					v2 = edgeMatrix[t.id][6];

					edgeMatrix[v1][6] = -1;
					edgeMatrix[v2][1] = -1;

					edgeMatrix[t.id][1] = -1;
					edgeMatrix[t.id][6] = -1;
				}
				else
				{
					v1 = edgeMatrix[t.id][3];
					v2 = edgeMatrix[t.id][4];

					edgeMatrix[v1][4] = -1;
					edgeMatrix[v2][3] = -1;

					edgeMatrix[t.id][3] = -1;
					edgeMatrix[t.id][4] = -1;
				}
			}
		}

		UpdateTilesInRange ();
	}

	void Update ()
	{
		RaycastHit raycastHit;
		if (Input.GetMouseButtonUp(0))
		{
			if (splineRenderer.enabled)
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
				{
					GameObject focusedObject = raycastHit.transform.gameObject;

					if (focusedTiles.Contains (focusedObject))
						MoveCapsule ();
				}
			}
		}

		if (handleCapsule) 
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
			{
				GameObject obj = raycastHit.transform.gameObject;
				Tile tile = obj.GetComponent<Tile> ();

				if (focusedTiles != null && obj != null && focusedTiles.Contains (obj))
				{
					int maxCount = tile.prevCount + 1;
					path.Clear ();
						Vector3 v = new Vector3 (obj.transform.position.x, 
						                         obj.transform.position.y + 0.1f, 
						                         obj.transform.position.z);
					path.Add (v);

					splineRenderer.enabled = true;
					splineRenderer.ClearVertices ();
					splineRenderer.AddVertex (v);
					
					for (int i = 1; i < maxCount; ++i)
					{
						GameObject prevObj = tiles[tile.prevId];
						Vector3 prev = new Vector3 (prevObj.transform.position.x, 
						                            prevObj.transform.position.y + 0.1f,
						                            prevObj.transform.position.z);

						float dy = Mathf.Abs(prev.y - v.y);
						if (dy < 0.4f && dy > 0.2f)
						{
							Vector3 v2 = new Vector3(0, 0, 0);
							v2.x = (prev.x + v.x) / 2;
							v2.y = (prev.y > 0.4f || prev.y < 0.2f) ? prev.y : v.y;
							v2.z = (prev.z + v.z) / 2;
							splineRenderer.AddVertex(v2);
							path.Add(v2);
						}
						splineRenderer.AddVertex (prev);
						path.Add (prev);

						obj = prevObj;
						v = prev;
						tile = obj.GetComponent<Tile> ();
					}
				}
			}
			else
			{
				splineRenderer.enabled = false;
			}
		}
	}

	public void OnStart ()
	{
		handleCapsule = false;
	}

	public void OnStop ()
	{
		handleCapsule = true;
		UpdateTilesInRange ();

        splineRenderer.ClearVertices();
	}

	public GameObject CalculateTile ()
	{
		int x = Mathf.CeilToInt (capsule.transform.position.x);
		int z = Mathf.CeilToInt (capsule.transform.position.z);
		return tiles [z * width + x];
	}

	public void MoveCapsule ()
	{
		if (path == null) 
			return;

		capsule.MoveWithPath (path, this);
	}

	public void UpdateTilesInRange ()
	{
		if (focusedTiles != null)
		{
			foreach (GameObject tile in focusedTiles)
				tile.GetComponent<Renderer>().material.color = colors[tile.GetComponent<Tile>().idx];
		}
		GameObject currentTile = CalculateTile ();
		focusedTiles = CalculateRange (currentTile);
		foreach (GameObject tile in focusedTiles)
		{
			Color32 color = colors[0];
			Color32 focusColor = new Color32 ((byte)(color.r >> 1), (byte)(color.g >> 1), 0, 0); 
			tile.GetComponent<Renderer>().material.color = focusColor;
		}
	}

	public List<GameObject> CalculateRange (GameObject selectedObject)
	{
		if (selectedObject == null)
			return null;

		List<GameObject> objectList = new List<GameObject> ();

		objectList.Add (selectedObject);
		Tile tile = selectedObject.GetComponent<Tile> ();
		tile.tmpDistance = 0.0f;
		tile.prevId = -1;
		tile.prevCount = 0;
		int position = 0;
		do
		{
			Tile curTileScript = objectList[position].GetComponent<Tile> ();
			for (int i = 0; i < 8; ++i)
			{
				int vertex = edgeMatrix[curTileScript.id][i];
				if (vertex > -1)
				{
					GameObject tileObject = tiles[vertex];
					Tile tileScript = tileObject.GetComponent<Tile> ();

					float rate = Mathf.Sqrt(Mathf.Pow(curTileScript.x - tileScript.x, 2) + Mathf.Pow(curTileScript.y - tileScript.y, 2));
					float distance = curTileScript.tmpDistance + rate;

					if (objectList.Contains (tileObject))
					{
						if (distance < tileScript.tmpDistance)
						{
							tileScript.prevId = curTileScript.id;
							tileScript.prevCount = curTileScript.prevCount + 1;
							tileScript.tmpDistance = distance;
						}
					}
					else
					{
						if (distance <= maxDistance)
						{
							objectList.Add (tileObject);
							tileScript.prevId = curTileScript.id;
							tileScript.prevCount = curTileScript.prevCount + 1;
							tileScript.tmpDistance = distance;
						}
					}
				}
			}

			position ++;
		}
		while (position < objectList.Count);

		objectList.RemoveAt (0);
		return objectList;
	}
}