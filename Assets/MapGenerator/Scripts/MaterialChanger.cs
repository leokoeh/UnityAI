using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [Header("Tile Configuration")]
    [SerializeField] private Tile tile;


    [Header("Material Configuration")]
    [SerializeField] private Material materialRoom;    
    [SerializeField] private Material materialHall;
    void Start()
    {
        switch (tile.Type)
        {
            case 0:
                GetComponent<Renderer>().material = materialRoom;
                break;

            case 1:
                GetComponent<Renderer>().material = materialHall;
                break;
        }
    }
}
