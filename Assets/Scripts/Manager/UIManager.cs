using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseSingleton<UIManager>
{
    [Header("-----UI-----")]
    [SerializeField] private GameObject uiParent;

    [SerializeField] private List<Button> buttonVolumes;
    public Image blackScreen;
    private Dictionary<UIScreen, BaseUI> screens;
    public BaseUI currentMainUI { get; private set; }
    public BaseUI curentOverlayUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        blackScreen.gameObject.SetActive(false);
        InitializeUIDictionary();
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.U))
    //     {
    //         ChangeUI(UIScreen.Battle);
    //         EntitySpawner.Instance.LoadBattle();
    //     }
    //     if (Input.GetKeyDown(KeyCode.H))
    //     {
    //         ChangeUI(UIScreen.Home);
    //     }
    // }


    private void Start()
    {
        foreach (KeyValuePair<UIScreen, BaseUI> kvp in screens)
        {
            BaseUI baseUIValue = kvp.Value;
            baseUIValue.Hide(() =>
            {
                if (kvp.Key == ResourceManager._StartUI)
                {
                    ChangeUI(ResourceManager._StartUI);
                }
            });
        }

    }
    public void ShowUnlockNewLevelUI(Texture _old, Texture _new)
    {
        GetUI<UnlockNewLevelUI>().SetIcon(_old,_new);
        ChangeUI(UIScreen.UnlockNewLevel);
    }
    public void HideUnlockNewLevelUI(Button bt)
    {
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                ChangeUI(UIScreen.Home);

            });
    }
    public void PauseGame(Button bt)
    {
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                ChangeUI(UIScreen.Pause);
                Time.timeScale = 0f;
            });
    }
    public void ResumeGame(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                if (!GameManager.Instance.isInBattle)
                    ChangeUI(UIScreen.Home);
                else
                    ChangeUI(UIScreen.Battle);
            });
    }
    public void RetryGame(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState() || GameManager.Instance.isLose || GameManager.Instance.isWin) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                ChangeUI(UIScreen.Gameplay);
                LoadLevel(LevelManager.Instance.currentLevelIndex);
            });
    }
    public void NextLevel(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState()) return;
        if (LevelManager.Instance.IsLastLevel())
        {
            AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                LoadLevel(1);
                ChangeUI(UIScreen.Gameplay);
            });
            //Debug.Log("Is Last Level");
            return;
        }
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                LoadLevel(LevelManager.Instance.currentLevelIndex + 1);
                ChangeUI(UIScreen.Gameplay);
            });
    }


    public void LoadSceneHome(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                LevelManager.Instance.ClearMap();
                ChangeUI(UIScreen.Home);
            });
    }
    public void LoadSceneSelectLevel(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                LevelManager.Instance.ClearMap();
                ChangeUI(UIScreen.Level);
            });
    }
    public void ExitGame(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                Application.Quit();
            });
    }


    public void PlayGame(Button bt)
    {
        Time.timeScale = 1f;
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                int currentLevel = PlayerPrefs.GetInt(PlayerDataKey.UnlockedLevel.ToString(), 1);
                LevelManager.Instance.LoadMap(currentLevel);
                ChangeUI(UIScreen.Gameplay);
            });
    }
    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        ChangeUI(UIScreen.Gameplay);
        LevelManager.Instance.LoadMap(levelIndex);
    }

    
    public void ToggleMute(Button bt)
    {
        if (IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {
                if (AudioManager.Instance.isMuted)
                {
                    AudioManager.Instance.ToggleAllAudio();
                    foreach (var item in buttonVolumes)
                    {
                        if (item != null) item.GetComponent<Image>().sprite = ResourceManager._UnMute;
                    }
                }
                else
                {
                    AudioManager.Instance.ToggleAllAudio();
                    foreach (var item in buttonVolumes)
                    {
                        if (item != null) item.GetComponent<Image>().sprite = ResourceManager._Mute;
                    }
                }
            });
    }
    public bool IsValidState()
    {
        if (AnimationManager.Instance.IsAnimating(UIAnimationState.ButtonClick)
            || AnimationManager.Instance.IsAnimating(UIAnimationState.ShowMainUI)
            || AnimationManager.Instance.IsAnimating(UIAnimationState.HideMainUI)) return true;
        return false;
    }
    private void InitializeUIDictionary()
    {
        screens = new Dictionary<UIScreen, BaseUI>();

        foreach (Transform child in uiParent.transform)
        {
            if (System.Enum.TryParse(child.name, out UIScreen uiName))
            {
                var uiComponent = child.GetComponent<BaseUI>();
                if (uiComponent != null)
                {
                    screens[uiName] = uiComponent;
                    uiComponent.Init(this);
                }
                else
                {
                    Debug.LogWarning($"No BaseUI component found on {child.name}. Skipping.");
                }
            }
            else
            {
                Debug.LogWarning($"Child name {child.name} does not match any UIName enum value. Skipping.");
            }
        }
    }

    public void ChangeUI(UIScreen ui)
    {
        BaseUI newUI = GetScreen(ui);

        if (ui == UIScreen.Pause || ui == UIScreen.Lose || ui == UIScreen.Win || ui == UIScreen.UnlockNewLevel)
        {
            curentOverlayUI = newUI;
            curentOverlayUI.Show();
            return;
        }

        if (currentMainUI != null)
        {
            if (curentOverlayUI)
            {
                curentOverlayUI.Hide(() =>
                {
                    curentOverlayUI = null;
                });
            }

            if (currentMainUI != newUI)
            {
                currentMainUI.Hide(() =>
                {
                    currentMainUI = newUI;
                    currentMainUI.Show();
                });
            }
            else
            {
                currentMainUI.Show();
            }
        }
        else
        {
            currentMainUI = newUI;
            currentMainUI.Show();
        }
    }
    public BaseUI GetScreen(UIScreen uiName)
    {
        if (screens.TryGetValue(uiName, out BaseUI screen))
        {
            return screen;
        }
        return null;
    }
    public T GetUI<T>() where T : BaseUI
    {
        foreach (var screen in screens.Values)
        {
            if (screen is T typedScreen)
            {
                return typedScreen;
            }
        }
        Debug.LogWarning($"UI of type {typeof(T).Name} not found.");
        return null;
    }


}
