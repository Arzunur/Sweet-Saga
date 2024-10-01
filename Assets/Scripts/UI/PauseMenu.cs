using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private Board board;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private bool paused = false;
    [SerializeField] private Image soundImg;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Sprite pauseSprite; 
    [SerializeField] private Sprite playSprite;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        pausePanel.SetActive(false);
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") ==0 )
            {
                soundImg.sprite = musicOff;
                pauseButtonImage.sprite = playSprite;

            }
            else 
            { 
            soundImg.sprite = musicOn;
                pauseButtonImage.sprite = pauseSprite;


            }
        }
        else
        {
            soundImg.sprite = musicOn;
            pauseButtonImage.sprite = pauseSprite;

        }
    }

    private void Update()
    {
        if (paused && !pausePanel.activeInHierarchy )   
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
            pauseButtonImage.sprite = playSprite;
        }
        if (!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
            pauseButtonImage.sprite = pauseSprite;

        }
    }
    public void SoundBtn()
    {
        pausePanel.SetActive(false);
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundImg.sprite = musicOn;
                PlayerPrefs.SetInt("Sound", 1); 
            }
            else
            {
                soundImg.sprite = musicOff;
                PlayerPrefs.SetInt("Sound", 0);
            }
        }
        else
        {
            soundImg.sprite = musicOff;
            PlayerPrefs.SetInt("Sound", 1);
        }

    }

    public void PauseGame()
    {
        paused =!paused;
        if (paused)
        {
           
            Time.timeScale = 0; 
            pausePanel.SetActive(true); 
            board.currentState = GameState.pause;
            pauseButtonImage.sprite = playSprite;
        }
        else
        {
          
            Time.timeScale = 1; 
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
            pauseButtonImage.sprite = pauseSprite;
        }
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenuScene");
        Time.timeScale = 1;
    }
}
