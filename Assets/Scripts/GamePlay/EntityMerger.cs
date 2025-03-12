using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EntityMerger : BaseSingleton<EntityMerger>
{
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private LayerMask groundLayer;
    private BaseEntity selectedEntity;
    private GameObject cloneCharacter;
    private GameData gameData;
    private SlotController currentSlot = null;
    public void LoadCharacterMerge(GameData gameData)
    {
        this.gameData = gameData;
    }
    private Dictionary<SpriteRenderer, float> originalAlphaValues = new Dictionary<SpriteRenderer, float>();
    private void Update()
    {
        HandleTouch();
    }
    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                TrySelectCharacter(touchPosition);
            }
            else if (touch.phase == TouchPhase.Moved && cloneCharacter != null)
            {
                MoveCloneCharacter(touchPosition);
            }
            else if (touch.phase == TouchPhase.Ended && cloneCharacter != null)
            {
                TryMergeOrCancel();
            }
        }
    }
    private void TrySelectCharacter(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, entityLayer);

        foreach (var hit in hits)
        {
            BaseEntity entity = hit.GetComponent<BaseEntity>();
            if (entity != null)
            {
                selectedEntity = entity;
                CreateCloneCharacter();
                SetAlpha(selectedEntity.gameObject, true);
                break;
            }
        }
    }

    private void CreateCloneCharacter()
    {
        if (selectedEntity == null) return;

        //cloneCharacter = PoolManager.Instance.Get(selectedEntity.gameObject, selectedEntity.transform.position);
        cloneCharacter = Instantiate(selectedEntity.gameObject, selectedEntity.transform.position, Quaternion.identity);
        SpriteRenderer[] renderers = cloneCharacter.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
        {
            sr.sortingOrder += 66;
        }
    }

    void MoveCloneCharacter(Vector2 position)
    {
        cloneCharacter.transform.position = position;
        Collider2D[] hitGrounds = Physics2D.OverlapPointAll(position, groundLayer);

        foreach (var hit in hitGrounds)
        {
            SlotController slot = hit.GetComponent<SlotController>();
            if (slot != null && slot != selectedEntity.slot && slot.IsBuy)
            {
                if (currentSlot != null)
                {
                    currentSlot.UnSelectedSlot();
                }

                currentSlot = slot;
                currentSlot.SelectedSlot();
                return;
            }
        }
        if (currentSlot != null)
        {
            currentSlot.UnSelectedSlot();
            currentSlot = null;
        }
    }
    public void TryMergeOrCancel()
    {
        if (currentSlot == null)
        {
            CancelMerge();
            return;
        }

        SlotData currentSlotData = currentSlot.slotData;
        SlotData selectedSlotData = selectedEntity.slot.slotData;

        if (currentSlotData.entityData == null || currentSlot.currentEntity == null)
        {
            SlotData updatedSlot = gameData.AssignCharacterToSlot(currentSlotData, selectedEntity.entityData, false);
            gameData.slotDatas.Remove(gameData.GetSlotDataByIndex(selectedSlotData.index));

            SaveSystem.SaveGame(gameData);
            SlotManager.Instance.UpdateSlot(updatedSlot.index);
            SlotManager.Instance.UpdateSlot(selectedSlotData.index);
        }
        else
        {
            // BaseEntity targetEntity = currentSlot.currentEntity;

            // if (targetEntity.GetType() == selectedEntity.GetType() &&
            //     targetEntity.entityData.level == selectedEntity.entityData.level)
            // {
            //     if (selectedEntity is Character character1 && targetEntity is Character character2)
            //     {
            //         ProcessMerge(character1, character2, EntityManager.Instance.GetCharacterDataByLevel);
            //     }
            //     else if (selectedEntity is Pet pet1 && targetEntity is Pet pet2)
            //     {
            //         ProcessMerge(pet1, pet2, EntityManager.Instance.GetPetDataByLevel);
            //     }
            //     return;
            // }
            // else
            // {
            //     (currentSlotData.entityData, selectedSlotData.entityData) = (selectedSlotData.entityData, currentSlotData.entityData);

            //     SaveSystem.SaveGame(gameData);
            //     SlotManager.Instance.UpdateSlot(currentSlotData.index);
            //     SlotManager.Instance.UpdateSlot(selectedSlotData.index);
            // }


            BaseEntity targetEntity = currentSlot.currentEntity;

            if (targetEntity.entityData.type != selectedEntity.entityData.type ||
            targetEntity.entityData.level != selectedEntity.entityData.level ||
            (targetEntity.entityData.type == EntityType.Pet && targetEntity.entityData.level == EntityLevel.Level_6) ||
            (selectedEntity.entityData.type == EntityType.Pet && selectedEntity.entityData.level == EntityLevel.Level_6) ||
            (selectedEntity.entityData.type == EntityType.Character && selectedEntity.entityData.level == EntityLevel.Level_10 ) ||
            (targetEntity.entityData.type == EntityType.Character && targetEntity.entityData.level == EntityLevel.Level_10))
            {
                (currentSlotData.entityData, selectedSlotData.entityData) = (selectedSlotData.entityData, currentSlotData.entityData);

                SaveSystem.SaveGame(gameData);
                SlotManager.Instance.UpdateSlot(currentSlotData.index);
                SlotManager.Instance.UpdateSlot(selectedSlotData.index);
                CancelMerge();
                return;
            }

            if (targetEntity.entityData.type == selectedEntity.entityData.type &&
                targetEntity.entityData.level == selectedEntity.entityData.level)
            {
                if (selectedEntity.entityData.type == EntityType.Character || selectedEntity.entityData.type == EntityType.Pet)
                {
                    ProcessMerge(selectedEntity, targetEntity, selectedEntity.entityData.type == EntityType.Character
                        ? EntityManager.Instance.GetCharacterDataByLevel
                        : EntityManager.Instance.GetPetDataByLevel);
                }
            }
            // else
            // {
            //     (currentSlotData.entityData, selectedSlotData.entityData) = (selectedSlotData.entityData, currentSlotData.entityData);

            //     SaveSystem.SaveGame(gameData);
            //     SlotManager.Instance.UpdateSlot(currentSlotData.index);
            //     SlotManager.Instance.UpdateSlot(selectedSlotData.index);
            // }
        }
        //CancelMerge();
    }

    private void ProcessMerge<T>(T entity1, T entity2, System.Func<EntityLevel, EntityData> getEntityData) where T : BaseEntity
    {
        SetAlpha(selectedEntity.gameObject, false);
        cloneCharacter.SetActive(false);
        int nextLevel = (int)entity1.entityData.level + 1;

        // EntityData newData = getEntityData((CharacterLevel)nextLevel);
        // bool hasUnlockedLevel = typeof(T) == typeof(Character)
        //     ? gameData.HasUnlockedCharacterLevel((CharacterLevel)nextLevel)
        //     : gameData.HasUnlockedPetLevel((CharacterLevel)nextLevel);
        EntityData newData = getEntityData((EntityLevel)nextLevel);
        bool hasUnlockedLevel = entity1.entityData.type == EntityType.Character
            ? gameData.HasUnlockedCharacterLevel((EntityLevel)nextLevel)
            : gameData.HasUnlockedPetLevel((EntityLevel)nextLevel);


        if (!hasUnlockedLevel)
        {
            if (entity1.entityData.type == EntityType.Character)
            {
                gameData.UnlockCharacterLevel((EntityLevel)nextLevel);
                UIManager.Instance.GetUI<UnlockNewLevelUI>().transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "NEW WARRIOR UNLOCKED";
                EntityManager.Instance.UpdateCurrentPurchasableCharacter();
            }
            else if (entity1.entityData.type == EntityType.Pet)
            {
                gameData.UnlockPetLevel((EntityLevel)nextLevel);
                UIManager.Instance.GetUI<UnlockNewLevelUI>().transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "NEW PET UNLOCKED";
                EntityManager.Instance.UpdateCurrentPurchasablePet();
            }
            UIManager.Instance.ShowUnlockNewLevelUI(entity1.entityData.renderTexture, newData.renderTexture);
        }

        SlotData slotData1 = gameData.GetSlotDataByIndex(entity1.slot.slotData.index);
        gameData.slotDatas.Remove(slotData1);
        SlotManager.Instance.UpdateSlot(slotData1.index);

        SlotData slotData2 = gameData.GetSlotDataByIndex(entity2.slot.slotData.index);
        slotData2.entityData = newData;
        SlotManager.Instance.UpdateEntity(slotData2.index);

        SaveSystem.SaveGame(gameData);
        CancelMerge();
    }

    private void CancelMerge()
    {
        SetAlpha(selectedEntity.gameObject, false);
        Destroy(cloneCharacter);
        if (currentSlot != null)
        {
            currentSlot.UnSelectedSlot();
            currentSlot = null;
        }
        selectedEntity = null;
    }

    private void SetAlpha(GameObject obj, bool toZero)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in renderers)
        {
            if (toZero)
            {
                if (!originalAlphaValues.ContainsKey(sr))
                {
                    originalAlphaValues[sr] = sr.color.a;
                }
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, 0.5f);
            }
            else
            {
                if (originalAlphaValues.TryGetValue(sr, out float savedAlpha))
                {
                    Color c = sr.color;
                    sr.color = new Color(c.r, c.g, c.b, savedAlpha);
                }
            }
        }

        if (!toZero)
        {
            originalAlphaValues.Clear();
        }
    }
}




