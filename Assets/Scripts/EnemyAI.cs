using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private float enemyTimer;

    Vector3 randomDirection;

    private Animator animController;

    private int currentWaypoint = 0;

    public NavMeshAgent agent;

    public Transform player;
    public Transform chasePoint; //ChasePoint that is different for each goblin.

    public GameManagerScript gameManagerScript;

    public Transform [] scatterLocation;

    public int fleeRadius;
    public int vicinityRadiusToPlayer;

    public Transform homeLocation;

    //Testing if chase point is on the nav mesh
    NavMeshHit hit;

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
        //Gets Animator For Enemies
        animController = GetComponentInChildren<Animator>();

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

        //Makes the enemies flee if the player has a sword and they are not already at their home (after being Killed)
        if (gameManagerScript.doesPlayerHaveSword && currentState != enemyState.ReturnHome)
        {
            currentState = enemyState.Flee;
        }


        //HERE we check if the goblin is at home, since it is the only time they dont move, we set the anim to idle.
        if (Vector3.Distance(transform.position, homeLocation.position) <= 0.5)
        {
            animController.SetBool("Home", true);
        }
        else
        {
            animController.SetBool("Home", false);
        }
    }

    void Chase()
    {
        //Checking if the chase point is on the navmesh
        NavMesh.SamplePosition(chasePoint.position, out NavMeshHit hit, 1, NavMesh.AllAreas);

        //Checks if chase point is on the nav mesh
        //If it is not it will switch to chasing the playeras position
        //It will also switch to the players position if the distance between the player and the enemy is below a certain range
        if (hit.hit == false || (Vector3.Distance(transform.position, player.position) <= 10))
        {
            agent.SetDestination(player.position);
        }
        //If the ball is on the navmesh it will chase the ball (Seeming like it is cutting off the player)
        else 
        { 
            agent.SetDestination(chasePoint.position);
        } 
            


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
             currentWaypoint = (currentWaypoint + 1) % scatterLocation.Length;
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
            randomDirection = Random.insideUnitSphere * fleeRadius;

        //Sets the random point based on where the Agent is
            Vector3 newPos = transform.position + randomDirection;

        //Sets that new point for the agent to move to
        agent.SetDestination(randomDirection);

        //Once the player no longer has the sword power up resume chasing
        if (gameManagerScript.doesPlayerHaveSword == false)
        {
            currentState = enemyState.Chase;
            enemyTimer = 20;
        }


        /*
     float distanceToPlayer = Vector3.Distance(transform.position, player.position);
         Vector3 dirToPlayer = transform.position - player.transform.position */
    }

    void ReturnHome()
    {
        agent.SetDestination(homeLocation.position);
        //checks if the AI has made it back to home
        if (Vector3.Distance(transform.position, homeLocation.position) <= 1 && gameManagerScript.doesPlayerHaveSword == false)
        {
            currentState = enemyState.Chase;
            enemyTimer = 20;
        }
        else if (gameManagerScript.doesPlayerHaveSword == false)
        {
            //after reaching home Scatter
            currentState = enemyState.Chase;
            enemyTimer = 20;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Checking if it colliding with the player and if they have a sword
        if (collision.gameObject.CompareTag("Player") && gameManagerScript.doesPlayerHaveSword)
        {
            enemyTimer = 5.0f;
            currentState = enemyState.ReturnHome;
        }
        else
        { 
                Debug.Log("Collision with player without sword");
            }
        }
    }
