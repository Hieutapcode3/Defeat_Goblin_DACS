using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : BaseSingleton<EffectManager>
{
    private Dictionary<VisualEffect, ParticleSystem> _effect;

    protected override void Awake()
    {
        base.Awake();
        LoadAllEffects();
    }
    private void LoadAllEffects()
    {
        _effect = new Dictionary<VisualEffect, ParticleSystem>();

        foreach (VisualEffect effectName in Enum.GetValues(typeof(VisualEffect)))
        {
            if (effectName == VisualEffect.None) continue;
            string path = $"Effect/{effectName}";
            ParticleSystem effect = Resources.Load<ParticleSystem>(path);

            if (effect != null)
            {
                _effect[effectName] = effect;
            }
            else
            {
                Debug.LogWarning($"Effect '{effectName}' not found at path: {path}");
            }
        }
        //Debug.Log("EffectDic Count:" + _effect.Count);
    }

    public ParticleSystem PlayEffect(VisualEffect name, bool isDestroy = true)
    {
        var newEffect = Instantiate(GetEffect(name));
        if (isDestroy) Destroy(newEffect.gameObject, newEffect.main.duration);
        return newEffect;
    }
    private ParticleSystem GetEffect(VisualEffect name)
    {
        if (_effect.TryGetValue(name, out var effect))
        {
            return effect;
        }

        Debug.LogWarning($"Effect '{name}' not found in dictionary.");
        return null;
    }
}
public static class VisualEffectExtensions
{
    public static ParticleSystem Play(this VisualEffect effect, bool isDestroy = true)
    {
        return EffectManager.Instance.PlayEffect(effect, isDestroy);
    }
}

