using DG.Tweening;
using UnityEngine;

public class ElementAnimator : MonoBehaviour
{
    public Tween[] uiTweens { get; private set; }
    public void SetAnimations(Tween[] tweens)
    {
        KillAnimations();
        uiTweens = tweens;
    }
    void OnDisable()
    {
        PauseAnimations();
    }
    void OnEnable()
    {
        RestartAnimations();
    }
    private void KillAnimations()
    {
        if (uiTweens != null)
        {
            foreach (var tween in uiTweens)
            {
                tween.Kill();
            }
        }
    }
    public void PauseAnimations()
    {
        if (uiTweens != null)
        {
            foreach (var tween in uiTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    tween.Pause();
                }
            }
        }
    }
    public void RestartAnimations()
    {
        if (uiTweens != null)
        {
            foreach (var tween in uiTweens)
            {
                if (tween == null)
                {
                    continue;
                }
                if (tween.IsActive() && !tween.IsPlaying())
                {
                    tween.Restart();
                }
            }
        }
    }
}
