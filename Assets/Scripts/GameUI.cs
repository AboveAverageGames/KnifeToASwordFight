using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    GameManagerScript gameManagerScript;
    AudioManager audioManager;

    public TMPro.TextMeshProUGUI scoreNumberText;
    public TMPro.TextMeshProUGUI highScoreNumberText;




    // Start is called before the first frame update
    void Start()
    {

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();


        //Updates the current score to the saved score
        gameManagerScript.UpdateScore(PlayerPrefs.GetInt("CurrentScoreSaved"));

    }

    // Update is called once per frame
    void Update()
    {
        scoreNumberText.text = (gameManagerScript.currentScore + "");

        highScoreNumberText.text = (PlayerPrefs.GetInt("HighScore") + "");
    }


    public void NextLevel()
    {
        //Sets the current score saved to the current score achieved this level
        PlayerPrefs.SetInt("CurrentScoreSaved", gameManagerScript.currentScore);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //Increase difficulty here
    }

    public void RestartGame()
    {

        //Resets the current score saved
        PlayerPrefs.DeleteKey("CurrentScoreSaved");
        Debug.Log(PlayerPrefs.GetInt("CurrentScoreSaved"));
        gameManagerScript.currentScore = 0;
        //Loads the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        //Resets the current score saved
        PlayerPrefs.DeleteKey("CurrentScoreSaved");
        Debug.Log(PlayerPrefs.GetInt("CurrentScoreSaved"));
        gameManagerScript.currentScore = 0;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }

}
