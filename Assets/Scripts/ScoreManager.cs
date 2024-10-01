using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreTxt;
    public int score;

    void Update()
    {
        ScoreTxt.text = "SCORE: " + score;
    }
    public void Score(int scoreIncrease)
    {
        score += scoreIncrease;
    }
}
