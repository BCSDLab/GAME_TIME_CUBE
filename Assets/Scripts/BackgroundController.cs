using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float scrollSpeed;

    private Material m_material;

    void Start()
    {
        m_material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (!GameManager.instance.isDialogueOn)
        {
            float offsetX = m_material.mainTextureOffset.x + scrollSpeed * Time.deltaTime;
            Vector2 offset = new Vector2(offsetX, 0);

            m_material.mainTextureOffset = offset;
        }

        else
        {
            float offsetX = m_material.mainTextureOffset.x;
            Vector2 offset = new Vector2(offsetX, 0);

            m_material.mainTextureOffset = offset;
        }
    }
}
