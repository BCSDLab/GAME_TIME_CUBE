using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(DialogueTrigger))]
public class Boss1 : Enemy
{
    #region PATHS
    [SerializeField]
    private Transform m_enterPos = null;
    [SerializeField]
    private Transform m_enterPos2 = null;
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
    private GameObject m_lifeItem = null;
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
    private GameObject m_lockOnMarkXL;
    [SerializeField]
    private GameObject m_instantExplodingBullet;
    [SerializeField]
    private GameObject m_lockOnMarkRhombus;
    [SerializeField]
    private GameObject m_arrowMark;

    private DialogueTrigger m_dialogueTrigger;
    private ParticleSystem m_inParticle;
    private ParticleSystem m_outParticle;
    private ParticleSystem m_hitParticle;
    private AudioSource m_inParticleAudio;
    private AudioSource m_outParticleAudio;

    private AudioSource m_chargeAudio;
    private AudioSource m_bigShotAudio;

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
    private DirectionalAimed m_bigShot;
    private DirectionalAimedRandom m_machinegun;
    private Homing m_bigLockOnMissile;
    private DirectionalAimedRandom m_machinegun2;
    #endregion

    void Awake()
    {
        m_dialogueTrigger = GetComponent<DialogueTrigger>();
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        m_inParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        m_outParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        m_hitParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
        m_inParticleAudio = transform.GetChild(0).GetComponent<AudioSource>();
        m_outParticleAudio = transform.GetChild(1).GetComponent<AudioSource>();
        AudioSource[] audioSources = transform.GetChild(2).GetComponents<AudioSource>();
        m_chargeAudio = audioSources[0];
        m_bigShotAudio = audioSources[1];

        // 패턴
        DirectionalAimed[] directionalAimeds = GetComponents<DirectionalAimed>();
        DirectionalAimedRandom[] directionalAimedRandoms = GetComponents<DirectionalAimedRandom>();
        Homing[] homings = GetComponents<Homing>();
        m_bigShot = directionalAimeds[0];
        m_machinegun = directionalAimedRandoms[0];
        m_bigLockOnMissile = homings[0];
        m_machinegun2 = directionalAimedRandoms[1];
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
        // 플레이어와 같은 수평선상으로 이동 후 크고 빠른 탄 사격
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        Vector3 toPos = transform.position;
        m_machinegun.StartPattern();

        while (true)
        {
            toPos.y = m_target.position.y;
            iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(m_moveTime - 0.5f);
            m_chargeAudio.Play();
            yield return new WaitForSeconds(0.8f);
            m_bigShotAudio.Play();
            m_bigShot.StartPattern();
            yield return new WaitForSeconds(0.1f);
            m_bigShot.StopPattern();
            yield return new WaitForSeconds(1.4f);
        }
    }
    IEnumerator Phase2()
    {
        // 상하 임의 위치로 이동 후 플레이어 락온 후 미사일 발사
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos2, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        Vector3 toPos = transform.position;

        while (true)
        {
            toPos.y = Random.Range(-3.7f, 2.9f);
            iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(m_moveTime - 0.5f);
            m_bigLockOnMissile.StartPattern();
            yield return new WaitForSeconds(0.5f);
            m_bigLockOnMissile.StopPattern();
            yield return new WaitForSeconds(2f);
        }
    }
    IEnumerator Phase3()
    {
        // y축 중앙으로 이동하고, 조준 후 해당 위치에 폭발탄 생성
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        m_machinegun2.StartPattern();
        GameObject lockOnMarkXL = Instantiate(m_lockOnMarkXL);

        while (true)
        {
            m_chargeAudio.Play();
            iTween.MoveTo(lockOnMarkXL, iTween.Hash("position", m_target.position, "time", 0.7f, "easetype", iTween.EaseType.easeInOutSine));
            yield return new WaitForSeconds(1.2f);
            GameObject instantExplodingBulletInst = PoolManager.instance.PopFromPool(m_instantExplodingBullet.name);
            instantExplodingBulletInst.transform.position = lockOnMarkXL.transform.position + Vector3.right * 0.05f;
            instantExplodingBulletInst.SetActive(true);
            yield return new WaitForSeconds(1.2f);
        }
    }
    IEnumerator Phase4()
    {
        // 플레이어가 이동하는 방향을 따라 폭격
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos2, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        Vector3 lockOnPos;
        Vector3 arrowMarkPos;
        Vector3 explodePos;

        while (true)
        {
            // 조준
            lockOnPos = m_target.position;
            GameObject lockOnMark = Instantiate(m_lockOnMarkRhombus, lockOnPos, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            // 화살표
            const float MIN_MAGNITUDE = 3f;
            Vector3 dist = m_target.position - lockOnPos;
            float angle;
            if (dist.magnitude > MIN_MAGNITUDE)  // 거리가 멀면 플레이어 위치에 생성
            {
                arrowMarkPos = m_target.position;
            }
            else  // 거리가 0이면 랜덤, 가까우면 해당 방향으로 생성
            {
                if (dist.magnitude < 0.001f) angle = Random.Range(0f, 2f * Mathf.PI);
                else
                {
                    float at = Mathf.Atan2(dist.y, dist.x);
                    angle = at * Mathf.Rad2Deg;
                }
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 vec = Vector3.right * cos + Vector3.up * sin;
                arrowMarkPos = lockOnPos + vec * MIN_MAGNITUDE;
            }
            Vector3 direction = arrowMarkPos - lockOnPos;
            float atan = Mathf.Atan2(direction.y, direction.x);
            float arrowAngle = atan * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
            GameObject arrowMark = Instantiate(m_arrowMark, arrowMarkPos, rotation);
            yield return new WaitForSeconds(0.5f);

            explodePos = lockOnPos;
            const int EXPLOSION_COUNT = 3;
            for (int i = 0; i < EXPLOSION_COUNT; i++)
            {
                GameObject instantExplodingBulletInst = PoolManager.instance.PopFromPool(m_instantExplodingBullet.name);
                instantExplodingBulletInst.transform.position = explodePos + Vector3.right * 0.05f;
                instantExplodingBulletInst.SetActive(true);
                Vector3 distance = direction / (EXPLOSION_COUNT - 1);
                explodePos += distance;
                yield return new WaitForSeconds(0.3f);
            }

            Destroy(lockOnMark);
            Destroy(arrowMark);
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator Phase5()
    {
        // 조합
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
        yield return new WaitForSeconds(m_moveTime + 0.5f);
        m_machinegun2.StartPattern();

        Vector3 toPos = transform.position;
        GameObject lockOnMarkXL;
        Vector3 lockOnPos;
        Vector3 arrowMarkPos;
        Vector3 explodePos;

        while (true)
        {
            // Phase 1
            toPos.y = m_target.position.y;
            iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(m_moveTime - 0.5f);
            m_chargeAudio.Play();
            yield return new WaitForSeconds(0.8f);
            m_bigShotAudio.Play();
            m_bigShot.StartPattern();
            yield return new WaitForSeconds(0.5f);
            m_bigShot.StopPattern();
            yield return new WaitForSeconds(1f);

            // Phase 2
            toPos.y = Random.Range(-3.7f, 2.9f);
            iTween.MoveTo(gameObject, iTween.Hash("position", toPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(m_moveTime - 0.5f);
            m_bigLockOnMissile.StartPattern();
            yield return new WaitForSeconds(0.5f);
            m_bigLockOnMissile.StopPattern();
            yield return new WaitForSeconds(2f);

            // Phase 3
            iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
            yield return new WaitForSeconds(m_moveTime + 0.5f);
            lockOnMarkXL = Instantiate(m_lockOnMarkXL);
            m_chargeAudio.Play();
            iTween.MoveTo(lockOnMarkXL, iTween.Hash("position", m_target.position, "time", 0.7f, "easetype", iTween.EaseType.easeInOutSine));
            yield return new WaitForSeconds(1.2f);
            GameObject instantExplodingBulletInst = PoolManager.instance.PopFromPool(m_instantExplodingBullet.name);
            instantExplodingBulletInst.transform.position = lockOnMarkXL.transform.position + Vector3.right * 0.05f;
            instantExplodingBulletInst.SetActive(true);
            yield return new WaitForSeconds(1.2f);
            Destroy(lockOnMarkXL);

            // Phase 4
            // 조준
            lockOnPos = m_target.position;
            GameObject lockOnMark = Instantiate(m_lockOnMarkRhombus, lockOnPos, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            // 화살표
            const float MIN_MAGNITUDE = 3f;
            Vector3 dist = m_target.position - lockOnPos;
            float angle;
            if (dist.magnitude > MIN_MAGNITUDE)  // 거리가 멀면 플레이어 위치에 생성
            {
                arrowMarkPos = m_target.position;
            }
            else  // 거리가 0이면 랜덤, 가까우면 해당 방향으로 생성
            {
                if (dist.magnitude < 0.001f) angle = Random.Range(0f, 2f * Mathf.PI);
                else
                {
                    float at = Mathf.Atan2(dist.y, dist.x);
                    angle = at * Mathf.Rad2Deg;
                }
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 vec = Vector3.right * cos + Vector3.up * sin;
                arrowMarkPos = lockOnPos + vec * MIN_MAGNITUDE;
            }
            Vector3 direction = arrowMarkPos - lockOnPos;
            float atan = Mathf.Atan2(direction.y, direction.x);
            float arrowAngle = atan * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
            GameObject arrowMark = Instantiate(m_arrowMark, arrowMarkPos, rotation);
            yield return new WaitForSeconds(0.5f);

            explodePos = lockOnPos;
            const int EXPLOSION_COUNT = 3;
            for (int i = 0; i < EXPLOSION_COUNT; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(m_instantExplodingBullet.name);
                bulletInst.transform.position = explodePos + Vector3.right * 0.05f;
                bulletInst.SetActive(true);
                Vector3 distance = direction / (EXPLOSION_COUNT - 1);
                explodePos += distance;
                yield return new WaitForSeconds(0.3f);
            }

            Destroy(lockOnMark);
            Destroy(arrowMark);
            yield return new WaitForSeconds(1f);
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
        m_hitParticle.Play();
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
        DropLifeItem();
        m_outParticleAudio.Play();
        yield return new WaitForSeconds(3f);

        GameManager.instance.StageClear();
        Destroy(gameObject);
    }

    void StopAllPatterns()
    {
        m_bigShot.StopPattern();
        m_machinegun.StopPattern();
        m_bigLockOnMissile.StopPattern();
        m_machinegun2.StopPattern();
    }

    void DropSubWeaponItem()
    {
        if (m_subWeaponItem)
        {
            m_subWeaponItem.GetComponent<SubWeaponItem>().PickUpWithName("Follower");
        }
    }

    void DropLifeItem()
    {
        if (m_lifeItem)
        {
            DropItem(m_lifeItem);
        }
    }
}
