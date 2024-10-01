using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    void Start()
    {
        ButtonEffect();
    }
    private void ButtonEffect()
    {
        quitButton.onClick.AddListener(() =>
        {
            quitButton.transform.DOScale(0.8f, 0.2f).SetEase(Ease.OutBounce);
        });
    }
    public void PlayButton()
    {
        SceneManager.LoadScene("GoalScene");
    }
    public void QuitButton()
    {
        Application.Quit();
    }
  
}
