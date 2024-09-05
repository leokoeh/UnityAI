using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int type = 0;
    private Vector2Int position;
    private Room room;
    private bool hasDoorway = false;

    private Dictionary<Vector2Int, Action> dictionary;

    private MapGenerator mapGenerator;

    [SerializeField] private int tileSize;

    [SerializeField] private SmartWall northWall;
    [SerializeField] private SmartWall eastWall;
    [SerializeField] private SmartWall southWall;
    [SerializeField] private SmartWall westWall;

    public void UpdateWalls()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int direction = new Vector2Int(i, j);
                Tile currentTile = mapGenerator.GetTileFromPosition(position + direction);
                int mode = 0;

                if (currentTile)
                {
                    if (currentTile.GetRoomType() == type) continue;
                    if (hasDoorway || currentTile.GetHasDoorway()) mode = 1;
                }

                dictionary = new Dictionary<Vector2Int, Action>()
                {
                    {Vector2Int.up, () => northWall.SetMode(mode)},
                    {Vector2Int.right, () => eastWall.SetMode(mode)},
                    {Vector2Int.down, () => southWall.SetMode(mode)},
                    {Vector2Int.left, () => westWall.SetMode(mode)},
                };

                if (dictionary.TryGetValue(direction, out Action action)) action();
            }
        } 
    }

    public void SetMapGenerator(MapGenerator mapGen)
    {
        mapGenerator = mapGen;
    }

    public bool GetHasDoorway()
    {
        return hasDoorway;
    }
    public void SetHasDoorway(bool value)
    {
        hasDoorway = value;
    }

    public int GetRoomType()
    {
        return type;
    }
    public void SetType(int vaule)
    {
        type = vaule;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }
    public void SetPosition(Vector2Int value)
    {
        position = value;
    }

    public Room GetRoom() 
    { 
        return room; 
    }
    public void SetRoom(Room vaule)
    {
        room = vaule;
    }

    public int GetTileSize()
    {
        return tileSize;
    }

    public void Remove()
    {
        mapGenerator.RemoveTile(this);
        Destroy(transform.gameObject);
    }
}
