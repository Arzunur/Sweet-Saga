using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadePanelController : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button okButton;
    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private float offScreenPositionY = -500f;
    [SerializeField] private float onScreenPositionY = 0f;

    private void Start()
    {
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, offScreenPositionY);
        panel.DOAnchorPosY(onScreenPositionY, animationDuration);
        okButton.onClick.AddListener(OkButtonClicked);
    }

    private void OkButtonClicked()
    {
        panel.DOAnchorPosY(offScreenPositionY, animationDuration);
    }
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
