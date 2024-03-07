using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private float enemyTimer;

    public float enemyFleeDistance;

    public NavMeshAgent agent;

    public Transform player;

    public Transform scatterLocation;
    public Transform secondScatterLocation;
    public Transform scatterLocationPlaceHolder;

    public Transform homeLocation;

    private enemyState currentState;
    private enum enemyState
    {
     Chase,
     Scatter,
     Flee,
     ReturnHome,
    }


    // Start is called before the first frame update
    void Start()
    {
        currentState = enemyState.Scatter;
        enemyTimer = 7f;
        enemyFleeDistance = 8f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case enemyState.Chase:
                Chase();
                break;

                case enemyState.Scatter:
                Scatter();
                break;

            case enemyState.Flee:
                Flee();
                break;

                case enemyState.ReturnHome:
                ReturnHome();
                break;
        }

    }

    void Chase()
    {
        agent.SetDestination(player.position);

        enemyTimer -=Time.deltaTime;
        if (enemyTimer <= 0)
        { 
            currentState = enemyState.Scatter;
            enemyTimer = 7;
        }
    }

    void Scatter()
    {
            agent.SetDestination(scatterLocation.position);

        
        enemyTimer -= Time.deltaTime;

        if (enemyTimer <= 0)
        {
            currentState = enemyState.Chase;
            enemyTimer = 20;
        }
    }

    void Flee()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < enemyFleeDistance)
        {
            Vector3 dirToPlayer = transform.position - player.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);
        }
        else if (distanceToPlayer > enemyFleeDistance)
        {
            agent.SetDestination(scatterLocation.position);
        }


        /*Tries to detect where the player is in the game world and run away from them.
        
        */
    }

    void ReturnHome()
    {
        agent.SetDestination(homeLocation.position);

        //checks if the AI has made it back to home, and then returns to chasing the player for 20s.
        if (transform.position.z == homeLocation.position.z)
        {
            currentState = enemyState.Chase;
            enemyTimer = 20;
        }
    }
}
