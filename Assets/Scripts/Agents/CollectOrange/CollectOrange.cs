using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CollectOrange : Agent
{
    [Header("Agent Configuration")]
    public CharacterController character;
    public float speed;

    [Header("Orange Configuration")]
    public GameObject orange;

    [Header("Respawn Configuration")]
    public Vector3 agentRespawnPosition;
    public Vector3 orangeRespawnPosition;
    public bool randomizeAgentPosition;
    public bool randomizeOrangePosition;

    [Header("Barrier Spawn Configuration")]
    public GameObject barrier;
    public bool barrierDoSpawn;
    
    // Sets the minimum distance between the barrier and the agent's and orange's respawn position 
    public float barrierMinDistance;

    // Sets the maximum amount of spawn checks that are ran 
    public int barrierMaxSpawnChecks;

    // This vector is used to calculate the world position of the barrier
    private Vector3 barrierInitialPosition;

    private void Start()
    {
        barrierInitialPosition = barrier.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = agentRespawnPosition;
        orange.transform.localPosition = orangeRespawnPosition;

        if (randomizeOrangePosition)
        {
            orange.transform.localPosition = new Vector3(Random.Range(-10, 10), orangeRespawnPosition.y, Random.Range(-10, 10));
        }

        if (randomizeAgentPosition)
        {
            transform.localPosition = new Vector3(Random.Range(-10, 10), agentRespawnPosition.y, Random.Range(-10, 10));
        }

        barrier.SetActive(false);

        if (barrierDoSpawn)
        {
            for (int i = 0; i < barrierMaxSpawnChecks; i++)
            {
                // Calculate new a random position based on the barrier's initial position
                Vector3 randomSpawnLocation = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)) + barrierInitialPosition;

                // Checks if the barrier's distance to both agent and orange is far enough
                if (Vector3.Distance(randomSpawnLocation, orange.transform.position) >= barrierMinDistance && Vector3.Distance(randomSpawnLocation, transform.position) >= barrierMinDistance)
                {
                    // Activates the barrier
                    barrier.transform.position = randomSpawnLocation;
                    barrier.SetActive(true);
                    break;
                }
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes orange's position
        sensor.AddObservation(orange.transform.localPosition);

        // Observes distance between own and orange's position
        sensor.AddObservation(Vector3.Distance(transform.position, orange.transform.position));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Calculate movement vector based on ContinuousActions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // Move character based on movement vector
        Physics.SyncTransforms();
        character.Move(move * speed * Time.deltaTime);
        transform.LookAt(transform.position + move);
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
        if (collision.gameObject.TryGetComponent<Reward>(out Reward reward))
        {
            // Rewards agent for touching orange and ends episode 
            AddReward(reward.rewardAmount);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Punishes agent for touching walls and ends episode 
            AddReward(-1f);
            EndEpisode();
        }
    }
}