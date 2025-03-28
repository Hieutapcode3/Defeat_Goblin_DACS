using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : BaseSingleton<LevelController>
{
    public Transform centerTransform;
    public Transform cameraFollow;
    private IsometricMapGenerator isometricMapGenerator;
    private List<SlotController> slotControllers = new List<SlotController>();
    public PolygonCollider2D mapLimit { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        mapLimit = GetComponent<PolygonCollider2D>();
        isometricMapGenerator = GetComponent<IsometricMapGenerator>();
        this.centerTransform = GameManager.Instance.centerTransform;
        this.transform.position = centerTransform.position;
        this.slotControllers = GetComponentsInChildren<SlotController>().ToList();
    }
    private void Start()
    {
        StartCoroutine(GenerateMapCoroutine());
    }
    public IEnumerator GenerateMapCoroutine(Action onComplete = null)
    {
        isometricMapGenerator.CreateMap(this.centerTransform, slotControllers);
        isometricMapGenerator.GenerateWaterClusters();
        yield return null;

        isometricMapGenerator.SmoothWaterClusters();
        yield return null;

        yield return StartCoroutine(isometricMapGenerator.GenerateMapCoroutine());
        AstarPath.active.data.gridGraph.center = new Vector3(centerTransform.position.x, centerTransform.position.y - 0.25f, 0);
        AstarPath.active.Scan();

        onComplete?.Invoke();
    }
    public List<GameObject> GetEntitiesGameObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (SlotController slot in slotControllers)
        {
            if (slot.currentEntity != null)
            {
                gameObjects.Add(slot.currentEntity.gameObject);
            }
        }

        return gameObjects;
    }
    public void RemoveEnemy(SlotController slotController) {
        slotControllers.Remove(slotController);
        if (slotControllers.Count() == 0) {
            GameManager.Instance.WinGame(0.3f);
            LevelManager.Instance.CompletedLevel();
        }
    }
}
