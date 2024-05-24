using Boxophobic.StyledGUI;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public GameObject ghostPrefab;
    public GameObject goblinPrefab;

    //Timer that controls transitions between states
    private float enemyTimer;

    //Check for if the goblin has already been killed
    public bool isGoblinDead;

    //Speed here to increase based on level
    private float goblinSpeed;
    private float goblinSpeedIncrease;

    //Speed when they are dead
    private float ghostSpeed = 10f;

    //Anim Controller
    private Animator animController;

    //Waypoints for scatter phase
    private int currentWaypoint = 0;

    //Reference to the navmesh agent
    public NavMeshAgent agent;

    //reference to player
    public Transform player;

    //ChasePoint Game object that is different for each goblin.
    public Transform chasePoint;

    //Reference to game manager script
    public GameManagerScript gameManagerScript;

    //Array of waypoints for scatter locations
    public Transform [] scatterLocation;

    //The radius of which it can choose random points to flee too
    public int fleeRadius;

    //Stores the location of "Home" which it returns to when killed
    public Transform homeLocation;

    //Testing if chase point is on the nav mesh
    NavMeshHit hit;

    //Stores the states of which it can be in
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


        //Level difficulty increase code below ------------------------------

        //This reduces the starting enemy timer based on the current level they are at. if the level is past level 7 that timer is set to 0
        if (enemyTimer <= (PlayerPrefs.GetInt("CurrentLevel")))
        {
            enemyTimer = 0;
        }
        else
        {
            enemyTimer = ((7) - (PlayerPrefs.GetInt("CurrentLevel")));
        }
        //Sets the goblin speed to the navmesh agent speed
        goblinSpeed = GetComponent<NavMeshAgent>().speed;

        //Increases the goblins speed based on what level it is (Incremented by (0.(LEVEL) then capping at a random range between 5.8 to 6.5 since the players speed is 7)
        //Also random range so the goblins are varying speeds and dont stack up on eachother
        goblinSpeedIncrease = (PlayerPrefs.GetInt("CurrentLevel") / 10f);
        goblinSpeed = (goblinSpeed + goblinSpeedIncrease);
        if (goblinSpeed > 6.5)
        {
            goblinSpeed = Random.Range(5.8f, 6.5f);
        }
        //Sets the navmesh speed to the goblin speed.
        GetComponent<NavMeshAgent>().speed = goblinSpeed;
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
            //Reduces the enemy timer based on the difficulty.. It divides it by whatever level it is, If the level is higher then the scatter timer Ill set scatter timer to 0.
            enemyTimer = 7;
            if (enemyTimer == (PlayerPrefs.GetInt("CurrentLevel")))
            {
                enemyTimer = 0;
            }
            else
            {
                enemyTimer =( (7 ) - (PlayerPrefs.GetInt("CurrentLevel")));
            }
        }

    }

    void Scatter()
    {
        Debug.Log("The current level is " + PlayerPrefs.GetInt("CurrentLevel"));
        Debug.Log("Scatter timer is " + enemyTimer);

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

    // Finds random points that are on the nav mesh for the flee state
    bool RandomPoint(Vector3 center, float fleeRadius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * fleeRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 0.5f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    void Flee()
    {
        //Random points found on navmesh for fleeing state
        Vector3 point;
        if (RandomPoint(transform.position, fleeRadius, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
        }


        /*
        //Gets a random point within a random radius of a sphere inside the nav mesh
        randomDirection = transform.position + Random.insideUnitSphere * fleeRadius;
        //Sets that new point for the agent to move to
        agent.SetDestination(randomDirection);
        */

        //Once the player no longer has the sword power up resume chasing
        if (gameManagerScript.doesPlayerHaveSword == false)
        {
            currentState = enemyState.Chase;
            enemyTimer = 20;
  
        }
        

    }


    void ReturnHome()
    {
        agent.SetDestination(homeLocation.position);
        //checks if the AI has made it back to home
        if (Vector3.Distance(transform.position, homeLocation.position) <= 1 && gameManagerScript.doesPlayerHaveSword == false)
        {
            isGoblinDead = false;
            //Make Ghost prefab invis and goblin Prefab Vis
            goblinPrefab.SetActive(true);
            ghostPrefab.SetActive(false);

            //Returns to normal speed
            GetComponent<NavMeshAgent>().speed = goblinSpeed;

            currentState = enemyState.Chase;
            enemyTimer = 20;
        }
        //Returns to normal state if player runs out of sword power up
        else if (gameManagerScript.doesPlayerHaveSword == false)
        {
            isGoblinDead = false;
            //Make Ghost prefab invis and goblin Prefab Vis
            goblinPrefab.SetActive(true);
            ghostPrefab.SetActive(false);
            //Returns to normal speed
            GetComponent<NavMeshAgent>().speed = goblinSpeed;

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
            isGoblinDead = true;
            //Make Ghost prefab vis and goblin Prefab invis
            goblinPrefab.SetActive(false);
            ghostPrefab.SetActive(true);

            //Changes speed to ghost speed (Faster to return home)
            GetComponent<NavMeshAgent>().speed = ghostSpeed;

            enemyTimer = 5.0f;
            currentState = enemyState.ReturnHome;
        }
        else
        { 
            }
        }
    }
