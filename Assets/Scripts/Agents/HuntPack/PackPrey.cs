using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PackPrey : Agent
{
    // Agent script of Prey in Cooperation

    [Header("Agent Configuration")]
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;

    [Header("Hunters Configuration")]
    [SerializeField] private Transform hunter1;
    [SerializeField] private Agent hunter1Agent;

    [SerializeField] private Transform hunter2;
    [SerializeField] private Agent hunter2Agent;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;

    [Header("Arena Configuration")]
    [SerializeField] ArenaManager arenaManager;

    public override void OnEpisodeBegin()
    {
        Physics.SyncTransforms();

        arenaManager.EpisodeCounter++;

        // Determine respawn position
        RespawnRandomly(transform, respawnY);
        Physics.SyncTransforms();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes both Hunter's positions
        sensor.AddObservation(hunter1.localPosition);
        sensor.AddObservation(hunter2.localPosition);

        // Observes distance between own and both Hunter's positions
        sensor.AddObservation(Vector3.Distance(transform.localPosition, hunter1.localPosition));
        sensor.AddObservation(Vector3.Distance(transform.localPosition, hunter2.localPosition));
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

        // Prey gets tiny rewards for existing based on distance to each Hunter
        AddReward(Vector3.Distance(transform.localPosition, hunter1.localPosition) / 10000f);
        AddReward(Vector3.Distance(transform.localPosition, hunter2.localPosition) / 10000f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allows player to control Agent via the horizontal and vertical axis (WASD and Arrow Keys)
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Punishes Agent for touching walls, rewards Hunters and ends Prey's episode
            AddReward(-1f);
            EndEpisode();

            hunter1Agent.AddReward(0.1f);
            hunter2Agent.AddReward(0.1f);
        }
    }
    private void RespawnRandomly(Transform objectTransform, float objectY)
    {
        // Teleport to a new respawn location determined by the Arena Manager's respawn function
        objectTransform.localPosition = arenaManager.GetRandomRespawnPosition(objectY);
    }
}