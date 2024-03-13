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

    public float speed;
    private Vector2 move;

    //Time he carries the sword for once picked up
    private float swordPowerUpTimer = 10f;

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
        //Gets the game manager script access for the variable
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();

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

        if (swordPowerUpTimer <= 0)
        {
            gameManagerScript.doesPlayerHaveSword = false;
            hasSwordAnimPlayed = false;
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

            //Checks if sword anim has played on pick up, if not it plays
            if (hasSwordAnimPlayed == false && gameManagerScript.doesPlayerHaveSword)
            {
                playerFrozen = true;
                animController.Play("LevelUp_Battle_SwordAndShield");
                hasSwordAnimPlayed = true;
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
        if (collision.gameObject.CompareTag("Enemy") && gameManagerScript.doesPlayerHaveSword == false)
        {
            Destroy(gameObject);
            Debug.Log("You have died");
        }
        //EDIT THIS NEXT PLS. THIS IS FOR THE ANIMATION OF THE COLLISION WITH A SWORD WITH TTHE PLAYER
        //if (collision.gameObject.CompareTag("Enemy") && gameManagerScript.doesPlayerHaveSword == true)
       // {
        //    animController.Play("Attack04_SwordAndShield");
       //    Debug.Log("We Have collided with a sword");
       // }

    }

}
