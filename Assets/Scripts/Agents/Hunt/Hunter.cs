using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Hunter : Agent
{
    [Header("Agent Configuration")]
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;
    [SerializeField] private GameObject prey;
    [SerializeField] private Agent preyAgent;

    [Header("Respawn Configuration")]
    [SerializeField] private float respawnY;

    [Header("Arena Configuration")]
    [SerializeField] ArenaManager arenaManager;

    [Header("Death Ball Configuration")]
    [SerializeField] private GameObject deathBall1;
    [SerializeField] private GameObject deathBall2;
    [SerializeField] private GameObject deathBall3;


    public override void OnEpisodeBegin()
    {
        arenaManager.EpisodeCounter++;

        // Determine respawn position
        Transform respawnPosition = arenaManager.respawnPositions[Random.Range(0, arenaManager.respawnPositions.Count)];
        transform.localPosition = new Vector3(respawnPosition.localPosition.x, respawnY, respawnPosition.localPosition.z);
        Physics.SyncTransforms();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes prey's position
        sensor.AddObservation(prey.transform.localPosition);

        // Observes distance between own and prey's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, prey.transform.localPosition));

        sensor.AddObservation(deathBall1.transform.localPosition);
        sensor.AddObservation(deathBall2.transform.localPosition);
        sensor.AddObservation(deathBall3.transform.localPosition);

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

        // Hunter gets tiny punishment for existing
        AddReward(-0.001f);
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
            // Punishes Agent for touching walls, rewards prey and ends each Agent's episode
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
}