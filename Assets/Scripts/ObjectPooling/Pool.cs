using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 본 클래스는 MonoBehavior의 상속을 받지 않음에 유의
[System.Serializable]
public class Pool
{
    public GameObject prefab = null;

    [SerializeField]
    [Tooltip("시작할 때 얼마나 풀링해둘까요?")]
    private int m_poolCount = 0;
    [Header("디버그 전용")]
    [SerializeField]
    private List<GameObject> m_pooledItems = new List<GameObject>();
    //private Stack<GameObject> m_pooledItems = new Stack<GameObject>();

    public void Initialize(Transform parent = null)
    {
        for (int i = 0; i < m_poolCount; i++)
        {
            m_pooledItems.Add(CreateGameObject(parent));
            //m_pooledItems.Push(CreateGameObject(parent));
        }
    }

    private GameObject CreateGameObject(Transform parent = null)
    {
        GameObject item = Object.Instantiate(prefab);
        item.name = prefab.name;
        item.transform.SetParent(parent);
        item.SetActive(false);

        return item;
    }

    public void PushToPool(GameObject item, Transform parent = null)
    {
        item.SetActive(false);
        item.transform.SetParent(parent);
        item.transform.rotation = Quaternion.identity;
        m_pooledItems.Add(item);
        //m_pooledItems.Push(item);
    }

    public GameObject PopFromPool(Transform parent = null)
    {
        if (m_pooledItems.Count == 0)
        {
            m_pooledItems.Add(CreateGameObject(parent));
            //m_pooledItems.Push(CreateGameObject(parent));
        }

        GameObject item = m_pooledItems[0];
        m_pooledItems.RemoveAt(0);

        return item;
        //return m_pooledItems.Pop();
    }
}
