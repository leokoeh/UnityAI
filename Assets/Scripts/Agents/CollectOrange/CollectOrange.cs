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
    [SerializeField] private float respawnY;
    [SerializeField] private float orangeRespawnY;
    [SerializeField] private bool changeOwnPosition;
       
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
        if(changeOwnPosition) 
        {
            Transform respawnPosition = arenaManager.respawnPositions[Random.Range(0, arenaManager.respawnPositions.Count)];
            transform.localPosition = new Vector3(respawnPosition.localPosition.x, respawnY, respawnPosition.localPosition.z);
        }


        // Determine orange's respawn position
        Transform orangeRespawnPosition = arenaManager.respawnPositions[Random.Range(0, arenaManager.respawnPositions.Count)];
        orange.transform.localPosition = new Vector3(orangeRespawnPosition.localPosition.x, orangeRespawnY, orangeRespawnPosition.localPosition.z);
        Physics.SyncTransforms();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observes own position
        sensor.AddObservation(transform.localPosition);

        // Observes orange's position
        sensor.AddObservation(orange.transform.localPosition);

        // Observes distance between own and orange's position
        sensor.AddObservation(Vector3.Distance(transform.localPosition, orange.transform.localPosition));

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
        character.Move(speed * Time.deltaTime * move);
        transform.LookAt(transform.position + move);
        Physics.SyncTransforms();
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
            // Punishes agent for touching walls and ends episode 
            AddReward(-1f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Reward"))
        {
            AddReward(1f);
            EndEpisode();
        }
    }
}