using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : BaseSingleton<PoolManager>
{
    private Dictionary<GameObject, Pool> dicPools = new Dictionary<GameObject, Pool>();
    GameObject tmp;
    protected override void Awake()
    {
        base.Awake();
    }
    public GameObject Get(GameObject obj)
    {
        if (dicPools.ContainsKey(obj) == false)
        {
            dicPools.Add(obj, new Pool(obj));
        }
        return dicPools[obj].Get();
    }
    public GameObject Get(GameObject obj, Vector3 position)
    {
        tmp = Get(obj);
        tmp.transform.position = position;
        return tmp;
    }
    public T Get<T>(T obj) where T : Component
    {
        tmp = Get(obj.gameObject);
        if (tmp == null) return default;
        return tmp.GetComponent<T>();
    }
    public T Get<T>(GameObject obj, Vector3 position) where T : Component
    {
        tmp = Get(obj);
        if (tmp == null) return default;
        tmp.transform.position = position;
        return tmp.GetComponent<T>();
    }
}
