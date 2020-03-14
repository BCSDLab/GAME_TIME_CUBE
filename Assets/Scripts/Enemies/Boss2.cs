using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(DialogueTrigger))]
public class Boss2 : Enemy
{
    #region PATHS
    [SerializeField]
    private Transform m_enterPos = null;
    [SerializeField]
    private Transform m_frontPos = null;
    [SerializeField]
    private string[] m_pathNames = null;
    [SerializeField]
    [Tooltip("일부 경로 이동 시간")]
    private float m_moveTime = 2f;
    [SerializeField]
    [Tooltip("일부 경로 이동 속도")]
    private float m_moveSpeed = 4f;
    #endregion

    [SerializeField]
    private GameObject m_bossTracker = null;
    [SerializeField]
    private GameObject m_subWeaponItem = null;
    [SerializeField]
    private ItemList[] m_dropItemLists = null;

    [SerializeField]
    [Header("자코 배치")]
    private MobInfo[] m_mobInfoes = null;

    [SerializeField]
    [Header("페이즈별 시간 제한")]
    private float[] m_phaseTimeLimits = null;

    [SerializeField]
    private Transform m_target = null;

    // 패턴용
    [SerializeField]
    private GameObject m_bullet = null;
    [SerializeField]
    private GameObject m_infCrackingBullet = null;

    private DialogueTrigger m_dialogueTrigger;
    private ParticleSystem m_inParticle;
    private ParticleSystem m_outParticle;
    private AudioSource m_inParticleAudio;
    private AudioSource m_outParticleAudio;

    private AudioSource m_chargeAudio;
    private AudioSource m_shotAudio;

    private const int PHASE_COUNT = 5;
    private int m_phase = 0;
    private int m_maxPhaseHP; // 각 페이즈 최대 HP
    private int m_phaseHP;  // 현재 페이즈 HP
    private int m_totalHP;  // 총 HP
    private bool m_hasTalked = false;

    // 보스 소환수
    private bool m_isSpawning = true;
    private int m_indexToSpawn = 0;
    private float m_zacoSpawnTime = 0f;

    #region PATTERNS
    DirectionalAimedNWay m_accelNWay = null;
    DirectionalAimedRandom m_deaccelRandom = null;
    DirectionalAimedRandom m_deaccelRandom2 = null;
    DirectionalAimed m_directionalAimed = null;
    Homing m_lockOnMissile = null;
    DirectionalAimedNWay m_sineNWay = null;
    #endregion

    void Awake()
    {
        m_dialogueTrigger = GetComponent<DialogueTrigger>();
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        m_inParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        m_outParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        m_inParticleAudio = transform.GetChild(0).GetComponent<AudioSource>();
        m_outParticleAudio = transform.GetChild(1).GetComponent<AudioSource>();
        AudioSource[] audioSources = transform.GetChild(2).GetComponents<AudioSource>();
        m_chargeAudio = audioSources[0];
        m_shotAudio = audioSources[1];

        // 패턴
        DirectionalAimedNWay[] m_directionalAimedNWays = GetComponents<DirectionalAimedNWay>();
        DirectionalAimedRandom[] m_directionalAimedRandoms = GetComponents<DirectionalAimedRandom>();
        DirectionalAimed[] m_directionalAimeds = GetComponents<DirectionalAimed>();
        Homing[] m_homings = GetComponents<Homing>();
        m_accelNWay = m_directionalAimedNWays[0];
        m_deaccelRandom = m_directionalAimedRandoms[0];
        m_deaccelRandom2 = m_directionalAimedRandoms[1];
        m_directionalAimed = m_directionalAimeds[0];
        m_lockOnMissile = m_homings[0];
        m_sineNWay = m_directionalAimedNWays[1];
    }

