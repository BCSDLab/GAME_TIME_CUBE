using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageChanger : MonoBehaviour
{
    public static StageChanger instance = null;

    [HideInInspector]
    public int savedSpellEnergy = GameManager.SPELL_ENERGY_MAX;
    [HideInInspector]
    public int savedPlayerPower = 0;
    [HideInInspector]
    public int savedPlayerHP = 5;
    [HideInInspector]
    public float savedTotalScore = 0;
    [HideInInspector]
    public int savedSubWeaponNum = 0;

    [Header("씬")]
    public string nextStageName = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InGameUIManager.instance.stageClearPanel.SetActive(false);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            GameManager.instance.isLoading = true;

            Invoke("LoadNextStage", 2f);
        }
    }

    private void LoadNextStage()
    {
        GameManager.instance.isLoading = false;

        SceneManager.LoadScene(nextStageName);
    }
    private void SetNextStage(string sceneName = null)
    {
        if (sceneName == null)
        {
            string currStageName = SceneManager.GetActiveScene().name;

            //Debug.Log(currStageName);

            if (currStageName.StartsWith("Stage"))
            {
                int currStage = int.Parse(currStageName.Substring(5));
                nextStageName = "Stage" + ++currStage;
            }
            else
            {
                nextStageName = "Menu";
            }
        }
        else
        {
            nextStageName = sceneName;
        }
    }

    public void SaveData()
    {
        savedSpellEnergy = GameManager.instance.spellEnergy;
        savedPlayerPower = GameManager.instance.playerPower;
        savedPlayerHP = GameManager.instance.GetPlayerHP();

        savedSubWeaponNum = GameManager.instance.subWeaponNum;

        savedTotalScore += GameManager.instance.GetScore();

        SetNextStage();
    }
}
