using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AiState
{

    float distance;
    float timer = 0;

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }

        timer -= Time.deltaTime;
        //Debug.Log(agent.navMeshAgent.hasPath);
        //if (!agent.navMeshAgent.hasPath)
        //{
        //    agent.navMeshAgent.destination = agent.player.position;
        //}

        

        if (timer < 0.0f)
        {
            distance = Vector3.Distance(agent.player.position, agent.navMeshAgent.destination);
            if (distance > agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = agent.player.position;
                    timer = agent.config.maxTime;
                }
            }
            
        }
        
        
        Debug.Log(timer);
        Debug.Log(distance);
    }

    public void Exit(AiAgent agent)
    {
    }
}
