using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size Configuration")]
    [SerializeField] [Range(1, 100)] private int sizeMax;
    [SerializeField] [Range(1, 100)] private int sizeMin;

    [Header("Room Count Configuration")]
    [SerializeField] [Range(1, 50)] private int roomCountMax;
    [SerializeField] [Range(1, 50)] private int roomCountMin;

    [Header("Prefab Configuration")]
    [SerializeField] public GameObject TilePrefab;
    [SerializeField] private GameObject roomPrefab;

    public Vector2Int MapSize { get; set; }
    public List<Tile> TileList { get; set; }
    public List<Room> RoomList { get; set; }
    private static readonly int tileSize = 5;

    void Start()
    {
        GenerateMap();
        if (RoomList.Count < roomCountMin) 
        {
            // If not enough rooms spawned, delete the map and log an error 
            List<Room> removing = new();
            foreach(Room room in RoomList) removing.Add(room);
            foreach(Room room in removing) room.Remove();
            Debug.LogError($"Unable to spawn map with at least {roomCountMin} rooms!");
        }
    }

    // Generates a map by placing rooms, expanding them and outlining them with hallways
    void GenerateMap() 
    {
        MapSize = new Vector2Int(Random.Range(sizeMin, sizeMax), Random.Range(sizeMin, sizeMax));

        TileList = new List<Tile>();
        RoomList = new List<Room>();

        // Place initial rooms
        for (int i = 0; i < Random.Range(roomCountMin, roomCountMax); i++)
        {
            Vector2Int spawnPosition = new(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y));

            GameObject newRoomModel = Instantiate(roomPrefab, new Vector3(spawnPosition.x * tileSize, 0, spawnPosition.y * tileSize), Quaternion.identity);
            Room newRoom = newRoomModel.GetComponent<Room>();
            newRoom.MapGenerator = this;

            GameObject newTileModel = Instantiate(TilePrefab, new Vector3(spawnPosition.x * tileSize, 0, spawnPosition.y * tileSize), Quaternion.identity);
            Tile newTile = newTileModel.GetComponent<Tile>();
            newTile.GridPosition = spawnPosition;
            newTile.Room = newRoom;
            newTile.MapGenerator = this;

            RoomList.Add(newRoom);
            TileList.Add(newTile);
            newRoom.RoomTiles.Add(newTile);

            if (!CheckNeighborVacancy(spawnPosition, newTile)) newRoom.Remove();
        }

        List<Room> removingList = new();

        // Try to expand all rooms until none can be expanded anymore
        while(true)
        {
            bool foundExpandable = false;
            foreach (Room room in RoomList) {room.AttemptExpansion(); if(room.CanExpand) foundExpandable = true;}
            if(!foundExpandable) break;
        }

        // Outline every room, remove rooms that are too small
        foreach (Room room in RoomList)
        {
            room.OutlineHallways();
            if(room.RoomDimensions.x <= 2 && room.RoomDimensions.y <= 2) removingList.Add(room);
        }

        foreach (Room room in removingList) room.Remove();
        foreach(Tile tile in TileList) tile.UpdateWalls();
    }

    // Check if neighboring tiles are vacant or members of the same room
    public bool CheckNeighborVacancy(Vector2Int position, Tile tile)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int checkPosition = position + new Vector2Int(i, j);
                Tile neighborTile = GetTileFromPosition(checkPosition);

                if (!CheckPositionInBounds(checkPosition)) return false;
                if (neighborTile == null || neighborTile == tile) continue;

                if (neighborTile.Room != tile.Room) return false;
            }
        }

        return true;
    }

    public bool CheckPositionInBounds(Vector2Int position)
    {
        return (
            position.x >= 0 &&
            position.x <= MapSize.x &&
            position.y >= 0 &&
            position.y <= MapSize.y);
    }

    public bool CheckPositionVacancy(Vector2Int position)
    {
        return !TileList.Any(obj => obj.GridPosition == position);
    }

    public Tile GetTileFromPosition(Vector2Int position)
    {
        Tile foundTile = TileList.FirstOrDefault(obj => obj.GridPosition == position);

        if(foundTile != null) { return foundTile; } else { return null; }

    }
}
