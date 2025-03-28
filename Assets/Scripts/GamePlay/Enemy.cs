using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : BaseEntity
{
    private List<BaseEntity> characters = new List<BaseEntity>();
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem explosion;
    protected override void Start()
    {
        base.Start();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y * 0.01f);
    }
    private void Update()
    {
        if (characters != null && characters.Count() != 0 && !IsAttacking)
        {
            Attack(null);
        }
    }
    public override void OnAttackHit(AnimationType animationType)
    {
        if (attackType == AttackType.Melee)
        {
            for (int i = characters.Count - 1; i >= 0; i--)
                if (characters[i]) characters[i].TakeDamage(entityData.damage);
        }
        else if (entityData.level == EntityLevel.Level_2 && characters.Count > 0)
        {
            SpawnMagicalBullet(characters[0], entityData.damage);
        }
        else if (entityData.level == EntityLevel.Level_4 && characters.Count > 0)
        {
            SpawnCannonBall(characters[0], entityData.damage);
        }else if (entityData.level == EntityLevel.Level_6 && characters.Count > 0)
        {
            SpawnArrow(characters[0], entityData.damage);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseEntity baseEntity = other.gameObject.GetComponent<BaseEntity>();
        if (baseEntity != null && !characters.Contains(baseEntity))
        {
            characters.Add(baseEntity);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        BaseEntity baseEntity = other.gameObject.GetComponent<BaseEntity>();
        if (characters.Contains(baseEntity))
        {
            characters.Remove(baseEntity);
        }
    }
    public void SpawnMagicalBullet(BaseEntity target, int damage)
    {
        if (!firePoint) return;
        GameObject magican = PoolManager.Instance.Get(ResourceManager._Magican, firePoint.position);
        magican.GetComponent<BulletMove>().SetTarget(target, damage);
    }
    public void SpawnCannonBall(BaseEntity target, int damage)
    {
        if (explosion != null)
            explosion.Play();
        if (!firePoint) return;
        GameObject cannonBall = PoolManager.Instance.Get(ResourceManager._CannonBall, firePoint.position);
        cannonBall.GetComponent<BulletMove>().enabled = false;
        cannonBall.GetComponent<Arrow>().SetTarget(target, damage);
    }
    public void SpawnArrow(BaseEntity target, int damage)
    {
        if (explosion != null)
            explosion.Play();
        if (!firePoint) return;
        AudioManager.Instance.PlaySoundEffect(SoundEffect.Arrow);
        GameObject arrow = PoolManager.Instance.Get(ResourceManager._EnemyArrow, firePoint.position);
        arrow.GetComponent<Arrow>().SetTarget(target, damage);
    }
}