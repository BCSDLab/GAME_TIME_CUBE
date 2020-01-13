using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TestController : MonoBehaviour
{
    public static TestController instance = null;

    [Header("◆ 이동")]
    public float movementSpeed = 6f;
    public float slowModeMultiplier = 0.5f;
    [Tooltip("이동 가능 범위")]
    public Boundary boundary;

    private Rigidbody2D m_rigidbody;
    private AudioSource[] m_audioSources;
    private Animator m_spriteAnimator;

    #region INPUTS
    private const string AXIS_HORIZONTAL = "Horizontal";
    private const string AXIS_VERTICAL = "Vertical";
    private const string AXIS_SLOWMODE = "SlowMode";
    private const string AXIS_FIRE = "Fire";
    private const string AXIS_SPELL = "Spell";
    private const string AXIS_TIMECONTROL = "TimeControl";
    #endregion

    private PlayerHitArea m_hitArea;

    private GameObject[] m_enemies;
    private Transform m_target;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        m_rigidbody = GetComponent<Rigidbody2D>();
        m_audioSources = GetComponents<AudioSource>();
        //m_spriteAnimator = GetComponentInChildren<Animator>();
        m_hitArea = GetComponentInChildren<PlayerHitArea>();
    }
    void FixedUpdate()
    {
        Move();
    }

    void Move()  // 이동 관련
    {
        float horizontal = Input.GetAxis(AXIS_HORIZONTAL);
        float vertical = Input.GetAxis(AXIS_VERTICAL);
        float slowMode = Input.GetAxis(AXIS_SLOWMODE);
        Vector2 velocity = m_rigidbody.velocity;

        //m_spriteAnimator.SetFloat("Horizontal", horizontal);
        //m_spriteAnimator.SetFloat("Velocity Y", m_rigidbody.velocity.y);

        float speed = movementSpeed * ((Mathf.Abs(slowMode) > 0f) ? slowModeMultiplier : 1f);

        //velocity.x = speed * ((horizontal > 0f) ? 1 : (horizontal < 0f) ? -1 : 0);
        //velocity.y = speed * ((vertical > 0f) ? 1 : (vertical < 0f) ? -1 : 0);
        velocity.x = speed * horizontal;
        velocity.y = speed * vertical;

        velocity.x = (horizontal > 0f && m_rigidbody.position.x >= boundary.xMax) || (horizontal < 0f && m_rigidbody.position.x <= boundary.xMin) ? 0 : velocity.x;
        velocity.y = (vertical > 0f && m_rigidbody.position.y >= boundary.yMax) || (vertical < 0f && m_rigidbody.position.y <= boundary.yMin) ? 0 : velocity.y;

        m_rigidbody.velocity = velocity;
    }
}
