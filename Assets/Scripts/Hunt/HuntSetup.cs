using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntSetup : MonoBehaviour
{
    [Header("Agent Configuration")]
    public GameObject hunter;
    public GameObject prey;

    [Header("Respawn Configuration")]
    public bool randomizeHunterRespawn;
    public bool randomizePreyRespawn;

    [Header("Respawn Default Configuration")]
    public Vector3 respawnPositionHunter;
    public Vector3 respawnPositionPrey;

}
