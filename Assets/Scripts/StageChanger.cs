using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageChanger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.moveNextStage = true;
            Invoke("LoadNextStage", 2f);
        }
    }

    private void LoadNextStage()
    {
        PlayerController.instance.moveNextStage = false;
        SceneManager.LoadScene("Menu");
    }
}
