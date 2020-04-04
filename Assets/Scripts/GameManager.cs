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
    public const int PLAYER_HP_INIT = 3;
    private const int PLAYER_HP_MAX = 5;
    private const int CUBE_ENERGY_USAGE = 20;
    private const int CUBE_ENERGY_RECOVER = 100;
    private const string AXIS_PAUSE = "Pause";
    #endregion

    #region NONSERIALIZED
    [System.NonSerialized]
    public int cubeEnergy = CUBE_ENERGY_MAX;
    [System.NonSerialized]
    public int spellEnergy = SPELL_ENERGY_MAX;
    [System.NonSerialized]
    public int playerHP = PLAYER_HP_INIT;
    [System.NonSerialized]
    public int playerPower = 0;
    [System.NonSerialized]
    public int[] subWeaponCount; // Orbitor, Follower
    [System.NonSerialized]
    public float totalScore = 0;
    [System.NonSerialized]
    public bool isPlayerAttacking = false;
    [System.NonSerialized]
    public bool isPlayerSpelling = false;
    [System.NonSerialized]
    public bool isBossDefeated = false;
    [System.NonSerialized]
    public bool isPaused = false;  // 일시정지 (P)
    [System.NonSerialized]
    public bool isDialogueOn = false;  // 대화
    [System.NonSerialized]
    public bool isLoading = false;  // 스테이지 로딩
    [System.NonSerialized]
    public bool isGameOver = false;  // 플레이어 사망
    #endregion

    private bool m_pauseToggled = false;
    private float m_score = 0;
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

        if (pause > 0.1f && !m_pauseToggled && playerHP > 0)
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
        StageChanger.instance.LoadData();
        SubWeaponItem sw = StageChanger.instance.subWeaponItem.GetComponent<SubWeaponItem>();

        Transform playerTR = GameObject.FindGameObjectWithTag("Player").transform;
        for(int swIdx = 0; swIdx < subWeaponCount.Length; swIdx++)
        {
            for(int i = 1; i <= subWeaponCount[swIdx]; i++)
            {
                Instantiate(sw.subWeapon[swIdx], playerTR.position, Quaternion.identity).name = sw.subWeapon[swIdx].name + "_" + i;
                sw.InitSubWeaponPosition(swIdx);
            }
        }

        FadeInOutController.instance.FadeInPanel();
        FadeInOutController.instance.FadeInText();
    }

    void InitializeUI()
    {
        InGameUIManager.instance.UpdateCubeSlider(cubeEnergy);
        InGameUIManager.instance.UpdateSpellSlider(spellEnergy);
        InGameUIManager.instance.UpdatePower(playerPower);
        InGameUIManager.instance.UpdateScoreText(m_score);
        for(int hpSlot = PLAYER_HP_MAX; hpSlot > playerHP; hpSlot--)
        {
            InGameUIManager.instance.DamagePlayer(hpSlot - 1);
        }
    }

    #region TIME CUBE
    public void UseCubeEnergy(int amount = CUBE_ENERGY_USAGE)
    {
        cubeEnergy -= amount;
        cubeEnergy = (cubeEnergy < 0) ? 0 : cubeEnergy;
        InGameUIManager.instance.UpdateCubeSlider(cubeEnergy);
    }

    public void AddCubeEnergy(int amount)
    {
        cubeEnergy += amount;
        cubeEnergy = (cubeEnergy > CUBE_ENERGY_MAX) ? CUBE_ENERGY_MAX : cubeEnergy;
        InGameUIManager.instance.UpdateCubeSlider(cubeEnergy);
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
            InGameUIManager.instance.UpdateCubeSlider(cubeEnergy);

            yield return new WaitForSeconds(0.3f);
        }

        InGameUIManager.instance.ResetCubeSliderColor();
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
        InGameUIManager.instance.UpdatePower(playerPower);
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

    public void UpdateHighScore(float score = 0)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if(sceneName.StartsWith("Stage "))
        {
            PlayerPrefs.SetFloat("HighScore_" + sceneName.Replace("Stage ", ""), score);

            Debug.Log("New HighScore_" + sceneName + " : " + score);
        }
    }

    public float GetHighScore(int stage = -1)
    {
        string highScoreName = "HighScore_", sceneName = SceneManager.GetActiveScene().name;

        if (stage == -1 && sceneName.StartsWith("Stage "))
        {
            highScoreName += sceneName.Replace("Stage ", "");
        }
        else
        {
            highScoreName += stage;
        }

        return PlayerPrefs.HasKey(highScoreName) ? PlayerPrefs.GetFloat(highScoreName) : 0;
    }

    void ResetScore()
    {
        m_score = 0;
        InGameUIManager.instance.UpdateScoreText(m_score);
    }

    public void AddKillCount()
    {
        m_killCount++;
    }
    #endregion

    #region PLAYER HP
    public void DamagePlayer()
    {
        m_hitCount++;
        playerHP--;
        InGameUIManager.instance.DamagePlayer(playerHP);

        // 파워 드롭
        playerPower /= 2;  // 50%
        InGameUIManager.instance.UpdatePower(playerPower);

        if (playerHP == 0)
        {
            PlayerController.instance.DisablePlayer();
            Invoke("GameOver", 1f);
        }
    }

    public void LifeUp(int scoreWhenMaxLife = 0)
    {
        if (playerHP == 0) {
            Debug.Log("플레이어 라이프 0 상태에서 라이프 아이템 획득");
            return;
        }

        if (playerHP == PLAYER_HP_MAX)
        {
            AddScore(scoreWhenMaxLife);
            return;
        }
        playerHP++;
        InGameUIManager.instance.HealPlayer(playerHP);
    }

    public int GetPlayerHP() { return playerHP; }
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
        StageChanger.instance.ResetStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        StageChanger.instance.ResetStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameObject.scene.name);
    }

    public void GameOver()
    {
        isGameOver = true;

        Invoke("LateGameOver", 2f);
    }
    public void LateGameOver()
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
            ParticleSystem particleSystem = bullet.GetComponentInChildren<ParticleSystem>();
            GameObject particleInst = Instantiate(particleSystem.gameObject, bullet.transform.position, Quaternion.identity, null);
            particleInst.GetComponent<ParticleSystem>().Play();
            Destroy(particleInst, particleSystem.main.duration + particleSystem.main.startLifetime.constant);

            PoolManager.instance.PushToPool(bullet);
        }
    }

    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Enemy enemyController = enemy.GetComponent<Enemy>();
            if (enemyController != null)
            {
            Destroy(enemy);
            enemyController.Die();
            }
        }
    }

    public void DestroyAllZacos()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.name != "Boss")
            {
                Enemy enemyController = enemy.GetComponent<Enemy>();
                if (enemyController != null) enemyController.Die();
                else Destroy(enemy);
            }
        }
    }

    public void BossDefeated()
    {
        AddKillCount();
        DestroyAllBullets();
        isPlayerAttacking = false;
        InGameUIManager.instance.DisplayBossHPSlider(display: false);
        InGameUIManager.instance.DisableBossTimer();
        isBossDefeated = true;
    }

    public void StageClear()
    {
        InGameUIManager.instance.ClearStage(m_killCount, m_hitCount, m_score, m_score + totalScore);
    }
}
