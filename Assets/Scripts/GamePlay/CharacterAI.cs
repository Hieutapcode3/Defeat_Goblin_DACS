using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class CharacterAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float nextWPDistance = 0.2f;
    private List<GameObject> enemies;
    private Seeker seeker;
    private Path path;
    private Transform target;
    private int currentWP = 0;
    private BaseEntity baseEntity;
    private AttackType attackType;
    private float attackRange;
    public void Init(Seeker seeker, List<GameObject> enemies)
    {
        baseEntity = GetComponent<BaseEntity>();
        this.attackType = baseEntity.attackType;
        this.attackRange = baseEntity.attackRange;
        this.seeker = seeker;
        this.enemies = enemies;
        //this.baseEntity.Reset();
        FindNewTarget();
    }

    void Update()
    {
        if (baseEntity == null || seeker == null) return;
        if (target == null || enemies.Count == 0)
        {
            baseEntity.Idle();
            FindNewTarget();
            return;
        }

        MoveAlongPath();
    }

    private void FindNewTarget()
    {
        enemies.RemoveAll(enemy => enemy == null);

        if (enemies.Count == 0)
        {
            path = null;
            return;
        }

        target = FindClosestTargetByPath();
        if (target != null)
        {
            Vector2 offsetPosition = GetRandomPositionAround(target.position, 0.25f);
            seeker.StartPath(transform.position, offsetPosition, OnPathCallBack);
        }
    }
    private Vector2 GetRandomPositionAround(Vector2 center, float radius)
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        return center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }
    Transform FindClosestTargetByPath()
    {
        enemies = LevelController.Instance.GetEntitiesGameObjects();
        Transform bestTarget = null;
        float bestPathDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Path tempPath = seeker.StartPath(transform.position, enemy.transform.position);
            tempPath.BlockUntilCalculated();

            if (!tempPath.error && tempPath.GetTotalLength() < bestPathDistance)
            {
                bestTarget = enemy.transform;
                bestPathDistance = tempPath.GetTotalLength();
            }
        }

        return bestTarget;
    }
    private void OnPathCallBack(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWP = 0;
        }
    }

    private void MoveAlongPath()
    {
        if (currentWP >= path.vectorPath.Count)
        {
            baseEntity.Idle();
            AttackTarget();
            return;
        }

        if (attackType == AttackType.Ranged && Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            baseEntity.Idle();
            AttackTarget();
            return;
        }

        if (path == null)
            return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWP] - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y * 0.01f);
        if (Vector2.Distance(transform.position, path.vectorPath[currentWP]) < nextWPDistance)
        {
            currentWP++;
        }

        baseEntity.Moving();

        if ((direction.x > 0 && !baseEntity.IsFacingRight) || (direction.x < 0 && baseEntity.IsFacingRight))
        {
            baseEntity.Flip();
        }
    }

    void AttackTarget()
    {
        if (target != null)
        {
            baseEntity.Attack(target.GetComponent<BaseEntity>());
        }
    }
}
