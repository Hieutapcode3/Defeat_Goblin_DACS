using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int gold = 800;
    public int maxSlots = 12;
    public int currentSlots = 4;
    public int currentLevel = 1;
    public EntityLevel characterLevelMax = EntityLevel.Level_10;
    public EntityLevel currentPurchasableCharacter;
    public EntityLevel petLevelMax = EntityLevel.Level_6;
    public EntityLevel currentPurchasablePet;
    public int purchasedCharacterCount = 0;
    public int purchasedPetCount = 0;
    public List<SlotData> slotDatas = new List<SlotData>();
    public List<EntityLevel> unlockedCharacterLevels = new List<EntityLevel>();
    public List<EntityLevel> unlockedPetLevels = new List<EntityLevel>();

    public Dictionary<EntityType, Dictionary<EntityLevel, int>> GetEntityDataByTypeAndLevel()
    {
        Dictionary<EntityType, Dictionary<EntityLevel, int>> entitySummary = new Dictionary<EntityType, Dictionary<EntityLevel, int>>();

        foreach (var slot in slotDatas)
        {
            if (slot.entityData == null) continue;

            EntityData entity = slot.entityData;
            EntityType type = entity.type;
            EntityLevel level = entity.level;

            if (!entitySummary.ContainsKey(type))
            {
                entitySummary[type] = new Dictionary<EntityLevel, int>();
            }
            if (!entitySummary[type].ContainsKey(level))
            {
                entitySummary[type][level] = 0;
            }

            entitySummary[type][level]++;
        }

        return entitySummary;
    }
    public void UnlockCharacterLevel(EntityLevel level)
    {
        if (!unlockedCharacterLevels.Contains(level))
        {
            if ((int)level >= (int)EntityLevel.Level_5)
            {
                currentPurchasableCharacter++;
            }
            unlockedCharacterLevels.Add(level);
            SaveSystem.SaveGame(this);
        }
    }
    public void UnlockPetLevel(EntityLevel level)
    {
        if (!unlockedPetLevels.Contains(level))
        {
            if ((int)level >= (int)EntityLevel.Level_5)
            {
                currentPurchasablePet++;
            }
            unlockedPetLevels.Add(level);
            SaveSystem.SaveGame(this);
        }
    }
    public bool HasUnlockedCharacterLevel(EntityLevel level)
    {
        return unlockedCharacterLevels.Contains(level);
    }
    public bool HasUnlockedPetLevel(EntityLevel level)
    {
        return unlockedPetLevels.Contains(level);
    }


    public GameData() { }

    public void AddSlot()
    {
        if (currentSlots < maxSlots)
        {
            currentSlots++;
            SaveSystem.SaveGame(this);
        }
    }
    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            gold += amount;
            SaveSystem.SaveGame(this);
        }
    }

    public bool SubtractGold(int amount)
    {
        if (amount > 0 && gold >= amount)
        {
            gold -= amount;
            SaveSystem.SaveGame(this);
            return true;
        }
        return false;
    }
    public void UpdatePurchasedCharacterCount()
    {
        purchasedCharacterCount++;
        SaveSystem.SaveGame(this);
    }
    public void UpdatePurchasedPetCount()
    {
        purchasedPetCount++;
        SaveSystem.SaveGame(this);
    }
    // public int GetCharacterPrice()
    // {
    //     int basePrice = 100;
    //     int price = basePrice;

    //     for (int i = 1; i <= purchasedCharacterCount; i++)
    //     {
    //         price += (i % 2 == 1) ? 27 : 28;
    //     }
    //     int characterLevel = (int)currentPurchasableCharacter + 1;
    //     if (characterLevel > 1)
    //     {
    //         price = Mathf.RoundToInt(price * 2.25f);

    //         int additionalPurchases = purchasedCharacterCount - GetInitialCharacterCountForLevel();
    //         if (additionalPurchases > 0)
    //         {
    //             price += additionalPurchases * 62;
    //         }
    //     }
    //     return price;
    // }
    private int GetInitialCharacterCountForLevel()
    {
        int price = 100;
        int count = 0;

        // while (true)
        // {
        //     int nextPrice = price + ((count % 2 == 0) ? 27 : 28);
        //     if (Mathf.RoundToInt(nextPrice * 2.25f) >= 1462) break;
        //     price = nextPrice;
        //     count++;
        // }
        while (true)
        {
            int nextPrice = price + ((count % 2 == 0) ? 27 : 28);

            if (nextPrice >= 1400)
            {
                nextPrice = Mathf.RoundToInt(nextPrice * 2.25f);
            }

            if (nextPrice >= 1462) break;

            price = nextPrice;
            count++;
        }

        return count;
    }
    public int GetCharacterPrice()
    {
        int basePrice = 100;
        int price = basePrice;

        for (int i = 1; i <= purchasedCharacterCount; i++)
        {
            price += (i % 2 == 1) ? 27 : 28;
        }

        int multiplierCount = (int)currentPurchasableCharacter;

        for (int i = 0; i < multiplierCount; i++)
        {
            price = Mathf.RoundToInt(price * 2.25f);
        }

        int additionalPurchases = purchasedCharacterCount - GetInitialCharacterCountForLevel();
        if (additionalPurchases > 0)
        {
            price += additionalPurchases * 62;
        }

        return price;
    }


    // public int GetPetPrice()
    // {
    //     int basePrice = 250;
    //     int price = basePrice;

    //     for (int i = 1; i <= purchasedPetCount; i++)
    //     {
    //         price += (i % 2 == 1) ? 27 : 28;
    //     }

    //     return price;
    // }
    public int GetPetPrice()
    {
        int basePrice = 250;
        int price = basePrice;

        for (int i = 1; i <= purchasedPetCount; i++)
        {
            price += (i % 2 == 1) ? 312 : 313;

            // if (price >= 5250 && price < 12600) price = Mathf.RoundToInt(price * 2.4f);
            // else if (price >= 18600 && price < 44600) price = Mathf.RoundToInt(price * 2.4f);
        }
        int multiplierCount = (int)currentPurchasablePet;

        for (int i = 0; i < multiplierCount; i++)
        {
            price = Mathf.RoundToInt(price * 2.4f);
        }
        return price;
    }

    // public void UpdatePurchasableCharacter()
    // {
    //     if (currentPurchasableCharacter < CharacterLevel.Level_10)
    //     {
    //         currentPurchasableCharacter++;
    //         SaveSystem.SaveGame(this);
    //     }
    // }

    public SlotData AssignCharacterToSlot(SlotData slotData, EntityData charData, bool isSave = true)
    {
        slotData.entityData = charData;
        slotDatas.Add(slotData);
        if (isSave) SaveSystem.SaveGame(this);
        return slotData;
    }
    public SlotData GetSlotDataByIndex(int index)
    {
        return slotDatas.Find(slot => slot.index == index);
    }
}
