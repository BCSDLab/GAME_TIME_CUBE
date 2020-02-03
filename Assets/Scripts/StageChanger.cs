using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageChanger : MonoBehaviour
{
    public static StageChanger instance = null;

    private int m_savedSpellEnergy = GameManager.SPELL_ENERGY_MAX;
    private int m_savedPlayerPower = 0;
    private int m_savedPlayerHP = GameManager.PLAYER_HP_INIT;
    private float m_savedTotalScore = 0;
    private int m_savedOrbitorCount = 0;

    [Header("보조무기")]
    public GameObject subWeaponItem = null;
    public GameObject Orbitor = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InGameUIManager.instance.DisactiveClearPanel();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            GameManager.instance.isLoading = true;
            Invoke("LoadNextStage", 2f);
        }
    }

    void LoadNextStage()
    {
        GameManager.instance.isLoading = false;
        SceneManager.LoadScene(StageManager.instance.nextSceneName);
    }

    public void SaveData()
    {
        m_savedSpellEnergy = GameManager.instance.spellEnergy;
        m_savedPlayerPower = GameManager.instance.playerPower;
        m_savedPlayerHP = GameManager.instance.GetPlayerHP();
        m_savedOrbitorCount = GameManager.instance.orbitorCount;
        m_savedTotalScore += GameManager.instance.GetScore();
    }

    public void LoadData()
    {
        GameManager.instance.spellEnergy = m_savedSpellEnergy;
        GameManager.instance.playerPower = m_savedPlayerPower;
        GameManager.instance.playerHP = m_savedPlayerHP;
        GameManager.instance.orbitorCount = m_savedOrbitorCount;
        GameManager.instance.totalScore = m_savedTotalScore;
    }

    public void ResetStat()
    {
        m_savedSpellEnergy = GameManager.SPELL_ENERGY_MAX;
        m_savedPlayerPower = 0;
        m_savedPlayerHP = GameManager.PLAYER_HP_INIT;
        m_savedTotalScore = 0;
        m_savedOrbitorCount = 0;
    }
}
