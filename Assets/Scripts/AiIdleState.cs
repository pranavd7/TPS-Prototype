using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiIdleState : AiState
{
    PlayerHealth playerHealth;
    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }
    public void Enter(AiAgent agent)
    {
        playerHealth = agent.player.GetComponent<PlayerHealth>();
        agent.navMeshAgent.ResetPath();
    }

    public void Update(AiAgent agent)
    {
        if (playerHealth.IsDead())
        {
            return;
        }

        Vector3 playerDirection = agent.player.position - agent.transform.position;
        if (playerDirection.magnitude > agent.config.maxSightDistance)
        {
            return;
        }

        Vector3 agentDirection = agent.transform.forward;
        playerDirection.Normalize();

        float dotP = Vector3.Dot(agentDirection, playerDirection);
        //Debug.Log(dotP);
        if (dotP > 0.0f)
        {
            agent.stateMachine.ChangeState(AiStateId.AttackPlayer);
        }
    }

    public void Exit(AiAgent agent)
    {
    }

}