// private void TryMergeOrCancel(Vector2 position)
// {
//     Collider2D[] hits = Physics2D.OverlapPointAll(position, entityLayer);

//     Character char1 = selectedEntity as Character;
//     Pet pet1 = selectedEntity as Pet;

//     if ((char1 != null && char1.entityData.level == characterLevelMax) || (pet1 != null && pet1.entityData.level == petLevelMax))
//     {
//         CancelMerge();
//         return;
//     }

//     foreach (var hit in hits)
//     {
//         BaseEntity targetEntity = hit.GetComponent<BaseEntity>();
//         if (targetEntity != null && targetEntity != selectedEntity && targetEntity != cloneCharacter.GetComponent<BaseEntity>())
//         {
//             if (targetEntity.GetType() == selectedEntity.GetType())
//             {
//                 Character char2 = targetEntity as Character;

//                 if (char1 != null && char2 != null && char1.entityData.level == char2.entityData.level)
//                 {
//                     SetAlpha(selectedEntity.gameObject, false);
//                     cloneCharacter.SetActive(false);
//                     int nextLevel = (int)char1.entityData.level + 1;

//                     EntityData characterData = EntityManager.Instance.GetCharacterDataByLevel((CharacterLevel)nextLevel);

//                     bool hasUnlockedCharacterLevel = gameData.HasUnlockedCharacterLevel((CharacterLevel)nextLevel);

