using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelProgression : MonoBehaviour
{
   [SerializeField] int scoreThreshold;
   [SerializeField] Button tryAgainButton;
   [SerializeField] Button nextLevelButton;
   [SerializeField] int nextLevelIndex;
   public TextMeshProUGUI scoreText;
   TextMeshProUGUI levelMessage;
  int score;

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        tryAgainButton = buttons[0];
        nextLevelButton = buttons[1];
        scoreText = texts[1];
        levelMessage = texts[0];
        gameObject.SetActive(false);

        tryAgainButton.onClick.AddListener(TryAgain);
        nextLevelButton.onClick.AddListener(ProceedToNextLevel);
    }

    public static LevelProgression instance;

    void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }

    public void InvokeActivation(int score)
    {
        this.score = score;
        Invoke("Activate", 0.7f);
    }
    public void Activate()
    {
       
        gameObject.SetActive(true);
        string levelMessageText = "Not Enough Points";
        nextLevelButton.gameObject.SetActive(false);
        

        string scoreTextText = "You Scored\n";
        scoreTextText += score.ToString();
        scoreTextText += " / ";
        scoreTextText += scoreThreshold.ToString();
        scoreText.text = scoreTextText;

        if(score>=scoreThreshold)
        {
            levelMessageText = "Level Complete!";
            nextLevelButton.gameObject.SetActive(true);
        }


        levelMessage.text = levelMessageText;
        
    }

    void ProceedToNextLevel()
    {
        string levelName = "Level ";
        levelName += nextLevelIndex;
        //SceneManager.LoadScene(levelName);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void TryAgain()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        MouseCursor.instance.isEnabled = true;
    }
    private void OnEnable()
    {
        MouseCursor.instance.isEnabled = false;
    }
}
