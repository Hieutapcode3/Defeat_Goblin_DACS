using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnim : MonoBehaviour
{
    [SerializeField] private bool adjustZAxis = true;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(state.fullPathHash, 0, Random.Range(0, 1f));
            animator.speed = Random.Range(0.6f, 1f);
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if (adjustZAxis)
        {
            Vector3 parentPos = transform.position;
            parentPos.z = parentPos.y * 0.01f;
            transform.position = parentPos;

            foreach (Transform child in transform)
            {
                Vector3 childPos = child.position;
                childPos.z = parentPos.z;
                child.position = childPos;
            }
        }
    }
}
