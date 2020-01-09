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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.moveNextStage = true;
            InGameUIManager.instance.stageClearPanel.SetActive(false);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            Invoke("LoadNextStage", 2f);
        }
    }

    private void LoadNextStage()
    {
        PlayerController.instance.moveNextStage = false;
        string currStageName = SceneManager.GetActiveScene().name;

        //Debug.Log(currStageName);

        if (currStageName.StartsWith("Stage"))
        {
            SaveData();

            int currStage = int.Parse(currStageName.Substring(5));
            SceneManager.LoadScene("Stage" + ++currStage);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void SaveData()
    {
        savedSpellEnergy = GameManager.instance.spellEnergy;
        savedPlayerPower = GameManager.instance.playerPower;
        savedPlayerHP = GameManager.instance.GetPlayerHP();
        savedTotalScore += GameManager.instance.GetScore();
    }
}
