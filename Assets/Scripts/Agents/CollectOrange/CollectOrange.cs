using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CollectOrange : Agent
{

    [Header("Agent Configuration")]
    public CharacterController character;
    public float speed;
    public GameObject orange;

    [Header("Respawn Configuration")]
    public bool randomizeOrangePosition;
    public bool randomizeAgentPosition;

    [Header("Respawn Position Configuration")]
    public Vector3 agentRespawnPosition;
    public Vector3 orangeRespawnPosition;

    [Header("Barrier Spawn Configuration")]
    public GameObject barrier;
    public bool barrierDoSpawn;
    public float barrierMinDistance;
    public int barrierMaxSpawnChecks;
    public GameObject barrierPositionAnchor;

    public override void OnEpisodeBegin()
    {
        character.enabled = false;

        if (randomizeOrangePosition)
        {
            orange.transform.localPosition = new Vector3(Random.Range(-10, 10), orangeRespawnPosition.y, Random.Range(-10, 10));

        }
        else
        {
            orange.transform.localPosition = orangeRespawnPosition;
        }

        if (randomizeAgentPosition)
        {
            transform.localPosition = new Vector3(Random.Range(-10, 10), agentRespawnPosition.y, Random.Range(-10, 10));

        }
        else
        {
            transform.localPosition = agentRespawnPosition;
        }

        barrier.SetActive(false);

        if (barrierDoSpawn)
        {
            Debug.Log("triggered");
            for (int i = 0; i < barrierMaxSpawnChecks; i++)
            {

                Vector3 randomSpawnLocation = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)) + barrierPositionAnchor.transform.position;

                if (Vector3.Distance(randomSpawnLocation, orange.transform.position) >= barrierMinDistance && Vector3.Distance(randomSpawnLocation, transform.position) >= barrierMinDistance)
                {
                    barrier.transform.position = randomSpawnLocation;
                    barrier.SetActive(true);
                    break;

                }

            }

        }

        character.enabled = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(orange.transform.localPosition);
        sensor.AddObservation(Vector3.Distance(transform.position, orange.transform.position));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0, moveZ);

        character.Move(move * speed * Time.deltaTime);

        transform.LookAt(transform.position + move);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }


    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.layer == 7)
        {
            AddReward(1f);
            EndEpisode();


        }
        else if (collision.gameObject.layer == 6)
        {
            AddReward(-1f);
            EndEpisode();

        }

    }

}
