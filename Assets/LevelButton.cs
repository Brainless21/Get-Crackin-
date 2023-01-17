using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour
{
    [SerializeField] string levelIndex;
    [SerializeField] LevelLoader loader;
    public Button levelButton;
    string levelName;
    private void Start() 
    {
        levelButton.onClick.AddListener(LoadLevel);
    }
    void LoadLevel()
    {
        levelName = "Level ";
        levelName += levelIndex;
        loader.LoadLevel(levelName);
    }


}
