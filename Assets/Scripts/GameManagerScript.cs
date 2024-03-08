using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private int numberOfCoins;
    public int totalCoinsCollected;
    public int currentScore;

    public bool doesPlayerHaveSword;


    // Start is called before the first frame update
    void Start()
    {
        //Finds the total number of coins in the scene at the start of the level
        numberOfCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    // Update is called once per frame
    void Update()
    {
        //If statement to check if ALL THE COINS in the level have been collected
        if (totalCoinsCollected == numberOfCoins)
        {
            Debug.Log("You got all the coins");
        }
        
    }

    //Updates the score, using the amount of points a collected item is worth
    public void UpdateScore(int points)
    {
        //Updates the current score with the INT Points that is passed into the function
        currentScore = currentScore + (points);
        Debug.Log("The current score is" +  currentScore + " The total coins collected is " + totalCoinsCollected);
    }
}
