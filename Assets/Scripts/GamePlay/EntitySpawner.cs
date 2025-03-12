using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntitySpawner : BaseSingleton<EntitySpawner>
{
    [Header("UI References")]
    [SerializeField] private Transform itemParent;

    [Header("Spawn Settings")]
    public LayerMask spawnLayer;
    private Dictionary<EntityType, Dictionary<EntityLevel, int>> entityDataDict;
    private EntityData selectedEntity;
    private Dictionary<EntityData, GameObject> uiItems = new Dictionary<EntityData, GameObject>();
    private GameData gameData;
    private List<BaseEntity> baseEntities = new List<BaseEntity>();
    public void LoadBattle(GameData gameData)
    {
        this.gameData = gameData;
        this.entityDataDict = gameData.GetEntityDataByTypeAndLevel();
        PopulateUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            TrySpawnEntity();
        }
    }

    private void PopulateUI()
    {
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
        uiItems.Clear();

        foreach (var typeEntry in entityDataDict)
        {
            foreach (var levelEntry in typeEntry.Value)
            {
                int count = levelEntry.Value;
                if (count <= 0) continue;

                EntityData entitySample = gameData.slotDatas
                    .Find(s => s.entityData != null && s.entityData.type == typeEntry.Key && s.entityData.level == levelEntry.Key)?.entityData;

                if (entitySample == null) continue;

                GameObject item = Instantiate(ResourceManager._UIEntityItem, itemParent);
                item.GetComponent<Button>().onClick.AddListener(() => SelectEntity(entitySample));

                RawImage icon = item.GetComponentInChildren<RawImage>();
                TextMeshProUGUI countText = item.GetComponentInChildren<TextMeshProUGUI>();
                Image buttonImage = item.GetComponent<Image>();

                icon.texture = entitySample.renderTexture;
                countText.text = count.ToString();
                buttonImage.sprite = (count > 0) ? ResourceManager._ItemDefault : ResourceManager._ItemDepleted;

                uiItems[entitySample] = item;
            }
        }

        AutoSelectFirstAvailable();
    }
    public void RemoveBaseEntity(BaseEntity baseEntity) {
        this.baseEntities.Remove(baseEntity);
        if (baseEntities.Count() == 0) {
            // Lose
        }
    }

    public void ClearEntities() {
        foreach (BaseEntity baseEntity in baseEntities) {
            Destroy(baseEntity.gameObject);
        }
        this.baseEntities.Clear();
    }

    private void SelectEntity(EntityData entity)
    {
        if (entityDataDict[entity.type][entity.level] <= 0) return;

        selectedEntity = entity;
        UpdateUISelection();
    }

    private void TrySpawnEntity()
    {
        if (selectedEntity == null) return;
        if (entityDataDict[selectedEntity.type][selectedEntity.level] <= 0) return;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, spawnLayer);

        if (hit.collider != null)
        {
            var newEntity = Instantiate(selectedEntity.prefab, worldPoint, Quaternion.identity);
            newEntity.transform.localScale = Vector3.one * 0.3f;
            BaseEntity baseEntity = newEntity.GetComponent<BaseEntity>();
            baseEntity.entityData = selectedEntity;

            Seeker seeker = newEntity.AddComponent<Seeker>();

            CharacterAI characterAI = newEntity.AddComponent<CharacterAI>();

            characterAI.moveSpeed = 1;
            characterAI.nextWPDistance = 0.2f;

            characterAI.Init(seeker,LevelController.Instance.GetEntitiesGameObjects());

            entityDataDict[selectedEntity.type][selectedEntity.level]--;

            baseEntities.Add(baseEntity);
            UpdateUICount(selectedEntity);
            AutoSelectNextIfDepleted();
        }
    }

    private void UpdateUICount(EntityData entity)
    {
        if (uiItems.ContainsKey(entity))
        {
            GameObject item = uiItems[entity];
            TextMeshProUGUI countText = item.GetComponentInChildren<TextMeshProUGUI>();
            Image buttonImage = item.GetComponent<Image>();

            int remaining = entityDataDict[entity.type][entity.level];
            countText.text = remaining.ToString();
            buttonImage.sprite = (remaining > 0) ? ResourceManager._ItemDefault : ResourceManager._ItemDepleted;
        }
    }

    private void AutoSelectFirstAvailable()
    {
        selectedEntity = null;
        foreach (var typeEntry in entityDataDict)
        {
            foreach (var levelEntry in typeEntry.Value)
            {
                if (levelEntry.Value > 0)
                {
                    EntityData entity = gameData.slotDatas
                        .Find(s => s.entityData != null && s.entityData.type == typeEntry.Key && s.entityData.level == levelEntry.Key)?.entityData;

                    if (entity != null)
                    {
                        SelectEntity(entity);
                        return;
                    }
                }
            }
        }
        UpdateUISelection();
    }

    private void AutoSelectNextIfDepleted()
    {
        if (selectedEntity != null && entityDataDict[selectedEntity.type][selectedEntity.level] <= 0)
        {
            AutoSelectFirstAvailable();
        }
    }

    private void UpdateUISelection()
    {
        foreach (var entity in uiItems.Keys)
        {
            Image buttonImage = uiItems[entity].GetComponent<Image>();
            buttonImage.color = (entity == selectedEntity) ? Color.green : Color.white;
        }
    }
}
