using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : BaseUI
{
    [SerializeField] private GameObject levelGroup;
    public bool isHideLevelText = false;
    public bool isChangeLevelTextColor = false;
    [HideInInspector] public Color lockTextColor;
    [HideInInspector] public Color unLockTextColor;
    private List<Button> buttonLevels;
    private Dictionary<Button, UnityEngine.Events.UnityAction> buttonListeners;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        InitLevelButton();
    }

    private void InitLevelButton()
    {
        buttonLevels = levelGroup.GetComponentsInChildren<Button>().ToList();
        buttonListeners = new Dictionary<Button, UnityEngine.Events.UnityAction>();

        for (int i = 0; i < buttonLevels.Count; i++)
        {
            int index = i;

            string levelText = (index + 1) < 10 ? "0" + (index + 1) : (index + 1).ToString();
            buttonLevels[index].GetComponentInChildren<TextMeshProUGUI>().text = levelText;


            UnityEngine.Events.UnityAction action = () =>
            {
                if (uiManager.IsValidState()) return;
                AnimationManager.Instance.OnButtonClick(
                    buttonLevels[index],
                    () =>
                    {
                        UIManager.Instance.LoadLevel(index + 1);
                    });
            };

            buttonListeners[buttonLevels[index]] = action;
            buttonLevels[index].onClick.AddListener(action);
        }
    }
    private void LoadStatusLevelButton()
    {
        int levelAt = PlayerPrefs.GetInt(PlayerDataKey.UnlockedLevel.ToString(), 0);
        int index = 0;
        float delay = 0.2f;
        foreach (var levelButton in buttonLevels)
        {
            levelButton.onClick.RemoveAllListeners();
            var textComponent = levelButton.GetComponentInChildren<TextMeshProUGUI>(true);

            if (index > levelAt)
            {
                levelButton.GetComponent<Image>().sprite = ResourceManager._LevelLock;
                if (isHideLevelText)
                {
                    textComponent.gameObject.SetActive(false);
                }
                if (isChangeLevelTextColor)
                {
                    textComponent.color = lockTextColor;
                }
                levelButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance.PlaySoundEffect(SoundEffect.Cancel);
                    AnimationManager.Instance.ShakeTransform(levelButton.transform);
                });
            }
            else
            {
                levelButton.GetComponent<Image>().sprite = ResourceManager._LevelUnLock;

                if (isHideLevelText)
                {
                    textComponent.gameObject.SetActive(true);
                }
                if (isChangeLevelTextColor)
                {
                    textComponent.color = unLockTextColor;
                }
                levelButton.onClick.AddListener(buttonListeners[levelButton]);
            }

            AnimationManager.Instance.ShowLevelItem(levelButton, 0.2f, delay);

            index++;
            delay += 0.05f;
        }
    }
    public override void Show()
    {
        LoadStatusLevelButton();
        base.Show();

    }

    public override void Hide(Action onComplete = null)
    {
        base.Hide(onComplete);
    }
}


