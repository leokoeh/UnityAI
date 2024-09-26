using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Hunter : Agent
{
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

    public override void OnEpisodeBegin()
    {
        arenaManager.EpisodeCounter++;

        // Determine respawn position
        RespawnRandomly(transform, respawnY);
        Physics.SyncTransforms();
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
        // Allows player to control agent
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = (Input.GetKey(KeyCode.H) ? 1f : 0f) - (Input.GetKey(KeyCode.F) ? 1f : 0f);
        continuousActions[1] = (Input.GetKey(KeyCode.T) ? 1f : 0f) - (Input.GetKey(KeyCode.G) ? 1f : 0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Punishes Agent for touching walls, rewards Prey and ends each Agent's episode
            AddReward(-1f);
            EndEpisode();
            
            preyAgent.AddReward(0.1f);
            preyAgent.EndEpisode();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("Prey"))
        {
            // Rewards hunter for touching prey, punishes prey and ends each Agent's episode
            AddReward(1f);
            EndEpisode();

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