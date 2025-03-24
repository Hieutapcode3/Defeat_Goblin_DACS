using System.Collections;
using System.Collections.Generic;
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
}
