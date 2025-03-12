using System;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    protected UIManager uiManager;
    protected bool isShow = false;
    public virtual void Init(UIManager uIManager)
    {
        uiManager = uIManager;
        isShow = gameObject.activeSelf;
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (!isShow)
        {
            isShow = true;
            AnimationManager.Instance.ShowMainUI(this);
        }
        //Debug.Log($"{gameObject.name} is shown");
    }

    public virtual void Hide(Action onComplete = null)
    {
        if (isShow)
        {
            isShow = false;
            AnimationManager.Instance.HideMainUI(this, 0.25f, 0.25f, () =>
            {
                //Debug.Log($"{gameObject.name} is hidden");
                onComplete?.Invoke();
            });
        } else {
            onComplete?.Invoke();
        }
    }
}
