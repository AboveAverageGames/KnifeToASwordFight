using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animController;
    private bool hasSwordAnimPlayed;
    private bool hasAttackAnimPlayed;
    private bool playerFrozen;
    private bool IsPlayerDead;



    AudioManager audioManager;

    private GameObject swordAttatchedToPlayer;

    public float speed;
    private Vector2 move;

    //Time he carries the sword for once picked up
    private float swordPowerUpTimer = 10f;
    public PowerUpBar powerUpBar;

    //Accessing the game manager script
    public GameManagerScript gameManagerScript;

    public void OnMove(InputAction.CallbackContext context)
    {
        //This function is Invoked through the Input manager
        //Takes the Input from the new Input System (Left stick and Keyboard Inputs)
        move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {

        //Getting access to power up slider
        powerUpBar = GameObject.FindGameObjectWithTag("PowerUpBar").GetComponent<PowerUpBar>();
        powerUpBar.SetMaxValue(swordPowerUpTimer);

        //Makes sure player dead is set to false
        IsPlayerDead = false;

        //Gets Sword object and disables it on start up
        swordAttatchedToPlayer = GameObject.FindWithTag("SwordInHand");
        swordAttatchedToPlayer.SetActive(false);

        //Gets the game manager script access for the variable
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();

        //Gets audio manager
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.PlaySFX(audioManager.UISound);

        //Gets player animator
        animController = GetComponentInChildren<Animator>();
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
                Debug.Log("Animation DONE");
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
        if (collision.gameObject.CompareTag("Enemy") && gameManagerScript.doesPlayerHaveSword == false && !IsPlayerDead)
        {
            animController.Play("Die01_SwordAndShield");
            IsPlayerDead = true;
            playerFrozen = true;

            audioManager.PlaySFX(audioManager.playerDeathSound);
            
        }

        //Handles Animation if they collide with enemy and have sword
        if (collision.gameObject.CompareTag("Enemy") && gameManagerScript.doesPlayerHaveSword == true && !IsPlayerDead)
        {
            animController.SetBool("AttackAnimHasPlayed", false);
            animController.Play("Attack04_SwordAndShield");
        }

    }

}
