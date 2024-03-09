using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private float enemyTimer;
    private float enemyKilledCooldown;

    private bool hasBeenKilled;

    private int currentWaypoint = 0;

    public NavMeshAgent agent;

    public Transform player;
    public GameManagerScript gameManagerScript;

    public Transform [] scatterLocation;

    public int fleeRadius;
    public int vicinityRadiusToPlayer;

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
        //Gets and Assigns the game manager script to the variable
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();

        //Starts the enemies in a state of scatter
        currentState = enemyState.Scatter;
        enemyTimer = 7f;
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

        if (gameManagerScript.doesPlayerHaveSword && currentState != enemyState.ReturnHome && enemyKilledCooldown <=0)
        {
            Debug.Log("THE PLAYER HAS A SWORD");
            currentState = enemyState.Flee;

        }

        //Reduces enemy kill cooldown
        if (enemyKilledCooldown > 0)
        {
            enemyKilledCooldown -= Time.deltaTime;
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);

        //Reduces the time each second
        enemyTimer -=Time.deltaTime;
        //When timer is done switch to scatter mode
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

        //Changes the waypoint when they reach one. Adds +1 to the Array of waypoints once they reach one, resets back to 0 and the end of the patrol.
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
        //Once timer is done Switch to chase Mode
        if (enemyTimer <= 0)
        {
            currentState = enemyState.Chase;
            enemyTimer = 20;
        }
    }

    void Flee()
    {
        //Gets a random point within a random radius of a sphere inside the nav mesh
            Vector3 randomDirection = Random.insideUnitSphere * fleeRadius;

        //Sets the random point based on where the Agent is
            Vector3 newPos = transform.position + randomDirection;

        //Sets that new point for the agent to move to
            agent.SetDestination(randomDirection);

        //Once the player no longer has the sword power up resume chasing
        if (gameManagerScript.doesPlayerHaveSword == false)
        {
            currentState = enemyState.Chase;
        }


        /*
     float distanceToPlayer = Vector3.Distance(transform.position, player.position);
         Vector3 dirToPlayer = transform.position - player.transform.position */
    }

    void ReturnHome()
    {
        enemyKilledCooldown = 20;
        agent.SetDestination(homeLocation.position);
        //checks if the AI has made it back to home
        if (Vector3.Distance(transform.position, homeLocation.position) <= 1 && enemyTimer > 0)
        {
            enemyTimer -= Time.deltaTime;
            //Start the timer for once they are home
        }
        else if (enemyTimer <= 0)
        {
            //after reaching home Scatter
            currentState = enemyState.Scatter;
            enemyTimer = 7;
            hasBeenKilled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameManagerScript.doesPlayerHaveSword)
        {
            enemyTimer = 5.0f;
            hasBeenKilled = true;
            currentState = enemyState.ReturnHome;
        }
        else
        { 
                Debug.Log("Collision with player without sword");
            }
        }
    }
