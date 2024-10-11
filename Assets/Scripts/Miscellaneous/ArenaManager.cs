using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    // This script manages the Arena (Environment) of Scenarios.

    [Header("Episode Configuration")]
    [SerializeField] public int EpisodeCounter;
    [SerializeField] private TextMeshPro episodeText;

    [Header("Stage Configuration")]
    [SerializeField][Range(1, 3)] public int currentStage;

    [Header("Object Configuration")]
    [SerializeField] public List<Transform> respawnPositions;
    [SerializeField] private List<GameObject> stage1Objects;
    [SerializeField] private List<GameObject> stage2Objects;
    [SerializeField] private List<GameObject> stage3Objects;

    [Header("Behavior Updater Configuration")]
    [SerializeField] public List<BehaviorUpdater> BehaviorUpdaters;

    private void UpdateStageObjects()
    {
        // Activate game objects depending on the current stage of the arena

        if (currentStage >= 1 && stage1Objects.Count > 0)
        {
            // Activate all objects of stage 1
            foreach (GameObject gameObject in stage1Objects) gameObject.SetActive(true);
        }

        if (currentStage >= 2 && stage2Objects.Count > 0)
        {
            // Activate all objects of stage 2
            foreach (GameObject gameObject in stage2Objects) gameObject.SetActive(true);
        }

        if (currentStage >= 3 && stage3Objects.Count > 0)
        {
            // Activate all objects of stage 3
            foreach (GameObject gameObject in stage3Objects) gameObject.SetActive(true);
        }

        // Deactivate game objects depending on the current stage of the arena

        if (currentStage < 3)
        {
            // Deactivate all objects of stage 3
            foreach (GameObject gameObject in stage3Objects) gameObject.SetActive(false);
        }

        if (currentStage < 2)
        {
            // Deactivate all objects of stage 2
            foreach (GameObject gameObject in stage2Objects) gameObject.SetActive(false);
        }

        // Adjust behavior of each Behavior Updater according to Arena's current stage, exclude manually controlled Agents
        foreach (BehaviorUpdater behaviorUpdater in BehaviorUpdaters) if (behaviorUpdater.currentModel != 0) behaviorUpdater.SetModel(currentStage);

    }

    void Start()
    {
        SetArenaStage(currentStage);
    }

    public void SetArenaStage(int stage)
    {
        currentStage = stage;
        UpdateStageObjects();
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
