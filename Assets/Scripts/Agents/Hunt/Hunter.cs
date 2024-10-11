using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Hunter : Agent
{
    // Agent script of Hunter in Competition

    [Header("Agent Configuration")]
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;

    [Header("Prey Configuration")]
    [SerializeField] private Transform prey;
    [SerializeField] private Agent preyAgent;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;

    [Header("Arena Configuration")]
    [SerializeField] ArenaManager arenaManager;

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

        // Observes Prey's position
        sensor.AddObservation(prey.localPosition);

        // Observes distance between own and Prey's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, prey.localPosition));
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

        // Hunter gets tiny punishment for existing based on its distance to Prey
        AddReward(Vector3.Distance(transform.localPosition, prey.localPosition) / -10000f);
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
            // Punishes Agent for touching walls, rewards Prey and ends each Hunter's episode
            AddReward(-1f);
            EndEpisode();

            preyAgent.AddReward(0.1f);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("Prey"))
        {
            // Rewards hunter for touching prey, punishes prey and ends Prey's episode
            AddReward(1f);

            particles.Play();

            preyAgent.AddReward(-1f);
            preyAgent.EndEpisode();
        }
    }
    private void RespawnRandomly(Transform objectTransform, float objectY)
    {
        // Teleport to a new respawn location determined by the Arena Manager's respawn function
        objectTransform.localPosition = arenaManager.GetRandomRespawnPosition(objectY);
    }
}