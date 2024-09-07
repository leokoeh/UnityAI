using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool CanExpand { get; set; } = true;
    public MapGenerator MapGenerator { get; set; }
    public List<Tile> RoomTiles {get; set;} = new();
    public Vector2Int RoomDimensions { get; set; } = new(1, 1);
    private readonly List<Vector2Int> expandableDirections = new() { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left};
    private static readonly int tileSize = 5;
    private static readonly int maxAspectRatio = 2;

    public void Remove()
    {
        foreach (Tile tile in RoomTiles) tile.Remove();
        MapGenerator.RoomList.Remove(this);

        Destroy(transform.gameObject);
    }

    private Vector2Int GetMinPosition()
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Tile tile in RoomTiles)
        {
            Vector2Int position = tile.GridPosition;
            if (position.x < minX) minX = position.x;
            if (position.y < minY) minY = position.y;
        }

        return new Vector2Int(minX, minY);
    }

    private Vector2Int GetMaxPosition()
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Tile tile in RoomTiles)
        {
            Vector2Int position = tile.GridPosition;
            if (position.x > maxX) maxX = position.x;
            if (position.y > maxY) maxY = position.y;
        }

        return new Vector2Int(maxX, maxY);
    }

    // Outputs a dictionary of all tiles on the room's edges and corners and their corresponding directional vectors
    public Dictionary<Tile, List<Vector2Int>> GetEdgeTiles() {

        Dictionary<Tile, List<Vector2Int>> tileDictionary = new();
        Vector2Int max = GetMaxPosition();
        Vector2Int min = GetMinPosition();

        // Adds value to key's list if it already exists, otherwise initializes new list
        void AddToDictionary(Tile key, Vector2Int value) {

            if(tileDictionary.ContainsKey(key)) tileDictionary[key].Add(value);
            else tileDictionary[key] = new List<Vector2Int>{value};
        
        }

        foreach (Tile tile in RoomTiles)
        {
            Vector2Int position = tile.GridPosition;

            // Find all edge tiles and add corresponding directional vector
            if (position.x == max.x) AddToDictionary(tile, Vector2Int.right);
            if (position.x == min.x) AddToDictionary(tile, Vector2Int.left);
            if (position.y == max.y) AddToDictionary(tile, Vector2Int.up);
            if (position.y == min.y) AddToDictionary(tile, Vector2Int.down);

            // For corner tiles, also add the sum of both corresponding directional vectors      
            if (position.x == max.x && position.y == max.y) AddToDictionary(tile, Vector2Int.right + Vector2Int.up);
            if (position.x == max.x && position.y == min.y) AddToDictionary(tile, Vector2Int.right + Vector2Int.down);
            if (position.x == min.x && position.y == max.y) AddToDictionary(tile, Vector2Int.left + Vector2Int.up);
            if (position.x == min.x && position.y == min.y) AddToDictionary(tile, Vector2Int.left + Vector2Int.down);
        }

        return tileDictionary;
    }

    // Attempts to expand the room on random axis
    public void AttemptExpansion()
    {
        if (expandableDirections.Count == 0) CanExpand = false;
        if (!CanExpand) return;

        Vector2Int expandingDirection = expandableDirections[Random.Range(0, expandableDirections.Count)];

        Dictionary<Tile, List<Vector2Int>> tileDictionary = GetEdgeTiles();
        List<Vector2Int> validExpansionPositions = new();

        foreach(KeyValuePair<Tile, List<Vector2Int>> obj in tileDictionary) 
        {
            if(!obj.Value.Contains(expandingDirection)) continue;

            Tile tile = obj.Key;
            Vector2Int checkPosition = tile.GridPosition + expandingDirection;

            if (!MapGenerator.CheckNeighborVacancy(checkPosition, tile))
            {
                expandableDirections.Remove(expandingDirection);
                return;
            }

            // When expanding on x axis, determine if an expansion would comply with room's max aspect ratio. If so, add check position to valid positions
            if (expandingDirection.x != 0 && RoomDimensions.x + 1 <= RoomDimensions.y * maxAspectRatio) validExpansionPositions.Add(checkPosition);

            // If max aspect ratio violation is found, determine if room can no longer expand on y-axis. If not, also remove current expanding direction from expandable directions
            else if (!expandableDirections.Contains(Vector2Int.up) && !expandableDirections.Contains(Vector2Int.down)) expandableDirections.Remove(expandingDirection);

            // When expanding on y axis, determine if an expansion would comply with room's max aspect ratio. If so, add check position to valid positions
            if (expandingDirection.y != 0 && RoomDimensions.y + 1 <= RoomDimensions.x * maxAspectRatio) validExpansionPositions.Add(checkPosition);

            // If max aspect ratio violation is found, determine if room can no longer expand on x-axis. If not, also remove current expanding direction from expandable directions
            else if (!expandableDirections.Contains(Vector2Int.right) && !expandableDirections.Contains(Vector2Int.left)) expandableDirections.Remove(expandingDirection);
        }

        if (validExpansionPositions.Count < 1) return;

        RoomDimensions += new Vector2Int(Mathf.Abs(expandingDirection.x), Mathf.Abs(expandingDirection.y));
        
        // Create a new room tile for each valid position
        foreach (Vector2Int newPosition in validExpansionPositions)
        {
            GameObject newTileModel = Instantiate(MapGenerator.TilePrefab, new Vector3(newPosition.x * tileSize, 0, newPosition.y * tileSize), Quaternion.identity);
            Tile newTile = newTileModel.GetComponent<Tile>();
            newTile.Type = 0;
            newTile.GridPosition = newPosition;
            newTile.Room = this;
            newTile.MapGenerator = MapGenerator;

            RoomTiles.Add(newTile);
            MapGenerator.TileList.Add(newTile);
        }
    }

    // Surrounds room with hallway tiles
    public void OutlineHallways()
    {
        Dictionary<Tile, List<Vector2Int>> tileDictionary = GetEdgeTiles();
        List<Vector2Int> validPositions = new();

        Vector2Int max = GetMaxPosition();
        Vector2Int min = GetMinPosition();

        foreach(KeyValuePair<Tile, List<Vector2Int>> kvp in tileDictionary) 
        {
            Tile tile = kvp.Key;
            Vector2Int position = tile.GridPosition;

            foreach(Vector2Int expansionVector in kvp.Value) 
            {
                Vector2Int checkPosition = position + expansionVector;
                if (MapGenerator.CheckPositionInBounds(checkPosition) && MapGenerator.CheckPositionVacancy(checkPosition)) validPositions.Add(checkPosition);
            }
        }

        if(validPositions.Count < 1) return;

        // Create a new hallway tile for each valid position
        foreach(Vector2Int newPosition in validPositions)
        {
            GameObject newTileModel = Instantiate(MapGenerator.TilePrefab, new Vector3(newPosition.x * tileSize, 0, newPosition.y * tileSize), Quaternion.identity);
            Tile newTile = newTileModel.GetComponent<Tile>();
            newTile.Type = 1;
            newTile.GridPosition = newPosition;
            newTile.MapGenerator = MapGenerator;
            newTile.Room = this;

            RoomTiles.Add(newTile);
            MapGenerator.TileList.Add(newTile);
        }

        List<Tile> doorwayCandidates = new();

        // Check each tile in the dictionary. If it has only one directional vector, add it to the candidates. This filters out corner tiles
        foreach(KeyValuePair<Tile, List<Vector2Int>> kvp in tileDictionary) if (kvp.Value.Count == 1) doorwayCandidates.Add(kvp.Key);

        if (doorwayCandidates.Count < 1) return;

        Tile newDoorwayTile = doorwayCandidates[Random.Range(0, doorwayCandidates.Count)];
        if (newDoorwayTile) newDoorwayTile.HasDoorway = true;
    }
}
