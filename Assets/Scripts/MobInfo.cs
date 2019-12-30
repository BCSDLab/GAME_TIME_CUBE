using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MobInfo
{
    [Tooltip("몹 프리팹")]
    public GameObject mob = null;
    [Tooltip("(필요 시) 해당 몹의 이동 경로")]
    public string pathName = "";
    [Tooltip("등장 시간")]
    public float engageTime = 0f;
    [Tooltip("반복 등장 횟수")]
    public int repeatCount = 1;
    [Tooltip("반복 등장하는 시간 간격")]
    public float repeatDelay = 1f;
}
