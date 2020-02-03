using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance = null;  // 싱글톤

    [SerializeField]
    [Header("자코 배치")]
    private MobInfo[] m_mobInfoes = null;
    [SerializeField]
    [Header("보스 등장 시간")]
    private float m_bossEngageTime = 0f;
    [SerializeField]
    private GameObject m_boss = null;
    [SerializeField]
    private GameObject m_bossTracker = null;

    [SerializeField]
    private int m_bossLimitTime = 600;  // 보스 제한 시간
    public string nextSceneName;  // 다음 스테이지 씬 이름

    private bool m_isSpawning = true;
    private int m_indexToSpawn = 0;
    private bool m_hasBossSpawned = false;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameManager.instance.isBossDefeated = false;
        m_mobInfoes = m_mobInfoes.OrderBy(x => x.engageTime).ToArray();

        BGMManager.instance.Play(0);
    }

    void Update()
    {
        if (m_hasBossSpawned) return;

        if (Time.timeSinceLevelLoad >= m_bossEngageTime)
        {
            SpawnBoss();
            return;
        }

        if (m_indexToSpawn >= m_mobInfoes.Length)
            return;

        if (Time.timeSinceLevelLoad >= m_mobInfoes[m_indexToSpawn].engageTime)
        {
            StartCoroutine("SpawnZaco", m_indexToSpawn);
            m_indexToSpawn++;
        }
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

    void SpawnBoss()
    {
        StopSpawning();

        GameManager.instance.DestroyAllBullets();
        GameManager.instance.DestroyAllEnemies();

        m_boss.SetActive(true);
        m_bossTracker.SetActive(true);

        m_hasBossSpawned = true;

        InGameUIManager.instance.InitBossTimer(m_bossLimitTime);
    }

    public void StopSpawning()
    {
        m_isSpawning = false;
        StopAllCoroutines();
    }
}
