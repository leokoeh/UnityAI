using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float speed;
    private int currentPointIndex = 0;
    
    void Update()
    {
        PatrolPoints();
    }

    void PatrolPoints() 
    {
        Transform target = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, target.position) < 0.1f) 
        {
            if(currentPointIndex == patrolPoints.Count - 1) currentPointIndex = 0;
            else currentPointIndex++;
        }

    }
}
