using UnityEngine;

public class ShooterCharacter : BaseEntity
{
    public Transform shootPoint;
    public float shootRange = 10f;

    public override void Attack(BaseEntity target)
    {
        if (target && !IsAttacking)
        {
            base.Attack(target);
            // Shoot(target);
        }
    }

    // private void Shoot(BaseEntity target)
    // {
    //     RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, (target.transform.position - shootPoint.position).normalized, shootRange, enemyLayer);

    //     if (hit.collider != null)
    //     {
    //         BaseEntity enemy = hit.collider.GetComponent<BaseEntity>();
    //         if (enemy)
    //         {
    //             enemy.TakeDamage(entityData.damage);
    //         }
    //     }
    // }
}
