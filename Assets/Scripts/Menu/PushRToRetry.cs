using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushRToRetry : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.ResetGame();
        }
    }
}
