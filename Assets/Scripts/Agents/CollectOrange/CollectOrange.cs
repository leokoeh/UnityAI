using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CollectOrange : Agent
{
    [Header("Agent Configuration")]
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;

    [Header("Orange Configuration")]
    [SerializeField] private Transform orange;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;
    [SerializeField] private float orangeRespawnY;
    [SerializeField] private bool changeOwnPosition;

    [Header("Arena Configuration")]
    [SerializeField] private ArenaManager arenaManager;

    public override void OnEpisodeBegin()
    {
        arenaManager.EpisodeCounter++;

        // Determine orange's respawn position
        RespawnRandomly(orange, orangeRespawnY);
        Physics.SyncTransforms();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes orange's position
        sensor.AddObservation(orange.localPosition);

        // Observes distance between own and orange's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, orange.localPosition));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Calculate movement vector based on ContinuousActions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 move = new(moveX, 0, moveZ);

        // Move Agent's character based on movement vector
        character.Move(speed * Time.deltaTime * move);
        transform.LookAt(transform.position + move);
        Physics.SyncTransforms();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allows player to control Agent via the horizontal and vertical axis, by default, this is WASD
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Punishes Agent for touching walls
            AddReward(-0.25f);

            // When colliding with walls, a new respawn location is always determined
            RespawnRandomly(transform, respawnY);

            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Reward"))
        {
            // Rewards Agent for touching the reward (orange)
            AddReward(1f);

            // Determine a new respawn location only if the changeOwnPosition bool is true, 
            if (changeOwnPosition) RespawnRandomly(transform, respawnY);

            EndEpisode();
        }
    }
    private void RespawnRandomly(Transform objectTransform, float objectY)
    {
        // Teleport to a new respawn location determined by the Arena Manager's respawn function
        objectTransform.localPosition = arenaManager.GetRandomRespawnPosition(objectY);
    }
}