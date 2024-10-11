using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Prey : Agent
{
    // Agent script of Prey in Competition

    [Header("Agent Configuration")]
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;

    [Header("Hunter Configuration")]
    [SerializeField] private Transform hunter;
    [SerializeField] private Agent hunterAgent;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;

    [Header("Arena Configuration")]
    [SerializeField] ArenaManager arenaManager;

    public override void OnEpisodeBegin()
    {
        RespawnRandomly(transform, respawnY);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes Hunter's position
        sensor.AddObservation(hunter.localPosition);

        // Observes distance between own and Hunter's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, hunter.localPosition));
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

        // Prey gets tiny reward for existing based on distance to Hunter
        AddReward(Vector3.Distance(transform.localPosition, hunter.localPosition) / 10000f);
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
            // Punishes Agent for touching walls, rewards Hunter and ends Prey's episode
            AddReward(-1f);
            EndEpisode();

            hunterAgent.AddReward(0.1f);
        }
    }

    public void RespawnRandomly(Transform objectTransform, float objectY)
    {
        // Teleport to a new respawn location determined by the Arena Manager's respawn function
        objectTransform.localPosition = arenaManager.GetRandomRespawnPosition(objectY);
    }
}