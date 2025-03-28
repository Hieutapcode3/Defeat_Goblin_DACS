// This file is auto-generated. Do not edit manually.
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private ResourceData ResourceData;
    public static UIScreen _StartUI { get; private set; }
    public static Sprite _Mute { get; private set; }
    public static Sprite _UnMute { get; private set; }
    public static Sprite _LevelLock { get; private set; }
    public static Sprite _LevelUnLock { get; private set; }
    public static GameObject _DamageTextPrefab { get; private set; }
    public static GameObject _ArrowPrefab { get; private set; }
    public static GameObject _UIEntityItem { get; private set; }
    public static Sprite _ItemDefault { get; private set; }
    public static Sprite _ItemDepleted { get; private set; }
    public static GameObject _GoldTextPrefab { get; private set; }
    public static GameObject _GoldIncrease { get; private set; }
    public static GameObject _Coin { get; private set; }
    public static GameObject _Magican { get; private set; }
    public static GameObject _CannonBall { get; private set; }
    public static GameObject _Bom_Dissolve { get; private set; }
    public static GameObject _EnemyArrow { get; private set; }

    private void Awake()
    {
        _StartUI = ResourceData.GetValueByKey<UIScreen>(ResourceKey.StartUI);
        _Mute = ResourceData.GetValueByKey<Sprite>(ResourceKey.Mute);
        _UnMute = ResourceData.GetValueByKey<Sprite>(ResourceKey.UnMute);
        _LevelLock = ResourceData.GetValueByKey<Sprite>(ResourceKey.LevelLock);
        _LevelUnLock = ResourceData.GetValueByKey<Sprite>(ResourceKey.LevelUnLock);
        _DamageTextPrefab = ResourceData.GetValueByKey<GameObject>(ResourceKey.DamageTextPrefab);
        _ArrowPrefab = ResourceData.GetValueByKey<GameObject>(ResourceKey.ArrowPrefab);
        _UIEntityItem = ResourceData.GetValueByKey<GameObject>(ResourceKey.UIEntityItem);
        _ItemDefault = ResourceData.GetValueByKey<Sprite>(ResourceKey.ItemDefault);
        _ItemDepleted = ResourceData.GetValueByKey<Sprite>(ResourceKey.ItemDepleted);
        _GoldTextPrefab = ResourceData.GetValueByKey<GameObject>(ResourceKey.GoldTextPrefab);
        _GoldIncrease = ResourceData.GetValueByKey<GameObject>(ResourceKey.GoldIncrease);
        _Coin = ResourceData.GetValueByKey<GameObject>(ResourceKey.Coin);
        _Magican = ResourceData.GetValueByKey<GameObject>(ResourceKey.Magican);
        _CannonBall = ResourceData.GetValueByKey<GameObject>(ResourceKey.CannonBall);
        _Bom_Dissolve = ResourceData.GetValueByKey<GameObject>(ResourceKey.Bom_Dissolve);
        _EnemyArrow = ResourceData.GetValueByKey<GameObject>(ResourceKey.EnemyArrow);
    }
}
