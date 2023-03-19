using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam0, cam1;
    [SerializeField] public GameObject pauseUI, player, invisibleTile, instruction;
    [SerializeField] private PlayerController pc;
    [SerializeField] private RectTransform title, GUI;
    [SerializeField] public bool isPaused, hiding, despawning;
    [SerializeField] public bool isTutorial, blockInputR, blockInputWASD, blockInputQE, blockInputSpace, blockInputEscape;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] public int tutorialPhrase, fallCount;
    [SerializeField] private GameObject grid1, grid2, grid3, grid4;
    [SerializeField] public GameObject teleporter1, teleporter2;
    [SerializeField] public bool teleporter1IsActive, teleporter2IsActive;
    [SerializeField] private string nextSceneString;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform.GetChild(1).gameObject;
        pc = player.GetComponent<PlayerController>();
        instructionText = instruction.transform.GetChild(0).transform.GetComponent<TMP_Text>();
        tutorialPhrase = 1;
        if (isTutorial)
        {
            blockInputR = true;
            blockInputWASD = true;
            blockInputQE = true;
            blockInputSpace = true;
            blockInputEscape = true;
        }
    }

    private void Update()
    {
        if (cam0.isActiveAndEnabled)
        {
            return;
        }
        
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (blockInputEscape)
            {
                return;
            }

            if (!isPaused)
            {
                isPaused = true;
                pauseUI.SetActive(true);
                Time.timeScale = 0;
                GUI.gameObject.SetActive(false);
                instruction.SetActive(false);
            }
            else if (isPaused)
            {
                isPaused = false;
                pauseUI.SetActive(false);
                Time.timeScale = 1;
                GUI.gameObject.SetActive(true);
                instruction.SetActive(true);
            }
        }
        
        if (isTutorial)
        {
            if (instruction.gameObject.activeSelf)
            {
                if (tutorialPhrase == 1)
                {
                    instructionText.text = "Hello and welcome to RollBlock!\n(Yes, it's Demo.)";
                
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 2)
                {
                    instructionText.text = "Try pressing\nW / A / S / D / Arrows\nto move around.";
                    blockInputWASD = false;
                    
                    if (fallCount == 1)
                    {
                        grid1.SetActive(false);
                        grid2.SetActive(true);
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 3)
                {
                    instructionText.text = "Ah.. I forgot to mention\nthat you could fall off.\nBut, no problem~!";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 4)
                {
                    instructionText.text = "Because I've created\na box for you, so you\ncan't fall down anymore.";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 5)
                {
                    instructionText.text = "But wait..\nThen..";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 6)
                {
                    instructionText.text = "How are you going to\nproceed to next stage!?";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 7)
                {
                    instructionText.text = "Okay, I this might help..\nTry pressing Q / E\nto look around.";
                    blockInputWASD = true;
                    blockInputQE = false;
                    
                    if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 8)
                {
                    instructionText.text = "Oh nice~!\nNow you could see!";
                    blockInputQE = true;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 9)
                {
                    if (!pc.isOnTeleporterTile)
                    {
                        tutorialPhrase = 10;
                    }
                    else if (pc.isOnTeleporterTile)
                    {
                        tutorialPhrase = 11;
                    }
                }
                else if (tutorialPhrase == 10)
                {
                    instructionText.text = "See that red button?\nTry stepping on it.";
                    blockInputWASD = false;
                    
                    if (pc.isOnTeleporterTile)
                    {
                        tutorialPhrase = 15;
                    }
                }
                else if (tutorialPhrase == 11)
                {
                    instructionText.text = "Oh? Are you standing on\ntop of something there?";
                    blockInputWASD = true;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 12)
                {
                    instructionText.text = "Could you step out\nfrom that tile?";
                    blockInputWASD = false;
                    
                    if (!pc.isOnTeleporterTile)
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 13)
                {
                    instructionText.text = "Ah yes~!\nIt's really a Teleporter!";
                    blockInputWASD = true;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 14)
                {
                    instructionText.text = "Come on!\nTry it out!";
                    blockInputWASD = false;
                    
                    if (pc.isOnTeleporterTile)
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 15)
                {
                    instructionText.text = "This button..\nIt's broken huh..?";
                    blockInputWASD = true;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 16)
                {
                    instructionText.text = "Okay.. I think..\nI could fix this.";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 17)
                {
                    instructionText.text = "Let's me fix this\nreal quick..";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 18)
                {
                    instructionText.text = "Okay! Done!";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 19)
                {
                    instructionText.text = "Could you try\nsomething for me?";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 20)
                {
                    instructionText.text = "Try pressing\nSpace Bar\nto test it out.";
                    blockInputSpace = false;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 21)
                {
                    instructionText.text = "Oh! Nice!\nIt's seem to work now!";
                    blockInputSpace = true;
                    blockInputQE = false;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 22)
                {
                    instructionText.text = "Hmm..?\nIsn't that some ice?";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 23)
                {
                    instructionText.text = "Why don't you try\nice-skating on it?";
                    blockInputWASD = false;
                    blockInputSpace = false;
                    
                    if (fallCount == 2)
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 24)
                {
                    instructionText.text = "Wow.. I don't know\nthat you're good at\nice-skating that much.";
                    
                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 25)
                {
                    instructionText.text = "Oops! I got a call.\nJust hang around ok?";
                    
                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 26)
                {
                    instructionText.text = "...";
                    
                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 27)
                {
                    instructionText.text = "Just so you know..";
                    
                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 28)
                {
                    instructionText.text = "You can press ESC\nto pause the game\nwhile waiting for me.";
                    blockInputEscape = false;
                    
                    if (Input.GetKeyUp(KeyCode.Escape))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 29)
                {
                    instructionText.text = "Oh, welcome back!\nI've also just\nfinished my call too.";
                    
                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 30)
                {
                    instructionText.text = "Okay..\nWhere were we again..?";
                    
                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 31)
                {
                    instructionText.text = "Oh yes! The next stage!";

                    if (teleporter1IsActive || teleporter2IsActive)
                    {
                        blockInputSpace = true;
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                            blockInputSpace = false;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            tutorialPhrase++;
                        }
                    }
                }
                else if (tutorialPhrase == 32)
                {
                    instructionText.text = "Go activate Teleporter\nand have a look!\nThere's a new destination.";

                    if ((teleporter1IsActive || teleporter2IsActive) && Input.GetKey(KeyCode.Space))
                    {
                        tutorialPhrase++;
                        pc.ResetCube();
                        grid2.SetActive(false);
                        grid3.SetActive(true);
                    }
                }
                else if (tutorialPhrase == 33)
                {
                    instructionText.text = "...";
                    blockInputSpace = false;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 34)
                {
                    instructionText.text = "Oops! Wrong stage..\nSorry about that.";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 35)
                {
                    instructionText.text = "I've set\na new destination now..";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 36)
                {
                    instructionText.text = "But wait..\nThere's no Teleporter..";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 37)
                {
                    instructionText.text = "Hmm.. Oh I know!";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 38)
                {
                    instructionText.text = "Try pressing R\nto reset your character.";
                    blockInputR = false;
                    
                    if (Input.GetKeyUp(KeyCode.R))
                    {
                        tutorialPhrase++;
                        grid3.SetActive(false);
                        grid4.SetActive(true);
                    }
                }
                else if (tutorialPhrase == 39)
                {
                    instructionText.text = "Yes, that's does reset\nyour character and\nyour roll counts too.";
                    blockInputWASD = true;
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 40)
                {
                    instructionText.text = "See that hole over there?";
                    
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        tutorialPhrase++;
                    }
                }
                else if (tutorialPhrase == 41)
                {
                    instructionText.text = "Try to get into it.\nIt's going to bring you\nto the next stage.\nGood luck.";
                    blockInputWASD = false;
                }
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
        instruction.gameObject.SetActive(true);
        
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
        var tempTime = timeToDespawn;
        var currentTimer = 0f;

        var playerScale = player.transform.GetComponent<PlayerController>().playerScale;

        while (currentTimer <= timeToDespawn)
        {
            timeToDespawn -= Time.deltaTime;
            if (timeToDespawn !< 0)
            {
                player.transform.localScale = new Vector3(timeToDespawn * playerScale, timeToDespawn * playerScale, timeToDespawn * playerScale);
            }
            yield return null;
        }
        
        player.transform.localScale = Vector3.zero;
        
        SceneManager.LoadScene("Sandbox");
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneString);
    }
}
