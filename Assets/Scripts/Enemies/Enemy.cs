using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class Enemy : MonoBehaviour
{
    [Tooltip("(필요 시) 이동 경로")]
    public string pathName = "";
    [Tooltip("(필요 시) 이동 속도")]
    public float speed = 5f;
    [SerializeField]
    protected int m_hp = 100;
    [SerializeField]
    [Tooltip("(필요 시) 경로 끝에서 파괴")]
    protected bool m_destroyOnArrival = true;

    protected Collider2D m_collider;
    protected SpriteRenderer m_spriteRenderer;

    protected bool m_isInvincible = true;

    private const float DROP_POWER = 300f;

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        if (pathName.Length > 0)
        {
            if (m_destroyOnArrival)
            {
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName), "easetype", iTween.EaseType.linear, "speed", speed, "oncomplete", "Die"));
            }
            else
            {
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName), "easetype", iTween.EaseType.linear, "speed", speed));
            }
        }
    }

    public int GetHP()
    {
        return m_hp;
    }
    public bool IsInvincible()
    {
        return m_isInvincible;
    }

    // 한 종류 아이템 드롭
    protected void DropItem(GameObject item, int count = 1)
    {
        for (int c = 0; c < count; c++)
        {
            Vector2 direction = new Vector2(Random.Range(0f, 0.2f), Random.Range(-0.2f, 0.2f));

            GameObject itemInst = Instantiate(item, transform.position, Quaternion.identity);
            itemInst.GetComponent<Rigidbody2D>().AddForce(direction * DROP_POWER);
        }
    }
    
    // 여러 종류 아이템 드롭
    protected void DropItems(ItemList itemList)
    {
        if (itemList == null) return;

        for (int i = 0; i < itemList.items.Count; i++)
        {
            DropItem(itemList.items[i], itemList.counts[i]);
        }
    }

    public abstract void Damage(int damage);
    public abstract void SpellDamage(int damage);
    public virtual void Die() { }
}
