using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXVolumeTester : MonoBehaviour
{
    public float delay = 0.1f;

    private AudioSource m_sfx;

    private void Start()
    {
        m_sfx = GetComponent<AudioSource>();
    }

    public void OnVolumeChange()
    {
        StopCoroutine("TestSFX");
        StartCoroutine("TestSFX");
    }

    private IEnumerator TestSFX()
    {
            yield return new WaitForSeconds(delay);
            m_sfx.Play();
    }
}
