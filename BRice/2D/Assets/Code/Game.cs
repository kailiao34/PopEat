using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Camera gameCamera;
    public GameTile tilePrefab;
    //public int gridWidth = 10;
    //public int gridHeight = 10;
    public int gridRadius = 4;
    private GameTile[,] tileGrid;
    private List<Vector2> swipePositions = new List<Vector2>();

    private void Start()
    {
        int size = gridRadius * 2;
        tileGrid = new GameTile[size, size];

        Plane[] planes = GameTile.CalculateCirclePlanes(gridRadius);

        Vector2 centerTilePos = GameTile.CalculatePosition(gridRadius, gridRadius);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pos = GameTile.CalculatePosition(x, y);

                pos -= centerTilePos;
                bool passed = true;
                for (int i = 0; i < planes.Length; i++)
                {
                    Plane plane = planes[i];
                    if (!plane.GetSide((Vector3)pos-plane.normal*Vector3.kEpsilon))
                    {
                        passed = false;
                        break;
                    }
                }

                if (!passed)
                {
                    continue;
                }

                GameTile tile = Instantiate(tilePrefab);
                tile.gameObject.name = "Tile_" + x + "-" + y;
                tile.x = x;
                tile.y = y;
                tileGrid[x, y] = tile;
            }
        }
    }

    private void Update()
    {
        Touch[] touches = Input.touches;
        if (touches.Length > 1)
        {
            //Do zooming.
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 pos = GetPositionUnderCursor();
                if (!swipePositions.Contains(pos))
                {
                    GameTile tile = GetTileAtPosition(pos+new Vector2(0.5f, 0.5f));
                    if (tile)
                    {
                        DestroyImmediate(tile.gameObject);
                    }
                    swipePositions.Add(pos);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                ReleaseSwipe();
            }
        }
    }

    private void ReleaseSwipe()
    {
        swipePositions.Clear();
    }

    private Vector2 GetPositionUnderCursor()
    {
        Vector2 cursorPos = Vector2.zero;
        if (gameCamera.orthographic)
        {
            cursorPos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.forward, Vector3.zero);

            float dist;
            if (plane.Raycast(ray, out dist))
            {
                cursorPos = ray.origin + ray.direction * dist;
                Debug.DrawLine(ray.origin, cursorPos);
            }
            else
            {
                throw new System.Exception("Shouldnt happen.");
            }
        }

        int row = Mathf.FloorToInt(cursorPos.y);
        if (Mathf.Repeat(row, 2) == 1)
        {
            cursorPos.x += 0.5f;
        }
        cursorPos.x = Mathf.Floor(cursorPos.x);
        cursorPos.y = Mathf.Floor(cursorPos.y);
        if (Mathf.Repeat(row, 2) == 1)
        {
            cursorPos.x -= 0.5f;
        }

        return cursorPos;
    }

    private GameTile GetTileUnderCursor()
    {
        return GetTileAtPosition(GetPositionUnderCursor()+new Vector2(0.5f, 0.5f));
    }

    private GameTile GetTileAtPosition(Vector2 position)
    {
        GameTile tile = null;
        Collider2D collider = Physics2D.OverlapPoint(position);
        if (collider)
        {
            tile = collider.GetComponent<GameTile>();
        }
        return tile;
    }
}
