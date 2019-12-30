using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager instance = null;  // 싱글톤

    private AudioSource[] m_audioSources;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        m_audioSources = GetComponents<AudioSource>();
    }

    public void Play(int idx, float time = 2f)
    {
        StopAllExcept(idx);
        StartCoroutine(FadeIn(idx, time));
    }
    public void Stop(int idx, float time = 2f)
    {
        StartCoroutine(FadeOut(idx, time));
    }
    public void StopAllExcept(int except = -1, float time = 2f)
    {
        for (int i = 0; i < m_audioSources.Length; i++)
        {
            if (i == except) continue;

            StartCoroutine(FadeOut(i, time));
        }
    }

    IEnumerator FadeIn(int idx, float time = 2f)
    {
        AudioSource audioSource = m_audioSources[idx];
        float targetVolume = audioSource.volume;
        audioSource.volume = 0f;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / time;

            yield return null;
        }

        audioSource.volume = targetVolume;
    }
    IEnumerator FadeOut(int idx, float time = 2f)
    {
        AudioSource audioSource = m_audioSources[idx];
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / time;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
