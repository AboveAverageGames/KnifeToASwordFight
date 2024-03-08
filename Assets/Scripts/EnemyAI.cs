using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private float enemyTimer;

    private int currentWaypoint = 0;

    public NavMeshAgent agent;

    public Transform player;

    public Transform [] scatterLocation;
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
        enemyTimer = 30f;
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
        //Changes destination to the current waypoint inside the scatter location Array
     agent.SetDestination(scatterLocation[currentWaypoint].position);

        //Changes the waypoint when they reach one in the scatter phase. Adds +1 to the Array of waypoints once they reach one.
        if (Vector3.Distance(transform.position, scatterLocation[currentWaypoint].position) < 1f)
        {
            if (currentWaypoint +1 != scatterLocation.Length)
            {
                currentWaypoint++;
            }
            else if (currentWaypoint +1 == scatterLocation.Length)
            {
                currentWaypoint = 0;
            }
        }


        //Counting down the time spent in scatter phase
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

    

            Vector3 dirToPlayer = transform.position - player.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);

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
