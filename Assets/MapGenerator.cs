using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int maxSizeX;
    public int maxSizeY;
    public int sizeDeviationMax;
    public GameObject floorTile;
    public int tileSize;

    public int roomCountMax;
    public int roomCountMin;

    public Vector2Int mapSize;
    private List<Vector2Int> occupiedPositions;
    private List<Room> roomList;

    public class Room
    {
        public Vector2Int roomDimensions { get; set; }
        public List<Vector2Int> roomTiles { get; set; }
        public MapGenerator MapGenerator { get; set; }
        public GameObject floorTile { get; set; }
        public int tileSize { get; set; }
        
        public Room(Vector2Int position, MapGenerator mapGenerator)
        {

            roomDimensions = new Vector2Int(1, 1);
            roomTiles = new List<Vector2Int>();
            MapGenerator = mapGenerator;
            floorTile = mapGenerator.floorTile;
            tileSize = mapGenerator.tileSize;
            mapGenerator.roomList.Add(this);

            PlaceTile(position);

        }

        public void PlaceTile(Vector2Int position)
        {
            if (MapGenerator.CheckTileVacancy(position))
            {
                MapGenerator.AddTile(position);
                roomTiles.Add(position);
                Instantiate(floorTile, new Vector3(position.x, 0, position.y) * tileSize, Quaternion.identity);
            }
        }

        public Vector2Int GetMinSize()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;

            foreach(Vector2Int position in roomTiles)
            {
                if (position.x < minX) minX = position.x;
                if (position.y < minY) minY = position.y;
            }

            return new Vector2Int(minX, minY);
        }

        public Vector2Int GetMaxSize()
        {
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (Vector2Int position in roomTiles)
            {
                if (position.x > maxX) maxX = position.x;
                if (position.y > maxY) maxY = position.y;
            }

            return new Vector2Int(maxX, maxY);
        }

        public void AttemptExpansion(Vector2Int direction)
        {
            List<Vector2Int> temporaryTiles = new List<Vector2Int>();
            bool granted = true;

            foreach (Vector2Int position in roomTiles)
            {
                Vector2Int max = GetMaxSize();
                Vector2Int min = GetMinSize();

                int directionX = direction.x;
                int directionY = direction.y;

                if (position.x == max.x && direction.x == 1 || position.x == min.x && direction.x == -1 || position.y == max.y && direction.y == 1 || position.y == min.y && direction.y == -1)
                {
                    // Expand
                    if (MapGenerator.CheckTileVacancy(position + direction) && (MapGenerator.CheckTileVacancy(position + direction * 2))) {

                        if (directionX != 0)
                        {
                            if(MapGenerator.CheckTileVacancy(position + direction + new Vector2Int(0,1)) && MapGenerator.CheckTileVacancy(position + direction + new Vector2Int(0, -1)) && roomDimensions.x + 1 <= roomDimensions.y * 2)
                            {
                                temporaryTiles.Add(position + direction);
                                //Debug.Log("Expansion granted.");
                            }
                            else { granted = false; break; }
                        }

                        if (directionY != 0)
                        {
                            if (MapGenerator.CheckTileVacancy(position + direction + new Vector2Int(1, 0)) && MapGenerator.CheckTileVacancy(position + direction + new Vector2Int(-1, 0)) && roomDimensions.y + 1 <= roomDimensions.x * 2)
                            {
                                temporaryTiles.Add(position + direction);
                                //Debug.Log("Expansion granted.");
                            }
                            else { granted = false; break; }
                        }

                    }
                    else { granted = false; break; }
                }
            }
            if(granted)
            {
                foreach (Vector2Int newPosition in temporaryTiles)
                {
                    roomDimensions += new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
                    PlaceTile(newPosition);
                }
            } else
            {
                //Debug.Log("Expansion denied.");
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        mapSize = new Vector2Int(maxSizeX + Random.Range(-sizeDeviationMax, sizeDeviationMax), maxSizeY + Random.Range(-sizeDeviationMax, sizeDeviationMax));

        occupiedPositions = new List<Vector2Int>();
        roomList = new List<Room>();

        // Place Rooms
        for (int i = 0; i < Random.Range(roomCountMin, roomCountMax); i++)
        {
            Vector2Int spawnPosition = new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));

            bool allowPlacement = true;

            for(int a=-1; a < 2; a++)
            {
                for(int b=-1; b < 2; b++)
                {
                    if(!CheckTileVacancy(spawnPosition + new Vector2Int(a,b)))
                    {
                        allowPlacement = false;
                        break;
                    }
                }
            }

            if(allowPlacement)
            {
                Room newRoom = new Room(spawnPosition, this);
                Instantiate(floorTile, new Vector3(spawnPosition.x, 10, spawnPosition.y) * tileSize, Quaternion.identity);
            }

        }

        bool roomsCanGrow = true;

        for (int i = 0; i < 500 ; i++)
        {
            foreach (Room room in roomList)
            {
                int random = Random.Range(0, 3);
                switch(random)
                {
                    case 0:
                        room.AttemptExpansion(new Vector2Int(1, 0));
                        break;
                    case 1:
                        room.AttemptExpansion(new Vector2Int(0, 1));
                        break;
                    case 2:
                        room.AttemptExpansion(new Vector2Int(-1, 0));
                        break;
                    case 3:
                        room.AttemptExpansion(new Vector2Int(0, -1));
                        break;
                }

            }

        }

    }

    public void AddTile(Vector2Int position)
    {
        occupiedPositions.Add(position);
    }

    public bool CheckTileVacancy(Vector2Int position)
    {
        return (!occupiedPositions.Contains(position) && position.x >= 0 && position.x <= mapSize.x && position.y >= 0 && position.y <= mapSize.y);
    }
}
