using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : BaseSingleton<LevelManager>
{
    [SerializeField] private Transform LevelParent;
    [SerializeField] private TextMeshProUGUI goldText;
    public List<int> goldWinList;

    private Dictionary<GameLevel, GameObject> _mapDic;
    public int currentLevelIndex { get; private set; } = -1;
    public GameObject currentLevel { get; private set; } = null;
    protected override void Awake()
    {
        base.Awake();
        LoadAllLevel();
    }
    private void LoadAllLevel()
    {
        _mapDic = new Dictionary<GameLevel, GameObject>();

        foreach (GameLevel mapName in Enum.GetValues(typeof(GameLevel)))
        {
            string path = $"Level/{mapName}";
            GameObject map = Resources.Load<GameObject>(path);

            if (map != null)
            {
                _mapDic[mapName] = map;
            }
            else
            {
                Debug.LogWarning($"'{mapName}' not found at path: {path}");
            }
        }
        //Debug.Log("MapDic Count:" + _mapDic.Count);
    }
    private GameObject GetMap(GameLevel mapName)
    {
        if (_mapDic.TryGetValue(mapName, out var map))
        {
            return map;
        }

        Debug.LogWarning($"'{mapName}' not found in dictionary.");
        return null;
    }

    public void LoadMap(int index)
    {
        GameManager.Instance.ResetStatus();
        ClearMap();
        var gameLevels = Enum.GetValues(typeof(GameLevel));
        int levelCount = gameLevels.Length;
        if (index > levelCount)
            currentLevelIndex = UnityEngine.Random.Range(1, levelCount + 1);
        else
            currentLevelIndex = index;
        Debug.Log("current: " + currentLevelIndex + " " +  levelCount);
        GameLevel mapName = (GameLevel)gameLevels.GetValue(currentLevelIndex - 1);
        currentLevel = Instantiate(GetMap(mapName), LevelParent);
    }
    public bool IsLastLevel()
    {
        return currentLevelIndex >= Enum.GetValues(typeof(GameLevel)).Length;
    }
    public void ClearMap()
    {
        if (currentLevel) DestroyImmediate(currentLevel);
    }
    public void CompletedLevel()
    {
        int gold = goldWinList[currentLevelIndex - 1];
        if (currentLevelIndex > 0)
        {
            goldText.text = "+" + GameDataManager.Instance.FormatPrice(gold);
        }
        currentLevelIndex++;
        GameData currentData = SaveSystem.LoadGame();
        currentData.currentLevel = currentLevelIndex;
        GoldManager.Instance.AddGold(gold);
        SaveSystem.SaveGame(currentData);
    }
}
