using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private int numberOfCoins;
    public int totalCoinsCollected;
    public int currentScore;
    public GameObject GameOverScreen;
    public GameObject LevelCompleteScreen;
    public bool hasPlayerBeatHighScoreThisRun;

   PlayerController playerController;

    public bool doesPlayerHaveSword;

    public TMPro.TextMeshProUGUI gameOverScreenScore;


    // Start is called before the first frame update
    void Start()
    {

        currentScore = 0;  

        //Makes sure the game is not paused
        Time.timeScale = 1;

        //Finds the total number of coins in the scene at the start of the level
        numberOfCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        //Gets Reference to player controller script
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        //If statement to check if ALL THE COINS in the level have been collected
        if (totalCoinsCollected == numberOfCoins)
        {
            Time.timeScale = 0;
            LevelCompleteScreen.SetActive(true);
        }

        //If Statement seeing if player is dead
        if (playerController.gameOver)
        { 
            gameOverScreenScore.text = currentScore.ToString(); 
            GameOverScreen.SetActive(true);
            Time.timeScale = 0;
        }

        //Updates high score if current score is higher then highscore
        if (currentScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            //Sets a bool that can be accessed via the GAME UI to display if the player beat the high score on this run!
            hasPlayerBeatHighScoreThisRun = true;

            PlayerPrefs.SetInt("HighScore", currentScore);


        }
    }

    //Updates the score, using the amount of points a collected item is worth
    public void UpdateScore(int points)
    {
        //Updates the current score with the INT Points that is passed into the function
        currentScore = currentScore + (points);
        
    }
}
