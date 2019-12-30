using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 대상 파괴
        if (collision.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
        }
    }
}
