using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public int pointsItemIsWorth;

    public GameManagerScript gameManagerScript;

    public AudioManager audioManager;



    void Start()
    {
        //Assigns the GameManagerScript to this object so I can access it when needed
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();

        //Assings the Audio manager so I can access it
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        //If Coin collides with player then it is destroyed + Updates the Score in the GameManagerScript
        if (collision.gameObject.CompareTag("Player"))
        {
            //Plays SFX On collection
            audioManager.PlaySFX(audioManager.coinCollection);

            //Updates the coin on game manager script if collectible is a coin
            if (gameObject.tag == "Coin")
            {
                gameManagerScript.totalCoinsCollected++;
            }

            //De activating object instead of destroying for object pooling
            gameObject.SetActive(false);
            gameManagerScript.UpdateScore(pointsItemIsWorth);
            
        }
    }
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
    }
}
