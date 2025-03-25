using UnityEngine;
using TMPro;
using AssetKits.ParticleImage;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class GoldManager : BaseSingleton<GoldManager>
{
    private Image goldGroup;
    private TextMeshProUGUI goldText;
    private TextMeshProUGUI goldPerSecondTxt;
    private GameData gameData;
    private int displayGold;
    private int targetGold;
    private Coroutine updateGoldCoroutine;

    private Image notEnoughGoldsText;
    private bool isShowingNotEnoughGold = false;

    public void LoadGold(GameData data)
    {
        HomeUI homeUI = UIManager.Instance.GetUI<HomeUI>();
        this.goldGroup = homeUI.goldGroup;
        this.notEnoughGoldsText = homeUI.notEnoughGoldsText;
        this.goldText = homeUI.goldText;
        this.gameData = data;
        this.goldPerSecondTxt = homeUI.goldEarnInSecond;

        targetGold = gameData.gold;
        displayGold = gameData.gold;
        UpdateUI();
    }
    public void UpdateGoldPerSecondTxt()
    {
        int goldAmount = SlotManager.Instance.GetGoldEarnInSecondAmout();
        goldPerSecondTxt.text = GameDataManager.Instance.FormatPrice(goldAmount) + "/Sec";
    }

    public void AddGold(int amount, ParticleImage particleImage = null)
    {
        if (gameData == null) return;
        gameData.AddGold(amount);
        targetGold = gameData.gold;
        StartGoldUpdate();
        PlayGoldEffect(particleImage);
    }

    public bool SpendGold(int amount, ParticleImage particleImage = null)
    {
        if (gameData.SubtractGold(amount))
        {
            targetGold = gameData.gold;
            StartGoldUpdate();
            PlayGoldEffect(particleImage);
            ShowGoldPopup(amount, new Vector3(0, 0.4f, 0));

            return true;
        }
        ShowNotEnoughGoldsText();
        return false;
    }
    private void ShowNotEnoughGoldsText()
    {
        if (isShowingNotEnoughGold) return;

        isShowingNotEnoughGold = true;
        notEnoughGoldsText.gameObject.SetActive(true);
        RectTransform rectTransform = notEnoughGoldsText.rectTransform;
        TextMeshProUGUI textComponent = notEnoughGoldsText.GetComponentInChildren<TextMeshProUGUI>();

        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.localScale = Vector3.zero;
        textComponent.alpha = 0;

        rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        textComponent.DOFade(1, 0.5f);

        rectTransform.DOAnchorPosY(100f, 1f).SetEase(Ease.OutQuad).SetDelay(0.5f);
        textComponent.DOFade(0, 1f).SetEase(Ease.InQuad).SetDelay(0.5f).OnComplete(() =>
        {
            notEnoughGoldsText.gameObject.SetActive(false);
            isShowingNotEnoughGold = false;
        });
    }



    private void ShowGoldPopup(int amount, Vector3 offset)
    {
        if (ResourceManager._GoldTextPrefab == null || goldGroup == null) return;

        Canvas canvas = goldGroup.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, goldGroup.transform.position);
        Vector3 worldPosition;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        }
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out worldPosition);
        }

        worldPosition.z = 0;

        GameObject goldPopup = PoolManager.Instance.Get(ResourceManager._GoldTextPrefab, worldPosition);

        TextMeshProUGUI popupText = goldPopup.GetComponentInChildren<TextMeshProUGUI>();
        if (popupText != null)
        {
            popupText.text = $"-{GameDataManager.Instance.FormatPrice(amount)}";
        }

        goldPopup.transform.DOMove(worldPosition - offset, 1f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => goldPopup.gameObject.SetActive(false));
    }


    private void PlayGoldEffect(ParticleImage particleImage)
    {
        if (particleImage == null) return;
        particleImage.onFirstParticleFinish.RemoveAllListeners();
        particleImage.onLastParticleFinish.RemoveAllListeners();
        if (particleImage.isPlaying)
        {
            particleImage.Stop();
        }
        particleImage.Play();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddGold(5000);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpendGold(100);
        }
    }
    private void StartGoldUpdate()
    {
        if (updateGoldCoroutine != null)
        {
            StopCoroutine(updateGoldCoroutine);
        }
        updateGoldCoroutine = StartCoroutine(UpdateGoldOverTime());
    }

    private IEnumerator UpdateGoldOverTime()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (displayGold != targetGold)
        {
            elapsed += Time.deltaTime;

            float goldPerSecond = (targetGold - displayGold) / (duration - elapsed);

            if (elapsed >= duration || Mathf.Abs(displayGold - targetGold) < 1)
            {
                displayGold = targetGold;
            }
            else
            {
                displayGold += Mathf.RoundToInt(goldPerSecond * Time.deltaTime);
            }

            UpdateUI();
            yield return null;
        }

        displayGold = targetGold;
        UpdateUI();
        updateGoldCoroutine = null;
    }



    private void UpdateUI()
    {
        goldText.text = GameDataManager.Instance.FormatPrice(displayGold);
    }

    public int GetGold()
    {
        return gameData != null ? gameData.gold : 0;
    }
}
