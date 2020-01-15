using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(DialogueTrigger))]
public class BossController : EnemyController
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

    #region PATTERNS
    private DirectionalNormal m_directionalNormal;
    private DirectionalAimed m_directionalAimed;
    private DirectionalAimedRandom m_directionalAimedRandom;
    private RadialMulti m_radialMulti;
    private DonutAimed m_roundGuided;
    private SpiralMulti m_spiralMulti;
    private Homing m_homing;
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

    private DialogueTrigger m_dialogueTrigger;

    private const int PHASE_COUNT = 6;
    private int m_phase = 0;
    private int m_maxPhaseHP = 1000; // 각 페이즈 최대 HP
    private int m_phaseHP = 0;  // 현재 페이즈 HP
    private int m_totalHP;  // 총 HP
    private bool m_hasTalked = false;

    // 보스 소환수 소환
    private bool m_isSpawning = true;
    private int m_indexToSpawn = 0; 
    private float m_zacoSpawnTime = 0f;

    protected override void Start()
    {
        base.Start();
        m_dialogueTrigger = GetComponent<DialogueTrigger>();

        // 패턴
        m_directionalAimed = GetComponent<DirectionalAimed>();
        m_directionalAimedRandom = GetComponent<DirectionalAimedRandom>();
        m_directionalNormal = GetComponent<DirectionalNormal>();
        m_radialMulti = GetComponent<RadialMulti>();
        m_roundGuided = GetComponent<DonutAimed>();
        m_spiralMulti = GetComponent<SpiralMulti>();
        m_homing = GetComponent<Homing>();

        // 진입
        m_isInvincible = true;
        iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "easetype", iTween.EaseType.easeOutQuint, "time", m_moveTime));

        // 다이얼로그
        m_dialogueTrigger.TriggerDialogue();

        // 보스 HP 초기화
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
        InGameUIManager.instance.DisplayBossHPSlider(hp: m_phaseHP);

        Debug.Log("보스 페이즈 전환: m_phase = " + m_phase.ToString() + " HP = " + m_totalHP.ToString());

        switch (m_phase)
        {
            case 1:
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(m_pathNames[0]), "speed", m_moveSpeed, "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop, "movetopath", false));
                m_homing.StartPattern();
                break;

            case 2:
                m_homing.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
                m_directionalAimed.StartPattern();
                break;

            case 3:
                m_directionalAimed.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(m_pathNames[0]), "speed", m_moveSpeed * 1.2f, "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop, "movetopath", false));
                m_directionalAimedRandom.StartPattern();
                break;

            case 4:
                m_directionalAimedRandom.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
                m_radialMulti.StartPattern();
                break;

            case 5:
                m_radialMulti.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(m_pathNames[1]), "speed", m_moveSpeed, "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop, "movetopath", false));
                m_roundGuided.StartPattern();
                break;

            case 6:
                m_roundGuided.StopPattern();
                GameManager.instance.DestroyAllZacos();
                iTween.MoveTo(gameObject, iTween.Hash("position", m_enterPos, "time", m_moveTime, "easetype", iTween.EaseType.easeOutQuint));
                m_spiralMulti.StartPattern();
                break;

            default:
                throw new System.ArgumentOutOfRangeException("보스 페이즈 초과 : m_phase = " + m_phase.ToString());
        }
    }

    public int GetTotalHP()
    {
        return m_totalHP;
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
                mobInst.GetComponent<EnemyController>().pathName = mobInfo.pathName;

            yield return new WaitForSeconds(mobInfo.repeatDelay);
        }
    }

    public override void Damage(int damage)
    {
        if (m_isInvincible)
            return;

        m_phaseHP -= damage;
        m_totalHP -= damage;

        InGameUIManager.instance.UpdateBossHPSlider(m_phaseHP);

        if (m_phaseHP <= 0)
        {
            InGameUIManager.instance.UpdateBossPhase(m_phase);

            if (m_totalHP <= 0)
            {
                StartCoroutine("Die");
                return;
            }

            // 페이즈 전환

            ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
            particleSystem.Play();

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
        GameManager.instance.isBossEnd = true;

        StopAllPatterns();
        GameManager.instance.BossDefeated();

        StartCoroutine("Blow");
    }

    private IEnumerator Blow()
    {
        // 이펙트
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
        yield return new WaitForSeconds(particleSystems[0].main.duration);

        m_collider.enabled = false;
        m_spriteRenderer.enabled = false;
        //yield return new WaitForSeconds(1f);

        DropSubWeaponItem();
        
        yield return new WaitForSeconds(3f);

        GameManager.instance.StageClear();
        m_bossTracker.SetActive(false);
        Destroy(gameObject);
    }

    void StopAllPatterns()
    {
        m_directionalAimed.StopPattern();
        m_directionalAimedRandom.StopPattern();
        m_directionalNormal.StopPattern();
        m_radialMulti.StopPattern();
        m_roundGuided.StopPattern();
        m_spiralMulti.StopPattern();
        m_homing.StopPattern();
    }

    void DropSubWeaponItem()
    {
        if (m_subWeaponItem)
        {
            DropItem(m_subWeaponItem);
        }
    }
}
