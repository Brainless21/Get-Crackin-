using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelLoader : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public Button optionsButton;
    public Button returnToMenuButtonLevel;
    public Button returnToMenuButtonSettings;

    public GameObject mainMenuPage;
    public GameObject levelSelectPage;
    public GameObject optionsMenuPage;

    GameObject activePage;

    public static LevelLoader instance;
    
    private void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        // DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        startButton.onClick.AddListener(OpenStartMenu);
        exitButton.onClick.AddListener(ExitApplication);
        optionsButton.onClick.AddListener(OpenOptionsMenu);
        returnToMenuButtonLevel.onClick.AddListener(ReturnToMenu);
        returnToMenuButtonSettings.onClick.AddListener(ReturnToMenu);
    }

    private void ReturnToMenu()
    {
        activePage.SetActive(false);
        mainMenuPage.SetActive(true);
        activePage = mainMenuPage;
    }

    private void OpenOptionsMenu()
    {
        mainMenuPage.SetActive(false);
        optionsMenuPage.SetActive(true);
        activePage = optionsMenuPage;
    }

    private void ExitApplication()
    {
        Debug.Log("shutting down....\n *beep boop* \n goodnight");
        Application.Quit();
    }

    private void OpenStartMenu()
    {
        mainMenuPage.SetActive(false);
        levelSelectPage.SetActive(true);
        activePage = levelSelectPage;
    }

    public void LoadLevel(string levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}
