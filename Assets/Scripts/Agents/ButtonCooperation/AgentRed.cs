using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentRed : Agent
{
    [Header("Agent Configuration")]
    public CharacterController character;
    public float speed;

    [Header("Button Configuration")]
    [SerializeField] private ScenarioButton button1;
    [SerializeField] private ScenarioButton button2;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;
    [SerializeField] private float buttonRespawnY;

    [Header("Arena Configuration")]
    [SerializeField] ArenaManager arenaManager;
    [SerializeField] private Agent teammate;
    public override void OnEpisodeBegin()
    {
        arenaManager.EpisodeCounter++;

        // Determine respawn position
        Transform respawnPosition = arenaManager.respawnPositions[Random.Range(0, arenaManager.respawnPositions.Count)];
        transform.localPosition = new Vector3(respawnPosition.localPosition.x, respawnY, respawnPosition.localPosition.z);

        // Determine button 1's respawn position
        Transform button1RespawnPosition = arenaManager.respawnPositions[Random.Range(0, arenaManager.respawnPositions.Count)];
        button1.gameObject.transform.localPosition = new Vector3(button1RespawnPosition.localPosition.x, buttonRespawnY, button1RespawnPosition.localPosition.z);

        // Determine button 2's respawn position
        Transform button2RespawnPosition = arenaManager.respawnPositions[Random.Range(0, arenaManager.respawnPositions.Count)];
        button2.gameObject.transform.localPosition = new Vector3(button2RespawnPosition.localPosition.x, buttonRespawnY, button2RespawnPosition.localPosition.z);
        Physics.SyncTransforms();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(teammate.gameObject.transform.localPosition);
        sensor.AddObservation(button1.gameObject.transform.localPosition);
        sensor.AddObservation(button2.gameObject.transform.localPosition);
        sensor.AddObservation(button1.IsActive);
        sensor.AddObservation(button2.IsActive);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Calculate movement vector based on ContinuousActions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 move = new(moveX, 0, moveZ);

        // Move character based on movement vector
        Physics.SyncTransforms();
        character.Move(speed * Time.deltaTime * move);
        transform.LookAt(transform.position + move);

        if(button1.IsActive || button2.IsActive) AddReward(0.0001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allows player to control agent
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Punishes agent for touching walls and resets scenario
            AddReward(-1f);
            EndEpisode();
        }
        
        if (collision.gameObject.CompareTag("Button")) 
        {
            AddReward(0.25f);
        }
    }

    private void OnTriggerExit(Collider collision) 
    {
        if (collision.gameObject.CompareTag("Button")) 
        {
            AddReward(-0.25f);
        }
    }

    void Update() 
    {
        if(button1.IsActive == true && button2.IsActive == true) 
        {
            AddReward(1f);
            button1.IsActive = false;
            button2.IsActive = false;
            EndEpisode();
        }
    }
}