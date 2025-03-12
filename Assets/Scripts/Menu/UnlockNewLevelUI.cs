using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnlockNewLevelUI : BaseUI
{
    public GameObject effectUpgradeLevel;
    public RawImage entityLeft;
    public RawImage entityRight;
    public RawImage entityMid;
    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 centerPos;
    public void SetIcon(Texture _old, Texture _new) {
        entityLeft.texture = _old;
        entityRight.texture = _old;
        entityMid.texture = _new;
    }
    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        float screenWidth = Screen.width;
        float offsetX = screenWidth * 0.25f;

        leftStartPos = new Vector3(-offsetX, 0, 0);
        rightStartPos = new Vector3(offsetX, 0, 0);
        effectUpgradeLevel.SetActive(false);

        centerPos = Vector3.zero;

        entityMid.transform.localScale = Vector3.zero;
        entityMid.gameObject.SetActive(false);
    }
    public override void Show()
    {
        base.Show();
        PlayUnlockAnimation();
    }
    private void PlayUnlockAnimation()
    {
        entityLeft.rectTransform.DOKill(true);
        entityRight.rectTransform.DOKill(true);
        entityLeft.transform.DOKill(true);
        entityRight.transform.DOKill(true);
        entityMid.transform.DOKill(true);

        entityLeft.transform.localScale = Vector3.one;
        entityRight.transform.localScale = Vector3.one;
        entityLeft.rectTransform.anchoredPosition = leftStartPos;
        entityRight.rectTransform.anchoredPosition = rightStartPos;
        entityMid.transform.localScale = Vector3.zero;
        entityMid.gameObject.SetActive(false);

        float moveDuration = 0.5f;
        float scaleDuration = 0.5f;

        Sequence unlockSequence = DOTween.Sequence();
        unlockSequence.Append(entityLeft.rectTransform.DOAnchorPos(centerPos, moveDuration).SetEase(Ease.OutQuad));
        unlockSequence.Join(entityRight.rectTransform.DOAnchorPos(centerPos, moveDuration).SetEase(Ease.OutQuad));

        unlockSequence.Append(entityLeft.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
        unlockSequence.Join(entityRight.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));

        unlockSequence.AppendCallback(() => {
            entityMid.gameObject.SetActive(true);
            effectUpgradeLevel.SetActive(true);
        });
        unlockSequence.Append(entityMid.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));

    }
    public override void Hide(Action onComplete = null)
    {
        base.Hide(onComplete);
        effectUpgradeLevel.SetActive(false);
    }
}
