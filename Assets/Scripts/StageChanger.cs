using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageChanger : MonoBehaviour
{
    public static StageChanger instance = null;

    public int spellEnergy = GameManager.SPELL_ENERGY_MAX;
    public int playerPower = 0;
    public int playerHP = 5;

    private void Awake()
    {
        //if (instance == null) instance = this;
        //else Destroy(gameObject);
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
        spellEnergy = GameManager.instance.spellEnergy;
        playerPower = GameManager.instance.playerPower;
        playerHP = GameManager.instance.getPlayerHP();
    }
}
