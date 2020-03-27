using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutController : MonoBehaviour
{
    public static FadeInOutController instance = null;
    [SerializeField]
    private Text stageText = null;
    [SerializeField]
    private Image panel = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void FadeOutPanel(float time = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(time));
    }

    IEnumerator FadeOutCoroutine(float time)
    {
        Color color = panel.color;
        while (color.a < 1f)
        {
            color.a += Time.deltaTime / time;
            panel.color = color;

            if (color.a >= 1f)
                color.a = 1f;

            yield return null;
        }
    }

    public void FadeInPanel(float time = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(time));
    }

    IEnumerator FadeInCoroutine(float time)
    {
        Color color = panel.color;

        while (color.a > 0f)
        {
            color.a -= Time.deltaTime/time;
            panel.color = color;

            if (color.a <= 0f)
                color.a = 0f;

            yield return null;
        }
    }

    public void FadeInText(float time = 2f)
    {
        StartCoroutine(FadeInTextCoroutine(time));
    }

    IEnumerator FadeInTextCoroutine(float time)
    {
        Color color = stageText.color;

        yield return new WaitForSeconds(1f);
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / time;
            stageText.color = color;

            if (color.a <= 0f)
                color.a = 0f;

            yield return null;
        }
    }
}
