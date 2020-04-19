using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance = null;

    #region SERIALIZED_FIELDS
    [Header("플레이어")]
    [SerializeField]
    private Transform m_playerHPSlots = null;
    [SerializeField]
    private Slider m_cubeSlider = null;
    [SerializeField]
    private Slider m_spellSlider1 = null;
    [SerializeField]
    private Slider m_spellSlider2 = null;
    [SerializeField]
    private Slider m_powerSlider = null;
    [SerializeField]
    private Text m_powerText = null;
    [SerializeField]
    private Transform m_powerIcons = null;
    [SerializeField]
    private Text m_scoreText = null;
    [SerializeField]
    private Slider m_dynCubeSlider = null;
    [SerializeField]
    private Slider m_dynSpellSlider= null;
    [Header("보스")]
    [SerializeField]
    private Slider m_bossHPSlider = null;
    [SerializeField]
    private Transform m_bossPhaseSlots = null;  // 빈 오브젝트
    [SerializeField]
    private GameObject m_bossPhaseSlot = null;  // 프리팹
    [SerializeField]
    private float m_phaseSlotDistance = 40f;
    [SerializeField]
    private GameObject m_bossTimer = null;
    [SerializeField]
    private Text m_bossTimerText = null;
    [Header("패널")]
    [SerializeField]
    private GameObject m_pausePanel = null;
    [SerializeField]
    private GameObject m_gameOverPanel = null;
    [SerializeField]
    private GameObject m_timeOverText = null;
    [SerializeField]
    private GameObject m_newText = null;
    [SerializeField]
    private GameObject m_stageClearPanel = null;
    [SerializeField]
    private Text m_killCountText = null;
    [SerializeField]
    private Text m_hitCountText = null;
    [SerializeField]
    private Text m_stageScoreText = null;
    [SerializeField]
    private Text m_totalScoreText = null;
    [SerializeField]
    private Text m_highScoreText = null;
    [SerializeField]
    private Text m_continueText = null;
    [SerializeField]
    private Image m_warningBar = null;
    #endregion

    private AudioSource m_scoreAudio;
    private AudioSource m_gameOverAudio;
    private object[] paramArr; // killCount, hitCount, stageScore, totalScore
    private int paramIdx;
    private const float UPDATE_DELAY = 0.05f;
    private Image m_cubeSliderFill;
    private Image m_spellSliderFill1;
    private Image m_spellSliderFill2;
    private Transform m_playerTransform;

    private int m_power = 0;
    private float m_score = 0f;
    private int m_bossHP = 0;
    private bool m_isFilling = false;
    private int m_bossPhaseCount = 0;
    private float m_bossTimeLimit = 0;

    // SliderFill colors
    private readonly Color CUBE_BASE_COLOR = new Color32(100, 150, 250, 200);
    private readonly Color CUBE_HEAL_COLOR = new Color32(100, 200, 200, 200);  // when cubeEnergy is regenerating
    private readonly Color CUBE_USE_COLOR = new Color32(200, 150, 250, 200);  // when cubeEnergy is being consumed
    private readonly Color SPELL_CHARGED_COLOR = new Color32(250, 100, 250, 200);
    private readonly Color SPELL_NOT_CHARGED_COLOR = new Color32(200, 100, 200, 150);

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();
        m_scoreAudio = audioSources[0];
        m_gameOverAudio = audioSources[1];
    }

    void OnEnable()
    {
        m_cubeSliderFill = m_cubeSlider.fillRect.gameObject.GetComponent<Image>();
        m_spellSliderFill1 = m_spellSlider1.fillRect.gameObject.GetComponent<Image>();
        m_spellSliderFill2 = m_spellSlider2.fillRect.gameObject.GetComponent<Image>();
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        m_cubeSlider.maxValue = GameManager.CUBE_ENERGY_MAX;
        m_spellSlider1.maxValue = GameManager.SPELL_ENERGY_USAGE;
        m_spellSlider2.maxValue = GameManager.SPELL_ENERGY_MAX - GameManager.SPELL_ENERGY_USAGE;
        m_powerSlider.maxValue = GameManager.PLAYER_POWER_MAX;
        m_dynCubeSlider.maxValue = GameManager.CUBE_ENERGY_MAX;
        m_dynCubeSlider.gameObject.SetActive(false);
        m_dynSpellSlider.gameObject.SetActive(false);

        m_bossHPSlider.gameObject.SetActive(false);
        m_pausePanel.SetActive(false);
        m_gameOverPanel.SetActive(false);
        m_stageClearPanel.SetActive(false);
    }

    public void DamagePlayer(int playerHP)
    {
        m_playerHPSlots.GetChild(playerHP).gameObject.SetActive(false);
    }

    public void HealPlayer(int playerHP)
    {
        m_playerHPSlots.GetChild(playerHP-1).gameObject.SetActive(true);
    }


    public void UpdateCubeSlider(int cubeEnergy)
    {
        m_cubeSliderFill.color = (cubeEnergy < m_cubeSlider.value) ? CUBE_USE_COLOR : CUBE_HEAL_COLOR;
        m_cubeSlider.value = cubeEnergy;
        m_dynCubeSlider.value = cubeEnergy;

        Camera camera = Camera.main;
        Vector3 screenPoint = camera.WorldToScreenPoint(m_playerTransform.position);
        m_dynCubeSlider.transform.position = screenPoint + Vector3.up * 50;
    }

    public void ResetCubeSliderColor()
    {
        m_cubeSliderFill.color = CUBE_BASE_COLOR;
    }

    public void EnableDynCubeSlider(bool enable = true)
    {
        m_dynCubeSlider.gameObject.SetActive(enable);
        if (enable)
        {
            Camera camera = Camera.main;
            Vector3 screenPoint = camera.WorldToScreenPoint(m_playerTransform.position);
            m_dynCubeSlider.transform.position = screenPoint + Vector3.up * 50;
        }
    }

    public void InitDynSpellSlider(float maxValue)
    {
        m_dynSpellSlider.maxValue = maxValue;
    }
    public void EnableDynSpellSlider(bool enable = true)
    {
        if (!m_dynSpellSlider) return;

        m_dynSpellSlider.gameObject.SetActive(enable);
        if (enable)
        {
            m_dynSpellSlider.value = m_dynSpellSlider.maxValue;
            Camera camera = Camera.main;
            Vector3 screenPoint = camera.WorldToScreenPoint(m_playerTransform.position);
            m_dynSpellSlider.transform.position = screenPoint + Vector3.up * 60;
        }
    }
    public void UpdateDynSpellSlider(float value)
    {
        m_dynSpellSlider.value -= value;
        Camera camera = Camera.main;
        Vector3 screenPoint = camera.WorldToScreenPoint(m_playerTransform.position);
        m_dynSpellSlider.transform.position = screenPoint + Vector3.up * 60;
    }

    public void UpdateSpellSlider(int spellEnergy)
    {
        int remainder = spellEnergy - GameManager.SPELL_ENERGY_USAGE;

        if (remainder >= 0)
        {
            m_spellSlider1.value = GameManager.SPELL_ENERGY_USAGE;
            m_spellSlider2.value = remainder;
            m_spellSliderFill1.color = SPELL_CHARGED_COLOR;
            m_spellSliderFill2.color = (remainder >= GameManager.SPELL_ENERGY_USAGE) ? SPELL_CHARGED_COLOR : SPELL_NOT_CHARGED_COLOR;
        }
        else
        {
            m_spellSlider1.value = spellEnergy;
            m_spellSlider2.value = 0f;
            m_spellSliderFill1.color = SPELL_NOT_CHARGED_COLOR;
            m_spellSliderFill2.color = SPELL_NOT_CHARGED_COLOR;
        }
    }

    public void UpdatePower(int power)
    {
        m_powerSlider.value = power;
        StopCoroutine("CountUpToPower");
        StartCoroutine("CountUpToPower", power);
        m_powerIcons.GetChild(0).gameObject.SetActive(power >= 1000);
        m_powerIcons.GetChild(1).gameObject.SetActive(power >= 2000);
        m_powerIcons.GetChild(2).gameObject.SetActive(power >= 3000);
    }

    IEnumerator CountUpToPower(int targetPower)
    {
        int origPower = m_power;
        float delta = 0;
        while (delta < 1f)
        {
            delta += 0.1f;
            m_power = (int)Mathf.Lerp(origPower, targetPower, delta);
            m_powerText.text = m_power.ToString();
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
            m_scoreText.text = ((int)m_score).ToString();
            yield return new WaitForSeconds(UPDATE_DELAY);
        }
    }

    #region Clear Panel
    private void InitClearPanel()
    {
        m_killCountText.text = "0";
        m_hitCountText.text = "0";
        m_stageScoreText.text = "0";
        m_totalScoreText.text = "0";
        m_highScoreText.text = ((int)GameManager.instance.GetHighScore()).ToString();
        m_continueText.text = "";
}

    IEnumerator UpdateClearPanel()
    {
        float delta = 0f;
        switch (paramIdx)
        {
            case 0: // killCount
                while (delta < 1f)
                {
                    delta += 0.1f;

                    m_score = Mathf.Lerp(0, (int)paramArr[paramIdx], delta);
                    m_killCountText.text = ((int)m_score).ToString();

                    yield return new WaitForSeconds(UPDATE_DELAY);
                }
                break;
            case 1: // hitCount
                while (delta < 1f)
                {
                    delta += 0.1f;

                    m_score = Mathf.Lerp(0, (int)paramArr[paramIdx], delta);
                    m_hitCountText.text = ((int)m_score).ToString();

                    yield return new WaitForSeconds(UPDATE_DELAY);
                }
                break;
            case 2: // stageScore
                while (delta < 1f)
                {
                    delta += 0.1f;

                    m_score = Mathf.Lerp(0, (float)paramArr[paramIdx], delta);
                    m_stageScoreText.text = ((int)m_score).ToString();

                    yield return new WaitForSeconds(UPDATE_DELAY);
                }
                break;
            case 3: // totalScore
                while (delta < 1f)
                {
                    delta += 0.1f;

                    m_score = Mathf.Lerp(0, (float)paramArr[paramIdx], delta);
                    m_totalScoreText.text = ((int)m_score).ToString();

                    yield return new WaitForSeconds(UPDATE_DELAY);
                }
                break;
            default:
                break;
        }

        paramIdx++;
        m_scoreAudio.Play();
    }
    public void CallUpdateClearPanel()
    {
        StopCoroutine("UpdateClearPanel");

        if (paramIdx < paramArr.Length)
        {
            StartCoroutine("UpdateClearPanel");
        }
        else
        {
            CancelInvoke("CallUpdateClearPanel");

            m_continueText.text = "CONTINUE >>>";

            float totalScore = (float)paramArr[paramArr.Length - 1];
            if (float.Parse(m_highScoreText.text) < totalScore) {
                GameManager.instance.UpdateHighScore(totalScore);

                m_highScoreText.text = totalScore.ToString();
                m_newText.SetActive(true);
            }
            paramIdx = 0;

            StageChanger.instance.GetComponent<BoxCollider2D>().enabled = true;
            StageChanger.instance.SaveData();
        }
    }
    public void DisactiveClearPanel()
    {
        m_stageClearPanel.SetActive(false);
    }
    #endregion

    #region BOSS
    public void InitStartPhase(int phaseHP, float phaseTimeLimit)
    {
        DisplayBossHPSlider(hp: phaseHP);
        InitBossTimer(phaseTimeLimit);
    }

    public void DisplayBossHPSlider(bool display = true, int hp = 0)
    {
        if (!display)
        {
            m_bossHPSlider.gameObject.SetActive(false);
            return;
        }

        m_bossHP = hp;
        m_bossHPSlider.maxValue = hp;
        m_bossHPSlider.value = 0f;
        m_bossHPSlider.gameObject.SetActive(true);
        StartCoroutine("FillBossHPSlider");
    }

    public void UpdateBossHPSlider(int hp)
    {
        if (!m_isFilling)
        {
            m_bossHPSlider.value = hp;
        }
        m_bossHP = hp;
    }

    IEnumerator FillBossHPSlider()
    {
        m_isFilling = true;

        int fill = m_bossHP / 100;
        while (m_bossHPSlider.value < m_bossHP)
        {
            m_bossHPSlider.value += fill;
            yield return new WaitForSeconds(0.01f);
        }

        m_isFilling = false;
        m_bossHPSlider.value = m_bossHP;
    }

    public void InitializeBossPhaseSlots(int phaseCount)
    {
        m_bossPhaseCount = phaseCount;
        for (int i = 0; i < phaseCount; i++)
        {
            GameObject slot = Instantiate(m_bossPhaseSlot, m_bossPhaseSlots);
            Vector3 newPosition = slot.transform.position;
            newPosition.x += m_phaseSlotDistance * i;
            slot.transform.position = newPosition;
        }
    }

    public void UpdateBossPhase(int phase)
    {
        m_bossPhaseSlots.GetChild(m_bossPhaseCount - phase).gameObject.SetActive(false);
    }

    public void InitBossTimer(float timeLimit)
    {
        m_bossTimeLimit = timeLimit;
        m_bossTimer.GetComponent<Image>().fillAmount = 1;
        m_bossTimer.SetActive(true);
        m_bossTimerText.text = timeLimit.ToString();
        m_bossTimerText.gameObject.SetActive(true);
    }

    public void UpdateBossTimer(float timeLeft)
    {
        // 게임 오버일 때 BossController에서 타이머를 정지시키면 더 좋을 것 같음
        if (GameManager.instance.isGameOver) return;

        m_bossTimer.GetComponent<Image>().fillAmount = timeLeft / m_bossTimeLimit;
        m_bossTimerText.text = timeLeft.ToString();

        int iTimeLeft = (int)timeLeft;
        if(timeLeft - iTimeLeft <= float.Epsilon)
        {
            if(iTimeLeft == 30 || (0 < iTimeLeft && iTimeLeft <= 5))
            {
                m_bossTimer.GetComponent<AudioSource>().Play();
            }
        }
    }

    public void ChangeBossTimerColor(Color color)
    {
        m_bossTimer.GetComponent<Image>().color = color;
    }

    public void DisableBossTimer()
    {
        m_bossTimer.SetActive(false);
    }
    #endregion

    public void WarningSide()
    {
        StartCoroutine("WarningLeft");
    }
    IEnumerator WarningLeft()
    {
        float dt = 0f;
        while(dt < 1f)
        {
            dt += 0.05f;
            Debug.Log(dt);
            Color color = m_warningBar.color;
            color.a = Mathf.Sin(dt * 3.14f);
            m_warningBar.color = color;

            yield return new WaitForSeconds(UPDATE_DELAY);
        }
    }

    public void PauseGame(bool pause)
    {
        m_pausePanel.SetActive(pause);
    }

    public void GameOver()
    {
        m_gameOverPanel.SetActive(true);
        m_gameOverAudio.Play();
    }

    public void EnableTimeOverText()
    {
        m_timeOverText.SetActive(true);
    }

    public void ClearStage(int killCount, int hitCount, float stageScore, float totalScore)
    {
        m_bossTimer.SetActive(false);
        m_bossTimerText.gameObject.SetActive(false);

        paramArr = new object[4] { killCount, hitCount, stageScore, totalScore };

        m_scoreText.gameObject.SetActive(false);

        InitClearPanel();
        m_stageClearPanel.SetActive(true);

        InvokeRepeating("CallUpdateClearPanel", 0, 1.5f);
    }
}
