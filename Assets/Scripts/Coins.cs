using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int pointsItemIsWorth = 10;

    public GameManagerScript gameManagerScript;

    private void OnCollisionEnter(Collision collision)
    {
        //If Coin collides with player then it is destroyed + Updates the Score in the GameManagerScript
        if (collision.gameObject.CompareTag("Player"))
            {
            gameManagerScript.totalCoinsCollected++;
            Destroy(gameObject);
            gameManagerScript.UpdateScore(pointsItemIsWorth);
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Assigns the GameManagerScript to this object so I can access it when needed
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
