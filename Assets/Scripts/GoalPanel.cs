using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public Sprite thisSprite;
    public string goalTag;
    public TextMeshProUGUI goalText;

    [SerializeField] private Image goalImage;
    void Start()
    {
        SetUp();
    }
    public void SetUp()
    {
        goalImage.sprite = thisSprite;
        goalText.text = goalTag;
    }
}
