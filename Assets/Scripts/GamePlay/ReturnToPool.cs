using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    public Pool pool;

    public void OnDisable()
    {
        pool.AddToPool(gameObject);
    }
}

public class Pool
{
    private Stack<GameObject> stack = new Stack<GameObject>();
    private GameObject baseObj;
    private GameObject tmp;
    private ReturnToPool returnPool;

    public Pool(GameObject baseObj)
    {
        this.baseObj = baseObj;
    }

    public GameObject Get()
    {
        while (stack.Count > 0)
        {
            tmp = stack.Pop();
            if (tmp != null)
            {
                tmp.SetActive(true);
                return tmp;
            } else
            {
                Debug.LogWarning($"game object with key {baseObj.name} has been destroyed!");
            }
        }
        tmp = GameObject.Instantiate(baseObj);
        returnPool = tmp.GetComponent<ReturnToPool>();
        if (returnPool == null) returnPool = tmp.AddComponent<ReturnToPool>();
        returnPool.pool = this;
        return tmp;
    }

    public void AddToPool(GameObject obj)
    {
        stack.Push(obj);
    }
}