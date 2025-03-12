using System;
using UnityEngine.UI;

public class WinUI : BaseUI
{
    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
    }
    public override void Show()
    {
        base.Show();
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.name == "NextBtn")
            {
                if (LevelManager.Instance.IsLastLevel())
                {
                    btn.interactable = false;

                }
                else
                {
                    btn.interactable = true;
                }
                break;
            }
        }
    }

    public override void Hide(Action onComplete = null)
    {
        base.Hide(onComplete);
    }
}