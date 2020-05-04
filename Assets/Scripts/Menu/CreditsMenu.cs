using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    private const string AXIS_SPELL = "Spell";
    private const float AXIS_MIN = 0.3f;

    void Update()
    {
        InputBack();
    }

    public void InputBack()
    {
        float back = Input.GetAxis(AXIS_SPELL);

        if (back > AXIS_MIN)
        {
            transform.GetChild(transform.childCount - 1).GetComponent<Button>().onClick.Invoke();
        }
    }
}
