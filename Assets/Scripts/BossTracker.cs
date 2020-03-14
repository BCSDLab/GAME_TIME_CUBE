using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTracker : MonoBehaviour
{
    [SerializeField]
    private Transform m_boss = null;

    private Vector3 position;

    void Start()
    {
        position = new Vector3(transform.position.x, m_boss.position.y, 0f);
        transform.position = position;
    }

    void Update()
    {
        if (m_boss == null)
        {
            Destroy(gameObject);
            return;
        }
        position.y = m_boss.position.y;
        transform.position = position;
    }
}
