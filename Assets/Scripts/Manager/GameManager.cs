using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : BaseSingleton<GameManager>
{

    public bool isWin { get; private set; } = false;
    public bool isLose { get; private set; } = false;
    public bool isInBattle { get; private set; }
    public GameDataCollection gameDataCollection;
    [SerializeField] private GameObject HomeObjects;
    public CinemachineVirtualCamera cinemachineVirtual;
    public Transform centerTransform;
    public GameData gameData { get; private set; }
    private Button btnFight;
    private Image blackScreen;
    private float defaultOrthoSize;
    private EntityMerger entityMerger;
    void Start()
    {
        this.entityMerger = GetComponentInParent<EntityMerger>();
        this.blackScreen = UIManager.Instance.blackScreen;
        this.btnFight = UIManager.Instance.GetUI<HomeUI>().btnFight;
        Vector3 camPos = this.transform.position;
        camPos.z = -10;
        cinemachineVirtual.transform.position = camPos;
        cinemachineVirtual.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = null;
        defaultOrthoSize = cinemachineVirtual.m_Lens.OrthographicSize;
        LoadGame();

        this.btnFight.onClick.AddListener(() =>
        {
            Battle();
        });
    }
    private void LoadGame()
    {
        gameData = SaveSystem.LoadGame();
        GoldManager.Instance.LoadGold(this.gameData);
        SlotManager.Instance.LoadSlot(this.gameData);
        EntityManager.Instance.LoadOwnedCharacter(this.gameData);
        EntityMerger.Instance.LoadCharacterMerge(this.gameData);
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Battle()
    {
        if (UIManager.Instance.IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            btnFight,
            () =>
            {
                entityMerger.enabled = false;
                ShowBlackScreen(
                    true,
                    () =>
                    {
                        isInBattle = true;
                        HomeObjects.SetActive(false);
                        LevelManager.Instance.LoadMap(gameData.currentLevel);
                        StartCoroutine(LevelController.Instance.GenerateMapCoroutine(onComplete: () =>
                        {
                            cinemachineVirtual.LookAt = LevelController.Instance.cameraFollow;
                            cinemachineVirtual.Follow = LevelController.Instance.cameraFollow;

                            cinemachineVirtual.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = LevelController.Instance.mapLimit;

                            UIManager.Instance.ChangeUI(UIScreen.Battle);
                            EntitySpawner.Instance.LoadBattle(this.gameData);
                            HideBlackScreen();
                        }));
                        // HideBlackScreen(true, () =>
                        // {
                        //     entityMerger.enabled = true;
                        //     UIManager.Instance.ChangeUI(UIScreen.Home);
                        // });
                    });
            });
    }
    private void ShowBlackScreen(bool adjustOrthoSize = false, Action complete = null)
    {
        blackScreen.gameObject.SetActive(true);
        blackScreen.transform.localScale = Vector3.zero;
        blackScreen.color = new Color(0, 0, 0, 0);

        if (adjustOrthoSize)
        {
            float newSize = defaultOrthoSize * 2;
            DOTween.To(() => cinemachineVirtual.m_Lens.OrthographicSize, x => cinemachineVirtual.m_Lens.OrthographicSize = x, newSize, 2f)
                .SetEase(Ease.InOutSine);
        }

        blackScreen.transform.DOScale(Vector3.one * 200, 1f).SetEase(Ease.InOutSine);
        blackScreen.DOFade(1f, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                complete?.Invoke();
            });
    }
    public void HideBlackScreen(bool adjustOrthoSize = false, Action complete = null)
    {
        if (adjustOrthoSize)
        {
            DOTween.To(() => cinemachineVirtual.m_Lens.OrthographicSize, x => cinemachineVirtual.m_Lens.OrthographicSize = x, defaultOrthoSize, 2f)
                .SetEase(Ease.InOutSine);
        }
        blackScreen.transform.DOScale(Vector3.zero, 2f).SetEase(Ease.InOutSine);
        blackScreen.DOFade(0f, 2f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                blackScreen.gameObject.SetActive(false);
                complete?.Invoke();
            });
    }
    public void Home(Button bt)
    {
        Time.timeScale = 1;
        if (UIManager.Instance.IsValidState()) return;
        AnimationManager.Instance.OnButtonClick(
            bt,
            () =>
            {

                UIManager.Instance.curentOverlayUI.Hide(() =>
                {
                    UIManager.Instance.GetUI<BattleUI>().Hide();
                });

                ShowBlackScreen(false,
                    () =>
                    {
                        isInBattle = false;
                        ClearMap();
                        EntitySpawner.Instance.ClearEntities();
                        cinemachineVirtual.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);
                        LevelManager.Instance.ClearMap();
                        HomeObjects.gameObject.SetActive(true);
                        HideBlackScreen(true, () =>
                        {
                            entityMerger.enabled = true;
                            UIManager.Instance.ChangeUI(UIScreen.Home);
                        });
                    });
            });

    }

    private void ClearMap()
    {
        foreach (Transform child in centerTransform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void ResetStatus()
    {
        isWin = false;
        isLose = false;
    }
    public void WinGame(float delay = 0f)
    {
        isWin = true;
        if (delay > 0f) StartCoroutine(DelayedWinGame(delay));
        else ExecuteWinGame();
    }

    public void LoseGame(float delay = 0f)
    {
        isLose = true;
        if (delay > 0f) StartCoroutine(DelayedLoseGame(delay));
        else ExecuteLoseGame();
    }

    private void ExecuteWinGame()
    {
        int levelUnLock = PlayerPrefsHelper.GetInt(PlayerDataKey.UnlockedLevel, 0);
        if (LevelManager.Instance.currentLevelIndex > levelUnLock)
        {
            PlayerPrefsHelper.SetInt(PlayerDataKey.UnlockedLevel, LevelManager.Instance.currentLevelIndex);
            PlayerPrefsHelper.Save();
        }
        UIManager.Instance.ChangeUI(UIScreen.Win);
        AudioManager.Instance.PlaySoundEffect(SoundEffect.Win);
    }

    private void ExecuteLoseGame()
    {
        // int HighScore = PlayerPrefsHelper.GetInt(PlayerDataKey.HighScore);
        // if (ScoreController.Instance.GetScore() > HighScore) {
        //     PlayerPrefsHelper.SetInt(PlayerDataKey.HighScore,ScoreController.Instance.GetScore());
        //     PlayerPrefsHelper.Save();
        // }
        UIManager.Instance.ChangeUI(UIScreen.Lose);
        AudioManager.Instance.PlaySoundEffect(SoundEffect.Lose);
    }

    private IEnumerator DelayedWinGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExecuteWinGame();
    }

    private IEnumerator DelayedLoseGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExecuteLoseGame();
    }


    [ContextMenu("ResetPlayerfabs")]
    private void ResetPlayerfabs()
    {
        PlayerPrefs.DeleteAll();
    }
}
