using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : BaseSingleton<AnimationManager>
{
    [SerializeField] private Ease Title_Scale_Ease = Ease.InOutQuad;
    [SerializeField] private Ease Title_Rotate_Ease = Ease.InOutQuad;


    [SerializeField] private Ease ShowMainUI_Ease = Ease.OutBack;
    [SerializeField] private Ease HideMainUI_Ease = Ease.InBack;
    private Dictionary<UIAnimationState, bool> animations = new Dictionary<UIAnimationState, bool>();

    protected override void Awake()
    {
        base.Awake();
    }
    public void ShowMainUI(BaseUI ui, float fadeDuration = 0.25f, float moveDuration = 0.25f, System.Action onComplete = null)
    {
        SetAnimationState(UIAnimationState.ShowMainUI, true);

        RectTransform menuRect = ui.GetComponent<RectTransform>();
        RectTransform[] elements = ui
            .GetComponentsInChildren<RectTransform>(true)
            .Where(element => element.parent == menuRect)
            .ToArray();

        int pendingAnimations = 0;
        void CheckComplete()
        {
            pendingAnimations--;
            if (pendingAnimations <= 0)
            {
                onComplete?.Invoke();
                SetAnimationState(UIAnimationState.ShowMainUI, false);
            }
        }
        RectTransform deleteChar = UIManager.Instance.GetUI<HomeUI>().deleteChar.GetComponent<RectTransform>();
        if (ui == UIManager.Instance.GetUI<HomeUI>())
        {
            deleteChar.DOAnchorPos(new Vector2(-150, -100), 0);
            GameManager.Instance.UpdateMissionLv();
        }

        foreach (var element in elements)
        {
            if (element.name == "BG")
            {
                element.GetComponent<Image>().DOFade(GetCurrentAlpha(element), fadeDuration)
                    .From(0)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        CheckComplete();
                    });
                continue;
            }
            if (element.name == "Title" && element.GetComponent<ElementAnimator>() == null)
            {
                Tween rotate = element.DORotate(new Vector3(3, 3, 3), 1, RotateMode.FastBeyond360)
                             .From(new Vector3(-3, -3, -3))
                             .SetEase(Title_Rotate_Ease)
                             .SetUpdate(true)
                             .SetLoops(-1, LoopType.Yoyo);
                Tween scale = element.DOScale(element.localScale + Vector3.one * 0.1f, 1f)
                    .From(element.localScale)
                    .SetEase(Title_Scale_Ease)
                    .SetUpdate(true)
                    .SetLoops(-1, LoopType.Yoyo);
                GetElementAnimator(element).SetAnimations(new Tween[]{
                        rotate, scale
                });
            }
            Vector3 currentPos = element.position;
            Vector3 startPos;

            //if (currentPos.y < 0)
            if (currentPos.y < Screen.height / 2)
            
                startPos = new Vector3(currentPos.x, currentPos.y - Screen.height / 2, currentPos.z);
            else
                startPos = new Vector3(currentPos.x, currentPos.y + Screen.height / 2, currentPos.z);

            element.DOMove(currentPos, moveDuration)
                .From(startPos)
                .SetEase(ShowMainUI_Ease)
                .SetUpdate(true)
                .OnComplete(CheckComplete);
        }
        if (ui == UIManager.Instance.GetUI<HomeUI>())
        {
            UIManager.Instance.GetUI<HomeUI>().deleteChar.gameObject.SetActive(true);
            deleteChar.DOAnchorPos(new Vector2(-150, 400), 0.25f).SetEase(Ease.OutBack);
        }
    }

    public void HideMainUI(BaseUI ui, float fadeDuration = 0.25f, float moveDuration = 0.25f, System.Action onComplete = null)
    {
        SetAnimationState(UIAnimationState.HideMainUI, true);

        RectTransform menuRect = ui.GetComponent<RectTransform>();
        RectTransform[] elements = ui.GetComponentsInChildren<RectTransform>(true);
        elements = elements.Where(element => element.parent == menuRect).ToArray();

        int pendingAnimations = 0;
        void CheckComplete()
        {
            pendingAnimations--;
            if (pendingAnimations <= 0)
            {
                ui.gameObject.SetActive(false);
                onComplete?.Invoke();
                SetAnimationState(UIAnimationState.HideMainUI, false);
            }
        }

        foreach (var element in elements)
        {
            pendingAnimations++;

            if (element.name == "BG")
            {
                float currentAlpha = GetCurrentAlpha(element);
                Image img = element.GetComponent<Image>();
                img.DOFade(0, fadeDuration)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        CheckComplete();
                        Color newColor = img.color;
                        newColor.a = currentAlpha;
                        img.color = newColor;
                    });
                continue;
            }

            Vector3 currentPos = element.position;
            Vector3 endPos;

            if (currentPos.y < Screen.height / 2)
            {
                endPos = new Vector3(currentPos.x, currentPos.y - Screen.height / 2, currentPos.z);
            }
            else
            {
                endPos = new Vector3(currentPos.x, currentPos.y + Screen.height / 2, currentPos.z);
            }

            element.DOMove(endPos, moveDuration)
                .SetEase(HideMainUI_Ease)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    element.position = currentPos;
                    CheckComplete();
                });
        }
    }
    public ElementAnimator GetElementAnimator(RectTransform element)
    {
        ElementAnimator animator = element.GetComponent<ElementAnimator>();
        if (animator == null)
        {
            animator = element.gameObject.AddComponent<ElementAnimator>();
        }
        return animator;
    }
    public void ShowLevelItem(Button buttonLevel, float duration = 0.2f, float delay = 0)
    {
        RectTransform rt = buttonLevel.GetComponent<RectTransform>();
        rt.DOScale(1, duration)
            .From(0)
            .SetEase(Ease.OutQuad)
            .SetDelay(delay);
    }


    public float GetCurrentAlpha(Transform element)
    {
        if (element.TryGetComponent(out Image image))
        {
            return image.color.a;
        }
        return 1f;
    }
    public bool IsAnimating(UIAnimationState animationName)
    {
        return animations.ContainsKey(animationName) && animations[animationName];
    }
    private void SetAnimationState(UIAnimationState animationName, bool isAnimating)
    {
        if (animations.ContainsKey(animationName))
        {
            animations[animationName] = isAnimating;
        }
        else
        {
            animations.Add(animationName, isAnimating);
        }
    }
    public void OnButtonClick(Button bt, System.Action onComplete = null)
    {
        if (IsAnimating(UIAnimationState.ButtonClick)) return;
        SetAnimationState(UIAnimationState.ButtonClick, true);
        AudioManager.Instance.PlaySoundEffect(SoundEffect.Click);
        RectTransform rt = bt.GetComponent<RectTransform>();
        Vector3 originScale = rt.localScale;

        rt.DOScale(originScale * 0.8f * 0.8f, 0.1f)
            .SetEase(Ease.InQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {

                rt.DOScale(originScale, 0.1f)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        SetAnimationState(UIAnimationState.ButtonClick, false);
                        onComplete?.Invoke();
                    });
            });
    }
    public void ShakeTransform(Transform target, float duration = 0.3f, float strength = 5f, int vibrato = 20, float randomness = 360f)
    {
        Vector3 originPos = target.position;
        if (target == null)
        {
            Debug.LogWarning("ShakeTransform: Target is null!");
            return;
        }
        target.DOShakePosition(
            duration,          // Thời gian rung
            strength,          // Độ mạnh của rung
            vibrato,           // Số lần rung
            randomness,        // Ngẫu nhiên
            snapping: false,   // Không cố định theo pixel
            fadeOut: true      // Giảm dần rung về cuối
            ).OnComplete(() =>
            {
                target.position = originPos;
            });
    }
    // public void ShakeCinemachineCamera(float intensity = 5f, float duration = 0.3f)
    // {
    //     if (IsAnimating(UIAnimationState.ShakeCamera)) return;
    //     SetAnimationState(UIAnimationState.ShakeCamera, true);
    //     CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    //     CinemachineBasicMultiChannelPerlin perlin =
    //         virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    //     perlin.m_AmplitudeGain = intensity;
    //     perlin.m_FrequencyGain = intensity;
    //     StartCoroutine(StopShakeAfterDuration(perlin, duration));

    // }
    // private IEnumerator StopShakeAfterDuration(CinemachineBasicMultiChannelPerlin perlin, float duration)
    // {
    //     yield return new WaitForSeconds(duration);
    //     perlin.m_AmplitudeGain = 0f;
    //     perlin.m_FrequencyGain = 0f;
    //     SetAnimationState(UIAnimationState.ShakeCamera, false);
    // }
}
