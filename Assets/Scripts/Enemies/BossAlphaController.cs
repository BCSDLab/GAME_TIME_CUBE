using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(DialogueTrigger))]
public class BossAlphaController : Enemy
{
    #region PATHS
    [SerializeField]
    private Transform m_enterPos = null;
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

    #region PATTERNS
    //Phase1
    private Homing m_homing;
    private DonutAimed m_donutAimed;
    //Phase2
    private SpiralMulti m_spiralMulti;
    private DirectionalNormal m_directionalNormal;
    //Phase3
    private Homing1 m_homing1;
    private RadialMulti m_radialMulti;
    //Phase4
    private PosToPos m_posToPos;
    private DirectionalAimedNWay m_directionalAimedNWay;
    //Phase5
    private VerticalLeft m_verticalLeft;
    private DirectionalAimedRandom m_directionalAimedRandom;
    //Phase6
    private BiDirectional m_biDirectional;
    #endregion

    private DialogueTrigger m_dialogueTrigger;
    private ParticleSystem m_inParticle;
    private ParticleSystem m_outParticle;
    private AudioSource m_inParticleAudio;
    private AudioSource m_outParticleAudio;

    private const int PHASE_COUNT = 6;
    private int m_phase = 0;
    private int m_maxPhaseHP; // 각 페이즈 최대 HP
    private int m_phaseHP;  // 현재 페이즈 HP
    private int m_totalHP;  // 총 HP
    private bool m_hasTalked = false;

    // 보스 소환수 소환
    private bool m_isSpawning = true;
    private int m_indexToSpawn = 0;
    private float m_zacoSpawnTime = 0f;

    void Awake()
    {
        m_dialogueTrigger = GetComponent<DialogueTrigger>();
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        m_inParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        m_outParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        m_inParticleAudio = transform.GetChild(0).GetComponent<AudioSource>();
        m_outParticleAudio = transform.GetChild(1).GetComponent<AudioSource>();

        // 패턴
        // Phase1
        m_homing = GetComponent<Homing>();
        m_donutAimed = GetComponent<DonutAimed>();
        // Phase2
        m_spiralMulti = GetComponent<SpiralMulti>();
        m_directionalNormal = GetComponent<DirectionalNormal>();
        // Phase3
        m_homing1 = GetComponent<Homing1>();
        m_radialMulti = GetComponent<RadialMulti>();
        // Phase4
        m_posToPos = GetComponent<PosToPos>();
        m_directionalAimedNWay = GetComponent<DirectionalAimedNWay>();
        // Phase5
        m_verticalLeft = GetComponent<VerticalLeft>();
        m_directionalAimedRandom = GetComponent<DirectionalAimedRandom>();
        // Phase6
        m_biDirectional = GetComponent<BiDirectional>();
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

        Debug.Log(m_indexToSpawn);
        m_zacoSpawnTime += Time.deltaTime;
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
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(m_pathNames[0]), "speed", m_moveSpeed, "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop, "movetopath", false));
                m_homing.StartPattern();
                m_donutAimed.StartPattern();
                break;

            case 2:
                m_homing.StopPattern();
                m_donutAimed.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
                m_spiralMulti.StartPattern();
                m_directionalNormal.StartPattern();
                break;

            case 3:
                m_spiralMulti.StopPattern();
                m_directionalNormal.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(m_pathNames[0]), "speed", m_moveSpeed * 1.2f, "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop, "movetopath", false));
                m_homing1.StartPattern();
                m_radialMulti.StartPattern();
                break;

            case 4:
                m_homing1.StopPattern();
                m_radialMulti.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
                m_posToPos.StartPattern();
                m_directionalAimedNWay.StartPattern();
                break;

            case 5:
                m_posToPos.StopPattern();
                m_directionalAimedNWay.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(m_pathNames[1]), "speed", m_moveSpeed, "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop, "movetopath", false));
                m_verticalLeft.StartPattern();
                m_directionalAimedRandom.StartPattern();
                break;

            case 6:
                m_verticalLeft.StopPattern();
                m_directionalAimedRandom.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
                m_biDirectional.StartPattern();
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
        StopAllPatterns();
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
        m_homing.StopPattern();
        m_donutAimed.StopPattern();
        m_spiralMulti.StopPattern();
        m_directionalNormal.StopPattern();
        m_homing1.StopPattern();
        m_radialMulti.StopPattern();
        m_posToPos.StopPattern();
        m_directionalAimedNWay.StopPattern();
        m_verticalLeft.StopPattern();
        m_directionalAimedRandom.StopPattern();
        m_biDirectional.StopPattern();
    }

    void DropSubWeaponItem()
    {
        if (m_subWeaponItem)
        {
            DropItem(m_subWeaponItem);
        }
    }
}
