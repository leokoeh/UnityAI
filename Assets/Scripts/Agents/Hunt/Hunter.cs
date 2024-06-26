using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Hunter : Agent
{
    [Header("Agent Configuration")]
    public CharacterController character;
    public float speed;
    public GameObject prey;
    public Agent preyAgent;

    [Header("Respawn Configuration")]
    public bool randomizeRespawnHunter;
    public Vector3 defaultRepsawnHunter;

    public override void OnEpisodeBegin()
    {
        // Determine respawn position
        if (randomizeRespawnHunter)
        {
            transform.localPosition = new Vector3(Random.Range(-10, 10), defaultRepsawnHunter.y, Random.Range(-10, 10));
        }
        else
        {
            transform.localPosition = defaultRepsawnHunter;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes prey's position
        sensor.AddObservation(prey.transform.localPosition);

        // Observes distance between own and prey's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, prey.transform.localPosition));
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

        // Hunter gets tiny punishment for existing
        AddReward(-0.0001f);
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
            // Punishes Agent for touching walls and ends episode 
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("Prey"))
        {
            // Rewards hunter for touching prey, punishes prey and ends each Agent's episode
            preyAgent.AddReward(-1f);
            preyAgent.EndEpisode();
            AddReward(1f);
            EndEpisode();
        }
    }
}