//                     if (!hasUnlockedCharacterLevel)
//                     {
//                         UIManager.Instance.ShowUnlockNewLevelUI(char1.entityData.renderTexture, characterData.renderTexture);
//                         gameData.UnlockCharacterLevel((CharacterLevel)nextLevel);
//                     }

//                     SlotData slotData_1 = gameData.GetSlotDataByIndex(char1.slot.slotData.index);
//                     gameData.slotDatas.Remove(slotData_1);
//                     SlotManager.Instance.UpdateSlot(slotData_1.index);

//                     SlotData slotData_2 = gameData.GetSlotDataByIndex(char2.slot.slotData.index);
//                     slotData_2.charData = characterData;
//                     SlotManager.Instance.UpdateEntity(slotData_2.index);

//                     SaveSystem.SaveGame(gameData);
//                     CancelMerge();
//                     return;
//                 }


//                 Pet pet2 = targetEntity as Pet;
//                 if (pet1 != null && pet2 != null && pet1.entityData.level == pet2.entityData.level)
//                 {

//                     SetAlpha(selectedEntity.gameObject, false);
//                     cloneCharacter.SetActive(false);
//                     int nextLevel = (int)pet1.entityData.level + 1;

//                     EntityData petData = EntityManager.Instance.GetPetDataByLevel((CharacterLevel)nextLevel);

//                     bool hasUnlockedPetLevel = gameData.HasUnlockedPetLevel((CharacterLevel)nextLevel);

//                     if (!hasUnlockedPetLevel)
//                     {
//                         UIManager.Instance.ShowUnlockNewLevelUI(pet1.entityData.renderTexture, petData.renderTexture);
//                         gameData.UnlockPetLevel((CharacterLevel)nextLevel);
//                     }

//                     SlotData slotData_1 = gameData.GetSlotDataByIndex(pet1.slot.slotData.index);
//                     gameData.slotDatas.Remove(slotData_1);
//                     SlotManager.Instance.UpdateSlot(slotData_1.index);

//                     SlotData slotData_2 = gameData.GetSlotDataByIndex(pet2.slot.slotData.index);
//                     slotData_2.charData = petData;
//                     SlotManager.Instance.UpdateEntity(slotData_2.index);

//                     SaveSystem.SaveGame(gameData);
//                     CancelMerge();
//                     return;
//                 }
//             }
//         }
//     }
//     CancelMerge();
// }