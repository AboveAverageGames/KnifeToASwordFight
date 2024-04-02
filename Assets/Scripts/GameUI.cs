using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    GameManagerScript gameManagerScript;
    public TMPro.TextMeshProUGUI scoreNumberText;


    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreNumberText.text = (gameManagerScript.currentScore + "");
    }
}
