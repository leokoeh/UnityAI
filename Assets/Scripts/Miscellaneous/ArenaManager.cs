using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public int EpisodeCounter;
    [SerializeField] public List<Transform> respawnPositions;
    [SerializeField] private TextMeshPro episodeText;
    [SerializeField] [Range(1, 3)] private int stage;
    [SerializeField] private List<GameObject> stage1Objects;
    [SerializeField] private List<GameObject> stage2Objects;
    [SerializeField] private List<GameObject> stage3Objects;
    void Start() 
    {
        if (stage >= 1 && stage1Objects.Count > 0) 
        {
            foreach(GameObject gameObject in stage1Objects) 
            {
                gameObject.SetActive(true);
            }
        }

        if (stage >= 2 && stage2Objects.Count > 0) 
        {
            foreach(GameObject gameObject in stage2Objects) 
            {
                gameObject.SetActive(true);
            }
        }

        if (stage >= 3 && stage3Objects.Count > 0) 
        {
            foreach(GameObject gameObject in stage3Objects) 
            {
                gameObject.SetActive(true);
            }
        }
    }

    void Update() 
    {
        episodeText.text = $"Episode {EpisodeCounter}";
    }
}
