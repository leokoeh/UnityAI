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

    public GameObject roomTile;
    public GameObject hallTile;
    public int tileSize;

    public int roomCountMax;
    public int roomCountMin;

    public Vector2Int mapSize;
    private List<Tile> tileList;
    private List<Room> roomList;

    public class Tile
    {
        public int Type;
        public Vector2Int Position;
        public MapGenerator MapGenerator;
        public Room Room;
        public GameObject Prefab;
        public GameObject TileModel;

        public GameObject NorthWall;
        public GameObject EastWall;
        public GameObject SouthWall;
        public GameObject WestWall;

        public bool HasDoorway = false;

        public Tile(int type, Vector2Int position, Room room, MapGenerator mapGenerator)
        {
            Type = type;
            Position = position;
            Room = room;
            MapGenerator = mapGenerator;

            switch (type)
            {
                case 0:
                    Prefab = mapGenerator.roomTile; break;
                case 1:
                    Prefab = mapGenerator.hallTile; break;
            }

            PlaceTile(position);

        }

        public void Remove()
        {
            MapGenerator.RemoveTile(this);
            Destroy(TileModel);
        }

        public void PlaceTile(Vector2Int position)
        {
            MapGenerator.AddTile(this);
            TileModel = Instantiate(Prefab, new Vector3(position.x, 0, position.y) * MapGenerator.tileSize, Quaternion.identity);

            if (Room != null) Room.AddTile(this);
            //if (Room != null) TileModel.GetComponent<Renderer>().material.color = Room.roomColor;

            NorthWall = TileModel.transform.Find("WallNorth").gameObject;
            EastWall = TileModel.transform.Find("WallEast").gameObject;
            SouthWall = TileModel.transform.Find("WallSouth").gameObject;
            WestWall = TileModel.transform.Find("WallWest").gameObject;
        }

        public void UpdateWalls()
        {
            for(int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    Vector2Int direction = new Vector2Int(i, j);
                    Vector2Int checkPosition = Position + direction;
                    Tile currentTile = MapGenerator.GetTileFromPosition(checkPosition);

                    if (currentTile == null) continue;

                    if (currentTile.Type == Type)
                    {
                        if(direction == Vector2Int.up) Destroy(NorthWall);
                        if (direction == Vector2Int.down) Destroy(SouthWall);
                        if (direction == Vector2Int.right) Destroy(EastWall);
                        if (direction == Vector2Int.left) Destroy(WestWall);
                    } 
                }
            }
        }

    }

    public class Room
    {
        public Vector2Int roomDimensions = new Vector2Int(1, 1);
        public List<Tile> roomTiles;
        public MapGenerator MapGenerator;
        public GameObject floorTile;
        public int tileSize;
        public Color roomColor;

        public List<Vector2Int> expandableDirections = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)};
        public bool canExpand = true;

        public Room(MapGenerator mapGenerator)
        {
            roomTiles = new List<Tile>();
            MapGenerator = mapGenerator;
            
            MapGenerator.AddRoom(this);

            roomColor = new Color(Random.value, Random.value, Random.value);
        }

        public void Remove()
        {
            foreach(Tile tile in roomTiles)
            {
                tile.Remove();
            }

            MapGenerator.RemoveRoom(this);
        }

        public void AddTile(Tile tile)
        {
            roomTiles.Add(tile);
        }

        public Vector2Int GetMinSize()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;

            foreach(Tile tile in roomTiles)
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
            if(!canExpand) return;

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
                    if (MapGenerator.CheckNeighborVacancy(target, this, tile))
                    {
                        if((direction.x != 0 && roomDimensions.x + 1 <= roomDimensions.y * 2) ||
                        (direction.y != 0 && roomDimensions.y + 1 <= roomDimensions.x * 2))
                        {
                            temporaryTiles.Add(target);

                        } else { return; }
                    }
                    else
                    {
                        expandableDirections.Remove(direction);
                        if(expandableDirections.Count == 0) canExpand = false;
                        return;
                    }
                }
            }

            roomDimensions += new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

            foreach (Vector2Int newPosition in temporaryTiles)
            {                    
                //Debug.Log("Spawning!");
                Tile newTile = new Tile(0, newPosition, this, MapGenerator);
            }
        }

        public void OutlineHallways()
        {
            foreach(Tile tile in roomTiles)
            {
                Vector2Int position = tile.Position;
                Vector2Int max = GetMaxSize();
                Vector2Int min = GetMinSize();

                // Vector Up (0, 1) = North
                // Vector Right (1, 0) = East
                // Vector Down (0, -1) = South
                // Vector Left (-1, 0) = West

                if (position.x == max.x)
                {
                    Vector2Int target = position + new Vector2Int(1, 0);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }
                }

                if(position.x == min.x)
                {
                    Vector2Int target = position + new Vector2Int(-1, 0);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }
                }

                if (position.y == max.y)
                {
                    Vector2Int target = position + new Vector2Int(0, 1);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }
                }

                if (position.y == min.y)
                {
                    Vector2Int target = position + new Vector2Int(0, -1);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }

                }

                if (position.x == max.x && position.y == max.y)
                {
                    Vector2Int target = position + new Vector2Int(1, 1);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }

                }

                if (position.x == max.x && position.y == min.y)
                {
                    Vector2Int target = position + new Vector2Int(1, -1);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }

                }

                if (position.x == min.x && position.y == max.y)
                {
                    Vector2Int target = position + new Vector2Int(-1, 1);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }

                }

                if (position.x == min.x && position.y == min.y)
                {
                    Vector2Int target = position + new Vector2Int(-1, -1);
                    if (MapGenerator.CheckPositionVacancy(target) && MapGenerator.CheckPositionInBounds(target))
                    {
                        Tile newHallway = new Tile(1, target, null, MapGenerator);
                    }

                }
            }
        }
    }

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

            Room newRoom = new Room(this);
            Tile newTile = new Tile(0, spawnPosition, newRoom, this);

            if (!CheckNeighborVacancy(spawnPosition, newRoom, newTile))
            {
                newRoom.Remove();
            }
        }

        IEnumerator Expansion() {

            for (int i = 0; i < 100; i++)
            {
                foreach (Room room in roomList)
                {
                    room.AttemptExpansion(); 

                }
            }

            yield return new WaitForSeconds(0f);

            List<Room> removingList = new List<Room>();

            foreach (Room room in roomList)
            {
                room.OutlineHallways();
                if(room.roomDimensions.x * room.roomDimensions.y < 9)
                {
                    removingList.Add(room);
                }
            }

            foreach (Room room in removingList)
            {
                room.Remove();
            }

            foreach(Tile tile in tileList)
            {
                tile.UpdateWalls();
            }

        }
    
        StartCoroutine(Expansion());
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

    public bool CheckNeighborVacancy(Vector2Int position, Room room, Tile tile)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int checkPosition = position + new Vector2Int(i, j);

                if(!CheckPositionInBounds(checkPosition))
                {
                    // Filter out all positions out of bounds
                    return false;
                }

                Tile neighborTile = GetTileFromPosition(checkPosition);

                if (neighborTile != null && neighborTile != tile)
                {

                    if (neighborTile.Room != room)
                    {
                        // Filter out all tiles that are not part of the own room
                        return false;
                    }
                }
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
        return (!tileList.Any(obj => obj.Position == position));
    }

    public Tile GetTileFromPosition(Vector2Int position)
    {
        Tile foundTile = tileList.FirstOrDefault(item => item.Position == position);

        if(foundTile != null) { return foundTile; } else { return null; }

    }
}
