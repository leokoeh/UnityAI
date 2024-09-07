using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Smart Wall Configuration")]
    [SerializeField] private SmartWall northWall;
    [SerializeField] private SmartWall eastWall;
    [SerializeField] private SmartWall southWall;
    [SerializeField] private SmartWall westWall;

    public int Type { get; set; } = 0;
    // 0 - Room tile
    // 1 - Hallway tile   
     
    public bool HasDoorway { get; set; } = false;
    public Room Room { get; set; }
    public Vector2Int GridPosition { get; set; }
    public MapGenerator MapGenerator { get; set; }

    public void UpdateWalls()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int scanDirection = new(i, j);
                Tile scannedTile = MapGenerator.GetTileFromPosition(GridPosition + scanDirection);

                int mode = 0;


                if (scannedTile)
                {
                    if (scannedTile.Type == Type) continue;
                    if (HasDoorway  || scannedTile.HasDoorway) mode = 1;
                }

                if(scanDirection == Vector2Int.up) northWall.SetMode(mode);
                if(scanDirection == Vector2Int.right) eastWall.SetMode(mode);
                if(scanDirection == Vector2Int.down) southWall.SetMode(mode);
                if(scanDirection == Vector2Int.left) westWall.SetMode(mode);
            }
        } 
    }
    public void Remove()
    {
        MapGenerator.TileList.Remove(this);
        Destroy(transform.gameObject);
    }
}
