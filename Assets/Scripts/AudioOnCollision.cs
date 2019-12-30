using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioOnCollision : MonoBehaviour
{
    private AudioSource m_sfx;

    private void Start()
    {
        m_sfx = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_sfx.Play();
    }
}
