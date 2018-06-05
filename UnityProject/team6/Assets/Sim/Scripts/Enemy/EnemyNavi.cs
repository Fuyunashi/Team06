using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavi : MonoBehaviour
{     
    public Transform[] wayPoint;
    private int pastPoint;
    private NavMeshAgent agent;
    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        EnemyNaviMove();
    }
    void EnemyNaviMove()
    {
        if (wayPoint.Length == 0)
            return;
        agent.destination = wayPoint[pastPoint].position;
        pastPoint = (pastPoint + 1) % wayPoint.Length;
    }
    // Update is called once per frame
    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            EnemyNaviMove();
    }
}