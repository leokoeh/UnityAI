using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Tile Configuration")]
    public GameObject tile;
    public int tileSize;
    public int tileHeight;

    [Header("Grid Configuration")]
    public int gridSize = 5;

    [Header("Focus Configuration")]
    public Camera mainCamera;
    public bool focusCamera = true;
    public Vector3 isolationVector = new Vector3(0, 100, 0);

    void Start()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {

                Instantiate(tile, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);

            }

        }

        Instantiate(tile, new Vector3(0, 100, 0), Quaternion.identity);


        if (focusCamera)
        {
            mainCamera.transform.position = new Vector3(isolationVector.x, isolationVector.y + tileHeight / 2, isolationVector.z - tileSize / 2);

        }

    }

}
