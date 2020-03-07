using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternTester : MonoBehaviour
{
    private Pattern m_testPattern;
    private Pattern m_testPattern2;

    void Start()
    {
        m_testPattern = GetComponent<DirectionalAimedNWay>();

        m_testPattern.StartPattern();
    }
}
