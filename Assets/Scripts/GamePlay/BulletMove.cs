using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private BaseEntity target;
    private float speed = 5f;
    public float stopDistance = 0.05f;
    private int damage;
    private Vector3 targetpos;

    public void SetTarget(BaseEntity entity,int dmg)
    {
        target = entity;
        this.damage = dmg;
        if(target != null)
            targetpos = target.transform.position + Vector3.up * 0.3f;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 direction = (targetpos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, targetpos) <= stopDistance)
        {
            target.TakeDamage(damage);
            this.gameObject.SetActive(false);
        }
    }
}