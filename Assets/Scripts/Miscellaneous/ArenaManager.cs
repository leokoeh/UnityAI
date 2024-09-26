using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] public int EpisodeCounter;
    [SerializeField] private TextMeshPro episodeText;
    [SerializeField] [Range(1, 3)] private int currentStage;

    [SerializeField] public List<Transform> respawnPositions;
    [SerializeField] private List<GameObject> stage1Objects;
    [SerializeField] private List<GameObject> stage2Objects;
    [SerializeField] private List<GameObject> stage3Objects;
    void Start() 
    {
        // Activate game objects depending on the current stage of the arena

        if (currentStage >= 1 && stage1Objects.Count > 0) 
        {
            foreach(GameObject gameObject in stage1Objects) 
            {
                gameObject.SetActive(true);
            }
        }

        if (currentStage >= 2 && stage2Objects.Count > 0) 
        {
            foreach(GameObject gameObject in stage2Objects) 
            {
                gameObject.SetActive(true);
            }
        }

        if (currentStage >= 3 && stage3Objects.Count > 0) 
        {
            foreach(GameObject gameObject in stage3Objects) 
            {
                gameObject.SetActive(true);
            }
        }
    }

    void Update() 
    {
        // Configure the text on floor to display current episode count
        episodeText.text = $"Episode {EpisodeCounter}";
    }

    public Vector3 GetRandomRespawnPosition(float respawnY)
    {
        // Choose random respawn position from all respawn positions
        Transform respawnPosition = respawnPositions[Random.Range(0, respawnPositions.Count)];

        // Return a new vector containing the respawn position's local position and the input y-position
        return new Vector3(respawnPosition.localPosition.x, respawnY, respawnPosition.localPosition.z);
    }
}
