using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;

    [Header("◆ 이동")]
    public float movementSpeed = 6f;
    public float slowModeMultiplier = 0.5f;
    [Tooltip("이동 가능 범위")]
    public Boundary boundary;
    [Header("◆ 전투")]
    public GameObject directionalBullet;
    public GameObject aimedBullet;
    public GameObject homingBullet;
    public float attackDelay = 0.2f;

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

    private bool m_isAttackToggled = false;
    private bool m_hasAttacked = false;
    private float m_attackTimer = 0f;

    private PlayerHitArea m_hitArea;

    private GameObject m_spellArea;
    private bool m_isCastingSpell = false;

    private GameObject m_timeControlArea;
    private bool m_isTimeControlEnabled = false;
    private bool m_isTimeControlShrinking = false;

    private GameManager m_gameManager;

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
        m_spellArea = GetComponentInChildren<SpellArea>().gameObject;
        m_timeControlArea = GetComponentInChildren<TimeControlArea>().gameObject;

        m_spellArea.SetActive(false);
        m_timeControlArea.SetActive(false);
        m_isCastingSpell = false;
        m_isTimeControlEnabled = false;
        m_isTimeControlShrinking = false;
    }

    void Start()
    {
        m_gameManager = GameManager.instance;
    }

    void Update()
    {
        if (m_gameManager.isDialogueOn)
        {
            m_gameManager.isPlayerAttacking = false;
            m_spellArea.SetActive(false);
            m_isCastingSpell = false;
            m_timeControlArea.SetActive(false);
            m_isTimeControlEnabled = false;
            m_isTimeControlShrinking = false;

            return;
        }

        Attack();
        Spell();
        TimeControl();
    }

    void FixedUpdate()
    {
        //if (m_gameManager.isDialogueOn)
        //{
        //    m_rigidbody.velocity = Vector2.zero;
        //    return;
        //}

        if (m_gameManager.isLoading) MoveOut();
        else Move();
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

    void MoveOut() // 화면 밖으로 탈출
    {
        Vector2 velocity = m_rigidbody.velocity;
        velocity.x = 8f;
        m_rigidbody.velocity = velocity;
    }

    #region ATTACK
    void Attack()  // 공격 관련
    {
        float axisFire = Input.GetAxis(AXIS_FIRE);

        // 공격 딜레이 적용
        if (m_hasAttacked && m_attackTimer < Time.time)
        {
            m_hasAttacked = false;
        }

        // 공격 토글
        if (axisFire > 0f && !m_isAttackToggled)
        {
            GameManager.instance.isPlayerAttacking = !GameManager.instance.isPlayerAttacking;
            m_isAttackToggled = true;
        }
        else if (axisFire == 0f)
        {
            m_isAttackToggled = false;
        }

        if (!GameManager.instance.isPlayerAttacking || m_hitArea.IsInvincible() || m_hasAttacked)
            return;

        Shoot();
    }

    void Shoot()
    {
        // TODO: 파워에 따른 패턴 구현 (0~3단계)

        if (GameManager.instance.playerPower < 1000)
        {
            ShootDirectional();
        }
        else if (GameManager.instance.playerPower < 2000)
        {
            ShootDirectional(0f, 0.1f, 0.6f);
            ShootDirectional(0f, -0.1f, 0.6f);
        }
        else if (GameManager.instance.playerPower < 3000)
        {
            ShootDirectional(0f, 0.1f, 0.6f);
            ShootDirectional(0f, -0.1f, 0.6f);
            ShootHoming(0.3f);
        }
        else if (GameManager.instance.playerPower <= 4000)
        {
            ShootDirectional(0f, 0.1f, 0.5f);
            ShootDirectional(0f, -0.1f, 0.5f);
            ShootHoming(0.3f);
            ShootAimed(0.3f);
        }

        // 딜레이
        m_hasAttacked = true;
        m_attackTimer = Time.time + attackDelay;
    }

    void ShootDirectional(float offX = 0f, float offY = 0f, float damageMultiplier = -1f)
    {
        GameObject bulletInst = PoolManager.instance.PopFromPool(directionalBullet.name);
        bulletInst.transform.position = transform.position + Vector3.right * offX + Vector3.up * offY;
        if (damageMultiplier != -1)
            bulletInst.GetComponent<PlayerBullet>().damage = (int)(bulletInst.GetComponent<PlayerBullet>().baseDamage * damageMultiplier);
        bulletInst.SetActive(true);
        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(directionalBullet.GetComponent<PlayerBullet>().speed, 0f);

        m_audioSources[0].Play();
    }

    void ShootAimed(float offX = 0f, float offY = 0f, float damageMultiplier = -1f)
    {
        if ((m_target = ChooseTarget()) == null) return;

        float speed = aimedBullet.GetComponent<PlayerBullet>().speed;
        Vector2 direction = m_target.position - transform.position;
        float atan = Mathf.Atan2(direction.y, direction.x);
        float angle = atan * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject bulletInst = PoolManager.instance.PopFromPool(aimedBullet.name);

        bulletInst.transform.position = transform.position + Vector3.right * offX + Vector3.up * offY;
        bulletInst.transform.rotation = rotation;
        if (damageMultiplier != -1)
            bulletInst.GetComponent<PlayerBullet>().damage = (int)(bulletInst.GetComponent<PlayerBullet>().baseDamage * damageMultiplier);
        bulletInst.SetActive(true);

        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(atan), speed * Mathf.Sin(atan));

        m_audioSources[1].Play();
    }

    void ShootHoming(float damageMultiplier = -1)
    {
        GameObject bulletInst = PoolManager.instance.PopFromPool(homingBullet.name);
        bulletInst.transform.position = transform.position;
        if (damageMultiplier != -1)
            bulletInst.GetComponent<PlayerBullet>().damage = (int)(bulletInst.GetComponent<PlayerBullet>().baseDamage * damageMultiplier);
        bulletInst.SetActive(true);

        m_audioSources[2].Play();
    }

    Transform ChooseTarget()
    {
        Transform target = null;
        float minDistance = Mathf.Infinity;
        float distance = Mathf.Infinity;
        m_enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in m_enemies)
        {
            distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = enemy.transform;
            }
        }
        return target;
    }
    #endregion ATTACK

    #region SPELL
    void Spell()
    {
        float axisSpell = Input.GetAxis(AXIS_SPELL);
        int spellEnergy = GameManager.instance.spellEnergy;

        if (axisSpell > 0f && !m_isCastingSpell && spellEnergy >= GameManager.SPELL_ENERGY_USAGE)
        {
            m_spellArea.SetActive(true);
            m_isCastingSpell = true;
            GameManager.instance.isPlayerSpelling = true;
        }
    }

    public void EndSpell()
    {
        m_isCastingSpell = false;
        GameManager.instance.isPlayerSpelling = false;
    }
    #endregion

    #region TIME CUBE
    void TimeControl()
    {
        float axisTimeControl = Input.GetAxis(AXIS_TIMECONTROL);
        int cubeEnergy = GameManager.instance.cubeEnergy;

        if (axisTimeControl > 0f && cubeEnergy > 0)
        {
            if (!m_isTimeControlEnabled && !m_isTimeControlShrinking)
            {
                m_timeControlArea.SetActive(true);
                m_isTimeControlEnabled = true;
            }
        }
        else
        {
            if (m_isTimeControlEnabled)
            {
                m_isTimeControlEnabled = false;
                if (!m_isTimeControlShrinking)
                {
                    m_timeControlArea.GetComponent<TimeControlArea>().Disable();
                    m_isTimeControlShrinking = true;
                }
            }
        }
    }

    public bool IsTimeControlEnabled()
    {
        return m_isTimeControlEnabled;
    }

    public void EndTimeControl()
    {
        m_isTimeControlShrinking = false;
        GameManager.instance.RecoverCube();
        InGameUIManager.instance.ResetTimeCubeSliderColor();
    }
    #endregion
}
