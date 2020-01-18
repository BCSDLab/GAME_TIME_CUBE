using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;  // 싱글톤

    #region CONSTANTS
    public const int CUBE_ENERGY_MAX = 10000;
    public const int SPELL_ENERGY_MAX = 10000;
    public const int SPELL_ENERGY_USAGE = 5000;
    public const int PLAYER_POWER_MAX = 4000;
    private const int CUBE_ENERGY_USAGE = 20;
    private const int CUBE_ENERGY_RECOVER = 100;
    private const int PLAYER_HP_MAX = 5;
    #endregion

    [System.NonSerialized]
    public int cubeEnergy = CUBE_ENERGY_MAX;
    [System.NonSerialized]
    public int spellEnergy = SPELL_ENERGY_MAX;
    [System.NonSerialized]
    public int playerPower = 0;
    [System.NonSerialized]
    public int subWeaponNum = 0;
    [System.NonSerialized]
    public bool isPlayerAttacking = false;
    [System.NonSerialized]
    public bool isPlayerSpelling = false;
    [System.NonSerialized]
    public bool isBossEnd = false;  // 보스 시작/끝 판별
    [System.NonSerialized]
    public bool isPaused = false;  // 일시정지 (P)
    [System.NonSerialized]
    public bool isDialogueOn = false;  // 대화
    [System.NonSerialized]
    public bool isLoading = false;  // 스테이지 로딩

    private const string AXIS_PAUSE = "Pause"; 
    private bool m_pauseToggled = false;
    
    private float m_score = 0;
    private float m_totalScore = 0;
    private int m_playerHP = PLAYER_HP_MAX;
    private int m_killCount = 0;
    private int m_hitCount = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeStat();
        InitializeUI();
        ResetScore();
    }

    void Update()
    {
        float pause = Input.GetAxisRaw(AXIS_PAUSE);  // timeScale 영향 없음

        if (!m_pauseToggled && pause > 0.1f)
        {
            m_pauseToggled = true;

            PauseGame(pause: !isPaused);
        }
        else if (m_pauseToggled && pause == 0f)
        {
            m_pauseToggled = false;
        }
    }

    void InitializeStat()
    {
        StageChanger sc = InGameUIManager.instance.stageChanger.GetComponent<StageChanger>();
        //if (sc) Debug.Log("Stat Initialized!");

        spellEnergy = sc.savedSpellEnergy;
        playerPower = sc.savedPlayerPower;
        m_playerHP = sc.savedPlayerHP;

        subWeaponNum = sc.savedSubWeaponNum;
        if (subWeaponNum != 0)
        {
            Transform playerTR = GameObject.FindGameObjectWithTag("Player").transform;
            for (int i = 1; i <= subWeaponNum; i++)
            {
                Instantiate(sc.Orbitor, playerTR.position, Quaternion.identity).name = sc.Orbitor.name + "_" + i;
            }
            sc.subWeaponItem.GetComponent<SubWeaponItem>().InitOrbitorPosition();
        }

        m_totalScore = sc.savedTotalScore;
    }

    void InitializeUI()
    {
        InGameUIManager.instance.UpdateTimeCubeSlider(cubeEnergy);
        InGameUIManager.instance.UpdateSpellSlider(spellEnergy);
        InGameUIManager.instance.UpdatePowerSlider(playerPower);
        InGameUIManager.instance.UpdateScoreText(m_score);
        for(int hpSlot = PLAYER_HP_MAX; hpSlot > m_playerHP; hpSlot--)
        {
            InGameUIManager.instance.DamagePlayer(hpSlot - 1);
        }
    }

    #region TIME CUBE
    public void UseCubeEnergy(int amount = CUBE_ENERGY_USAGE)
    {
        cubeEnergy -= amount;
        cubeEnergy = (cubeEnergy < 0) ? 0 : cubeEnergy;
        InGameUIManager.instance.UpdateTimeCubeSlider(cubeEnergy);
    }

    public void AddCubeEnergy(int amount)
    {
        cubeEnergy += amount;
        cubeEnergy = (cubeEnergy > CUBE_ENERGY_MAX) ? CUBE_ENERGY_MAX : cubeEnergy;
        InGameUIManager.instance.UpdateTimeCubeSlider(cubeEnergy);
    }

    public void RecoverCube(bool recover = true)
    {
        StopCoroutine("RecoverCubeEnergy");
        if (recover) StartCoroutine("RecoverCubeEnergy");
    }

    IEnumerator RecoverCubeEnergy()
    {
        yield return new WaitForSeconds(1f);

        while (cubeEnergy < CUBE_ENERGY_MAX)
        {
            cubeEnergy += CUBE_ENERGY_RECOVER;
            cubeEnergy = (cubeEnergy > CUBE_ENERGY_MAX) ? CUBE_ENERGY_MAX : cubeEnergy;
            InGameUIManager.instance.UpdateTimeCubeSlider(cubeEnergy);

            yield return new WaitForSeconds(0.3f);
        }

        InGameUIManager.instance.ResetTimeCubeSliderColor();
    }

    public void StopRecoverCube()
    {
        StopCoroutine("RecoverCubeEnergy");
    }
    #endregion

    #region SPELL
    public void UseSpell()
    {
        spellEnergy -= SPELL_ENERGY_USAGE;
        InGameUIManager.instance.UpdateSpellSlider(spellEnergy);
    }

    public void AddSpellEnergy(int amount)
    {
        if (spellEnergy >= SPELL_ENERGY_USAGE)
        {
            amount = (int)(amount * 0.7f);
        }

        spellEnergy += amount;
        spellEnergy = (spellEnergy > SPELL_ENERGY_MAX) ? SPELL_ENERGY_MAX : spellEnergy;
        InGameUIManager.instance.UpdateSpellSlider(spellEnergy);
    }
    #endregion

    #region POWER
    public void AddPower(int amount)
    {
        playerPower += amount;
        playerPower = (playerPower > PLAYER_POWER_MAX) ? PLAYER_POWER_MAX : (playerPower < 0) ? 0 : playerPower;
        InGameUIManager.instance.UpdatePowerSlider(playerPower);
    }
    #endregion

    #region SCORE
    public void AddScore(float amount)
    {
        m_score += amount;
        InGameUIManager.instance.UpdateScoreText(m_score);
    }

    public float GetScore()
    {
        return m_score;
    }

    void ResetScore()
    {
        m_score = 0;
        InGameUIManager.instance.UpdateScoreText(m_score);
    }
    #endregion

    #region PLAYER HP
    public void DamagePlayer()
    {
        m_hitCount++;
        m_playerHP--;
        InGameUIManager.instance.DamagePlayer(m_playerHP);

        // 파워 드롭
        playerPower /= 2;  // 50%
        InGameUIManager.instance.UpdatePowerSlider(playerPower);

        if (m_playerHP == 0)
        {
            Invoke("GameOver", 1f);
        }
    }

    public void LifeUp(int scoreWhenMaxLife = 0)
    {
        if (m_playerHP == PLAYER_HP_MAX)
        {
            AddScore(scoreWhenMaxLife);
            return;
        }
        m_playerHP++;
        InGameUIManager.instance.HealPlayer(m_playerHP);
    }

    public int GetPlayerHP() { return m_playerHP; }
    #endregion

    #region GAME FLOW
    public void PauseGame(bool pause = true)
    {
        isPaused = pause;
        InGameUIManager.instance.PauseGame(pause);

        Time.timeScale = (pause) ? 0f : 1f;
    }

    public void ToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameObject.scene.name);
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        InGameUIManager.instance.GameOver();
    }
    #endregion

    public void DestroyAllBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject bullet in bullets)
        {
            PoolManager.instance.PushToPool(bullet);
        }
    }

    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
            Destroy(enemy);
            enemyController.Die();
            }
        }
    }

    public void BossDefeated()
    {
        InGameUIManager.instance.DisplayBossHPSlider(display: false);
        m_killCount++;
        DestroyAllBullets();
        isPlayerAttacking = false;
    }

    public void StageClear()
    {
        InGameUIManager.instance.ClearStage(m_killCount, m_hitCount, m_score, m_score + m_totalScore);
    }
}
