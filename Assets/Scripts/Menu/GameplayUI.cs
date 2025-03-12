
using TMPro;
using UnityEngine;

public class GameplayUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI levelText;
    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
    }
    public override void Show()
    {
        base.Show();
        if (levelText) levelText.text = "LEVEL " + LevelManager.Instance.currentLevelIndex;
    }
    public override void Hide(System.Action onComplete = null)
    {
        base.Hide(onComplete);
    }
}