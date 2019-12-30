using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null;

    public List<Pool> poolList = new List<Pool>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            poolList[i].Initialize(transform);
        }
    }

    public bool PushToPool(GameObject item, Transform parent = null)
    {
        Pool pool = FindPool(item.name);
        if (pool == null)
        {
            Debug.Log("풀링하려는 오브젝트가 풀링 리스트에 없습니다!");
            return false;
        }

        pool.PushToPool(item, parent ?? transform);  // transform IF parent IS NULL

        return true;
    }

    /// <summary>
    /// 오브젝트 SetActive(true) 필요
    /// </summary>
    public GameObject PopFromPool(string name, Transform parent = null)
    {
        Pool pool = FindPool(name);
        if (pool == null)
        {
            return null;
        }

        return pool.PopFromPool(parent);
    }

    Pool FindPool(string itemName)
    {
        // 이름이 같은 오브젝트를 찾아 반환 (람다 식 활용)
        return poolList.Find((Pool pool) => { return pool.prefab.name.Equals(itemName); });
    }
}
