using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템이 밖으로 튀어나가지 않게 밀어주는 장치
[RequireComponent(typeof(Collider2D))]
public class ItemBorder : MonoBehaviour
{
    private const float PUSHING_FORCE = 5f;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            float direction = (collision.transform.position.y >= 0) ? -1f : 1f;
            collision.attachedRigidbody.AddForce(new Vector2(0f, direction * PUSHING_FORCE));
        }
    }
}
