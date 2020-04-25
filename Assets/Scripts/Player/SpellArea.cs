using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpellArea : MonoBehaviour
{
    public float time = 3f;
    public int damage = 10;
    [Tooltip("영역 확장 초기값")]
    [SerializeField]
    private float m_sizeAdderBase = 1f;
    [Tooltip("영역 확장 속도 감소 계수")]
    [SerializeField]
    private float m_sizeAdderMultiplier = 0.9f;

    private const float ATTACK_DELAY = 0.01f;

    private PlayerController m_playerController;
    private float m_sizeAdder;
    private Vector3 m_originalScale;
    private Quaternion m_originalRotation;

    void Start()
    {
        m_playerController = GetComponentInParent<PlayerController>();
        m_originalScale = transform.localScale;
        m_originalRotation = transform.localRotation;

        InGameUIManager.instance.InitDynSpellSlider(time);
    }

    void FixedUpdate()
    {
        transform.Rotate(-Vector3.forward * 0.6f);
    }

    void OnEnable()
    {
        InGameUIManager.instance.EnableDynSpellSlider(true);
        m_sizeAdder = m_sizeAdderBase;
        StartCoroutine("Spell");
    }

    void OnDisable()
    {
        InGameUIManager.instance.EnableDynSpellSlider(false);
        StopAllCoroutines();
        transform.localScale = m_originalScale;
        transform.localRotation = m_originalRotation;
    }

    IEnumerator Spell()
    {
        yield return new WaitForEndOfFrame();
        GameManager gameManager = GameManager.instance;
        gameManager.UseSpell();

        float repeatCount = time / ATTACK_DELAY;
        for (int i = 0; i < repeatCount; i++)
        {
            yield return new WaitForSeconds(ATTACK_DELAY);
            transform.localScale += new Vector3(m_sizeAdder, m_sizeAdder, 0f);
            m_sizeAdder *= m_sizeAdderMultiplier;

            InGameUIManager.instance.UpdateDynSpellSlider(ATTACK_DELAY);
        }

        // 스펠 끝을 알리고 스스로 비활성화
        m_playerController.EndSpell();
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet == null || !bullet.m_spellDestroyed)
                PoolManager.instance.PushToPool(collision.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyController = collision.GetComponent<Enemy>();
            enemyController.SpellDamage(damage);
            GameManager.instance.AddScore(damage);
        }
    }
}
