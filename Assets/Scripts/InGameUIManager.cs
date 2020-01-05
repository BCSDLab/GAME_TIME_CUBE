﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance = null;

    #region SERIALIZED_FIELDS
    [Header("플레이어")]
    [SerializeField]
    private Transform playerHPSlots = null;
    [SerializeField]
    private Slider timeCubeSlider = null;
    [SerializeField]
    private Slider spellSlider1 = null;
    [SerializeField]
    private Slider spellSlider2 = null;
    [SerializeField]
    private Slider powerSlider = null;
    [SerializeField]
    private Text powerText = null;
    [SerializeField]
    private Transform powerIcons = null;
    [SerializeField]
    private Text scoreText = null;
    [Header("보스")]
    [SerializeField]
    private Slider bossHPSlider = null;
    [SerializeField]
    private Transform bossPhaseSlots = null;  // 빈 오브젝트
    [SerializeField]
    private GameObject bossPhaseSlot = null;  // 프리팹
    [SerializeField]
    private float phaseSlotDistance = 40f;
    [Header("패널")]
    [SerializeField]
    private GameObject pausePanel = null;
    [SerializeField]
    private GameObject gameOverPanel = null;
    [SerializeField]
    private GameObject stageClearPanel = null;
    [SerializeField]
    private Text killCountText = null;
    [SerializeField]
    private Text hitCountText = null;
    [SerializeField]
    private Text stageScoreText = null;
    [SerializeField]
    private Text totalScoreText = null;
    #endregion

    private const float UPDATE_DELAY = 0.05f;
    private Image timeCubeSliderFill;
    private Image spellSliderFill1;
    private Image spellSliderFill2;

    private int m_power = 0;
    private float m_score = 0f;
    private int m_bossHP = 0;
    private bool m_isFilling = false;
    private int m_bossPhaseCount = 0;

    // SliderFill colors
    private readonly Color CUBE_BASE_COLOR = new Color32(100, 150, 250, 200);
    private readonly Color CUBE_HEAL_COLOR = new Color32(100, 200, 200, 200);  // when cubeEnergy is regenerating
    private readonly Color CUBE_USE_COLOR = new Color32(200, 150, 250, 200);  // when cubeEnergy is being consumed
    private readonly Color SPELL_CHARGED_COLOR = new Color32(250, 100, 250, 200);
    private readonly Color SPELL_NOT_CHARGED_COLOR = new Color32(250, 100, 250, 150);

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        timeCubeSliderFill = timeCubeSlider.fillRect.gameObject.GetComponent<Image>();
        spellSliderFill1 = spellSlider1.fillRect.gameObject.GetComponent<Image>();
        spellSliderFill2 = spellSlider2.fillRect.gameObject.GetComponent<Image>();

        timeCubeSlider.maxValue = GameManager.CUBE_ENERGY_MAX;
        spellSlider1.maxValue = GameManager.SPELL_ENERGY_USAGE;
        spellSlider2.maxValue = GameManager.SPELL_ENERGY_MAX - GameManager.SPELL_ENERGY_USAGE;
        powerSlider.maxValue = GameManager.PLAYER_POWER_MAX;

        bossHPSlider.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        stageClearPanel.SetActive(false);
    }

    public void DamagePlayer(int playerHP)
    {
        playerHPSlots.GetChild(playerHP).gameObject.SetActive(false);
    }

    public void HealPlayer(int playerHP)
    {
        playerHPSlots.GetChild(playerHP-1).gameObject.SetActive(true);
    }

    public void UpdateTimeCubeSlider(int cubeEnergy)
    {
        timeCubeSliderFill.color = (cubeEnergy < timeCubeSlider.value) ? CUBE_USE_COLOR : CUBE_HEAL_COLOR;
        timeCubeSlider.value = cubeEnergy;
    }

    public void ResetTimeCubeSliderColor()
    {
        timeCubeSliderFill.color = CUBE_BASE_COLOR;
    }

    public void UpdateSpellSlider(int spellEnergy)
    {
        int remainder = spellEnergy - GameManager.SPELL_ENERGY_USAGE;

        if (remainder >= 0)
        {
            spellSlider1.value = GameManager.SPELL_ENERGY_USAGE;
            spellSlider2.value = remainder;
            spellSliderFill1.color = SPELL_CHARGED_COLOR;
            spellSliderFill2.color = (remainder >= GameManager.SPELL_ENERGY_USAGE) ? SPELL_CHARGED_COLOR : SPELL_NOT_CHARGED_COLOR;
        }
        else
        {
            spellSlider1.value = spellEnergy;
            spellSlider2.value = 0f;
            spellSliderFill1.color = SPELL_NOT_CHARGED_COLOR;
            spellSliderFill2.color = SPELL_NOT_CHARGED_COLOR;
        }
    }

    public void UpdatePower(int power)
    {
        powerSlider.value = power;
        StopCoroutine("CountUpToPower");
        StartCoroutine("CountUpToPower", power);
        powerIcons.GetChild(0).gameObject.SetActive(power >= 1000);
        powerIcons.GetChild(1).gameObject.SetActive(power >= 2000);
        powerIcons.GetChild(2).gameObject.SetActive(power >= 3000);
    }

    IEnumerator CountUpToPower(int targetPower)
    {
        int origPower = m_power;
        float delta = 0;
        while (delta < 1f)
        {
            delta += 0.1f;
            m_power = (int)Mathf.Lerp(origPower, targetPower, delta);
            powerText.text = m_power.ToString();
            yield return new WaitForSeconds(UPDATE_DELAY);
        }
    }

    public void UpdateScoreText(float score)
    {
        StopCoroutine("CountUpToScore");
        StartCoroutine("CountUpToScore", score);
    }

    IEnumerator CountUpToScore(float targetScore)
    {
        float origScore = m_score;
        float delta = 0f;
        while (delta < 1f)
        {
            delta += 0.1f;
            m_score = Mathf.Lerp(origScore, targetScore, delta);
            scoreText.text = ((int)m_score).ToString();
            yield return new WaitForSeconds(UPDATE_DELAY);
        }
    }

    #region BOSS
    public void DisplayBossHPSlider(bool display = true, int hp = 0)
    {
        if (!display)
        {
            bossHPSlider.gameObject.SetActive(false);
            return;
        }

        m_bossHP = hp;
        bossHPSlider.maxValue = hp;
        bossHPSlider.value = 0f;
        bossHPSlider.gameObject.SetActive(true);
        StartCoroutine("FillBossHPSlider");
    }

    public void UpdateBossHPSlider(int hp)
    {
        if (!m_isFilling)
        {
            bossHPSlider.value = hp;
        }
        m_bossHP = hp;
    }

    IEnumerator FillBossHPSlider()
    {
        m_isFilling = true;

        int fill = m_bossHP / 200;
        while (bossHPSlider.value < m_bossHP)
        {
            bossHPSlider.value += fill;
            yield return new WaitForSeconds(0.01f);
        }

        m_isFilling = false;
        bossHPSlider.value = m_bossHP;
    }

    public void InitializeBossPhaseSlots(int phaseCount)
    {
        m_bossPhaseCount = phaseCount;
        for (int i = 0; i < phaseCount; i++)
        {
            GameObject slot = Instantiate(bossPhaseSlot, bossPhaseSlots);
            Vector3 newPosition = slot.transform.position;
            newPosition.x += phaseSlotDistance * i;
            slot.transform.position = newPosition;
        }
    }

    public void UpdateBossPhase(int phase)
    {
        bossPhaseSlots.GetChild(m_bossPhaseCount - phase).gameObject.SetActive(false);
    }
    #endregion

    public void PauseGame(bool pause)
    {
        pausePanel.SetActive(pause);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ClearStage(int killCount, int hitCount, float stageScore, float totalScore)
    {
        killCountText.text = killCount.ToString();
        hitCountText.text = hitCount.ToString();
        stageScoreText.text = stageScore.ToString();
        totalScoreText.text = totalScore.ToString();
        stageClearPanel.SetActive(true);
    }
}