    protected override void Start()
    {
        base.Start();

        // 진입
        m_isInvincible = true;
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "easetype", iTween.EaseType.easeOutQuint, "time", m_moveTime));

        // 다이얼로그
        m_dialogueTrigger.TriggerDialogue();

        // 보스 HP 초기화
        m_maxPhaseHP = m_hp;
        m_totalHP = m_maxPhaseHP * PHASE_COUNT;

        // 플레이어 타깃
        if (m_target == null)
        {
            m_target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (!m_hasTalked)
        {
            if (!GameManager.instance.isDialogueOn && transform.position == m_enterPos.position)
            {
                m_isInvincible = false;
                m_hasTalked = true;
                InGameUIManager.instance.InitializeBossPhaseSlots(PHASE_COUNT);  // 페이즈 슬롯 표시

                BGMManager.instance.Play(1);
                StartPhase();
            }
        }

        if (m_indexToSpawn >= m_mobInfoes.Length)
            return;

        if (m_zacoSpawnTime >= m_mobInfoes[m_indexToSpawn].engageTime && m_mobInfoes[m_indexToSpawn].engagePhase == m_phase)
        {
            StartCoroutine("SpawnZaco", m_indexToSpawn);
            m_indexToSpawn++;
        }

        m_zacoSpawnTime += Time.deltaTime;
    }

    IEnumerator Phase1()
    {
        // 탄 생성 후 한꺼번에 공격
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        Vector3 toPos = transform.position;
        List<GameObject> bullets = new List<GameObject>();

        while (true)
        {
            for (int i = 0; i < 7; i++)
            {
                toPos.y = Random.Range(-3.2f, 2.4f);
                iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", 0.3f, "easetype", iTween.EaseType.easeOutQuint));
                yield return new WaitForSeconds(0.3f);

                m_chargeAudio.Play();
                Vector3 bulletPos = toPos + Vector3.left * 0.5f;
                GameObject bulletInst = PoolManager.instance.PopFromPool(m_bullet.name);
                bullets.Add(bulletInst);
                bulletInst.transform.position = bulletPos;
                bulletInst.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }

            iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(0.5f);

            foreach (GameObject bullet in bullets)
            {
                m_shotAudio.Play();
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-8f, 0f);
                yield return new WaitForSeconds(0.1f);
            }

            bullets.Clear();
        }
    }
    IEnumerator Phase2()
    {
        // SineBullet
        iTween.MoveTo(gameObject, iTween.Hash("position", m_frontPos, "time", 3f, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(3.5f);

        m_sineNWay.StartPattern();
    }
    IEnumerator Phase3()
    {
        // 맵 바깥쪽에서 탄 생성
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);

        m_directionalAimed.StartPattern();
        Vector3 from, to;
        while (true)
        {
            if (Random.Range(0, 2) == 0) // 화면 횡단
            {
                if (Random.Range(0, 2) == 0)  // 좌 --> 우
                {
                    from = new Vector3(-10f, Random.Range(-6f, 6f));
                    to = new Vector3(10f, Random.Range(-6f, 6f));
                }
                else  // 우 --> 좌
                {
                    from = new Vector3(10f, Random.Range(-6f, 6f));
                    to = new Vector3(-10f, Random.Range(-6f, 6f));
                }
            }
            else  // 화면 종단
            {
                if (Random.Range(0, 2) == 0) // 상 --> 하
                {
                    from = new Vector3(Random.Range(-10f, 10f), 6f);
                    to = new Vector3(Random.Range(-10f, 10f), -6f);
                }
                else
                {
                    from = new Vector3(Random.Range(-10f, 10f), -6f);
                    to = new Vector3(Random.Range(-10f, 10f), 6f);
                }
            }

            Vector3 direction = to - from;
            float atan = Mathf.Atan2(direction.y, direction.x);
            float cos = Mathf.Cos(atan);
            float sin = Mathf.Sin(atan);
            float angle = atan * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bulletInst = PoolManager.instance.PopFromPool(m_bullet.name);
            bulletInst.transform.position = from;
            bulletInst.transform.rotation = rotation;
            bulletInst.SetActive(true);
            float speed = 1f;
            bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * cos, speed * sin);

            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator Phase4()
    {
        // Cracking
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        Vector3 toPos = transform.position;

        while (true)
        {
            toPos.y = Random.Range(-3.7f, 2.9f);
            iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(m_moveTime - 0.5f);
            m_lockOnMissile.StartPattern();
            yield return new WaitForSeconds(0.5f);
            m_lockOnMissile.StopPattern();
            yield return new WaitForSeconds(5f);
        }
    }
    IEnumerator Phase5()
    {
        // 이동하면서 감속탄 + 가속탄
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);

        bool isPhase5_2 = false;
        m_accelNWay.StartPattern();
        m_deaccelRandom.StartPattern();
        Vector3 toPos = transform.position;

        while (true)
        {
            if (!isPhase5_2 && m_phaseHP < m_maxPhaseHP / 2)
            {
                isPhase5_2 = true;
                m_deaccelRandom.StopPattern();
                m_deaccelRandom2.StartPattern();
            }

            toPos.y = Random.Range(-3.2f, 2.4f);
            iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(5f);
        }
    }
    void StartPhase()
    {
        m_isInvincible = false;
        m_phase++;
        m_phaseHP += m_maxPhaseHP;
        m_zacoSpawnTime = 0f;
        GameManager.instance.DestroyAllBullets();
        InGameUIManager.instance.UpdateBossPhase(m_phase);
        StopCoroutine("Timer");
        StartCoroutine("Timer");
        InGameUIManager.instance.InitStartPhase(phaseHP: m_phaseHP, phaseTimeLimit: m_phaseTimeLimits[m_phase - 1]);
        if (m_phase == PHASE_COUNT) InGameUIManager.instance.ChangeBossTimerColor(Color.red);

        Debug.Log("보스 페이즈 전환: m_phase = " + m_phase.ToString() + " HP = " + m_totalHP.ToString());

        switch (m_phase)
        {
            case 1:
                StartCoroutine("Phase1");
                break;

            case 2:
                StopCoroutine("Phase1");
                StopAllPatterns();
                GameManager.instance.DestroyAllZacos();
                StartCoroutine("Phase2");
                break;

            case 3:
                StopCoroutine("Phase2");
                StopAllPatterns();
                GameManager.instance.DestroyAllZacos();
                StartCoroutine("Phase3");
                break;

            case 4:
                StopCoroutine("Phase3");
                StopAllPatterns();
                StartCoroutine("Phase4");
                GameManager.instance.DestroyAllZacos();
                break;

            case 5:
                StopCoroutine("Phase4");
                StopAllPatterns();
                StartCoroutine("Phase5");
                GameManager.instance.DestroyAllZacos();
                break;

            default:
                throw new System.ArgumentOutOfRangeException("보스 페이즈 초과 : m_phase = " + m_phase.ToString());
        }
    }

    IEnumerator Timer()
    {
        float timeLimit = m_phaseTimeLimits[m_phase - 1];
        float timeLeft = timeLimit;
        while (timeLeft > 0.0001f)
        {
            yield return new WaitForSeconds(1f);

            timeLeft -= 1f;
            InGameUIManager.instance.UpdateBossTimer(timeLeft);
        }

        if (m_phase != PHASE_COUNT) SkipPhase();
        else
        {
            GameManager.instance.GameOver();
            InGameUIManager.instance.EnableTimeOverText();
        }
    }

    public void SkipPhase()
    {
        m_totalHP -= m_phaseHP;
        m_phaseHP = 0;

        StartPhase();
    }

    IEnumerator SpawnZaco(int index)
    {
        MobInfo mobInfo = m_mobInfoes[index];
        while (true)
        {
            if (!m_isSpawning || mobInfo.repeatCount <= 0)
                yield break;

            mobInfo.repeatCount--;

            GameObject mobInst = Instantiate(mobInfo.mob);

            if (mobInfo.pathName.Length > 0)
                mobInst.GetComponent<Enemy>().pathName = mobInfo.pathName;

            yield return new WaitForSeconds(mobInfo.repeatDelay);
        }
    }

    public override void Damage(int damage)
    {
        if (m_isInvincible) return;

        m_phaseHP -= damage;
        m_totalHP -= damage;

        InGameUIManager.instance.UpdateBossHPSlider(m_phaseHP);

        if (m_phaseHP <= 0)
        {
            if (m_totalHP <= 0)
            {
                StartCoroutine("Die");
                return;
            }

            // 페이즈 전환

            m_inParticle.Play();
            m_inParticleAudio.Play();
            DropItems(m_dropItemLists[m_phase - 1]);  // 아이템 드롭

            StartPhase();
        }
    }
    public override void SpellDamage(int damage)
    {
        Damage(damage);
    }

    public override void Die()
    {
        m_isInvincible = true;
        StopAllCoroutines();
        StopAllPatterns();
        GameManager.instance.DestroyAllZacos();
        GameManager.instance.DestroyAllBullets();
        GameManager.instance.BossDefeated();
        StartCoroutine("Blow");
    }

    private IEnumerator Blow()
    {
        m_inParticle.Play();
        m_inParticleAudio.Play();
        m_outParticle.Play();
        yield return new WaitForSeconds(m_inParticle.main.duration);

        m_collider.enabled = false;
        m_spriteRenderer.enabled = false;
        m_bossTracker.SetActive(false);
        yield return new WaitForSeconds(0.4f);

        DropSubWeaponItem();
        m_outParticleAudio.Play();
        yield return new WaitForSeconds(3f);

        GameManager.instance.StageClear();
        Destroy(gameObject);
    }

    void StopAllPatterns()
    {
        m_accelNWay.StopPattern();
        m_deaccelRandom.StopPattern();
        m_deaccelRandom2.StopPattern();
        m_directionalAimed.StopPattern();
        m_lockOnMissile.StopPattern();
        m_sineNWay.StopPattern();
    }

    void DropSubWeaponItem()
    {
        if (m_subWeaponItem)
        {
            DropItem(m_subWeaponItem);
        }
    }
}
