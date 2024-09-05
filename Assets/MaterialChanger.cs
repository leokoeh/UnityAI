using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private Tile Tile;
    [SerializeField] private Material MaterialRoom;
    [SerializeField] private Material MaterialHall;
    [SerializeField] private Renderer Renderer;

    void Start()
    {
        switch (Tile.GetRoomType())
        {
            case 0:
                Renderer.material = MaterialRoom;
                break;
            case 1:
                Renderer.material = MaterialHall;
                break;
        }
    }
}
