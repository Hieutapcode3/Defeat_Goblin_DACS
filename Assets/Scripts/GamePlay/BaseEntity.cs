using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public AttackType attackType = AttackType.Melee;
    public float attackRange = 1.5f;
    public EntityData entityData;
    public SlotController slot;
    public Animator animator { get; private set; }
    public bool IsAttacking { get; protected set; } = false;
    public bool IsFacingRight = false;
    protected BaseEntity target;
    protected Health health;
    [SerializeField] protected ParticleSystem splashAnim;
    private Coroutine goldIncreaseCoroutine;

    protected virtual void Start()
    {
        this.health = GetComponent<Health>();
        this.animator = GetComponent<Animator>();
        if (animator)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(state.fullPathHash, 0, Random.Range(0, 1f));
            animator.speed = Random.Range(0.6f, 1f);
        }
        if(entityData != null && entityData.type != EntityType.Enemy && slot != null)
            goldIncreaseCoroutine = StartCoroutine(GoldIncreaseRoutine());
    }

    private IEnumerator GoldIncreaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (!GameManager.Instance.isInBattle)
            {
                IncreaseGold();
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (goldIncreaseCoroutine != null)
        {
            StopCoroutine(goldIncreaseCoroutine);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!health.TakeDamage(damage))
        {
            if (entityData.type == EntityType.Enemy)
                LevelController.Instance.RemoveEnemy(slot);
            else
                EntitySpawner.Instance.RemoveBaseEntity(this);
            Destroy(gameObject);
        }
    }

    public virtual void Attack(BaseEntity target)
    {
        if (IsAttacking) return;
        this.target = target;
        IsAttacking = true;
        animator.SetTrigger("Attack");
    }

    public virtual void OnAttackHit(AnimationType animationType)
    {
        if (target) ((Enemy)target).TakeDamage(entityData.damage);
    }

    public virtual void OnAnimationEnd(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.Idle:
                break;
            case AnimationType.Moving:
                break;
            case AnimationType.Attaking:
                this.IsAttacking = false;
                break;
        }
    }

    public void Moving()
    {
        if (!this.animator.GetBool("isMoving"))
        {
            this.animator.SetBool("isMoving", true);
            IsAttacking = false;
        }
    }

    public void SplashAnim()
    {
        if (target && splashAnim) splashAnim.Play();
    }

    public void Idle()
    {
        if (this.animator.GetBool("isMoving"))
        {
            this.animator.SetBool("isMoving", false);
            IsAttacking = false;
        }
    }

    public void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 bodyLocalScale = this.transform.localScale;
        bodyLocalScale.x *= -1;
        this.transform.localScale = bodyLocalScale;
        if (splashAnim)
        {
            Vector3 splashAnimLocalScale = this.splashAnim.transform.localScale;
            splashAnimLocalScale.x *= -1;
            this.splashAnim.transform.localScale = splashAnimLocalScale;
        }
    }

    public void Reset()
    {
        health.ResetHealth();
        IsAttacking = false;
    }

    public void IncreaseGold()
    {
        if (!entityData) return;
        GameObject goldIncrease = PoolManager.Instance.Get(ResourceManager._GoldIncrease, transform.position + Vector3.up * 0.35f);
        TextMeshPro textMeshPro = goldIncrease.GetComponent<TextMeshPro>();
        GameObject coin =  PoolManager.Instance.Get(ResourceManager._Coin, transform.position + Vector3.up * 0.4f);
        coin.GetComponent<CoinEffect>().MoveEffect();
        int amount = entityData.goldPerSecond * 5;
        textMeshPro.text = "+" + (GameDataManager.Instance.FormatPrice(amount)).ToString();
        goldIncrease.transform.localScale = Vector3.zero;
        goldIncrease.transform.DOScale(1, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            GoldManager.Instance.AddGold(amount);
            goldIncrease.transform.DOScale(0, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                goldIncrease.SetActive(false);
            }); ;
        });
        goldIncrease.transform.DOMove(goldIncrease.transform.position + Vector3.up * 0.3f, 2f)
            .SetEase(Ease.InOutQuad);
            
    }
}