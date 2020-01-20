using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 15;
    public float speed = 20f;
    public int spellCharge = 10;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyController = collision.GetComponent<Enemy>();
            if (!enemyController.IsInvincible())
            {
                enemyController.Damage(damage);
                GameManager.instance.AddSpellEnergy(spellCharge);
                GameManager.instance.AddScore(damage);
            }

            PoolManager.instance.PushToPool(gameObject);
            //Destroy(this.gameObject);  // 풀링 성능 테스트용
        }
    }
}
