using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance = null;

    private AudioSource m_audioSource;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        m_audioSource.Play();
    }
}
