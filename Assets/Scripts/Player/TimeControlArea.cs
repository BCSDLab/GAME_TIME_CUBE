using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimeControlArea : MonoBehaviour
{
    [Tooltip("통과 개체의 속도 감소 계수")]
    public float velocityMultiplier = 0.2f;
    [Tooltip("영역 확장 초기값")]
    [SerializeField]
    private float m_sizeAdderBase = 3f;
    [Tooltip("영역 확장 속도 감소 계수")]
    [SerializeField]
    private float m_sizeAdderMultiplier = 0.95f;

    [System.NonSerialized]
    public float reverseMultiplier;  // 원래 속도로 복귀시키기 위한 계수

    private Vector3 m_originalScale;  // 시간 제어 영역 초기 크기
    private List<Rigidbody2D> m_collidingBullets = new List<Rigidbody2D>();  // 통과 중인 탄 리스트

    void Start()
    {
        reverseMultiplier = 1f / velocityMultiplier;
        m_originalScale = transform.localScale;
    }

    void OnEnable()  // SetActive(true) 시 호출됨
    {
        m_collidingBullets = new List<Rigidbody2D>();
        StartCoroutine("Enlarge");

        if (GameManager.instance)
        {
            GameManager.instance.StopCoroutine("RecoverCubeEnergy");
            InGameUIManager.instance.ResetTimeCubeSliderColor();
        }
    }

    void OnDisable()  // SetActive(false) 시 호출됨
    {
        StopAllCoroutines();
        transform.localScale = m_originalScale;

        foreach (Rigidbody2D bullet in m_collidingBullets)
        {
            if (!bullet) break;

            HomingBullet homingBullet = bullet.GetComponent<HomingBullet>();
            if (homingBullet != null)
            {
                homingBullet.speed *= reverseMultiplier;
                return;
            }

            bullet.velocity *= reverseMultiplier;
        }
    }

    IEnumerator Enlarge()  // 시간 제어 영역 확장
    {
        yield return new WaitForEndOfFrame();

        GameManager gameManager = GameManager.instance;
        float sizeAdder = m_sizeAdderBase;

        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            gameManager.UseCubeEnergy();
            transform.localScale += new Vector3(sizeAdder, sizeAdder, 0f);
            sizeAdder *= m_sizeAdderMultiplier;
        }
    }

    public void Disable()
    {
        StopCoroutine("Enlarge");
        StartCoroutine("Shrink");
    }

    IEnumerator Shrink()
    {
        float sizeSubtractor = m_sizeAdderBase;
        float sizeSubvtractorDivider = 1f / m_sizeAdderMultiplier;

        while (transform.localScale.x > sizeSubtractor)
        {
            transform.localScale -= new Vector3(sizeSubtractor, sizeSubtractor, 0f);
            sizeSubtractor *= sizeSubvtractorDivider;

            yield return new WaitForSeconds(0.01f);
        }

        Clear();
    }

    void Clear()
    {
        StopAllCoroutines();
        PlayerController.instance.EndTimeControl();

        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Rigidbody2D rigidbody = collision.attachedRigidbody;
            rigidbody.velocity *= velocityMultiplier;
            m_collidingBullets.Add(rigidbody);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Rigidbody2D rigidbody = collision.attachedRigidbody;
            rigidbody.velocity *= reverseMultiplier;
            m_collidingBullets.Remove(rigidbody);
        }
    }
}
