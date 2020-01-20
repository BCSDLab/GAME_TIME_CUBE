using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pattern : MonoBehaviour
{
    [SerializeField]
    [Tooltip("미지정 시 해당 오브젝트 위치")]
    protected Transform m_spawnPos = null;
    [SerializeField]
    protected float m_startDelay;

    protected AudioSource m_audioSource;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void StartPattern()
    {
        if (m_spawnPos == null)
        {
            m_spawnPos = transform;
        }

        StartCoroutine("Fire");
    }

    public virtual void StopPattern()
    {
        StopCoroutine("Fire");
    }

    protected abstract IEnumerator Fire();
}
