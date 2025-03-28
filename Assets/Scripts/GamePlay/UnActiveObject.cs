using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnActiveObject : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(UnActiveCroutine());
    }
    void Start()
    {
        StartCoroutine(UnActiveCroutine());
    }
    IEnumerator UnActiveCroutine()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(false);
    }
}
