using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDataCollection", menuName = "Game Data/Game Data Collection")]
public class GameDataCollection : ScriptableObject
{
    public List<EntityData> characterDataList = new List<EntityData>();
    public List<EntityData> petDataList = new List<EntityData>();

    public int[] slotPrices;
}
