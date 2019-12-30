using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 대상 파괴
        if (collision.CompareTag("Bullet") || collision.CompareTag("PlayerBullet"))
        {
            PoolManager.instance.PushToPool(collision.gameObject);
        }
    }
}
