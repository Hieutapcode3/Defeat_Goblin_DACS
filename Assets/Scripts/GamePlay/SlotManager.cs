using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using AssetKits.ParticleImage;
using UnityEngine.UI;
using DG.Tweening;

public class SlotManager : BaseSingleton<SlotManager>
{
    [SerializeField] private Transform parentObject;
    private List<SlotController> slots = new List<SlotController>();
    private GameData gameData;

    public bool isBuySlot { get; private set; } = false;
    private ParticleImage GoldDecreaseParticle;
    private HomeUI homeUI;
    protected override void Awake()
    {
        base.Awake();
        FindSlots();
    }


    public void LoadSlot(GameData gameData)
    {
        this.homeUI = UIManager.Instance.GetUI<HomeUI>();
        this.GoldDecreaseParticle = homeUI.GoldDecreaseParticle;

        this.gameData = gameData;
        for (int i = 0; i < slots.Count(); i++)
        {
            UpdateSlot(i + 1);
        }

        if (gameData.currentSlots == gameData.maxSlots)
        {
            homeUI.btnBuySlot.interactable = false;
            homeUI.btnBuySlot.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
        }
        else
        {
            homeUI.btnBuySlot.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(GameManager.Instance.gameDataCollection.slotPrices[gameData.currentSlots]);
            homeUI.btnBuySlot.onClick.AddListener(() =>
            {
                if (UIManager.Instance.IsValidState() || isBuySlot || EntityManager.Instance.isBuyChar || EntityManager.Instance.isBuyPet) return;
                AnimationManager.Instance.OnButtonClick(
                    homeUI.btnBuySlot,
                    () =>
                    {
                        AddSlot(homeUI.btnBuySlot);
                        AudioManager.Instance.PlaySoundEffect(SoundEffect.Buy);
                        if (gameData.currentSlots == gameData.maxSlots)
                        {
                            homeUI.btnBuySlot.interactable = false;
                            homeUI.btnBuySlot.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
                        }
                    });
            });
        }

    }
    private void FindSlots()
    {
        slots.Clear();
        slots = parentObject.GetComponentsInChildren<SlotController>(true).ToList();
    }
    public void UpdateSlot(int index)
    {
        SlotController slot = slots[index - 1];
        slot.Init(index <= gameData.currentSlots, index, gameData.GetSlotDataByIndex(index)?.entityData);
        // Transform activeSlot = slot.Slot_Active;
        // Transform flag = slot.Flag;
        if (index <= gameData.currentSlots)
        {
            if (slot.currentEntity != null)
            {
                Destroy(slot.currentEntity.gameObject);
            }
            if (slot.slotData.entityData != null)
            {
                slot.SpawnNewEntity();
            }
            else
            {
                slot.Empty();
            }
        }
        else
        {
            slot.Occupied();
        }
        GoldManager.Instance.UpdateGoldPerSecondTxt();
    }

    public void UpdateEntity(int index)
    {
        SlotController slot = slots[index - 1];
        slot.slotData.index = index;
        slot.slotData.entityData = gameData.GetSlotDataByIndex(index)?.entityData;

        slot.currentEntity.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Destroy(slot.currentEntity.gameObject);
                var newEffect = VisualEffect.VFX_Confetti.Play();
                newEffect.transform.position = slot.transform.position;
                slot.SpawnNewEntity();
            });
    }

    public void AddSlot(Button bt)
    {
        if (gameData.currentSlots < gameData.maxSlots)
        {
            if (GoldManager.Instance.SpendGold(GameManager.Instance.gameDataCollection.slotPrices[gameData.currentSlots], GoldDecreaseParticle))
            {
                isBuySlot = true;
                gameData.AddSlot();
                GoldDecreaseParticle.attractorTarget = bt.transform;
                GoldDecreaseParticle.onFirstParticleFinish.AddListener(() =>
                {
                    UpdateSlot(gameData.currentSlots);
                    if (gameData.currentSlots < gameData.maxSlots)
                        homeUI.btnBuySlot.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(GameManager.Instance.gameDataCollection.slotPrices[gameData.currentSlots]);
                    isBuySlot = false;
                });
                // GoldDecreaseParticle.onLastParticleFinish.AddListener(() =>
                // {
                //     isBuySlot = false;
                // });
            }

        }
    }
    public SlotController GetFirstEmptySlot()
    {
        return slots
            .Where(slot => slot.slotData.entityData == null && slot.slotData.index <= gameData.currentSlots)
            .OrderBy(slot => slot.slotData.index)
            .FirstOrDefault();
    }
    public int GetGoldEarnInSecondAmout()
    {
        int count = 0;
        foreach(SlotController slot in slots)
        {
            if (slot.IsBuy && slot.currentEntity != null && slot.slotData.entityData != null)
            {
                count += slot.currentEntity.entityData.goldPerSecond;
            }
        }
        return count;
    }
}
