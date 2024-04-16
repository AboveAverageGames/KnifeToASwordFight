using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animController;
    private bool hasSwordAnimPlayed;
    private bool hasAttackAnimPlayed;
    private bool playerFrozen;
    public bool isPlayerDead;
    public bool gameOver;

    public GameObject pauseMenu;



    AudioManager audioManager;

    private GameObject swordAttatchedToPlayer;

    public float speed;
    private Vector2 move;


    //Time he carries the sword for once picked up
    private float swordPowerUpTimer = 10f;
    public PowerUpBar powerUpBar;

    //Accessing the game manager script
    public GameManagerScript gameManagerScript;

    public EnemyAI enemyAI;


    // Pauses or Unpauses the game using the new input action
    public void Pause(InputAction.CallbackContext context)
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }    
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //This function is Invoked through the Input manager
        //Takes the Input from the new Input System (Left stick and Keyboard Inputs)
        move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Gets the game manager script access for the variable
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();

        //Gets audio manager
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        //Gets player animator
        animController = GetComponentInChildren<Animator>();


        //Initalises the game being over as false
        gameOver = false;

        //Getting access to power up slider
        powerUpBar = GameObject.FindGameObjectWithTag("PowerUpBar").GetComponent<PowerUpBar>();
        powerUpBar.SetMaxValue(swordPowerUpTimer);

        //Makes sure player dead is set to false
        isPlayerDead = false;

        //Gets Sword object and disables it on start up
        swordAttatchedToPlayer = GameObject.FindWithTag("SwordInHand");
        swordAttatchedToPlayer.SetActive(false);

       
    }

    //Move Player
    public void MovePlayer()
    {
        //Creates a Vector for the movement read from the Input of the player
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        //Applies the movement to the player keeping track of the world space and time
        transform.Translate(movement * speed * Time.deltaTime, Space.World);


        //Rotating player towards direction, In a if statement so the rotation doesnt RESET when there is no movement
        if (movement != Vector3.zero) {
        //Rotates the player towards the direction, the number at the end controls the SPEED of the rotation
         transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), 1000f * Time.deltaTime);
         //Plays running animation if moving
            animController.SetBool("IsRunning", true);
        }
        else
        {
            //Plays the IDLE animation if not running
            animController.SetBool("IsRunning", false);
        }
    }

    public void PowerUpTimer()
    {
    swordPowerUpTimer -=Time.deltaTime;
    powerUpBar.SetCurrentValue(swordPowerUpTimer);

        if (swordPowerUpTimer <= 0)
        {
            //Takes the sword out of the players hand
            swordAttatchedToPlayer.SetActive(false);

            //Changes music back to original score
            
            audioManager.ChangeBGMusic(audioManager.backgroundMusic);
            audioManager.PlaySFX(audioManager.powerdownSound);

            gameManagerScript.doesPlayerHaveSword = false;
            hasSwordAnimPlayed = false;
            swordPowerUpTimer = 10;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if the animation for him laying down has played before sending a game over to the game manager script
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Die01_SwordAndShield") && animController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            gameOver = true;
        }

        //Calls the function to move the player
        if (!playerFrozen)
        {
            MovePlayer();
        }

        if (gameManagerScript.doesPlayerHaveSword)
        {
            //Starts a timer for how long the player has the sword for
            PowerUpTimer();

            //Checks if the animation for attacking has played, and if it has finished playing it switches back to the Idle / Running animation
            if (animController.GetCurrentAnimatorStateInfo(0).IsName("Attack04_SwordAndShield") && animController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
            {
                playerFrozen = false;
                animController.SetBool("AttackAnimHasPlayed", true);
            }

            //Checks if sword anim has played on pick up, if not it plays
            if (hasSwordAnimPlayed == false && gameManagerScript.doesPlayerHaveSword)
            {
                playerFrozen = true;
                animController.Play("LevelUp_Battle_SwordAndShield");
                hasSwordAnimPlayed = true;

                //Enables the sword in the players hand
                swordAttatchedToPlayer.SetActive(true);
            }

            //Once animation is detected as finished, it will switch back into its transitionable states
            if (animController.GetCurrentAnimatorStateInfo(0).IsName("LevelUp_Battle_SwordAndShield") && animController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99 && hasSwordAnimPlayed == true)
            {
                animController.Play("Idle_Battle_SwordAndShield");
                playerFrozen = false;
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && gameManagerScript.doesPlayerHaveSword == false && !isPlayerDead)
        {
            animController.Play("Die01_SwordAndShield");
            isPlayerDead = true;
            playerFrozen = true;
            audioManager.PlaySFX(audioManager.playerDeathSound);

        

        }

        //Handles Animation if they collide with enemy and have sword
        //Checks if it collides with the enemy. If the player has a sword. The player is not dead. And if the enemy it is colliding with is not ALREADY dead (To avoid spamming for points)
        if (collision.gameObject.CompareTag("Enemy") && gameManagerScript.doesPlayerHaveSword == true && !isPlayerDead && collision.gameObject.GetComponent<EnemyAI>().isGoblinDead == false)
        {
            //Plays death sound
            audioManager.PlaySFX(audioManager.enemyDeathSound);

            //Plays a spin attack
            animController.SetBool("AttackAnimHasPlayed", false);
            animController.Play("Attack04_SwordAndShield");

            //Adds 100 points for killing a goblin
            gameManagerScript.currentScore = gameManagerScript.currentScore + 100;
        }

    }

}
