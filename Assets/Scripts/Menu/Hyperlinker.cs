using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyperlinker : MonoBehaviour
{
    public void Hyperlink(string adress)
    {
        Application.OpenURL(adress);
    }
}
