using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceKey", menuName = "Game/ResourceKey")]
public class ResourceKeyGenerator : ScriptableObject
{
    public List<string> keys = new List<string>();
}
