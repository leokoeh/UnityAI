using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Tile Tile;
    public Material MaterialRoom;
    public Material MaterialHall;

    private Renderer Renderer;

    void Start()
    {
        Renderer = transform.GetComponent<Renderer>();
        int type = Tile.Type;

        switch (type)
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
