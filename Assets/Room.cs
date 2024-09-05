using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int roomDimensions = new Vector2Int(1, 1);
    public List<Tile> roomTiles = new List<Tile>();
    public MapGenerator MapGenerator;
    public GameObject floorTile;
    public int tileSize;
    public Color roomColor;
    public GameObject tilePrefab;

    private List<Vector2Int> expandableDirections = new List<Vector2Int>(){ new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
    public bool canExpand = true;

    void Start()
    {
        tilePrefab = MapGenerator.TilePrefab;

    }

    public void Remove()
    {
        foreach (Tile tile in roomTiles)
        {
            tile.Remove();
        }

        MapGenerator.RemoveRoom(this);
        Destroy(transform.gameObject);
    }

    public void AddTile(Tile tile)
    {
        roomTiles.Add(tile);
    }

    public Vector2Int GetMinSize()
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.Position;
            if (position.x < minX) minX = position.x;
            if (position.y < minY) minY = position.y;
        }

        return new Vector2Int(minX, minY);
    }

    public Vector2Int GetMaxSize()
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.Position;
            if (position.x > maxX) maxX = position.x;
            if (position.y > maxY) maxY = position.y;
        }

        return new Vector2Int(maxX, maxY);
    }


    public void AttemptExpansion()
    {

        if (!canExpand) return;
        Vector2Int direction = expandableDirections[Random.Range(0, expandableDirections.Count)];
        List<Vector2Int> temporaryTiles = new List<Vector2Int>();

        foreach (Tile tile in roomTiles)
        {
            //Debug.Log($"Checking Tile {tile} at {tile.Position}. Target: {tile.Position + direction}");
            Vector2Int position = tile.Position;
            Vector2Int max = GetMaxSize();
            Vector2Int min = GetMinSize();

            // Filter out all tiles that are on the edge
            if (
                position.x == max.x &&
                direction.x == 1 ||

                position.x == min.x &&
                direction.x == -1 ||

                position.y == max.y &&
                direction.y == 1 ||

                position.y == min.y &&
                direction.y == -1)
            {

                Vector2Int target = position + direction;
                if (MapGenerator.CheckNeighborVacancy(target, tile))
                {
                    if ((direction.x != 0 && roomDimensions.x + 1 <= roomDimensions.y * 2) ||
                    (direction.y != 0 && roomDimensions.y + 1 <= roomDimensions.x * 2))
                    {
                        temporaryTiles.Add(target);

                    }
                    else {return; }
                }
                else
                {
                    expandableDirections.Remove(direction);
                    if (expandableDirections.Count == 0) canExpand = false;
                    return;
                }
            }
        }

        roomDimensions += new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

        foreach (Vector2Int newPosition in temporaryTiles)
        {
            GameObject newTileModel = Instantiate(tilePrefab, new Vector3(newPosition.x * 5, 0, newPosition.y * 5), Quaternion.identity);
            Tile newTile = newTileModel.GetComponent<Tile>();
            newTile.Type = 0;
            newTile.Position = newPosition;
            newTile.Room = this;
            newTile.MapGenerator = MapGenerator;

            AddTile(newTile);
            MapGenerator.AddTile(newTile);
        }
    }

    public void OutlineHallways()
    {
        List<Vector2Int> doorwayCandidateList = new List<Vector2Int>();

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.Position;
            Vector2Int max = GetMaxSize();
            Vector2Int min = GetMinSize();
            List<Vector2Int> targetList = new List<Vector2Int>();


            // Vector Up (0, 1) = North
            // Vector Right (1, 0) = East
            // Vector Down (0, -1) = South
            // Vector Left (-1, 0) = West

            if (position.x == max.x)
            {
                Vector2Int target = position + new Vector2Int(1, 0);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); doorwayCandidateList.Add(position); }
            }

            if (position.x == min.x)
            {
                Vector2Int target = position + new Vector2Int(-1, 0);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); doorwayCandidateList.Add(position); };
            }

            if (position.y == max.y)
            {
                Vector2Int target = position + new Vector2Int(0, 1);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); doorwayCandidateList.Add(position); };
            }

            if (position.y == min.y)
            {
                Vector2Int target = position + new Vector2Int(0, -1);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); doorwayCandidateList.Add(position); };
            }

            if (position.x == max.x && position.y == max.y)
            {
                Vector2Int target = position + new Vector2Int(1, 1);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if (position.x == max.x && position.y == min.y)
            {
                Vector2Int target = position + new Vector2Int(1, -1);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if (position.x == min.x && position.y == max.y)
            {
                Vector2Int target = position + new Vector2Int(-1, 1);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if (position.x == min.x && position.y == min.y) 
            {
                Vector2Int target = position + new Vector2Int(-1, -1);
                if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if(targetList.Count > 0)
            {
                foreach(Vector2Int newPosition in targetList)
                {
                    GameObject newTileModel = Instantiate(tilePrefab, new Vector3(newPosition.x * 5, 0, newPosition.y * 5), Quaternion.identity);
                    Tile newTile = newTileModel.GetComponent<Tile>();
                    newTile.Type = 1;
                    newTile.Position = newPosition;
                    newTile.MapGenerator = MapGenerator;
                    MapGenerator.AddTile(newTile);
                }
            }     
        }

        Tile newDoorwayTile = MapGenerator.GetTileFromPosition(doorwayCandidateList[Random.Range(0, doorwayCandidateList.Count)]);
        if (newDoorwayTile != null) { newDoorwayTile.HasDoorway = true; }

    }
}
