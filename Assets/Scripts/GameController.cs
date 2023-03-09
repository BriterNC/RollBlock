using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] public bool isPaused;
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace))
        {
            if (!isPaused)
            {
                isPaused = true;
                pauseUI.SetActive(true);
                Time.timeScale = 0;
            }
            else if (isPaused)
            {
                isPaused = false;
                pauseUI.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
