using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternTester : MonoBehaviour
{
    private BulletPattern m_testPattern;
    void Start()
    {
        m_testPattern = GetComponent<DirectionalAimed_NWay>();

        m_testPattern.StartPattern();
    }
}
