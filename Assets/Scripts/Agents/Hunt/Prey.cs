using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Prey : Agent
{
    [Header("Agent Configuration")]
    public CharacterController character;
    public float speed;
    public GameObject hunter;

    [Header("Respawn Configuration")]
    public bool randomizeRespawnPrey;
    public Vector3 defaultRepsawnPrey;

    public override void OnEpisodeBegin()
    {
        // Determine respawn position
        if (randomizeRespawnPrey)
        {
            transform.localPosition = new Vector3(Random.Range(-10, 10), defaultRepsawnPrey.y, Random.Range(-10, 10));
        }
        else
        {
            transform.localPosition = defaultRepsawnPrey;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes hunter's position
        sensor.AddObservation(hunter.transform.localPosition);

        // Observes distance between own and hunter's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, hunter.transform.localPosition));
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

        // Prey gets tiny reward for existing
        AddReward(0.0001f);
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
            // Punishes Agent for touching walls and ends episode 
            AddReward(-1f);
            EndEpisode();
        }
    }
}