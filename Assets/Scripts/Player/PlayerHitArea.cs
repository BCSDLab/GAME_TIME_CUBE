using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class PlayerHitArea : MonoBehaviour
{
    public float invincibleDuration = 3f;
    public int cubeEnergyLossAmount = 2000;

    #region ITEM  
    public GameObject cubeChargeItem;
    public GameObject spellChargeItem;
    public GameObject powerUpItem;
    private const float DROP_POWER = 280f;
    #endregion

    private const float BLINK_DELAY = 0.05f;

    private Collider2D m_collider;
    private SpriteRenderer m_colliderRenderer;
    [SerializeField]
    private SpriteRenderer m_characterRenderer = null;
    private ParticleSystem m_particleSystem;
    private AudioSource m_audioSource;

    private PlayerController m_playerController;
    private bool m_isInvincible = false;

    private readonly Color HIT_COLOR = new Color32(250, 100, 100, 150);

    void Start()
    {
        m_collider = GetComponent<Collider2D>();
        m_colliderRenderer = GetComponent<SpriteRenderer>();
        m_particleSystem = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();
        m_playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") || collision.CompareTag("Enemy"))
        {
            if (m_isInvincible || GameManager.instance.isPlayerSpelling) return;

            if (!m_isInvincible && !GameManager.instance.isDialogueOn)
            {
                int powerToDrop = (int)(GameManager.instance.playerPower * 0.3f);  // 30%
                StartCoroutine("Damage");
                PoolManager.instance.PushToPool(collision.gameObject);

                // 아이템 드롭
                DropItem(cubeChargeItem);
                DropPowerItems(powerToDrop);

                m_particleSystem.Play();
                m_audioSource.Play();
            }
        }
    }

    IEnumerator Damage()
    {
        GameManager.instance.DamagePlayer();

        if (m_playerController.IsTimeControlEnabled())
        {
            GameManager.instance.UseCubeEnergy(cubeEnergyLossAmount);
        }

        m_isInvincible = true;

        float loopCount = invincibleDuration / BLINK_DELAY / 2f;
        Color rendererOriginalColor = m_colliderRenderer.color;
        Color characterOriginalColor = m_characterRenderer.color;

        for (int i = 0; i < loopCount; i++)
        {
            if (GameManager.instance.isGameOver) break;

            m_colliderRenderer.color = Color.red;
            m_characterRenderer.color = HIT_COLOR;
            yield return new WaitForSeconds(BLINK_DELAY);

            m_colliderRenderer.color = rendererOriginalColor;
            m_characterRenderer.color = characterOriginalColor;
            yield return new WaitForSeconds(BLINK_DELAY);
        }

        m_isInvincible = false;
    }

    public bool IsInvincible()
    {
        return m_isInvincible;
    }

    private void DropPowerItems(int powerToDrop)
    {
        int originalPower = powerUpItem.GetComponent<PowerUpItem>().power;
        Vector2 originalScale = powerUpItem.transform.localScale;

        int power1000 = powerToDrop / 1000;
        powerUpItem.GetComponent<PowerUpItem>().power = 1000;
        powerUpItem.transform.localScale = originalScale * 1.5f;
        for (int i = 0; i < power1000; i++)
        {
            DropItem(powerUpItem);
        }

        int power100 = powerToDrop % 1000 / 100;
        powerUpItem.GetComponent<PowerUpItem>().power = 100;
        powerUpItem.transform.localScale = originalScale * 1.1f;
        for (int i = 0; i < power100; i++)
        {
            DropItem(powerUpItem);
        }

        int power10 = powerToDrop % 100 / 10;
        powerUpItem.GetComponent<PowerUpItem>().power = 10;
        powerUpItem.transform.localScale = originalScale * 0.7f;
        for (int i = 0; i < power10; i++)
        {
            DropItem(powerUpItem);
        }

        powerUpItem.GetComponent<PowerUpItem>().power = originalPower;
        powerUpItem.transform.localScale = originalScale;
    }

    private void DropItem(GameObject item)
    {
        Vector2 direction = new Vector2(Random.Range(1f, 1.5f), Random.Range(-0.2f, 0.2f));

        GameObject itemInst = Instantiate(item, transform.position + new Vector3(1f,0), Quaternion.identity);
        itemInst.GetComponent<Rigidbody2D>().AddForce(direction * DROP_POWER);
    }
}
