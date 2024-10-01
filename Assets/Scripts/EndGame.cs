using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameType  //oyunun bitis türü
{
    Moves,
    Time
}
[System.Serializable]
public class EndGameRequirement
{
    public GameType gameType;
    public int CounterValue;
}
public class EndGame : MonoBehaviour
{
    public EndGameRequirement requirement; //oyun sonu gereksinimler
    private Board board;

    private float timerSecond;
    [SerializeField] private GameObject movesTxt;
    [SerializeField] private GameObject timeTxt;
    [SerializeField] private GameObject youWin;
    [SerializeField] private GameObject tryAgain;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private int currentCounterValue;
    [SerializeField] private GameObject winPanel;

    public void Start()
    {
        board = FindObjectOfType<Board>();
        SetUpGame();
    }
    private void Update()
    {
        if (requirement.gameType == GameType.Time && currentCounterValue > 0)
        {
            UpdateTimer();

        }
    }
    private void UpdateTimer()
    {
        timerSecond -= Time.deltaTime;
        if (timerSecond <= 0)
        {
            DecreaseCounterValue();
            timerSecond = 1;
        }
    }
    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = currentCounterValue.ToString();

            if (currentCounterValue <= 0) LoseGame();
        }
    }
    public void WinGame()
    {
        youWin.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();

    }
    private void LoseGame()
    {
        tryAgain.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("lose");
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();
    }
    private void SetUpGame()
    {
        currentCounterValue = requirement.CounterValue;
        if (requirement.gameType == GameType.Moves)
        {
            movesTxt.SetActive(true);
            timeTxt.SetActive(false);
        }
        else
        {
            timerSecond = 1;
            movesTxt.SetActive(false);
            timeTxt.SetActive(true);
        }
        counter.text = currentCounterValue.ToString();
    }
    public void WinGameOKBtn()
    {
        winPanel.SetActive(false);
        SceneManager.LoadScene("MainMenuScene"); //Level ekranýna yönlendirilebilir.
    }
    public void TryAgainOKBtn()
    {
        tryAgain.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }
}
