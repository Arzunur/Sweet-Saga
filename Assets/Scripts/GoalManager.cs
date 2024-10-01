using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BlankGoal
{

    public Sprite goalSprite;
    public int levelGoal; //hedef numberNeeded

    public int collected;//toplanan
    public string goalTag;//matchValue
}
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoal;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    private EndGame endGame;

    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject goalIntroParent;
    [SerializeField] private GameObject goalGameParent;

   private void Start()
    {
        SetupGoal();
        endGame = FindObjectOfType<EndGame>();

    }

    private void SetupGoal()
    {
        for (int i = 0; i < levelGoal.Length; i++)
        {
            //Crete a new Goal Panel at the goalIntroParent position
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform);

            GoalPanel panel= goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoal[i].goalSprite;
            panel.goalTag = "0/" + levelGoal[i].levelGoal;

            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoal[i].goalSprite;
            panel.goalTag = "0/" + levelGoal[i].levelGoal;
        }
    }
    public void UpdateGoals()
    {
        int goalCompleted = 0;
        for (int i = 0; i < levelGoal.Length; i++)
        {
            currentGoals[i].goalText.text = "" + levelGoal[i].collected + "/" + levelGoal[i].levelGoal;
            if (levelGoal[i].collected >= levelGoal[i].levelGoal)
            {
                goalCompleted++;
                currentGoals[i].goalText.text = "" + levelGoal[i].levelGoal + "/" + levelGoal[i].levelGoal;
            }
        }
        if(goalCompleted>= levelGoal.Length)
        {
            if (endGame!=null)
            {
                endGame.WinGame();
            }
            Debug.Log("winn");
        }
    }
    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoal.Length; i++)
        {
            if (goalToCompare == levelGoal[i].goalTag)
            {
                levelGoal[i].collected++;
            }
        }
    }
    public void OKBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
