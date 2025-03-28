using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class EntityMerger : BaseSingleton<EntityMerger>
{
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private RectTransform deleteChar;
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
        if (UIManager.Instance.GetUI<UnlockNewLevelUI>().gameObject.activeSelf) return;
        HandleMouseInput();
    }
    private void HandleMouseInput()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectCharacter(mousePosition);
        }
        else if (Input.GetMouseButton(0) && cloneCharacter != null)
        {
            MoveCloneCharacter(mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && cloneCharacter != null)
        {
            TryMergeOrCancel();
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
                AudioManager.Instance.PlaySoundEffect(SoundEffect.Click);
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
        cloneCharacter = Instantiate(selectedEntity.gameObject, selectedEntity.transform.position, Quaternion.identity);
        cloneCharacter.GetComponent<BaseEntity>().slot = null;
        SpriteRenderer[] renderers = cloneCharacter.GetComponentsInChildren<SpriteRenderer>();
        cloneCharacter.GetComponent<SortingGroup>().sortingOrder += 66;
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
        Vector2 dropPosition = Input.mousePosition; 
        if (RectTransformUtility.RectangleContainsScreenPoint(deleteChar, dropPosition, Camera.main))
        {
            SlotData selectedSlot = selectedEntity.slot.slotData;
            int amount = selectedEntity.entityData.goldPerSecond * 5;
            GoldManager.Instance.AddGold(amount);
            gameData.slotDatas.Remove(gameData.GetSlotDataByIndex(selectedSlot.index));
            SlotManager.Instance.UpdateSlot(selectedSlot.index);
            SaveSystem.SaveGame(gameData);
            Destroy(selectedEntity.gameObject);
            Destroy(cloneCharacter);
            selectedEntity = null;
            if (currentSlot != null)
            {
                currentSlot.UnSelectedSlot();
                currentSlot = null;
            }
            GoldManager.Instance.UpdateGoldPerSecondTxt();
            return;
        }

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
            CancelMerge();
        }
        else
        {
            BaseEntity targetEntity = currentSlot.currentEntity;
            if (targetEntity.entityData.type != selectedEntity.entityData.type ||
                targetEntity.entityData.level != selectedEntity.entityData.level ||
                (targetEntity.entityData.type == EntityType.Pet && targetEntity.entityData.level == EntityLevel.Level_6) ||
                (selectedEntity.entityData.type == EntityType.Pet && selectedEntity.entityData.level == EntityLevel.Level_6) ||
                (selectedEntity.entityData.type == EntityType.Character && selectedEntity.entityData.level == EntityLevel.Level_10) ||
                (targetEntity.entityData.type == EntityType.Character && targetEntity.entityData.level == EntityLevel.Level_10))
            {
                (currentSlotData.entityData, selectedSlotData.entityData) = (selectedSlotData.entityData, currentSlotData.entityData);
                SlotManager.Instance.UpdateSlot(currentSlotData.index);
                SlotManager.Instance.UpdateSlot(selectedSlotData.index);
                SaveSystem.SaveGame(gameData);
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
        }
    }

    private void ProcessMerge<T>(T entity1, T entity2, System.Func<EntityLevel, EntityData> getEntityData) where T : BaseEntity
    {
        SetAlpha(selectedEntity.gameObject, false);
        cloneCharacter.SetActive(false);
        int nextLevel = (int)entity1.entityData.level + 1;
        EntityData newData = getEntityData((EntityLevel)nextLevel);
        bool hasUnlockedLevel = entity1.entityData.type == EntityType.Character
            ? gameData.HasUnlockedCharacterLevel((EntityLevel)nextLevel)
            : gameData.HasUnlockedPetLevel((EntityLevel)nextLevel);

        if (!hasUnlockedLevel)
        {
            AudioManager.Instance.PlaySoundEffect(SoundEffect.UnlockNewChar);
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
        else
        {
            AudioManager.Instance.PlaySoundEffect(SoundEffect.Merge);
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
        GoldManager.Instance.UpdateGoldPerSecondTxt();
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