using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;

    public TMPro.TextMeshProUGUI highScoreNumberText;

  

    public void Start()
    {
        Time.timeScale = 1f;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    public void Update()
    {
        highScoreNumberText.text = (PlayerPrefs.GetInt("HighScore") + "");
    }
    public void PlayGame()
    {

        //Resets the current score saved
        PlayerPrefs.DeleteKey("CurrentScoreSaved");
        //Resets the current Level
        PlayerPrefs.SetInt("CurrentLevel", 1);

        PlayerPrefs.SetInt("CurrentScoreSaved", 0);
        //Updates the score if it already holds a value
   
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        //Resets the current score saved
        PlayerPrefs.DeleteKey("CurrentScoreSaved");
        //Resets the level
        PlayerPrefs.SetInt("CurrentLevel", 1);

        PlayerPrefs.SetInt("CurrentScoreSaved", 0);

        Application.Quit();

    }
}
