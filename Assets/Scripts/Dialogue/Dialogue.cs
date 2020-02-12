using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public string nameL = "플레이어";
    public string nameR = "보스";
    [Tooltip("참이면 왼쪽, 거짓이면 오른쪽 대사입니다.")]
    public bool[] isSpeakerLeft;
    [TextArea(3, 10)]
    public string[] sentences;
    public int[] spriteNs;

    [Header("※ 화면에 처음 나올 때 스프라이트는 0번입니다.")]
    [Header("◆ 왼쪽 스프라이트")]
    public Sprite[] imagesL;
    [Header("◆ 오른쪽 스프라이트")]
    public Sprite[] imagesR;
}
