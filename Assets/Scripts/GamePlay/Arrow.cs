using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private BaseEntity target;
    private int damage;
    private float flightDuration = 0.5f;
    private Vector3 startPosition;
    private Vector3 controlPoint;
    private float t;
    private Vector3 targetPosition;
    public void SetTarget(BaseEntity target, int damage)
    {
        this.target = target;
        this.damage = damage;
        startPosition = transform.position;

        if (target)
        {
            targetPosition = target.transform.position;
            Vector3 midPoint = (startPosition + target.transform.position) / 2;
            midPoint.y += 2f;

            controlPoint = midPoint;
            StartCoroutine(MoveArrow());
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    private IEnumerator MoveArrow()
    {
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / flightDuration;

            Vector3 a = Vector3.Lerp(startPosition, controlPoint, t);
            Vector3 b = Vector3.Lerp(controlPoint, targetPosition, t);
            transform.position = Vector3.Lerp(a, b, t);

            transform.right = (b - a).normalized;

            yield return null;
        }

        if (target)
        {
            target.TakeDamage(damage);
            
        }
        gameObject.SetActive(false);

    }
}
