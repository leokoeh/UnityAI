using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CollectOrange : Agent
{
    // Agent script of Prey in Solo

    [Header("Agent Configuration")]
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;

    [Header("Orange Configuration")]
    [SerializeField] private Transform orange;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;
    [SerializeField] private float orangeRespawnY;

    [Header("Arena Configuration")]
    [SerializeField] private ArenaManager arenaManager;

    [Header("Particle Configuration")]
    [SerializeField] ParticleSystem particles;

    public override void OnEpisodeBegin()
    {
        RespawnRandomly(transform, respawnY);
        arenaManager.EpisodeCounter++;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes Orange's position
        sensor.AddObservation(orange.localPosition);

        // Observes distance between own and Orange's position
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

        // Agent gets tiny punishment for existing based on its distance to Orange
        AddReward(Vector3.Distance(transform.localPosition, orange.localPosition) / -10000f);
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
            // Punishes Agent for touching walls
            AddReward(-0.25f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Reward"))
        {
            // Rewards Agent for touching the reward (Orange)
            AddReward(1f);
            RespawnRandomly(orange, orangeRespawnY);

            particles.Play();
        }
    }
    private void RespawnRandomly(Transform objectTransform, float objectY)
    {
        // Teleport to a new respawn location determined by the Arena Manager's respawn function
        objectTransform.localPosition = arenaManager.GetRandomRespawnPosition(objectY);
    }
}