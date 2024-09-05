using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public int maxSizeX;
    public int maxSizeY;
    public int sizeDeviationMax;

    public GameObject TilePrefab;
    public GameObject RoomPrefab;

    public int roomCountMax;
    public int roomCountMin;

    public Vector2Int mapSize;
    private List<Tile> tileList;
    private List<Room> roomList;

    // Start is called before the first frame update
    void Start()
    {
        mapSize = new Vector2Int(maxSizeX + Random.Range(-sizeDeviationMax, sizeDeviationMax), maxSizeY + Random.Range(-sizeDeviationMax, sizeDeviationMax));

        tileList = new List<Tile>();
        roomList = new List<Room>();

        // Place Rooms
        for (int i = 0; i < Random.Range(roomCountMin, roomCountMax); i++)
        {
            Vector2Int spawnPosition = new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));

            // TODO - Assign this based on TileSize
            Vector2Int spawnWorldPosition = spawnPosition * 5;

            GameObject newRoomModel = Instantiate(RoomPrefab, new Vector3(spawnWorldPosition.x, 0, spawnWorldPosition.y), Quaternion.identity);
            Room newRoom = newRoomModel.GetComponent<Room>();
            newRoom.SetMapGenerator(this);

            GameObject newTileModel = Instantiate(TilePrefab, new Vector3(spawnWorldPosition.x, 0, spawnWorldPosition.y), Quaternion.identity);
            Tile newTile = newTileModel.GetComponent<Tile>();
            newTile.SetPosition(spawnPosition);
            newTile.SetRoom(newRoom);
            newTile.SetMapGenerator(this);

            roomList.Add(newRoom);
            tileList.Add(newTile);
            newRoom.AddTile(newTile);

            if (!CheckNeighborVacancy(spawnPosition, newTile)) newRoom.Remove();
        }

        List<Room> removingList = new List<Room>();

        for (int i = 0; i < 500; i++)
        {
            foreach (Room room in roomList) room.AttemptExpansion();

        }

        foreach (Room room in roomList)
        {
            Vector2Int roomDimensions = room.GetDimensions();
            room.OutlineHallways();
            if(roomDimensions.x * roomDimensions.y < 6) removingList.Add(room);
        }

        foreach (Room room in removingList) room.Remove();
        foreach(Tile tile in tileList) tile.UpdateWalls();
    }
    
    public void AddTile(Tile tile)
    {
        tileList.Add(tile);
    }

    public void RemoveTile(Tile tile)
    {
        tileList.Remove(tile);
    }

    public void AddRoom(Room room)
    {
        roomList.Add(room);
    }

    public void RemoveRoom(Room room)
    {
        roomList.Remove(room);
    }

    public bool CheckNeighborVacancy(Vector2Int position, Tile tile)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int checkPosition = position + new Vector2Int(i, j);
                Room room = tile.GetRoom();

                Tile neighborTile = GetTileFromPosition(checkPosition);

                // Stop checking if at least one position is out of bounds
                if (!CheckPositionInBounds(checkPosition)) return false;

                // If no neighbor is found continue checking
                if (neighborTile == null) continue;

                // Tile itself is ignored in the check
                if (neighborTile == tile) continue;

                // Filter out all tiles that are not part of the own room
                if (neighborTile.GetRoom() != room) return false;
            }
        }

        return true;
    }

    public bool CheckPositionInBounds(Vector2Int position)
    {
        return (
            position.x >= 0 &&
            position.x <= mapSize.x &&
            position.y >= 0 &&
            position.y <= mapSize.y);
    }

    public bool CheckPositionVacancy(Vector2Int position)
    {
        return (!tileList.Any(obj => obj.GetPosition() == position));
    }

    public Tile GetTileFromPosition(Vector2Int position)
    {
        Tile foundTile = tileList.FirstOrDefault(obj => obj.GetPosition() == position);

        if(foundTile != null) { return foundTile; } else { return null; }

    }
}
