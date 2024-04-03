using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameManagerScript gameManagerScript;
    public int pointsItemIsWorth;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        //If Coin collides with player then it is destroyed + Updates the Score in the GameManagerScript
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManagerScript.doesPlayerHaveSword = true;
            audioManager.PlaySFX(audioManager.powerupSound);
            audioManager.ChangeBGMusic(audioManager.battleMusic);
            Destroy(gameObject);
            gameManagerScript.UpdateScore(pointsItemIsWorth);

        }
    }
}
