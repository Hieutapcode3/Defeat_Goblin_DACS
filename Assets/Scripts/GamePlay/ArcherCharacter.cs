using System.Collections;
using UnityEngine;

public class ArcherCharacter : BaseEntity
{
    public Transform shootPoint;


    public override void OnAttackHit(AnimationType animationType)
    {
        if (target)
        {
            GameObject arrow = PoolManager.Instance.Get(ResourceManager._ArrowPrefab, shootPoint.position);
            Arrow arrowScript = arrow.GetComponent<Arrow>();

            if (arrowScript)
            {
                arrowScript.SetTarget(target, entityData.damage);
            }
        }
    }
}
