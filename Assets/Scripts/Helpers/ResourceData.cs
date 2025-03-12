
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Game/ResourceData")]
public class ResourceData : ScriptableObject
{
    [System.Serializable]
    public class ResourceItem
    {
        public ResourceKey key;
        public ResourceType type;
        [HideInInspector] public string stringValue;
        [HideInInspector] public int intValue;
        [HideInInspector] public float floatValue;
        [HideInInspector] public bool boolValue;
        [HideInInspector] public string enumTypeName;
        [HideInInspector] public string enumValue;
        [HideInInspector] public Sprite spriteValue;
        [HideInInspector] public Transform transformValue;
        [HideInInspector] public GameObject gameObjectValue;
    }

    public enum ResourceType
    {
        String,
        Int,
        Float,
        Bool,
        Enum,
        Sprite,
        Transform,
        GameObject
    }
    public List<ResourceItem> resourceDatas = new List<ResourceItem>();

    public T GetValueByKey<T>(ResourceKey key)
    {
        ResourceItem resource = resourceDatas.Find(e => e.key == key);

        if (resource == null)
        {
            Debug.LogWarning($"Key '{key}' not found in config.");
            return default;
        }

        switch (resource.type)
        {
            case ResourceType.String:
                if (typeof(T) == typeof(string))
                    return (T)(object)resource.stringValue;
                break;

            case ResourceType.Int:
                if (typeof(T) == typeof(int))
                    return (T)(object)resource.intValue;
                break;

            case ResourceType.Float:
                if (typeof(T) == typeof(float))
                    return (T)(object)resource.floatValue;
                break;

            case ResourceType.Bool:
                if (typeof(T) == typeof(bool))
                    return (T)(object)resource.boolValue;
                break;

            case ResourceType.Sprite:
                if (typeof(T) == typeof(Sprite))
                    return (T)(object)resource.spriteValue;
                break;
            case ResourceType.Transform:
                if (typeof(T) == typeof(Transform))
                    return (T)(object)resource.transformValue;
                break;
            case ResourceType.GameObject:
                if (typeof(T) == typeof(GameObject))
                    return (T)(object)resource.gameObjectValue;
                break;

            case ResourceType.Enum:
                if (typeof(T).IsEnum)
                {
                    System.Type enumType = System.Type.GetType(resource.enumTypeName);
                    if (enumType != null && enumType == typeof(T))
                    {
                        return (T)System.Enum.Parse(enumType, resource.enumValue);
                    }
                }
                break;
        }
        Debug.LogError($"Type mismatch or unsupported type for key '{key}'. Expected type: {typeof(T).Name}");
        return default;
    }

}
