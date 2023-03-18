using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam0, cam1;
    [SerializeField] private GameObject pauseUI, player, invisibleTile;
    [SerializeField] private RectTransform title, GUI;
    [SerializeField] public bool isPaused, hiding, despawning;
    
    private void Update()
    {
        if (cam0.isActiveAndEnabled)
        {
            return;
        }
        
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

    public void StartGame()
    {
        player.transform.GetComponent<PlayerController>().enabled = true;
        cam0.gameObject.SetActive(false);
        cam1.gameObject.SetActive(true);
            
        StartCoroutine(HideTitle());
        Destroy(invisibleTile);
    }

    public void Credits()
    {
        StartCoroutine(DespawnCube());
    }

    private IEnumerator HideTitle()
    {
        if (hiding)
        {
            yield break;
        }
        
        hiding = true;
        
        var timer = 0f;
        var setTime = 0.2f;
        var angleToRotate = 90;
        
        while (timer < setTime)
        {
            timer += Time.deltaTime;
            title.rotation = Quaternion.Euler(new Vector3(( timer / setTime ) * angleToRotate, 0, 0));
            yield return null;
        }
        
        title.rotation = Quaternion.Euler(new Vector3(angleToRotate, 0, 0));
        GUI.gameObject.SetActive(true);
        
        title.gameObject.SetActive(false);
    }

    private IEnumerator DespawnCube()
    {
        if (despawning)
        {
            yield break;
        }
        
        despawning = true;
        
        player.transform.GetComponent<Rigidbody>().useGravity = false;
        
        var timeToDespawn = 1f;
        var currentTimer = 0f;

        var playerScale = player.transform.GetComponent<PlayerController>().playerScale;

        while (currentTimer <= timeToDespawn)
        {
            timeToDespawn -= Time.deltaTime;
            player.transform.localScale = new Vector3(timeToDespawn * playerScale, timeToDespawn * playerScale, timeToDespawn * playerScale);
            yield return null;
        }
        
        player.transform.localScale = Vector3.zero;
        
        SceneManager.LoadScene("Sandbox");
    }
}
