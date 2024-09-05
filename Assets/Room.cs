using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<Tile> roomTiles = new List<Tile>();
    [SerializeField] private int tileSize;
    [SerializeField] private GameObject tilePrefab;

    private Vector2Int roomDimensions = new Vector2Int(1, 1);

    private MapGenerator mapGenerator;
    private List<Vector2Int> expandableDirections = new List<Vector2Int>(){ Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};
    private bool canExpand = true;

    public void Remove()
    {
        foreach (Tile tile in roomTiles) tile.Remove();

        mapGenerator.RemoveRoom(this);
        Destroy(transform.gameObject);
    }

    public void AddTile(Tile tile)
    {
        roomTiles.Add(tile);
    }

    private Vector2Int GetMinSize()
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.GetPosition();
            if (position.x < minX) minX = position.x;
            if (position.y < minY) minY = position.y;
        }

        return new Vector2Int(minX, minY);
    }
    private Vector2Int GetMaxSize()
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.GetPosition();
            if (position.x > maxX) maxX = position.x;
            if (position.y > maxY) maxY = position.y;
        }

        return new Vector2Int(maxX, maxY);
    }

    public Vector2Int GetDimensions()
    {
        return roomDimensions;
    }

    public bool GetCanExpand()
    {
        return canExpand;
    }

    public void SetMapGenerator(MapGenerator mapGen)
    {
        mapGenerator = mapGen;
    }

    public void AttemptExpansion()
    {

        if (!canExpand) return;

        Vector2Int direction = expandableDirections[Random.Range(0, expandableDirections.Count)];
        List<Vector2Int> temporaryPositions = new List<Vector2Int>();

        Vector2Int max = GetMaxSize();
        Vector2Int min = GetMinSize();

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.GetPosition();

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
                if (mapGenerator.CheckNeighborVacancy(target, tile))
                {
                    if ((direction.x != 0 && roomDimensions.x + 1 <= roomDimensions.y * 2) ||
                    (direction.y != 0 && roomDimensions.y + 1 <= roomDimensions.x * 2))
                    {
                        temporaryPositions.Add(target);

                    }
                    else {return;}
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

        foreach (Vector2Int newPosition in temporaryPositions)
        {
            GameObject newTileModel = Instantiate(tilePrefab, new Vector3(newPosition.x * 5, 0, newPosition.y * 5), Quaternion.identity);
            Tile newTile = newTileModel.GetComponent<Tile>();
            newTile.SetType(0);
            newTile.SetPosition(newPosition);
            newTile.SetRoom(this);
            newTile.SetMapGenerator(mapGenerator);

            AddTile(newTile);
            mapGenerator.AddTile(newTile);
        }
    }

    public void OutlineHallways()
    {

        List<Vector2Int> doorwayCandidateList = new List<Vector2Int>();

        Vector2Int max = GetMaxSize();
        Vector2Int min = GetMinSize();

        foreach (Tile tile in roomTiles)
        {
            Vector2Int position = tile.GetPosition();
            List<Vector2Int> targetList = new List<Vector2Int>();

            // Vector Up (0, 1) = North
            // Vector Right (1, 0) = East
            // Vector Down (0, -1) = South
            // Vector Left (-1, 0) = West

            if (position.x == max.x)
            {
                Vector2Int target = position + new Vector2Int(1, 0);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); if(position.y != max.y && position.y != min.y) doorwayCandidateList.Add(position); }
            }

            if (position.x == min.x)
            {
                Vector2Int target = position + new Vector2Int(-1, 0);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); if (position.y != max.y && position.y != min.y) doorwayCandidateList.Add(position); };
            }

            if (position.y == max.y)
            {
                Vector2Int target = position + new Vector2Int(0, 1);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); if (position.x != max.x && position.x != min.x) doorwayCandidateList.Add(position); };
            }

            if (position.y == min.y)
            {
                Vector2Int target = position + new Vector2Int(0, -1);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) { targetList.Add(target); if (position.x != max.x && position.x != min.x) doorwayCandidateList.Add(position); };
            }

            if (position.x == max.x && position.y == max.y)
            {
                Vector2Int target = position + new Vector2Int(1, 1);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if (position.x == max.x && position.y == min.y)
            {
                Vector2Int target = position + new Vector2Int(1, -1);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if (position.x == min.x && position.y == max.y)
            {
                Vector2Int target = position + new Vector2Int(-1, 1);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if (position.x == min.x && position.y == min.y) 
            {
                Vector2Int target = position + new Vector2Int(-1, -1);
                if (mapGenerator.CheckPositionVacancy(target) && mapGenerator.CheckPositionInBounds(target)) targetList.Add(target);
            }

            if(targetList.Count > 0)
            {
                foreach(Vector2Int newPosition in targetList)
                {
                    GameObject newTileModel = Instantiate(tilePrefab, new Vector3(newPosition.x * 5, 0, newPosition.y * 5), Quaternion.identity);
                    Tile newTile = newTileModel.GetComponent<Tile>();
                    newTile.SetType(1);
                    newTile.SetPosition(newPosition);
                    newTile.SetMapGenerator(mapGenerator);
                    mapGenerator.AddTile(newTile);
                }
            }     
        }

        if(doorwayCandidateList.Count < 1) return;
        Tile newDoorwayTile = mapGenerator.GetTileFromPosition(doorwayCandidateList[Random.Range(0, doorwayCandidateList.Count)]);
        if (newDoorwayTile != null) newDoorwayTile.SetHasDoorway(true);

    }
}
