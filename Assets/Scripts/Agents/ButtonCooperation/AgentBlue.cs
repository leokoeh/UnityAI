using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentBlue : Agent
{
    [Header("Agent Configuration")]
    public CharacterController character;
    public float speed;

    [Header("Resetter Configuration")]
    // Ability to link scenario resetter to reward
    public ScenarioResetter linkedResetter;

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
        continuousActions[0] = (Input.GetKey(KeyCode.H) ? 1f : 0f) - (Input.GetKey(KeyCode.F) ? 1f : 0f);
        continuousActions[1] = (Input.GetKey(KeyCode.T) ? 1f : 0f) - (Input.GetKey(KeyCode.G) ? 1f : 0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Door"))
        {
            // Punishes agent for touching walls and resets scenario
            AddReward(-0.5f);
            linkedResetter.Reset();
        }
    }
}