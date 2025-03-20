using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "Game/Entity Data")]
public class EntityData : ScriptableObject
{
    public EntityLevel level;
    public GameObject prefab;
    public RenderTexture renderTexture;
    public EntityType type;
    public int damage;
    //public float speed;
    //public int health;
    public int goldPerSecond;
}
