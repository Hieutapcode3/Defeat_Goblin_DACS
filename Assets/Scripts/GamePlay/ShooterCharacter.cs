using UnityEngine;

public class ShooterCharacter : BaseEntity
{
    public Transform shootPoint;
    [SerializeField] private ParticleSystem explosion;
    public override void OnAttackHit(AnimationType animationType)
    {
        SpawnBullet(target,entityData.damage);
    }
    protected void SpawnBullet(BaseEntity target, int damage)
    {
        if(explosion != null)
            explosion.Play();
        if (!shootPoint) return;
        GameObject cannonBall = PoolManager.Instance.Get(ResourceManager._CannonBall, shootPoint.position);
        cannonBall.GetComponent<Arrow>().enabled = false;
        cannonBall.GetComponent<BulletMove>().SetTarget(target, damage);
    }
}
