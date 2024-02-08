using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 move;

    public void OnMove(InputAction.CallbackContext context)
    {
        //This function is Invoked through the Input manager
        //Takes the Input from the new Input System (Left stick and Keyboard Inputs)
        move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    //Move Player
    public void MovePlayer()
    {
        //Creates a Vector for the movement read from the Input of the player
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        //Applies the movement to the player keeping track of the world space and time
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        //Calls the function to move the player
        MovePlayer();
    }
}
