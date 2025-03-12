using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityManager : BaseSingleton<EntityManager>
{
    private List<EntityData> characterDatas = new List<EntityData>();
    private List<EntityData> petDatas = new List<EntityData>();
    private List<EntityData> ownedCharacters = new List<EntityData>();
    private GameData gameData;
    private HomeUI homeUI;
    private Button btnBuyChar;
    private Button btnBuyPet;
    private ParticleImage GoldDecreaseParticle;
    public bool isBuyChar { get; private set; } = false;
    public bool isBuyPet { get; private set; } = false;
    public void LoadOwnedCharacter(GameData gameData)
    {
        this.homeUI = UIManager.Instance.GetUI<HomeUI>();
        this.gameData = gameData;

        this.btnBuyChar = homeUI.btnBuyChar;
        this.btnBuyPet = homeUI.btnBuyPet;
        this.GoldDecreaseParticle = homeUI.GoldDecreaseParticle;

        this.characterDatas = new List<EntityData>(GameManager.Instance.gameDataCollection.characterDataList);
        this.petDatas = new List<EntityData>(GameManager.Instance.gameDataCollection.petDataList);

        btnBuyChar.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(gameData.GetCharacterPrice());
        btnBuyPet.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(gameData.GetPetPrice());
        UpdateCurrentPurchasableCharacter();
        UpdateCurrentPurchasablePet();
        btnBuyChar.onClick.AddListener(() =>
        {
            if (UIManager.Instance.IsValidState() || isBuyChar || isBuyPet || SlotManager.Instance.isBuySlot) return;
            AnimationManager.Instance.OnButtonClick(
                btnBuyChar,
                () =>
                {
                    SlotController slotController = SlotManager.Instance.GetFirstEmptySlot();
                    if (slotController != null)
                    {
                        int price = gameData.GetCharacterPrice();
                        if (GoldManager.Instance.SpendGold(price, GoldDecreaseParticle))
                        {
                            isBuyChar = true;
                            EntityData characterData = GetCharacterDataByLevel(gameData.currentPurchasableCharacter);
                            //slotController.slotData.charData = characterData;
                            slotController.slotData = gameData.AssignCharacterToSlot(slotController.slotData, characterData, false);

                            gameData.UpdatePurchasedCharacterCount();

                            GoldDecreaseParticle.attractorTarget = btnBuyChar.transform;
                            GoldDecreaseParticle.onFirstParticleFinish.AddListener(() =>
                            {
                                SlotManager.Instance.UpdateSlot(slotController.slotData.index);
                                btnBuyChar.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(gameData.GetCharacterPrice());
                                isBuyChar = false;

                            });
                            GoldDecreaseParticle.onLastParticleFinish.AddListener(() =>
                            {
                                isBuyChar = false;
                            });
                        }
                    }
                });
        });
        btnBuyPet.onClick.AddListener(() =>
        {
            if (UIManager.Instance.IsValidState() || isBuyChar || isBuyPet || SlotManager.Instance.isBuySlot) return;
            AnimationManager.Instance.OnButtonClick(
                btnBuyPet,
                () =>
                {
                    SlotController slotController = SlotManager.Instance.GetFirstEmptySlot();
                    if (slotController != null)
                    {
                        int price = gameData.GetPetPrice();
                        if (GoldManager.Instance.SpendGold(price, GoldDecreaseParticle))
                        {
                            isBuyPet = true;
                            EntityData petData = GetPetDataByLevel(gameData.currentPurchasablePet);
                            //slotController.slotData.charData = characterData;
                            slotController.slotData = gameData.AssignCharacterToSlot(slotController.slotData, petData, false);

                            gameData.UpdatePurchasedPetCount();

                            GoldDecreaseParticle.attractorTarget = btnBuyPet.transform;
                            GoldDecreaseParticle.onFirstParticleFinish.AddListener(() =>
                            {
                                SlotManager.Instance.UpdateSlot(slotController.slotData.index);
                                btnBuyPet.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(gameData.GetPetPrice());
                                isBuyPet = false;
                            });
                            GoldDecreaseParticle.onLastParticleFinish.AddListener(() =>
                            {
                                //isBuyPet = false;
                            });
                        }
                    }
                });
        });
    }
    public EntityData GetCharacterDataByLevel(EntityLevel level)
    {
        return characterDatas.FirstOrDefault(character => character.level == level);
    }
    public EntityData GetPetDataByLevel(EntityLevel level)
    {
        return petDatas.FirstOrDefault(pet => pet.level == level);
    }
    public void UpdateCurrentPurchasableCharacter()
    {
        EntityData characterData = GetCharacterDataByLevel(gameData.currentPurchasableCharacter);
        btnBuyChar.GetComponentInChildren<RawImage>().texture = characterData.renderTexture;
        btnBuyChar.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(gameData.GetCharacterPrice());
    }

    public void UpdateCurrentPurchasablePet()
    {
        EntityData characterData = GetCharacterDataByLevel(gameData.currentPurchasableCharacter);
        btnBuyChar.GetComponentInChildren<RawImage>().texture = characterData.renderTexture;
        btnBuyPet.GetComponentInChildren<TextMeshProUGUI>().text = GameDataManager.Instance.FormatPrice(gameData.GetPetPrice());
    }
}
