using System.Collections;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    private float duration = 1f;
    private Vector3 startPosition;
    private Vector3 peakPoint;
    private Vector3 endPosition;
    private float t;
    public void MoveEffect()
    {
        startPosition = transform.position;
        peakPoint = startPosition + Vector3.up * 1f;
        endPosition = startPosition + Vector3.right * 0.8f + Vector3.down * 3f;
        transform.localScale = Vector3.zero;
        StartCoroutine(MoveCoin());
    }

    private IEnumerator MoveCoin()
    {
        t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime / duration;
            Vector3 a = Vector3.Lerp(startPosition, peakPoint, t);
            Vector3 b = Vector3.Lerp(peakPoint, endPosition, t);
            transform.position = Vector3.Lerp(a, b, t);
            if (t <= 1f)
            {
                float scaleT = t / 0.5f;
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one* 0.2f, scaleT);
            }
            yield return null;
        }
        gameObject.SetActive(false);
    }
}