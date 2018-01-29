using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public const float tileSize = 0.5f;
    static public float tileWidth { get; private set; }
    static public float tileHeight { get; private set; }
    static public float horizontalSpacing { get; private set; }
    static public float verticalSpacing { get; private set; }

    public int x;
    public int y;
   
    static GameTile()
    {
        tileHeight = tileSize * 2;
        tileWidth = Mathf.Sqrt(3) / 2 * tileHeight;
        horizontalSpacing = tileWidth;
        verticalSpacing = tileHeight * (3.0f / 4);
    }

    private void Start()
    {
        transform.position = CalculatePosition();

        PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();
        Vector2[] path = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            Vector2 point = CalculateHexCorner(1.0f/transform.localScale.x * tileSize, i);
            path[i] = point;
        }
        collider.SetPath(0, path);
    }

    Vector2 CalculateHexCorner(float size, int index)
    {
        var angle = (60.0f*index+30.0f)*Mathf.Deg2Rad;
        return new Vector2(size * Mathf.Cos(angle), size * Mathf.Sin(angle));
    }

    public Vector2 CalculatePosition()
    {
        return CalculatePosition(x, y);
    }

    public static Vector2 CalculatePosition(int x, int y)
    {
        Vector2 position = new Vector2(horizontalSpacing * x, verticalSpacing * y);
        if (y % 2 == 1)
        {
            position.x += horizontalSpacing / 2;
        }

        return position;
    }

    public static Plane[] CalculateCirclePlanes(int radius)
    {
        float dist = verticalSpacing * radius;
        return new Plane[]
        {
            //North
            new Plane(Quaternion.AngleAxis(0, Vector3.forward)*Vector3.up, dist),
            //North east
            new Plane(Quaternion.AngleAxis(60, Vector3.forward)*Vector3.up, dist),
            //South east
            new Plane(Quaternion.AngleAxis(120, Vector3.forward)*Vector3.up, dist),
            //South
            new Plane(Quaternion.AngleAxis(180, Vector3.forward)*Vector3.up, dist),
            //South west
            new Plane(Quaternion.AngleAxis(180+60, Vector3.forward)*Vector3.up, dist),
            //South west
            new Plane(Quaternion.AngleAxis(180+120, Vector3.forward)*Vector3.up, dist),
        };
    }
   
}
