using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int Type;
    public Vector2Int Position;
    public MapGenerator MapGenerator;
    public Room Room;
    public GameObject Prefab;
    public GameObject TileModel;

    public int tileSize = 5;

    public SmartWall NorthWall;
    public SmartWall EastWall;
    public SmartWall SouthWall;
    public SmartWall WestWall;

    public bool HasDoorway = false;

    public void Remove()
    {
        MapGenerator.RemoveTile(this);
        Destroy(transform.gameObject);
    }

    public void UpdateWalls()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int direction = new Vector2Int(i, j);
                Vector2Int checkPosition = Position + direction;
                Tile currentTile = MapGenerator.GetTileFromPosition(checkPosition);

                if (currentTile == null || currentTile.Type != Type)
                {
                    int mode = 0;

                    if (currentTile != null) 
                    {
                        if (HasDoorway || currentTile.HasDoorway) mode = 1;
                    }

                    if (direction == Vector2Int.up) NorthWall.SetMode(mode);
                    if (direction == Vector2Int.down) SouthWall.SetMode(mode);
                    if (direction == Vector2Int.right) EastWall.SetMode(mode);
                    if (direction == Vector2Int.left) WestWall.SetMode(mode);
                }
            }
        }
    }
}
