using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracker : MonoBehaviour
{
    public Transform player;

    private Vector3 position;

    void Start()
    {       
        position = new Vector3(transform.position.x, player.position.y, 0f);
        transform.position = position;
    }

    void Update()
    {
        position.y = player.position.y;
        transform.position = position;
    }
}
