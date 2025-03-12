using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : BaseEntity
{
    private List<BaseEntity> characters = new List<BaseEntity>();
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
        for (int i = characters.Count - 1; i >= 0; i--)
        {
            if (characters[i]) characters[i].TakeDamage(entityData.damage);
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
}
