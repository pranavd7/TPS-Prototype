using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackPlayerState : AiState
{
    PlayerHealth playerHealth;
    float range = 10f;
    Vector3 distance;
    public AiStateId GetId()
    {
        return AiStateId.AttackPlayer;
    }
    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 7.0f;
        playerHealth = agent.player.GetComponent<PlayerHealth>();
    }



    public void Update(AiAgent agent)
    {
        //if (Physics.Raycast(agent.gun.rayOrigin.transform.position, player.position, out hit))
        //{
        //    agent.target.position = agent.player.position + offset;
        //}
        //else
        //{
        //    agent.target.position = agent.transform.forward * 10;
        //}
        if (!GameManager.gm.isRobot)
        {
            agent.target.position = agent.player.position + agent.playerOffset;
            agent.navMeshAgent.destination = agent.player.position;
        }
        else
        {
            agent.target.position = agent.robot.position + agent.robotOffset;
            agent.navMeshAgent.destination = agent.robot.position;
        }

        distance = agent.target.position - agent.transform.position;
        if (distance.magnitude <= range)
        {
            agent.weapon.ShootUpdate();
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(distance.x, 0, distance.z));
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        if (playerHealth.IsDead())
        {
            agent.stateMachine.ChangeState(AiStateId.Idle);
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 0;
    }
}
